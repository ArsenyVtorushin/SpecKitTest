# Implementation Plan: Name Greeting Page

**Branch**: `001-name-greeting` | **Date**: 2026-10-13 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-name-greeting/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

A single web page asks for the visitor's name and, on submit, sends it to a REST endpoint
(`POST /api/greeting`). The server trims and validates the name and returns it; the page shows
"Hello, {name}!" and clears the input, or shows the server's error and keeps the input. Built as
one ASP.NET Core 8 minimal-API project that also serves a static HTML page with plain
JavaScript. No storage. Tested with xUnit (unit tests for validation, in-memory integration
tests for the HTTP contract).

## Technical Context

**Language/Version**: C# 12 on .NET 8 (`net8.0`); built with installed SDK 9.0.312, runs on installed ASP.NET Core runtime 8.0.25 ([research R1](./research.md))

**Primary Dependencies**: ASP.NET Core minimal API (part of the .NET shared framework — no NuGet packages for the app). Front end: plain HTML + JavaScript, no libraries.

**Storage**: N/A — nothing is stored (FR-011)

**Testing**: xUnit + `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`) for in-memory integration tests ([research R7](./research.md))

**Target Platform**: Local developer machine (Windows; cross-platform capable); modern desktop browser

**Project Type**: Web application — single project serving both API and static page

**Performance Goals**: Greeting visible < 1 s after submit on localhost (SC-002) — trivially met

**Constraints**: Single-command run; no external services; server-side validation only; output rendered as plain text

**Scale/Scope**: One user at a time, one endpoint, one page, ~4 source files + ~3 test files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle / Rule | Check | Status |
|------------------|-------|--------|
| I. Minimal & Simple | One project, one endpoint in `Program.cs`, one small validator class, no layers/interfaces/DI abstractions | ✅ Pass |
| II. Fewest Dependencies | App: zero NuGet packages. Tests: xUnit trio + `Mvc.Testing` (justified in research R7). Front end: no framework, no build step | ✅ Pass |
| III. Learner-Readable Code | Descriptive names, short functions, "why" comments; `quickstart.md` provided | ✅ Pass |
| IV. Basic Automated Tests — endpoint success + invalid-input tests | Integration tests for 200 and both 400 cases | ✅ Pass |
| IV. Basic Automated Tests — all server-side behavior automated | `NameValidator` unit tests + `POST /api/greeting` and `GET /` integration tests | ✅ Pass |
| IV. Basic Automated Tests — browser-only behavior | Input clearing, button disabling, literal rendering in the page, error display: covered by the manual checklist in [quickstart.md](./quickstart.md) (permitted by constitution v1.1.0) | ✅ Pass |
| IV. All tests pass with one command | `dotnet test` | ✅ Pass |
| Constraint: single-command run | `dotnet run --project src/NameGreeting` | ✅ Pass |
| Constraint: no external services | None | ✅ Pass |
| Constraint: server-side validation + safe output | `NameValidator` on server; `textContent` on page | ✅ Pass |
| Workflow: plan includes Constitution Check | This section | ✅ Pass |

**Post-design re-check (after Phase 1)**: No change. The contract, data model, and quickstart
introduce no new dependencies or layers. Gate passes.

**Re-check after constitution v1.1.0 (2026-10-13)**: Principle IV now permits a manual checklist
for browser-only behavior, so the earlier deviation is resolved. Full compliance; no Complexity
Tracking entries needed.

## Project Structure

### Documentation (this feature)

```text
specs/001-name-greeting/
├── plan.md              # This file
├── research.md          # Phase 0: decisions and rationale
├── data-model.md        # Phase 1: request/response shapes, validation rules
├── quickstart.md        # Phase 1: run/test guide + manual checklist
├── contracts/
│   └── greeting-api.md  # Phase 1: HTTP contract
├── checklists/
│   └── requirements.md  # From /speckit-specify
└── tasks.md             # Phase 2 (/speckit-tasks — not created here)
```

### Source Code (repository root)

```text
NameGreeting.sln

src/
└── NameGreeting/
    ├── NameGreeting.csproj      # Microsoft.NET.Sdk.Web, net8.0, no PackageReferences
    ├── Program.cs               # App setup, static files, POST /api/greeting, records
    ├── NameValidator.cs         # Trim + validate; returns cleaned name or error message
    └── wwwroot/
        ├── index.html           # Form, label, button, aria-live result element, minimal CSS
        └── app.js               # Submit handler: disable button, fetch, show result, re-enable

tests/
└── NameGreeting.Tests/
    ├── NameGreeting.Tests.csproj    # xunit, runner, Test.Sdk, Mvc.Testing; references src project
    ├── NameValidatorTests.cs        # Unit tests for validation rules
    ├── GreetingEndpointTests.cs     # Integration tests for POST /api/greeting
    └── HomePageTests.cs             # Integration test: GET / returns the page with form elements
```

**Structure Decision**: Single web project (not separate frontend/backend) because the page is
two static files served by the same ASP.NET Core app — no separate build, no CORS. Tests live
in a sibling `tests/` folder in one test project. `Program.cs` ends with
`public partial class Program { }` so `WebApplicationFactory<Program>` can reach it.

## Complexity Tracking

No violations. (An earlier entry for untested browser-only behavior was resolved by
constitution v1.1.0, Principle IV.)
