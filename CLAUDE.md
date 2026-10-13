# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Repository Is

This is a **SpecKit** project — a Specification-Driven Development (SDD) workspace initialized with the Claude integration (v1.1.4.dev0). It contains no application source code of its own; instead it provides a structured workflow for specifying, planning, and implementing features via AI-assisted commands.

## SDD Workflow

Features flow through these phases in order:

1. `/speckit-specify <description>` — Creates `specs/<NNN>-<short-name>/spec.md` from a natural-language description. Writes `.specify/feature.json` to track the active feature directory.
2. `/speckit-clarify` *(optional)* — Asks up to 5 clarification questions and encodes answers back into the spec.
3. `/speckit-plan` — Runs `.specify/scripts/bash/setup-plan.sh --json` and generates `plan.md`, `research.md`, `data-model.md`, `contracts/`, and `quickstart.md` inside the feature directory.
4. `/speckit-tasks` — Runs `.specify/scripts/bash/setup-tasks.sh --json` and generates a dependency-ordered `tasks.md`.
5. `/speckit-analyze` *(optional)* — Read-only cross-artifact consistency check across `spec.md`, `plan.md`, and `tasks.md`. Never modifies files.
6. `/speckit-implement` — Runs `.specify/scripts/bash/check-prerequisites.sh --json --require-tasks --include-tasks` and executes all tasks in `tasks.md`, marking each `[X]` on completion.

Helper commands:
- `/speckit-checklist` — Generates a custom requirements-quality checklist for the current feature.
- `/speckit-converge` — Assesses the codebase against the feature artifacts and appends remaining unbuilt work to `tasks.md`.
- `/speckit-constitution` — Creates or updates the project constitution at `.specify/memory/constitution.md`.

## Key Paths

| Path | Purpose |
|------|---------|
| `.specify/feature.json` | Tracks the active feature directory; read by plan/tasks/implement/analyze |
| `.specify/memory/constitution.md` | Project-wide principles; loaded by specify/plan/tasks/implement as a governance constraint |
| `.specify/init-options.json` | Feature numbering scheme (`"sequential"` → `NNN-name`; `"timestamp"` → `YYYYMMDD-HHMMSS-name`) |
| `specs/<NNN>-<name>/` | All artifacts for a single feature live here |
| `.specify/templates/` | Source templates for spec, plan, tasks, checklist, constitution |
| `.specify/scripts/bash/` | Shell scripts invoked by plan/tasks/implement skills |
| `.specify/extensions.yml` | Optional extension hooks (`before_specify`, `after_plan`, etc.) — does not exist yet |

## Spec Quality Rules (enforced by `/speckit-specify`)

- Specs must be **technology-agnostic** — no frameworks, languages, or API names.
- Success Criteria must be **measurable and user-facing** (e.g., "Users complete checkout in under 3 minutes", not "API responds in 200ms").
- Maximum **3 `[NEEDS CLARIFICATION]` markers** per spec; everything else gets a reasonable default.
- Checklists in `checklists/requirements.md` are maintained by specify/clarify — treat them as read-only gates in implement.

## Task Format (enforced by `/speckit-tasks`)

Every task line must follow:
```
- [ ] T001 [P] [US1] Description with exact/file/path.ext
```
- `[P]` — parallelizable (different files, no incomplete dependencies)
- `[US1]` — maps to a user story from spec.md (required in user-story phases, absent in setup/foundational/polish phases)
- Completed tasks are marked `[X]` by `/speckit-implement`

## Constitution

`.specify/memory/constitution.md` currently contains only the unfilled template. Run `/speckit-constitution` to establish project principles before beginning feature work. Once filled, the constitution is non-negotiable in analysis — conflicts are always CRITICAL severity.
