use serde::Serialize;
use std::sync::Mutex;
use sysinfo::{Networks, System};
use tauri::State;

struct AppState {
    sys: Mutex<System>,
}

#[derive(Serialize)]
struct SystemInfo {
    system_name: String,
    kernel_version: String,
    os_version: String,
    host_name: String,
}

#[derive(Serialize)]
struct CPUInfo {
    nb_cpus: usize,
    global_usage: f32,
}

#[derive(Serialize)]
struct MemoryInfo {
    total_memory: u64,
    used_memory: u64,
}

#[derive(Serialize)]
struct SwapInfo {
    total_swap: u64,
    used_swap: u64,
}

#[derive(Serialize)]
struct NetworkInfo {
    networks: Vec<String>,
}

#[derive(Serialize)]
struct FullSystemInfo {
    memory_info: MemoryInfo,
    swap_info: SwapInfo,
    system_info: SystemInfo,
    cpu_info: CPUInfo,
    network_info: NetworkInfo,
}

#[tauri::command]
fn get_system_info(state: State<AppState>) -> FullSystemInfo {
    let sys = state.sys.lock();

    if let Ok(mut sys) = sys {
        sys.refresh_all();

        let system_info = FullSystemInfo {
            system_info: SystemInfo {
                system_name: System::name().unwrap_or_else(|| "Unknown".into()),
                kernel_version: System::kernel_version().unwrap_or_else(|| "Unknown".into()),
                os_version: System::os_version().unwrap_or_else(|| "Unknown".into()),
                host_name: System::host_name().unwrap_or_else(|| "Unknown".into()),
            },
            cpu_info: CPUInfo {
                nb_cpus: sys.cpus().len(),
                global_usage: sys.global_cpu_usage(),
            },
            memory_info: MemoryInfo {
                total_memory: sys.total_memory(),
                used_memory: sys.used_memory(),
            },
            swap_info: SwapInfo {
                total_swap: sys.total_swap(),
                used_swap: sys.used_swap(),
            },
            network_info: NetworkInfo {
                networks: Networks::new_with_refreshed_list()
                    .iter()
                    .map(|(name, _)| name.to_string())
                    .collect(),
            },
        };

        system_info
    } else {
        FullSystemInfo {
            memory_info: MemoryInfo {
                total_memory: 0,
                used_memory: 0,
            },
            swap_info: SwapInfo {
                total_swap: 0,
                used_swap: 0,
            },
            system_info: SystemInfo {
                system_name: "Unknown".into(),
                kernel_version: "Unknown".into(),
                os_version: "Unknown".into(),
                host_name: "Unknown".into(),
            },
            cpu_info: CPUInfo {
                nb_cpus: 0,
                global_usage: 0.0,
            },
            network_info: NetworkInfo {
                networks: Vec::new(),
            },
        }
    }
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    let sys = System::new_all();

    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .manage(AppState {
            sys: Mutex::new(sys),
        })
        .invoke_handler(tauri::generate_handler![get_system_info])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
