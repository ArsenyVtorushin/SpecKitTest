---

description: "Task list for the Name Greeting Page feature"
---

# Tasks: Name Greeting Page

**Input**: Design documents from `/specs/001-name-greeting/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/greeting-api.md, quickstart.md

**Tests**: Included — required by Constitution Principle IV (every endpoint needs a success test
and an invalid-input test). Browser-only behaviors are verified with the manual checklist in
quickstart.md, as permitted by constitution v1.1.0 Principle IV.

**Organization**: Tasks are grouped by user story so each story can be implemented and tested
independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1, US2)
- All paths are relative to the repository root

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the solution and the two projects described in plan.md → Project Structure

- [X] T001 Create `src/NameGreeting/NameGreeting.csproj` using `Sdk="Microsoft.NET.Sdk.Web"` with `TargetFramework` `net8.0`, `Nullable` `enable`, `ImplicitUsings` `enable`, and **no** `PackageReference` items; create `NameGreeting.sln` at the repository root and add the project to it
- [X] T002 Create `tests/NameGreeting.Tests/NameGreeting.Tests.csproj` (`Sdk="Microsoft.NET.Sdk"`, `net8.0`, `Nullable` `enable`, `ImplicitUsings` `enable`, `IsPackable` `false`) with package references `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `Microsoft.AspNetCore.Mvc.Testing` (8.0.x), a global `using Xunit;`, and a `ProjectReference` to `src/NameGreeting/NameGreeting.csproj`; do NOT include `coverlet.collector`; add the project to `NameGreeting.sln`
- [X] T003 [P] Create `.gitignore` at the repository root ignoring `bin/`, `obj/`, `.vs/`, `*.user`, `TestResults/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: A running app that serves the (not yet interactive) page, plus a passing test harness

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T004 Create `src/NameGreeting/Program.cs`: build the app, call `app.UseDefaultFiles()` then `app.UseStaticFiles()` (comment why the order matters: default-files rewrites `/` to `/index.html` before static-files serves it), `app.Run()`, and end the file with `public partial class Program { }` plus a comment explaining it lets `WebApplicationFactory<Program>` in tests find the app
- [X] T005 [P] Create `src/NameGreeting/wwwroot/index.html`: `<form id="greeting-form">` containing `<label for="name-input">Your name</label>`, `<input id="name-input" type="text" autocomplete="off">` (do NOT add `required` or `maxlength` attributes — validation is server-only per FR-007), and `<button id="submit-button" type="submit">Greet me</button>`; below the form `<p id="result" aria-live="polite"></p>`; a small inline `<style>` block (centered column, readable font, distinct style for `#result.error`); `<script src="app.js" defer></script>`; `<title>Name Greeting</title>`; `<meta charset="utf-8">` and `<meta name="viewport" content="width=device-width, initial-scale=1">` so the page is usable on small screens
- [X] T006 [P] Create `tests/NameGreeting.Tests/HomePageTests.cs` using `IClassFixture<WebApplicationFactory<Program>>`: test that `GET /` returns 200, `Content-Type` `text/html`, and the body contains `id="greeting-form"`, `id="name-input"`, `id="submit-button"`, and `aria-live="polite"` (contract: `GET /` in contracts/greeting-api.md)
- [X] T007 Run `dotnet build` and `dotnet test` from the repository root; both must succeed with the HomePageTests passing

**Checkpoint**: `dotnet run --project src/NameGreeting` shows the static form in a browser

---

## Phase 3: User Story 1 - Greet the visitor by name (Priority: P1) 🎯 MVP

**Goal**: Visitor types a name, submits (button or Enter), sees "Hello, {name}!" from the
server's reply, and the input clears

**Independent Test**: Open the page, enter "Ada", press the button → "Hello, Ada!" is shown and
the input is empty (quickstart.md checklist rows 1–4, 9, 10)

### Tests for User Story 1 ⚠️

> Write these first and confirm they fail (compile errors count as failing) before implementing

- [X] T008 [P] [US1] Create `tests/NameGreeting.Tests/NameValidatorTests.cs` with valid-name cases for `NameValidator.Validate`: `"Ada"`→`"Ada"`, `"  Ada  "`→`"Ada"`, and unchanged return for `"Zoë"`, `"O'Brien"`, `"Jean-Luc"`, `"Анна"`, `"<b>Bob</b>"`; a name of exactly 100 `a` characters is valid; assert `IsValid` is true and `Error` is null in each case (use `[Theory]`/`[InlineData]`)
- [X] T009 [P] [US1] Create `tests/NameGreeting.Tests/GreetingEndpointTests.cs` using `IClassFixture<WebApplicationFactory<Program>>`: `POST /api/greeting` with `{"name":"  Ada  "}` returns 200 and JSON `{"name":"Ada"}` (assert the raw JSON uses the camelCase property `name`); `{"name":"<b>Bob</b>"}` returns `{"name":"<b>Bob</b>"}` unchanged; `{"name":"Zoë"}` returns `{"name":"Zoë"}` (examples table in contracts/greeting-api.md)
- [X] T010 [P] [US1] Add a test to `tests/NameGreeting.Tests/HomePageTests.cs` that `GET /app.js` returns 200 with a JavaScript content type

### Implementation for User Story 1

- [X] T011 [US1] Create `src/NameGreeting/NameValidator.cs`: `public record NameValidationResult(string? CleanedName, string? Error)` with a computed `bool IsValid => Error is null`, and `public static class NameValidator` with `public static NameValidationResult Validate(string? rawName)` that trims leading/trailing whitespace (treat `null` as `""`) and returns the cleaned name as valid (rejection rules come in US2)
- [X] T012 [US1] In `src/NameGreeting/Program.cs` add `record GreetingRequest(string? Name)`, `record GreetingResponse(string Name)`, `record ErrorResponse(string Error)` and `app.MapPost("/api/greeting", ...)` that calls `NameValidator.Validate(request.Name)` and returns `Results.Ok(new GreetingResponse(cleanedName))` when valid, otherwise `Results.BadRequest(new ErrorResponse(error))`; map the endpoint before `app.Run()`
- [X] T013 [US1] Create `src/NameGreeting/wwwroot/app.js`: listen for `submit` on `#greeting-form` (covers button click and Enter, FR-006); call `event.preventDefault()`; if a request is already in progress return early; set in-progress flag and `submitButton.disabled = true` (FR-012); `fetch("/api/greeting", { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ name: nameInput.value }) })`; on `response.ok` set `result.textContent = "Hello, " + data.name + "!"` using the name **from the server response** (FR-002, FR-004), never `innerHTML` (FR-009), remove the `error` class, and clear `nameInput.value` (FR-005); in a `finally` block clear the flag and re-enable the button; add short "why" comments
- [X] T014 [US1] Run `dotnet test` (all US1 tests pass), then run the app and walk quickstart.md checklist rows 1, 2, 3, 4, 9, 10

**Checkpoint**: MVP complete — greeting works end-to-end via the REST API

---

## Phase 4: User Story 2 - Helpful feedback for invalid input or failures (Priority: P2)

**Goal**: Empty, whitespace-only, or too-long names and server failures produce a clear message,
no greeting, and the typed text stays in the input

**Independent Test**: Submit an empty field → "Please enter your name." and no greeting; stop the
server and submit "Ada" → retrieval error shown and "Ada" remains in the input
(quickstart.md checklist rows 5–8)

### Tests for User Story 2 ⚠️

- [X] T015 [P] [US2] Add invalid-name cases to `tests/NameGreeting.Tests/NameValidatorTests.cs`: `null`, `""`, `"   "`, `"\t\n"` → `IsValid` false with `Error` `"Please enter your name."`; 101 `a` characters → `Error` `"Name must be 100 characters or fewer."`; 100 copies of `"é"` (e + combining accent, 100 user-perceived characters) is **valid**; 101 copies is invalid (research.md R4: count text elements)
- [X] T016 [P] [US2] Add invalid-input tests to `tests/NameGreeting.Tests/GreetingEndpointTests.cs`: `{"name":""}`, `{"name":"   "}`, and `{}` → 400 with `{"error":"Please enter your name."}`; 101 × `a` → 400 with `{"error":"Name must be 100 characters or fewer."}`; body `not json` with `Content-Type: application/json` → 400 (status only)

### Implementation for User Story 2

- [X] T017 [US2] Extend `NameValidator.Validate` in `src/NameGreeting/NameValidator.cs` per data-model.md validation table, in order after trimming: (1) "Cleaned name is empty (covers missing, `null`, `""`, and whitespace-only)" → error `"Please enter your name."`; (2) "Cleaned name is longer than 100 characters" counted with `System.Globalization.StringInfo(cleaned).LengthInTextElements` → error `"Name must be 100 characters or fewer."`; expose the limit as `public const int MaxNameLength = 100;` and comment why text elements are counted
- [X] T018 [US2] Extend `src/NameGreeting/wwwroot/app.js`: when the response is 400 and the JSON body has an `error` string, show it via `result.textContent` with the `error` class and keep `nameInput.value` (FR-008); for any other non-OK status or a thrown network error (`catch`), show `"Sorry, the greeting could not be retrieved. Please try again."` with the `error` class and keep the input; a later success replaces the message and removes the `error` class (FR-010)
- [X] T019 [US2] Run `dotnet test` (all tests pass), then run the app and walk quickstart.md checklist rows 5, 6, 7, 8

**Checkpoint**: Both user stories work; all automated tests green

---

## Phase 5: Polish & Cross-Cutting Concerns

- [X] T020 [P] Create `README.md` at the repository root: one-paragraph description, prerequisites, and the `dotnet test` / `dotnet run --project src/NameGreeting` commands, linking to `specs/001-name-greeting/quickstart.md`
- [X] T021 Review `src/NameGreeting/Program.cs`, `src/NameGreeting/NameValidator.cs`, and `src/NameGreeting/wwwroot/app.js` for Constitution III: descriptive names, short functions, "why" comments only, no unused code; confirm no code logs submitted names (FR-011, research.md R8)
- [X] T022 Run full quickstart.md validation: `dotnet test` passes and all 10 manual checklist rows behave as expected

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup — blocks all user stories
- **US1 (Phase 3)**: Depends on Foundational
- **US2 (Phase 4)**: Depends on Foundational; extends files created in US1 (`NameValidator.cs`, `app.js`, test files), so run after US1
- **Polish (Phase 5)**: Depends on US1 and US2

### Task-level dependencies

- T001 → T002 (solution must exist); T003 independent
- T004 → T006, T007; T005 → T006
- T008, T009, T010 (tests) → T011, T012, T013 (implementation) → T014
- T011 → T012 (endpoint calls the validator)
- T015, T016 → T017, T018 → T019

### Parallel Opportunities

- T003 alongside T001/T002
- T005 and T006 together after T004
- T008, T009, T010 together (three different test files)
- T015 and T016 together
- T020 alongside T021

---

## Parallel Example: User Story 1

```bash
# Write all US1 tests at once (different files):
Task: "Valid-name cases in tests/NameGreeting.Tests/NameValidatorTests.cs"
Task: "Success-path tests in tests/NameGreeting.Tests/GreetingEndpointTests.cs"
Task: "GET /app.js test in tests/NameGreeting.Tests/HomePageTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1: Setup → Phase 2: Foundational
2. Phase 3: US1
3. **STOP and VALIDATE**: `dotnet test` + quickstart rows 1–4, 9, 10 — a working greeting app

### Incremental Delivery

1. Setup + Foundational → page loads
2. + US1 → greeting works (MVP)
3. + US2 → errors handled gracefully
4. Polish → README, readability review, full quickstart pass

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks
- Verify tests fail before implementing
- Browser-only behaviors are covered by the manual checklist in quickstart.md, not automated tests (constitution v1.1.0, Principle IV)
- Stop at any checkpoint to validate a story independently
