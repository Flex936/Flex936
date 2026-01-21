import { createSignal, createEffect } from "solid-js";
import { invoke } from "@tauri-apps/api/core";
import "./App.css";

function App() {
  const [password, setPassword] = createSignal("");
  const [isCopied, setIsCopied] = createSignal(false);

  // Default length 16 is better for security
  const [requirements, setRequirements] = createSignal({
    symbols: true,
    uppercase: true,
    numbers: true,
    length: 16
  });

  async function generatePassword() {
    try {
      const result = await invoke<string>("generate_password", { req: requirements() });
      setPassword(result);
      setIsCopied(false); // Reset copy status on new generation
    } catch (error) {
      console.error(error);
    }
  }

  // Generate on mount so the box isn't empty
  createEffect(() => {
    generatePassword();
  });

  const copyToClipboard = async () => {
    if (!password()) return;
    await navigator.clipboard.writeText(password());
    setIsCopied(true);
    setTimeout(() => setIsCopied(false), 2000);
  };

  // Simple heuristic for strength color
  const getStrengthColor = () => {
    const len = requirements().length;
    if (len < 8) return "var(--danger)";
    if (len < 12) return "var(--warning)";
    return "var(--success)";
  };

  return (
    <main class="container">
      <div class="card">
        <h1>PassGen</h1>

        {/* Hero Section: Password Display */}
        <div class="result-container">
          <div class="password-display">
            {password()}
          </div>
          <button
            class={`copy-btn ${isCopied() ? "copied" : ""}`}
            onClick={copyToClipboard}
            title="Copy to clipboard"
          >
            {isCopied() ? (
              // Checkmark Icon
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polyline points="20 6 9 17 4 12"></polyline>
              </svg>
            ) : (
              // Copy Icon
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect>
                <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path>
              </svg>
            )}
          </button>
        </div>

        {/* Visual Strength Bar */}
        <div class="strength-bar-container">
          <div
            class="strength-bar"
            style={{
              width: `${Math.min(requirements().length * 5, 100)}%`,
              "background-color": getStrengthColor()
            }}
          />
        </div>

        {/* Controls Section */}
        <form
          class="controls"
          onSubmit={(e) => {
            e.preventDefault();
            generatePassword();
          }}
        >
          {/* Length Slider & Input */}
          <div class="control-group">
            <label class="group-label">Length: {requirements().length}</label>
            <div class="slider-row">
              <input
                type="range"
                min="4"
                max="64"
                value={requirements().length}
                onInput={(e) => setRequirements(prev => ({ ...prev, length: Number(e.currentTarget.value) }))}
              />
            </div>
          </div>

          {/* Toggles Grid */}
          <div class="toggles-grid">
            <label class="toggle-row">
              <span>Uppercase (A-Z)</span>
              <input
                type="checkbox"
                checked={requirements().uppercase}
                onChange={(e) => setRequirements(prev => ({ ...prev, uppercase: e.currentTarget.checked }))}
              />
              <span class="toggle-switch"></span>
            </label>

            <label class="toggle-row">
              <span>Numbers (0-9)</span>
              <input
                type="checkbox"
                checked={requirements().numbers}
                onChange={(e) => setRequirements(prev => ({ ...prev, numbers: e.currentTarget.checked }))}
              />
              <span class="toggle-switch"></span>
            </label>

            <label class="toggle-row">
              <span>Symbols (!@#)</span>
              <input
                type="checkbox"
                checked={requirements().symbols}
                onChange={(e) => setRequirements(prev => ({ ...prev, symbols: e.currentTarget.checked }))}
              />
              <span class="toggle-switch"></span>
            </label>
          </div>

          <button type="submit" class="generate-btn">
            Generate New Password
          </button>
        </form>
      </div>
    </main>
  );
}

export default App;