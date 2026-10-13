# Quickstart: Name Greeting Page

**Feature**: [spec.md](./spec.md) | **API**: [contracts/greeting-api.md](./contracts/greeting-api.md)

## Prerequisites

- .NET SDK 8.0 or newer (`dotnet --list-sdks`)
- ASP.NET Core runtime 8.0 (`dotnet --list-runtimes` shows `Microsoft.AspNetCore.App 8.0.x`)
- Any modern web browser

All commands run from the repository root.

## Run the automated tests

```bash
dotnet test
```

**Expected**: all tests pass (validator unit tests + API/page integration tests).

## Run the app

```bash
dotnet run --project src/NameGreeting
```

**Expected**: console prints `Now listening on: http://localhost:<port>`. Open that URL.

## Try the API directly (optional)

In a second terminal (replace `<port>`):

```bash
curl -X POST http://localhost:<port>/api/greeting -H "Content-Type: application/json" -d "{\"name\":\"  Ada  \"}"
```

**Expected**: `{"name":"Ada"}`

## Manual validation checklist (browser)

These cover browser-only behavior that the automated tests do not (permitted by constitution
v1.1.0, Principle IV).

| # | Do this | Expect | Spec |
|---|---------|--------|------|
| 1 | Type `Ada`, click the button | "Hello, Ada!" shown; input is empty | US1-1, FR-004, FR-005 |
| 2 | Type `Grace`, press **Enter** | Greeting replaced by "Hello, Grace!"; input empty | US1-2, US1-3, FR-006, FR-010 |
| 3 | Type `  Zoë  `, submit | "Hello, Zoë!" (no extra spaces) | Edge cases |
| 4 | Type `<b>Bob</b>`, submit | Shows `Hello, <b>Bob</b>!` literally — not bold | FR-009, SC-005 |
| 5 | Leave input empty, submit | "Please enter your name."; no greeting | US2-1 |
| 6 | Paste 101 characters, submit | "Name must be 100 characters or fewer."; text stays in input | US2-2, FR-008 |
| 7 | Submit a valid name after step 5 or 6 | Error replaced by the greeting | Edge cases, FR-010 |
| 8 | Stop the server (Ctrl+C), submit `Ada` | "Sorry, the greeting could not be retrieved…"; `Ada` stays in input | US2-3, FR-008 |
| 9 | In browser DevTools → Network, set throttling to "Slow 3G", submit and click repeatedly | Button is greyed out while waiting; only one request appears in the Network tab | FR-012 |
| 10 | Refresh the page | Empty input, no greeting | Assumptions |
