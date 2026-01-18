use anyhow::{anyhow, Result};
use serde::Deserialize;
use std::path::Path;
use std::process::Command;
use tokio::fs;

#[derive(Deserialize)]
struct FFProbeOutput {
    streams: Vec<Stream>,
}

#[derive(Deserialize)]
struct Stream {
    codec_name: Option<String>,
    codec_type: String,
}

const VIDEOS_DIR: &str = "./videos";
const VIDEO_EXTENSIONS: [&str; 7] = ["mp4", "mkv", "avi", "mov", "flv", "wmv", "webm"];

async fn probe_file(path: &Path) -> Result<FFProbeOutput> {
    let output = Command::new("ffprobe")
        .args([
            "-v",
            "error",
            "-show_entries",
            "stream=codec_name,codec_type",
            "-of",
            "json",
        ])
        .arg(path)
        .output()?;

    let probe: FFProbeOutput = serde_json::from_slice(&output.stdout)?;
    Ok(probe)
}

async fn convert_file(input: &Path, output: &Path) -> Result<()> {
    println!("Starting conversion: {:?}", input.file_name().unwrap());

    let status = Command::new("ffmpeg")
        .args([
            "-i",
            input.to_str().unwrap(),
            "-c:v",
            "libsvtav1",
            "-preset",
            "8",
            "-crf",
            "35",
            "-c:a",
            "libopus",
            "-b:a",
            "128k",
            output.to_str().unwrap(),
        ])
        .status()?;

    if status.success() {
        Ok(())
    } else {
        Err(anyhow!("FFmpeg exited with error status"))
    }
}

#[tokio::main]
async fn main() -> Result<()> {
    let video_dir = Path::new(VIDEOS_DIR);
    if !video_dir.exists() {
        return Err(anyhow!("Directory not found"));
    }

    // Using walkdir for recursive file discovery
    for entry in walkdir::WalkDir::new(video_dir)
        .into_iter()
        .filter_map(|e| e.ok())
    {
        let path = entry.path();
        if !path.is_file() {
            continue;
        }

        let ext = path
            .extension()
            .and_then(|s| s.to_str())
            .unwrap_or("")
            .to_lowercase();
        if !VIDEO_EXTENSIONS.contains(&ext.as_str()) {
            continue;
        }

        let mut needs_conversion = false;
        let file_name = path.file_name().unwrap().to_string_lossy();

        if ext != "webm" {
            println!("[{}] is not .webm. Conversion required.", file_name);
            needs_conversion = true;
        } else {
            match probe_file(path).await {
                Ok(metadata) => {
                    let is_av1 = metadata
                        .streams
                        .iter()
                        .any(|s| s.codec_type == "video" && s.codec_name.as_deref() == Some("av1"));
                    let is_opus = metadata.streams.iter().any(|s| {
                        s.codec_type == "audio" && s.codec_name.as_deref() == Some("opus")
                    });

                    if !is_av1 || !is_opus {
                        println!("[{}] wrong codecs. Conversion required.", file_name);
                        needs_conversion = true;
                    } else {
                        println!("[{}] is valid. Skipping.", file_name);
                    }
                }
                Err(_) => {
                    println!("Failed to probe {}, forcing conversion.", file_name);
                    needs_conversion = true;
                }
            }
        }

        if needs_conversion {
            let stem = path.file_stem().unwrap().to_string_lossy();
            let temp_path = path.with_file_name(format!("{}.temp.webm", stem));
            let final_path = path.with_file_name(format!("{}.webm", stem));

            if let Err(e) = convert_file(path, &temp_path).await {
                eprintln!("Error converting: {}", e);
                if temp_path.exists() {
                    fs::remove_file(&temp_path).await?;
                }
            } else {
                fs::remove_file(path).await?;
                fs::rename(&temp_path, &final_path).await?;
                println!(
                    "Replaced original with: {:?}",
                    final_path.file_name().unwrap()
                );
            }
        }
    }

    Ok(())
}
