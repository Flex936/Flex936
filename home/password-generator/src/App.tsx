import { createSignal } from "solid-js";
import { invoke } from "@tauri-apps/api/core";
import "./App.css";

function App() {
  const [greetMsg, setGreetMsg] = createSignal("");
  const [requirements, setRequirements] = createSignal({
    symbols: true,
    uppercase: true,
    numbers: true,
    length: 0
  });

  async function generatePassword() {
    setGreetMsg(await invoke("generate_password", { req: requirements() }));
  }


  return (
    <main class="container">

      <form
        class="row"
        onSubmit={(e) => {
          e.preventDefault();
          generatePassword();
        }}
      >
        <input
          id="greet-input"
          onChange={(e) => setRequirements(prev => ({ ...prev, length: Number(e.currentTarget.value) }))}
          placeholder="Enter desired length..."
        />
        <div class="checkbox">
          <input type="checkbox" name="uppercase" id="uppercase" checked={requirements().uppercase} onChange={(e) => setRequirements(prev => ({ ...prev, uppercase: e.currentTarget.checked }))} />
          <label for="uppercase">Contain uppercase</label>
        </div>
        <div class="checkbox">
          <input type="checkbox" name="numbers" id="numbers" checked={requirements().numbers} onChange={(e) => setRequirements(prev => ({ ...prev, numbers: e.currentTarget.checked }))} />
          <label for="numbers">Contain numbers</label>
        </div>
        <div class="checkbox">
          <input type="checkbox" name="symbols" id="symbols" checked={requirements().symbols} onChange={(e) => setRequirements(prev => ({ ...prev, symbols: e.currentTarget.checked }))} />
          <label for="symbols">Contain symbols</label>
        </div>
        <button type="submit">Generate</button>
      </form>
      <p>{greetMsg()}</p>
    </main>
  );
}

export default App;
