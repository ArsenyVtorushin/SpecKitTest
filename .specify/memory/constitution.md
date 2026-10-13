<!--
Sync Impact Report
- Version change: 1.0.0 → 1.1.0 (MINOR: materially expanded guidance in Principle IV)
- Modified principles:
  - IV. Basic Automated Tests — "every user-facing behavior MUST have an automated test" replaced
    by: server-side behavior MUST be automated; browser-only behavior MAY use a documented manual
    checklist in quickstart.md. Resolves /speckit-analyze finding C1 (conflict with Governance).
- Modified sections:
  - Additional Constraints — reworded external-services rule as "MUST NOT require" (finding A1)
- Added sections: none
- Removed sections: none
- Dependent artifacts: specs/001-name-greeting/plan.md Constitution Check + Complexity Tracking
  updated to reflect full compliance.
- Follow-up TODOs: none
-->

# SpecKitTest Constitution

## Core Principles

### I. Minimal & Simple

- Every feature MUST be implemented with the smallest design that satisfies its spec.
- Code, files, layers, and abstractions MUST NOT be added for hypothetical future needs (YAGNI).
- When two approaches meet the requirements, the one with fewer moving parts MUST be chosen.

**Rationale**: This project exists to learn the Spec-Driven Development workflow; extra
complexity hides the workflow behind incidental detail.

### II. Fewest Dependencies

- The platform's standard library and built-in framework features MUST be preferred over
  third-party packages.
- Each third-party dependency MUST be justified in the feature's `plan.md` (what it provides and
  why the built-in option is insufficient).
- Frontend code MUST NOT use a framework or build toolchain unless the plan justifies it.

**Rationale**: Every dependency adds setup, versioning, and concepts a learner must understand.

### III. Learner-Readable Code

- Names MUST be descriptive; abbreviations and clever tricks MUST be avoided.
- Functions SHOULD be short and do one thing; control flow MUST be easy to follow top to bottom.
- Non-obvious decisions MUST carry a brief comment explaining *why*, not *what*.
- Each feature MUST include a `quickstart.md` explaining how to run and test it.

**Rationale**: The primary audience of this code is a person learning from it.

### IV. Basic Automated Tests

- All server-side behavior (validation, endpoints, data rules) MUST be covered by automated tests.
- Every API endpoint MUST have at least one test for its success case and one for invalid input.
- Browser-only behavior (e.g., what the page displays, focus, enabling/disabling controls) MAY be
  verified by a documented manual checklist in the feature's `quickstart.md` instead of
  automated tests.
- All tests MUST pass with a single documented command before a feature is considered done.
- Tests SHOULD stay simple; exhaustive coverage is not required.

**Rationale**: Tests prove the implementation matches the spec and give a safe base for change.

## Additional Constraints

- The application MUST run locally with a single command after prerequisites are installed.
- The application MUST NOT require external services (databases, cloud APIs) unless a spec
  explicitly needs one.
- User input MUST be validated on the server side, and output MUST be safely encoded
  when displayed (no injection of user-supplied markup).

## Development Workflow

- Features MUST follow the SpecKit flow: specify → (clarify) → plan → tasks → (analyze) →
  implement.
- Each phase's artifacts MUST be reviewed by the user before the next phase begins.
- A feature is done only when all tasks are marked `[X]`, all tests pass, and the quickstart
  steps work as written.

## Governance

- This constitution supersedes other practices in this repository; conflicts found by
  `/speckit-analyze` are CRITICAL.
- Amendments are made via `/speckit-constitution`, must state a rationale, and must update the
  version below.
- Versioning: MAJOR for removing or redefining a principle, MINOR for adding a principle or
  section, PATCH for wording clarifications.
- Every `plan.md` MUST include a Constitution Check confirming compliance; any deviation MUST be
  justified in its Complexity Tracking section.

**Version**: 1.1.0 | **Ratified**: 2026-10-13 | **Last Amended**: 2026-10-13
