# Data Model: Name Greeting Page

**Feature**: [spec.md](./spec.md) | **Date**: 2026-10-13

There is no database and nothing is stored (FR-011). The "data model" is just the shapes that
travel between the page and the server during one request.

## GreetingRequest

What the page sends to the server.

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `name` | string | no | Raw text from the input field. A missing or `null` value is treated as an empty name. |

C# shape: `record GreetingRequest(string? Name);`

## GreetingResponse

What the server returns when the name is valid (HTTP 200).

| Field | Type | Notes |
|-------|------|-------|
| `name` | string | The cleaned name: input with leading/trailing whitespace removed. Otherwise unchanged (same letters, same case, markup characters kept as-is). |

C# shape: `record GreetingResponse(string Name);`

## ErrorResponse

What the server returns when the name is invalid (HTTP 400).

| Field | Type | Notes |
|-------|------|-------|
| `error` | string | A human-readable reason the page shows verbatim. |

C# shape: `record ErrorResponse(string Error);`

## Validation rules (FR-007)

Applied by the server in this order, after trimming leading and trailing whitespace:

| # | Condition | Result | `error` message |
|---|-----------|--------|-----------------|
| 1 | Cleaned name is empty (covers missing, `null`, `""`, and whitespace-only) | 400 | `Please enter your name.` |
| 2 | Cleaned name is longer than 100 characters* | 400 | `Name must be 100 characters or fewer.` |
| 3 | Otherwise | 200 | — (returns `GreetingResponse`) |

\* "Characters" are counted as user-perceived characters (text elements), so accented letters and
emoji count as one each. See [research.md](./research.md) R4.

## State (page only)

The page has one piece of state: whether a request is in progress.

```text
Idle ──submit──▶ Waiting (button disabled, Enter ignored)
Waiting ──200──▶ Idle (show "Hello, {name}!", clear input)
Waiting ──400──▶ Idle (show server's error, keep input)
Waiting ──network failure / other error──▶ Idle (show "Sorry, the greeting could not be retrieved. Please try again.", keep input)
```
