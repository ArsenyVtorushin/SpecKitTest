# Contract: Greeting API

**Feature**: [spec.md](../spec.md) | **Data shapes**: [data-model.md](../data-model.md)

The application exposes two HTTP interfaces on the same origin (default `http://localhost:5000`;
the actual port is printed by `dotnet run`).

---

## `GET /`

Serves the single web page (`index.html`) and its script (`/app.js`).

| Response | When |
|----------|------|
| `200 OK`, `Content-Type: text/html` | Always |

The page MUST contain: a text input with a label, a submit button inside a form, and one result
element with `aria-live="polite"`.

---

## `POST /api/greeting`

Accepts a name and returns the cleaned name.

**Request**

```http
POST /api/greeting
Content-Type: application/json

{ "name": "  Ada  " }
```

**Responses**

| Status | Body | When |
|--------|------|------|
| `200 OK` | `{ "name": "Ada" }` | Name is valid (non-empty after trimming, ≤ 100 characters) |
| `400 Bad Request` | `{ "error": "Please enter your name." }` | Name missing, `null`, empty, or whitespace-only |
| `400 Bad Request` | `{ "error": "Name must be 100 characters or fewer." }` | Name longer than 100 characters after trimming |
| `400 Bad Request` | (framework default) | Body is not valid JSON |

**Guarantees**

- The returned `name` is exactly the submitted text with leading/trailing whitespace removed — no
  other changes (case, accents, punctuation, and markup characters such as `<` `>` are kept).
- Nothing is stored; each request is independent.
- JSON property names are camelCase (`name`, `error`).

**Examples**

| Request body | Status | Response body |
|--------------|--------|---------------|
| `{"name":"Ada"}` | 200 | `{"name":"Ada"}` |
| `{"name":"  Zoë  "}` | 200 | `{"name":"Zoë"}` |
| `{"name":"<b>Bob</b>"}` | 200 | `{"name":"<b>Bob</b>"}` |
| `{"name":""}` | 400 | `{"error":"Please enter your name."}` |
| `{"name":"   "}` | 400 | `{"error":"Please enter your name."}` |
| `{}` | 400 | `{"error":"Please enter your name."}` |
| `{"name":"<101 × a>"}` | 400 | `{"error":"Name must be 100 characters or fewer."}` |
