<div align="center">

# NGO Issue Matcher

**Match social issues to the NGOs fighting them — powered by F# and functional programming**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![F#](https://img.shields.io/badge/F%23-functional-378BBA?style=flat-square&logo=fsharp&logoColor=white)](https://fsharp.org/)
[![React](https://img.shields.io/badge/React-Vite-61DAFB?style=flat-square&logo=react&logoColor=black)](https://vitejs.dev/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)


<br/>

> A full-stack web app where users type a social issue  like `"water"`, `"education"`, or `"health"` and get back a ranked list of NGOs working on that cause.
>
> **Built as a first F# project** to learn functional programming through a real, end-to-end application.

<br/>

[Getting Started](#-getting-started) · [How It Works](#-how-it-works) · [Why F#?](#-why-f) · [F# vs C#](#-f-vs-c--the-real-differences) · [Challenges](#-challenges-i-faced)

</div>

---

## 📸 Demo

```
User types:  "education"

Response:
┌─────────────────────────────────────────────────────────────┐
│  Score: 10  │  Teach For All   │  Education, youth literacy  │
│  Score: 10  │  Room to Read    │  Education, girls schooling │
│  Score:  1  │  WaterAid        │  Clean water access         │
│  Score:  1  │  MSF             │  Emergency medical care     │
└─────────────────────────────────────────────────────────────┘
```

---

## 📌 Table of Contents

- [What It Does](#-what-it-does)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [How It Works](#-how-it-works)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [API Reference](#-api-reference)
- [Why F#?](#-why-f)
- [F# Features Used](#-f-features-used-in-this-project)
- [F# vs C# - The Real Differences](#-f-vs-c--the-real-differences)
- [Challenges I Faced](#-challenges-i-faced)
- [Current Limitations & Roadmap](#-current-limitations--roadmap)
- [What I Learned](#-what-i-learned)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🔍 What It Does

1. **User enters** a social issue keyword (e.g. `"water"`, `"health"`, `"climate"`)
2. **React frontend** sends the input to the backend via `POST /match`
3. **F# backend** scores each NGO in the database by relevance
4. **Ranked results** are returned as JSON
5. **Frontend renders** the matched NGOs with their scores

Simple premise. Real architecture. Built to learn.

---

## 🛠 Tech Stack

| Layer | Technology | Why |
|---|---|---|
| Frontend | React + Vite | Fast dev server, modern JSX |
| Backend | F# + ASP.NET Core | Functional-first, full .NET ecosystem |
| API | REST `POST /match` | Simple, stateless, easy to test |
| Language paradigm | Functional | Immutability, pipelines, no side effects |

---

## 🏗 Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                        BROWSER                               │
│                                                              │
│   ┌─────────────────────────────────────┐                    │
│   │          React (Vite)               │                    │
│   │                                     │                    │
│   │   [Input Field]  →  [Search Button] │                    │
│   │         ↓                           │                    │
│   │   fetch("POST /match", issue)       │                    │
│   │         ↓                           │                    │
│   │   [NGO Results List]                │                    │
│   └──────────────┬──────────────────────┘                    │
└──────────────────┼───────────────────────────────────────────┘
                   │ HTTP (JSON)
                   │ POST /match
                   ↓
┌──────────────────────────────────────────────────────────────┐
│                   F# ASP.NET CORE BACKEND                    │
│                                                              │
│   Program.fs          Models.fs          Matcher.fs          │
│   ┌───────────┐      ┌───────────┐      ┌─────────────────┐  │
│   │  Routing  │ ───► │ NGO type  │ ───► │  scoreMatch     │  │
│   │  CORS     │      │Issue type │      │  matchNGOs      │  │
│   │  Startup  │      └───────────┘      │  (pipeline |>)  │  │
│   └───────────┘                         └─────────────────┘  │
│                                                 ↓             │
│                                   Ranked NGO list (JSON)      │
└──────────────────────────────────────────────────────────────┘
```

**Data flow:**

```
React input  →  POST /match  →  F# deserializes JSON
                             →  scoreMatch runs per NGO
                             →  List.sortByDescending score
                             →  JSON response
                             →  React renders results
```

---

## ⚙️ How It Works

### The Matching Logic (Core of the App)

The heart of the backend lives in `Matcher.fs`:

```fsharp
// Score a single NGO against the user's issue
let scoreMatch (issue: string) (ngo: NGO) =
    if ngo.Category.ToLower() = issue.ToLower() then 10
    else 1

// Score all NGOs and return them sorted by score
let matchNGOs (issue: string) (ngos: NGO list) =
    ngos
    |> List.map (fun ngo -> (ngo, scoreMatch issue ngo))    // pair each NGO with its score
    |> List.sortByDescending (fun (_, score) -> score)       // sort highest score first
```

Notice the `|>` operator. This is F#'s **pipeline operator** — it passes the result of one expression directly into the next function. The code reads like a sentence: *"take the list, map each one to a score, then sort descending."* No nesting, no intermediate variables.

### Data Models

```fsharp
// Models.fs
type NGO = {
    Name: string
    Category: string
    Description: string
}

type Issue = {
    IssueText: string
}
```

Types in F# are immutable records by default. No constructors. No getters/setters. No `public` keyword. Just clean, named data shapes.

### API Endpoint

```fsharp
// Program.fs
app.MapPost("/match", fun (issue: Issue) ->
    let results = Matcher.matchNGOs issue.IssueText ngoDatabase
    Results.Ok(results)
)
```

One endpoint. One function. No class needed.

---

## 📁 Project Structure

```
ngo-issue-matcher/
│
├── backend/
│   ├── Models.fs           # NGO and Issue type definitions
│   ├── Matcher.fs          # Core matching and scoring logic
│   ├── Program.fs          # App startup, routing, CORS config
│   └── backend.fsproj      # Project file — FILE ORDER MATTERS in F#
│
├── frontend/
│   ├── src/
│   │   ├── App.jsx         # Main React component + fetch logic
│   │   └── main.jsx        # Entry point
│   ├── index.html
│   └── vite.config.js      # Proxy config to avoid CORS in dev
│
└── README.md
```

> ⚠️ **Critical F# note:** File order in `.fsproj` matters. `Models.fs` must appear before `Matcher.fs` because F# compiles strictly top-to-bottom. This is very different from C#.

---

## Getting Started

### Prerequisites

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Git](https://git-scm.com/)

### Clone the Repo

```bash
git clone https://github.com/YOUR_USERNAME/ngo-issue-matcher.git
cd ngo-issue-matcher
```

### Run the Backend

```bash
cd backend
dotnet restore
dotnet run
# Starts at http://localhost:5000
```

### Run the Frontend

```bash
cd frontend
npm install
npm run dev
# Starts at http://localhost:5173
```

Open `http://localhost:5173` in your browser.

---

## 📡 API Reference

### `POST /match`

Match an issue keyword to relevant NGOs.

**Request Body**
```json
{
  "issueText": "education"
}
```

**Response** `200 OK`
```json
[
  {
    "item1": {
      "name": "Room to Read",
      "category": "education",
      "description": "Focuses on literacy and girls' education"
    },
    "item2": 10
  },
  {
    "item1": {
      "name": "WaterAid",
      "category": "water",
      "description": "Clean water access in developing countries"
    },
    "item2": 1
  }
]
```

> **Note:** F# tuples serialize to `item1`/`item2` in JSON by default. Mapping to named DTOs is on the roadmap.

**Test with curl:**
```bash
curl -X POST http://localhost:5000/match \
  -H "Content-Type: application/json" \
  -d '{"issueText": "water"}'
```

---

## 🤔 Why F#?

Most backend tutorials default to C#, Java, or Node.js. F# was a deliberate choice:

**1. Learn functional programming for real** - not through toy exercises, but by building something end-to-end with real HTTP requests, real data, and real debugging.

**2. Immutability by default** - in F#, values don't change unless you explicitly ask them to. This eliminates entire classes of bugs caused by accidental mutation or shared mutable state.

**3. Less code, same power** - the matching logic that would require a class, constructor, properties, and LINQ chains in C# is 5 clean lines in F#.

**4. Full .NET ecosystem** - F# runs on the same runtime as C#. It has full access to ASP.NET Core, Entity Framework, NuGet, and every .NET library. You get functional programming without giving anything up.

**5. Transferable skills** - immutability, pure functions, pipelines, and `map`/`filter`/`reduce` are universal ideas. Learning them in F# makes you better at TypeScript, Scala, Rust, and even modern C#.

---

## 🔬 F# Features Used in This Project

### 1. Record Types (Immutable by Default)

```fsharp
type NGO = {
    Name: string
    Category: string
    Description: string
}
```

No class. No `new`. No constructor. No `public` keyword. Records are immutable by default, you can't accidentally change a field after creation. To "update" a record you create a copy with `{ ngo with Name = "New Name" }`.

### 2. The Pipeline Operator `|>`

```fsharp
ngos
|> List.map (fun ngo -> (ngo, scoreMatch issue ngo))
|> List.sortByDescending (fun (_, score) -> score)
```

`|>` passes the result on the left as the last argument to the function on the right. Instead of deeply nested calls like `Sort(Map(ngos, fn), fn2)`, you get a readable top-to-bottom pipeline. Each step is clear and testable on its own.

### 3. `fun` - Anonymous Functions (Lambdas)

```fsharp
List.map (fun ngo -> (ngo, scoreMatch issue ngo))
```

`fun` is F#'s lambda syntax. Equivalent to `ngo => (ngo, scoreMatch(issue, ngo))` in C#. Used wherever you need a short, unnamed function inline.

### 4. `List.map` / `List.sortByDescending`

F# lists are immutable. Instead of mutating in place, every operation returns a new list:

```fsharp
List.map               // transform every element → new list
List.filter            // keep elements matching a predicate → new list
List.sortByDescending  // sort into a new list, highest first
```

The original list is never touched. This makes functions predictable and easy to test.

### 5. Tuple Destructuring

```fsharp
fun (ngo, score) -> score
//   ^^^^^^^^^^^  destructured directly in the parameter
```

Tuples are a built-in F# type. You can unpack them directly in function parameters, `let` bindings, and pattern matches no intermediate variable needed.

### 6. Strong Type Inference

```fsharp
let scoreMatch issue ngo =   // no type annotations written
    if ngo.Category = issue then 10
    else 1
```

F# infers that `issue` is a `string` and `ngo` is an `NGO` from how they're used. You get full static typing without writing most of the annotations. The compiler catches type errors at compile time, not at runtime.

---

## ⚔️ F# vs C# - The Real Differences

Both run on .NET. They can be used together in the same solution. But they have very different philosophies about how code should be written.

| Feature | F# | C# |
|---|---|---|
| **Primary paradigm** | Functional-first | Object-oriented first |
| **Immutability** | On by default | Opt-in (`readonly`, `init`) |
| **Null safety** | Uses `Option<T>` - no null by default | Nullable everywhere |
| **Syntax** | Concise, indentation-based (like Python) | Verbose, uses `{}` braces |
| **Classes** | Rarely needed | Core building block |
| **Loops** | Prefer recursion + `List.map` | `for`, `while`, `foreach` |
| **Type inference** | Very strong - rarely annotate | Moderate (`var` helps) |
| **Boilerplate** | Very low | Higher (constructors, properties) |
| **File order** | Strictly matters top-down compilation | Doesn't matter |
| **Error handling** | `Result<T, E>` type (explicit success/fail) | Exceptions (`try/catch`) |
| **Pattern matching** | First-class, exhaustive by compiler | Available, but not enforced |
| **Interop** | Full .NET interop with C# | Full .NET interop with F# |

### Same logic both languages:

**C# (object-oriented style):**
```csharp
public class NgoMatcher
{
    public List<(NGO Ngo, int Score)> MatchNGOs(string issue, List<NGO> ngos)
    {
        return ngos
            .Select(ngo => (Ngo: ngo, Score: ngo.Category.ToLower() == issue.ToLower() ? 10 : 1))
            .OrderByDescending(x => x.Score)
            .ToList();
    }
}
```

**F# (functional style):**
```fsharp
let matchNGOs issue ngos =
    ngos
    |> List.map (fun ngo -> ngo, if ngo.Category = issue then 10 else 1)
    |> List.sortByDescending snd
```

Same result. F# version has no class, no constructor, no return type, no `.ToList()`. It just expresses the transformation directly.

> **The bottom line:** C# is more familiar to most developers. F# is more concise, enforces better habits around immutability, and is excellent for data transformation logic. They're not rivals — they're complementary tools in the same ecosystem.

---

## 🧱 Challenges I Faced

These are real problems I hit while building this not things from a tutorial.

### 1. F# File Order in `.fsproj` Broke Compilation

F# compiles files in the exact order listed in the `.fsproj` file. There is no implicit resolution. The first time I tried to use the `NGO` type in `Matcher.fs` before it was defined in `Models.fs`, I got a compiler error that made no sense until I understood how F# works.

```xml
<!-- This order WORKS -->
<Compile Include="Models.fs" />    <!-- defines NGO type -->
<Compile Include="Matcher.fs" />   <!-- uses NGO type -->
<Compile Include="Program.fs" />   <!-- uses matchNGOs function -->

<!-- This order BREAKS -->
<Compile Include="Matcher.fs" />   <!-- ❌ NGO type doesn't exist yet -->
<Compile Include="Models.fs" />
```

This enforced thinking about dependency order in a way that C# never had. It's actually a feature once you understand it.

### 2. Indentation Errors That Were Hard to Find

F# uses significant whitespace like Python. A tab where spaces were expected caused cryptic errors, and worse - the compiler often pointed to the line *after* the bad indentation, not the actual problem. I learned to always use spaces, never tabs, and to be consistent within `let` bindings.

### 3. Tuple Serialization Produced Ugly JSON

F# tuples are a first-class language type, but `System.Text.Json` doesn't know how to serialize them cleanly. When I returned `(ngo, score)` from the endpoint, it came out as:

```json
{ "item1": { ... }, "item2": 10 }
```

Instead of the clean `{ "ngo": { ... }, "score": 10 }` I expected. The correct fix is to map tuples into named record types before returning them from the endpoint — still on the roadmap.

### 4. `Ok` vs `ok` - One Character, Total Failure

F# discriminated unions are case-sensitive. The success case of the `Result` type is `Ok` with a capital O. I typed `ok` (lowercase) and got a type error I couldn't understand for nearly half an hour. F# has zero tolerance for casing on union cases.

### 5. CORS Blocked Every Frontend Request

The React app runs on port `5173`. The F# API runs on port `5000`. Browsers block cross-origin requests by default. Every `fetch()` call silently failed until I configured two things:

**In `Program.fs` (backend):**
```fsharp
builder.Services.AddCors(fun options ->
    options.AddPolicy("AllowAll", fun policy ->
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader() |> ignore))

app.UseCors("AllowAll")
```

**In `vite.config.js` (frontend dev server):**
```js
server: {
  proxy: {
    '/match': 'http://localhost:5000'
  }
}
```

### 6. Namespace / Module Not Found Errors

F# has a strict module system. Using a function from another file requires either `open Matcher` at the top of the file, or fully qualifying it as `Matcher.matchNGOs`. Forgetting `open` caused "identifier not defined" errors that looked like the file wasn't compiling at all — even when it was.

---

---

## 📖 Official Resources & Further Reading

This project was built by referring to the following official documentation. If you want to go deeper on any part of the stack, these are the best places to start:

### Core Technologies

| Resource | URL |
|---|---|
| **.NET Documentation** | https://learn.microsoft.com/en-us/dotnet/ |
| **F# Official Docs** | https://learn.microsoft.com/en-us/dotnet/fsharp/ |
| **ASP.NET Core Web API** | https://learn.microsoft.com/en-us/aspnet/core/web-api/ |

### F# Learning Path 

| Resource | What You'll Learn |
|---|---|
| [F# Tour](https://learn.microsoft.com/en-us/dotnet/fsharp/tour) | Quick overview of all major F# features |
| [F# for Fun and Profit](https://fsharpforfunandprofit.com/) | Deep, practical F# — best free resource available |
| [F# Cheatsheet](https://dungpa.github.io/fsharp-cheatsheet/) | Quick syntax reference |
| [Exercism F# Track](https://exercism.org/tracks/fsharp) | Hands-on practice with community feedback |

### ASP.NET Core

| Resource | What You'll Learn |
|---|---|
| [Minimal APIs Overview](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis) | The approach used in this project |
| [CORS in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/cors) | Why CORS matters and how to configure it |
| [JSON Serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview) | Fix the `item1`/`item2` tuple issue |


---

> 💡 **Tip for beginners:** Start with the [F# Tour](https://learn.microsoft.com/en-us/dotnet/fsharp/tour) to get the lay of the land, then work through [F# for Fun and Profit](https://fsharpforfunandprofit.com/) alongside building your own project. Reading docs is most effective when you have a real problem to solve.
---

## 📚 What I Learned

**Backend concepts:**
- Designing a REST API from scratch - routing, request/response lifecycle, status codes
- How JSON serialization works and where it fails (F# tuples)
- ASP.NET Core startup pipeline - builder, middleware, and app

**F# / Functional programming:**
- Immutability as a default, not an afterthought
- Writing transformations as pipelines with `|>`
- `List.map`, `List.filter`, `List.sortByDescending`
- Type inference - how the compiler figures out types from context
- Why file order matters and how to structure a compiled F# project
- Anonymous functions with `fun`

**Real-world debugging:**
- Reading compiler errors carefully - F# errors often point at the symptom, not the cause
- Isolating frontend vs backend problems methodically
- Fixing one thing at a time - changing multiple things at once makes it impossible to know what worked

---

## 👤 Author

Built by a developer exploring F# and functional programming for the first time through a real full-stack project.


---

<div align="center">

⭐ If this helped you understand F# or full-stack architecture, a star means a lot.

</div>
