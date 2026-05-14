# Internship Application Tracker

A small Windows desktop app that tracks the internship applications I've sent: company, position, date, status and notes. Everything is saved to a local JSON file — no database, no server.

Built with **C# and Windows Forms** (.NET 8). The whole project is a few files long on purpose: it's meant to be a clean, readable v1 — not a CV-padding monster.

## Features

- Add an application with company, position, date, status and notes
- Update or delete the selected application
- Filter the list by status (Applied / Interview / Rejected / Accepted)
- Auto-saves to `applications.json` after every change
- Reloads `applications.json` on startup if it exists
- Sorts the grid by date, newest first

## Technologies

- C# (.NET 8)
- Windows Forms
- `System.Text.Json` for serialization
- No external NuGet packages

## How to run

You need **.NET 8 SDK** on Windows.

```bash
git clone https://github.com/Gonulaldek/internship-application-tracker.git
cd internship-application-tracker
dotnet run
```

Or open `InternshipApplicationTracker.csproj` in Visual Studio 2022 and press F5.

The app will create `applications.json` next to the executable on the first save.

## Project structure

```
InternshipApplicationTracker/
├── ApplicationEntry.cs           # data model (POCO)
├── ApplicationStatus.cs          # status constants + combo box source arrays
├── ApplicationStorage.cs         # JSON save / load
├── Form1.cs                      # UI built in code + event handlers
├── Program.cs                    # WinForms entry point
├── InternshipApplicationTracker.csproj
├── README.md
└── .gitignore
```

## How it works

- `Form1` holds a `List<ApplicationEntry>` as the source of truth.
- The `DataGridView` is bound to a `BindingList<ApplicationEntry>` that wraps either the full list or a filtered view.
- The grid rows hold references to the same entry objects, so "Update Selected" mutates the real entry in place.
- After every Add / Update / Delete the list is written to disk by `ApplicationStorage.Save`.
- On startup `ApplicationStorage.Load` reads `applications.json` (if it exists) and fills the list before the grid is built.

## What I learned

- Writing a Windows Forms UI in code (no designer file) — every control is created and laid out explicitly, which made the form much easier to read in code review than auto-generated `.Designer.cs` output.
- Using `System.Text.Json` for round-tripping a list of POCOs to disk.
- Keeping a single source of truth (`_applications`) and rebinding the grid through a `BindingList<T>` view, instead of trying to keep two collections in sync.
- Separating concerns: model (`ApplicationEntry`), storage (`ApplicationStorage`), constants (`ApplicationStatus`), UI (`Form1`).

## Current limitations

- Single user, single machine — there is no login, no cloud sync.
- No search box; only status filtering.
- No edit history / undo.
- If `applications.json` becomes corrupt the app shows a warning and starts with an empty list (it does not auto-backup the file).
- No unit tests yet.

## Planned improvements

- Search by company / position
- Backup `applications.json` to `applications.json.bak` before each save
- Add a "Days since applied" column
- Export to CSV
- Light unit tests for `ApplicationStorage` (round-trip, missing file, bad JSON)

## Security / data note

`applications.json` is written in plain text next to the executable. The notes field is stored as-is. Don't put passwords or any other secrets in there. The file is in `.gitignore` so it won't be committed by accident — your application history stays on your machine.

---

**Author:** Melih Gönülal · [github.com/Gonulaldek](https://github.com/Gonulaldek)
