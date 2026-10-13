# Research: Name Greeting Page

**Feature**: [spec.md](./spec.md) | **Plan**: [plan.md](./plan.md) | **Date**: 2026-10-13

All Technical Context items were resolved; no `NEEDS CLARIFICATION` remain.

## R1. Target framework vs installed SDK

- **Decision**: Target `net8.0`. Build with the installed .NET 9 SDK (9.0.312); run on the
  installed .NET 8 runtime (Microsoft.AspNetCore.App 8.0.25).
- **Rationale**: The user asked for .NET 8 (LTS). A newer SDK can build older targets, and the
  matching ASP.NET Core 8 runtime is already installed, so nothing extra needs to be installed.
- **Alternatives considered**: Target `net9.0` (works too, but deviates from the user's stated
  choice); add a `global.json` pinning an 8.x SDK (would require installing another SDK).

## R2. API style

- **Decision**: ASP.NET Core **minimal API** with a single endpoint `POST /api/greeting`,
  declared directly in `Program.cs` via `app.MapPost(...)`.
- **Rationale**: Minimal APIs need no controllers, attributes, or extra classes — the smallest
  readable way to expose one endpoint (Constitution I, III).
- **Alternatives considered**: MVC controllers (more ceremony for one endpoint); `GET` with the
  name in the query string (names would end up in URLs, browser history, and logs — POST with a
  JSON body is the conventional choice for submitting form data).

## R3. Request / response format

- **Decision**: JSON body `{ "name": "..." }`. Success → `200` with `{ "name": "<trimmed>" }`.
  Validation failure → `400` with `{ "error": "<human-readable reason>" }`.
- **Rationale**: The spec requires the server to return the cleaned name (FR-002, FR-003) and a
  clear reason on rejection (FR-007). A flat `error` string is the easiest shape for a learner to
  read and for the page to display verbatim.
- **Alternatives considered**: RFC 7807 `ProblemDetails` / `Results.ValidationProblem` (standard,
  but nested `errors` dictionary is more to explain for one message); returning the full
  greeting sentence from the server (spec says the server returns the *name*; the page formats
  "Hello, {name}!").

## R4. Validation rules and where they live

- **Decision**: A small static class `NameValidator` with one method that trims the input and
  returns either the cleaned name or an error message. The endpoint calls it. The page does
  **no** validation (Clarification Q2 / FR-007).
- **Rationale**: Keeping the rule in a pure function makes it unit-testable without HTTP and
  keeps `Program.cs` short.
- **Length counting**: Use `System.Globalization.StringInfo.LengthInTextElements` so that what a
  person perceives as one character (e.g., "é" written as e + accent, or an emoji) counts as
  one. This matches the spec's "100 characters" from a user's point of view (SC-003).
  Built into .NET — no dependency.
- **Missing / null `name`** (e.g., body `{}`) is treated the same as an empty name.
- **Malformed JSON** is rejected by ASP.NET Core automatically with `400` before our code runs;
  the page shows a generic error in that (unlikely) case.
- **Alternatives considered**: DataAnnotations attributes (`[Required]`, `[MaxLength]`) — minimal
  APIs in .NET 8 don't run them automatically, so it would need extra code or a package;
  FluentValidation — third-party dependency (Constitution II).

## R5. Serving the page

- **Decision**: Put `index.html` and `app.js` in `wwwroot/` and enable
  `app.UseDefaultFiles()` + `app.UseStaticFiles()`. One project serves both page and API.
- **Rationale**: Same origin → no CORS setup; one `dotnet run` starts everything
  (Constitution: single-command run). Built-in middleware, no dependency.
- **Alternatives considered**: Separate frontend project/dev server (needs two processes and
  CORS); inline JS inside the HTML (works, but a separate `app.js` is easier to read).

## R6. Front-end behavior (plain JavaScript)

- **Decision**: A `<form>` with a `submit` event listener (covers both button click and Enter —
  FR-006). The handler: `preventDefault()`, disable the button, `fetch` the API, then show the
  result and re-enable the button in a `finally` block (FR-012). Text is written with
  `textContent`, never `innerHTML` (FR-009). One `<p>` element shows either the greeting or the
  error, so only one message is visible at a time (FR-010).
- **Ignoring Enter while waiting**: Disabling the form's only submit button prevents implicit
  submission by Enter in browsers; the handler additionally returns early if a request is
  already in progress, as a belt-and-braces guard.
- **Alternatives considered**: Any frontend framework (excluded by user input and Constitution II).

## R7. Testing approach

- **Decision**: One xUnit test project with two kinds of tests:
  - **Unit tests** for `NameValidator` (trimming, empty, whitespace, 100 vs 101 characters,
    Unicode names, markup-looking text returned unchanged).
  - **Integration tests** using `WebApplicationFactory<Program>` from
    `Microsoft.AspNetCore.Mvc.Testing`: these start the real app in memory and call
    `POST /api/greeting` and `GET /` over a test HTTP client.
- **Rationale**: Covers every server-side requirement with fast, dependency-light tests, and
  satisfies "each endpoint has a success test and an invalid-input test" (Constitution IV).
- **Front-end behavior** (input clears, button disables, text shown literally) is verified by
  the manual checklist in [quickstart.md](./quickstart.md). Automating it would require a
  browser-automation tool such as Playwright (large dependency + browser download). Constitution
  v1.1.0 Principle IV explicitly permits a manual checklist for browser-only behavior.
- **Packages** (all Microsoft or xUnit, standard for .NET tests): `xunit`,
  `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Microsoft.AspNetCore.Mvc.Testing`.
  The default template's `coverlet.collector` is removed as unnecessary.
- **Alternatives considered**: Playwright for .NET (see above); testing only via unit tests (would
  not prove the HTTP contract).

## R8. Logging / privacy (deferred item from clarify)

- **Decision**: Use ASP.NET Core default logging; do not add any code that logs submitted names.
- **Rationale**: Default request logging does not record request bodies, so names are not
  persisted anywhere (FR-011). No extra configuration needed.

## R9. Accessibility (deferred item from clarify)

- **Decision**: The input has a visible `<label>`; the result `<p>` has `aria-live="polite"` so
  screen readers announce the greeting or error.
- **Rationale**: Two attributes, zero cost, sensible default.
