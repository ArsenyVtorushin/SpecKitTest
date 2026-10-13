# Name Greeting

A single web page that asks for your name and greets you with "Hello, {name}!". The page sends
the name to a small REST endpoint (`POST /api/greeting`), which trims and validates it and sends
it back; empty or overly long names get a helpful error instead. Built with ASP.NET Core 8
minimal APIs and plain HTML/JavaScript, with no database and no external services. It was
created as a learning project for the Spec-Driven Development (SpecKit) workflow.

## Prerequisites

- .NET SDK 8.0 or newer (`dotnet --list-sdks`)
- ASP.NET Core runtime 8.0 (`dotnet --list-runtimes` shows `Microsoft.AspNetCore.App 8.0.x`)
- Any modern web browser

## Run the tests

```bash
dotnet test
```

## Run the app

```bash
dotnet run --project src/NameGreeting
```

Then open the URL printed after `Now listening on:`.

## More

See [specs/001-name-greeting/quickstart.md](specs/001-name-greeting/quickstart.md) for an API
example and the manual browser checklist.
