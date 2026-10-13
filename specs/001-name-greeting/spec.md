# Feature Specification: Name Greeting Page

**Feature Branch**: `001-name-greeting`

**Created**: 2026-10-13

**Status**: Draft

**Input**: User description: "A simple website where a visitor enters their name in a form and when the user hits a button the page then displays a greeting with that name and blanks the form. The name is submitted to a backend REST API, which returns it to be shown on the page."

## Clarifications

### Session 2026-10-13

- Q: What should the page do while it's waiting for the server's reply after the visitor presses the button? → A: Disable the button (and Enter) until the reply arrives; re-enable it afterwards.
- Q: When a visitor submits an empty or too-long name, should the page catch it itself, or always send it to the server and show the server's error? → A: Server-only: the page always sends the name and shows the server's error message.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Greet the visitor by name (Priority: P1)

A visitor opens the website, types their name into a single input field, and presses the
submit button. The name is sent to the server, the server returns it, and the page shows a
greeting containing that name (for example, "Hello, Ada!"). The input field is then cleared so
the visitor can enter another name.

**Why this priority**: This is the entire purpose of the feature; without it nothing of value
exists.

**Independent Test**: Open the page, enter "Ada", press the button, and confirm the page shows
"Hello, Ada!" and the input field is empty.

**Acceptance Scenarios**:

1. **Given** the page is open and the input is empty, **When** the visitor types "Ada" and
   presses the button, **Then** the page displays "Hello, Ada!" and the input field becomes
   empty.
2. **Given** a greeting for "Ada" is already displayed, **When** the visitor enters "Grace" and
   presses the button, **Then** the greeting is replaced with "Hello, Grace!" (only one greeting
   is shown at a time) and the input field becomes empty.
3. **Given** the page is open, **When** the visitor types a name and presses Enter instead of
   clicking the button, **Then** the result is the same as clicking the button.

---

### User Story 2 - Helpful feedback for invalid input or failures (Priority: P2)

If the visitor submits an empty or invalid name, or the server cannot be reached, the page
explains what went wrong instead of showing a broken or misleading greeting.

**Why this priority**: The core flow works without it, but without clear feedback the visitor
cannot tell why nothing happened.

**Independent Test**: Press the button with an empty field and confirm an explanatory message
appears and no greeting is shown; stop the server, submit a name, and confirm an error message
appears and the typed name is still in the field.

**Acceptance Scenarios**:

1. **Given** the input is empty or contains only spaces, **When** the visitor presses the button,
   **Then** the page shows a message asking them to enter a name and no greeting is displayed.
2. **Given** the visitor enters a name longer than the allowed maximum, **When** they press the
   button, **Then** the page shows a message stating the maximum length and no greeting is
   displayed.
3. **Given** the server is unavailable, **When** the visitor submits a valid name, **Then** the
   page shows a message that the greeting could not be retrieved and the typed name remains in
   the input field so they can retry.

---

### Edge Cases

- Leading and trailing spaces around a name are removed before the greeting is shown
  ("  Ada  " → "Hello, Ada!").
- Names containing non-English letters, accents, hyphens, or apostrophes (e.g., "Zoë",
  "O'Brien", "Jean-Luc", "Анна") are greeted exactly as entered.
- Text that looks like markup or script (e.g., `<b>Bob</b>`) is displayed literally as typed and
  is never interpreted by the page.
- Rapid repeated clicks or Enter presses while a submission is in progress are ignored; exactly
  one request is sent per submission.
- A previous error message disappears once a subsequent submission succeeds.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The website MUST present a single page with one text input for a name and one
  submit button.
- **FR-002**: When the visitor submits, the page MUST send the name to the server and display
  the name returned by the server; the page MUST NOT build the greeting from its own local copy
  of the input.
- **FR-003**: The server MUST accept a submitted name and return the cleaned name (trimmed of
  leading/trailing whitespace) in its response.
- **FR-004**: The page MUST display the greeting in the form "Hello, {name}!".
- **FR-005**: After a successful submission, the page MUST clear the input field.
- **FR-006**: Submitting MUST be possible both by clicking the button and by pressing Enter in
  the input field.
- **FR-007**: The server MUST reject names that are empty or whitespace-only and names longer
  than 100 characters (after trimming), returning a clear reason. The server is the only place
  this validation happens; the page MUST NOT pre-validate and MUST send every submission to the
  server, displaying the server's reason when it rejects the name.
- **FR-008**: The page MUST show a human-readable message when the server rejects a name or
  cannot be reached, and MUST keep the typed text in the input field in those cases.
- **FR-009**: The page MUST display names as plain text so that any characters, including markup
  characters, are shown literally and never executed.
- **FR-010**: Only the most recent greeting or message MUST be visible at a time.
- **FR-011**: The system MUST NOT store submitted names beyond handling the single request.
- **FR-012**: While a submission is waiting for the server's reply, the page MUST disable the
  submit button and ignore Enter in the input field; both MUST be re-enabled when the reply (or
  an error) arrives.

### Key Entities

- **Name Submission**: The text a visitor types and sends to the server. Attributes: the raw
  name text. Valid when non-empty after trimming and at most 100 characters.
- **Greeting Result**: What the server returns for a valid submission. Attributes: the cleaned
  name. Alternatively an error reason for invalid submissions.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A first-time visitor can enter their name and see their greeting within 10 seconds
  of opening the page, without any instructions.
- **SC-002**: The greeting appears within 1 second of pressing the button under normal local
  conditions.
- **SC-003**: 100% of valid names (1–100 characters, any language or script) are greeted exactly
  as entered, minus surrounding spaces.
- **SC-004**: 100% of invalid submissions (empty, whitespace-only, over 100 characters) produce
  an explanatory message and no greeting.
- **SC-005**: 0 cases where text entered by a visitor changes the page's appearance or behavior
  other than appearing literally inside the greeting.

## Assumptions

- The site is for a single anonymous visitor at a time; no sign-in, accounts, or personalization
  across visits.
- Names are not saved anywhere; refreshing the page returns it to its initial empty state.
- The maximum name length of 100 characters is a reasonable default for human names.
- The greeting wording "Hello, {name}!" in English is sufficient; localization is out of scope.
- The site runs locally on the developer's machine for learning purposes; public hosting,
  scaling, and multi-user load are out of scope.
- A modern desktop web browser is the target; dedicated mobile layout is out of scope, though
  the page should remain usable on small screens.
