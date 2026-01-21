import { createSignal, onMount, onCleanup, Show, Component } from "solid-js";
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
  global_usage: number;
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

const formatBytes = (bytes: number) => {
  if (bytes === 0) return "0 B";
  const k = 1024;
  const sizes = ["B", "KB", "MB", "GB", "TB"];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
};

const ProgressBar: Component<{ label: string; value: number; max: number; unit?: string }> = (props) => {
  const percentage = () => Math.min(100, Math.max(0, (props.value / props.max) * 100));

  return (
    <div class="stat-group">
      <div class="stat-header">
        <span>{props.label}</span>
        <span>{props.unit ? `${props.value.toFixed(1)}${props.unit}` : `${formatBytes(props.value)} / ${formatBytes(props.max)}`}</span>
      </div>
      <div class="progress-bg">
        <div class="progress-fill" style={{ width: `${percentage()}%` }}></div>
      </div>
    </div>
  );
};

function App() {
  const [info, setInfo] = createSignal<FullSystemInfo | null>(null);

  async function fetchInfo() {
    try {
      const data = await invoke<FullSystemInfo>("get_system_info");
      setInfo(data);
    } catch (e) {
      console.error(e);
    }
  }

  onMount(() => {
    fetchInfo();
    const timer = setInterval(fetchInfo, 1000);
    onCleanup(() => clearInterval(timer));
  });

  return (
    <main class="dashboard">
      <header>
        <h1>System Monitor</h1>
        <div class="status-dot"></div>
      </header>

      <Show when={info()} fallback={<div class="loading">Initializing Sensors...</div>}>
        <div class="grid-layout">
          <div class="card system-card">
            <h2>OS Information</h2>
            <div class="info-row">
              <span class="label">Host</span>
              <span class="value">{info()?.system_info.host_name}</span>
            </div>
            <div class="info-row">
              <span class="label">OS</span>
              <span class="value">{info()?.system_info.os_version}</span>
            </div>
            <div class="info-row">
              <span class="label">Kernel</span>
              <span class="value">{info()?.system_info.kernel_version}</span>
            </div>
          </div>

          <div class="card cpu-card">
            <h2>CPU</h2>
            <div class="big-stat">
              {info()?.cpu_info.nb_cpus} <span class="sub">Cores</span>
            </div>
            <ProgressBar
              label="Global Load"
              value={info()?.cpu_info.global_usage || 0}
              max={100}
              unit="%"
            />
          </div>

          <div class="card memory-card">
            <h2>Memory</h2>
            <ProgressBar
              label="RAM"
              value={info()?.memory_info.used_memory || 0}
              max={info()?.memory_info.total_memory || 1}
            />
            <div class="spacer"></div>
            <ProgressBar
              label="Swap"
              value={info()?.swap_info.used_swap || 0}
              max={info()?.swap_info.total_swap || 1}
            />
          </div>

          <div class="card net-card">
            <h2>Network Interfaces</h2>
            <div class="tags">
              {info()?.network_info.networks.map((net) => (
                <span class="tag">{net}</span>
              ))}
            </div>
          </div>
        </div>
      </Show>
    </main>
  );
}

export default App;