use rand::rng;
use rand::Rng;
use serde::Deserialize;

#[derive(Deserialize)]
struct PasswordRequest {
    symbols: bool,
    uppercase: bool,
    numbers: bool,
    length: usize,
}

#[tauri::command]
fn generate_password(req: PasswordRequest) -> String {
    if req.length == 0 {
        return "Please enter a length > 0".to_string();
    }
    let safe_length = if req.length > 1000 {
        return "Please enter a length < 1000".to_string();
    } else {
        req.length
    };

    let mut rng = rng();

    let lowercase = "abcdefghijklmnopqrstuvwxyz";
    let uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    let symbols = "!@#$%^&*()_+-=[]{}|";
    let numbers = "0123456789";

    let mut charset = String::from(lowercase);

    if req.uppercase {
        charset.push_str(uppercase);
    }
    if req.numbers {
        charset.push_str(numbers);
    }
    if req.symbols {
        charset.push_str(symbols);
    }

    let chars: Vec<char> = charset.chars().collect();

    let password: String = (0..safe_length)
        .map(|_| {
            let idx = rng.random_range(0..chars.len());
            chars[idx]
        })
        .collect();

    password
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .invoke_handler(tauri::generate_handler![generate_password])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
