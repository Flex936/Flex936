import { createSignal, createEffect, onMount, onCleanup } from "solid-js";
import { invoke } from "@tauri-apps/api/core";
import "./App.css";

interface SystemInfo {
  system_name: string;
  kernel_version: string;
  os_version: string;
  host_name: string;
}

interface CPUInfo {
  nb_cpus: number;
}

interface MemoryInfo {
  total_memory: number;
  used_memory: number;
}

interface SwapInfo {
  total_swap: number;
  used_swap: number;
}

interface NetworkInfo {
  networks: string[];
}

interface FullSystemInfo {
  memory_info: MemoryInfo;
  swap_info: SwapInfo;
  system_info: SystemInfo;
  cpu_info: CPUInfo;
  network_info: NetworkInfo;
}

function App() {
  const [systemInfo, setSystemInfo] = createSignal<FullSystemInfo | null>(null);

  async function getSystemInfo() {
    const info = await invoke<FullSystemInfo>("get_system_info");
    setSystemInfo(info);
  }

  onMount(() => {
    getSystemInfo();

    const intervalId = setInterval(getSystemInfo, 1000);

    onCleanup(() => clearInterval(intervalId));
  });

  createEffect(() => {
    const data = systemInfo();
    if (data) {
      console.log("System Info Updated:", data);
    }
  });

  return (
    <main class="container">
      <h1>System Dashboard</h1>

      {systemInfo() ? (
        <div style={{ "text-align": "left" }}>
          <p><strong>System Name:</strong> {systemInfo()?.system_info.system_name}</p>
          <p><strong>Kernel Version:</strong> {systemInfo()?.system_info.kernel_version}</p>
          <p><strong>OS:</strong> {systemInfo()?.system_info.os_version}</p>
          <p><strong>Host Name:</strong> {systemInfo()?.system_info.host_name}</p>
          <p><strong>Memory:</strong> {systemInfo()?.memory_info.used_memory} / {systemInfo()?.memory_info.total_memory}</p>
          <p><strong>Swap:</strong> {systemInfo()?.swap_info.used_swap} / {systemInfo()?.swap_info.total_swap}</p>
          <p><strong>CPUs:</strong> {systemInfo()?.cpu_info.nb_cpus}</p>
          <p><strong>Networks:</strong> {systemInfo()?.network_info.networks.join(", ")}</p>

        </div>
      ) : (
        <p>Loading system info...</p>
      )}
    </main>
  );
}

export default App;