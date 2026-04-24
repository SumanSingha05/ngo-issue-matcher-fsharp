import { useState } from "react";


function App() {
  const [category, setCategory] = useState("");
  const [result, setResult] = useState([]);

  const submit = async () => {
    const res = await fetch("http://localhost:5138/match", {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        id: 1,
        category: category,
        description: "test"
      })
    });

    const data = await res.json();

    console.log("API RESPONSE:", data);
    setResult(data);
  };

  return (
    <div style={{ padding: "20px" }}>
      <h2>NGO Matcher</h2>

      <input
        placeholder="Enter category (water, education...)"
        onChange={(e) => setCategory(e.target.value)}
      />

      <button onClick={submit}>Submit</button>

      <h3>Results:</h3>

      {result.map((item, i) => (
        <p key={i}>
          {item.name} - {item.score}
        </p>
      ))}
    </div>
  );
}


export default App;