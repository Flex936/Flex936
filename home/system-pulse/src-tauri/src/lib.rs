// Learn more about Tauri commands at https://tauri.app/develop/calling-rust/
use serde::Serialize;
use sysinfo::{Networks, System};

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
fn get_system_info() -> FullSystemInfo {
    let mut sys = System::new_all();
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
}

#[cfg_attr(mobile, tauri::mobile_entry_point)]
pub fn run() {
    tauri::Builder::default()
        .plugin(tauri_plugin_opener::init())
        .invoke_handler(tauri::generate_handler![get_system_info])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
