const greetingForm = document.getElementById("greeting-form");
const nameInput = document.getElementById("name-input");
const submitButton = document.getElementById("submit-button");
const result = document.getElementById("result");

// Tracks whether we are waiting for the server, so repeated clicks or Enter presses
// do not send duplicate requests (FR-012).
let isRequestInProgress = false;

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
        }
    } finally {
        // Runs whether the request succeeded or failed, so the button never stays stuck.
        isRequestInProgress = false;
        submitButton.disabled = false;
    }
});
