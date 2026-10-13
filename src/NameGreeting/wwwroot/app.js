const greetingForm = document.getElementById("greeting-form");
const nameInput = document.getElementById("name-input");
const submitButton = document.getElementById("submit-button");
const result = document.getElementById("result");

// Tracks whether we are waiting for the server, so repeated clicks or Enter presses
// do not send duplicate requests (FR-012).
let isRequestInProgress = false;

const retrievalErrorMessage = "Sorry, the greeting could not be retrieved. Please try again.";

// Errors keep the typed text in the input so the visitor can fix it and resubmit (FR-008).
function showError(message) {
    result.textContent = message;
    result.classList.add("error");
}

// A 400 carries a message written for the visitor; anything else gets a generic one.
async function readErrorMessage(response) {
    if (response.status !== 400) {
        return retrievalErrorMessage;
    }
    try {
        const data = await response.json();
        return typeof data.error === "string" ? data.error : retrievalErrorMessage;
    } catch {
        // e.g. the framework's own 400 for malformed JSON, which has no "error" field.
        return retrievalErrorMessage;
    }
}

// Listening for "submit" on the form (not "click" on the button) means both the
// button and the Enter key trigger a greeting (FR-006).
greetingForm.addEventListener("submit", async (event) => {
    // Stop the browser from reloading the page, which is what a form does by default.
    event.preventDefault();

    if (isRequestInProgress) {
        return;
    }

    isRequestInProgress = true;
    submitButton.disabled = true;

    try {
        const response = await fetch("/api/greeting", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ name: nameInput.value }),
        });

        if (response.ok) {
            const data = await response.json();
            // Use the name the server sent back (already trimmed), not the raw input (FR-002).
            // textContent shows the text literally, so "<b>" is never treated as HTML (FR-009).
            result.textContent = "Hello, " + data.name + "!";
            result.classList.remove("error");
            nameInput.value = "";
        } else {
            showError(await readErrorMessage(response));
        }
    } catch {
        // fetch throws when the server cannot be reached at all (e.g. it is stopped).
        showError(retrievalErrorMessage);
    } finally {
        // Runs whether the request succeeded or failed, so the button never stays stuck.
        isRequestInProgress = false;
        submitButton.disabled = false;
    }
});
