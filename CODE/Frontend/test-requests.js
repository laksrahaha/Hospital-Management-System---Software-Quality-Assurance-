const apiUrl = "http://localhost:5297/api/test-requests";

const storedUser = sessionStorage.getItem("reserveHealthUser");
const message = document.getElementById("requests-message");
const table = document.getElementById("requests-table");
const tbody = document.getElementById("requests-body");

const resultSection = document.getElementById("result-section");
const resultForm = document.getElementById("result-form");
const resultInformation = document.getElementById("result-information");
const resultMessage = document.getElementById("result-message");
const selectedRequestText = document.getElementById("selected-request");
const saveResultButton = document.getElementById("save-result-button");

let selectedRequestId = null;
let user;

document.getElementById("logout-button").addEventListener("click", () => {
    sessionStorage.removeItem("reserveHealthUser");
    window.location.href = "login.html";
});

try {
    user = storedUser ? JSON.parse(storedUser) : null;
} catch {
    user = null;
}

if (!user?.token) {
    window.location.href = "login.html";
} else if (user.role !== "Lab Technician") {
    message.textContent = "You do not have access to laboratory requests.";
} else {
    loadRequests();
}

async function loadRequests() {
    table.hidden = true;
    tbody.replaceChildren();
    message.textContent = "Loading test requests...";

    try {
        const response = await fetch(apiUrl, {
            headers: {
                Authorization: `Bearer ${user.token}`
            }
        });

        if (response.status === 401) {
            sessionStorage.removeItem("reserveHealthUser");
            window.location.href = "login.html";
            return;
        }

        if (response.status === 403) {
            message.textContent =
                "You do not have access to laboratory requests.";
            return;
        }

        if (!response.ok) {
            throw new Error("Could not load test requests.");
        }

        const requests = await response.json();

        if (requests.length === 0) {
            message.textContent = "No test requests yet.";
            return;
        }

        for (const request of requests) {
            const row = document.createElement("tr");

            for (const value of [
                request.patientName,
                request.testType,
                request.status,
                new Date(request.requestedAt).toLocaleString("en-NZ")
            ]) {
                const cell = document.createElement("td");
                cell.textContent = value ?? "";
                row.appendChild(cell);
            }

            const actionCell = document.createElement("td");

            if (request.status === "Completed") {
                row.classList.add("completed");
                actionCell.textContent = "Completed";
            } else {
                const enterButton = document.createElement("button");
                enterButton.type = "button";
                enterButton.textContent = "Enter result";

                enterButton.addEventListener("click", () => {
                    selectedRequestId = request.testRequestId;
                    selectedRequestText.textContent =
                        `${request.patientName} — ${request.testType}`;
                    resultInformation.value = "";
                    resultMessage.textContent = "";
                    resultSection.hidden = false;
                    resultInformation.focus();
                });

                actionCell.appendChild(enterButton);
            }

            row.appendChild(actionCell);
            tbody.appendChild(row);
        }

        table.hidden = false;
        message.textContent = "";
    } catch (error) {
        console.error(error);
        message.textContent =
            "Could not load test requests. Please try again.";
    }
}

document.getElementById("cancel-result-button")
    .addEventListener("click", () => {
        selectedRequestId = null;
        resultForm.reset();
        resultMessage.textContent = "";
        resultSection.hidden = true;
    });

resultForm.addEventListener("submit", async event => {
    event.preventDefault();

    const information = resultInformation.value.trim();

    if (selectedRequestId === null) {
        resultMessage.textContent = "Select a test request first.";
        return;
    }

    if (!information) {
        resultMessage.textContent = "Result information is required.";
        resultInformation.focus();
        return;
    }

    saveResultButton.disabled = true;
    resultMessage.textContent = "Saving result...";

    try {
        const response = await fetch(
            `${apiUrl}/${selectedRequestId}/result`,
            {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${user.token}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    resultInformation: information
                })
            }
        );

        if (response.status === 401) {
            sessionStorage.removeItem("reserveHealthUser");
            window.location.href = "login.html";
            return;
        }

        if (response.status === 403) {
            resultMessage.textContent =
                "You do not have permission to enter results.";
            return;
        }

        if (response.status === 404) {
            resultMessage.textContent =
                "This test request no longer exists.";
            return;
        }

        if (response.status === 409) {
            resultMessage.textContent =
                "A result has already been entered for this request.";
            await loadRequests();
            return;
        }

        if (!response.ok) {
            resultMessage.textContent =
                "Could not save the result. Please check it and try again.";
            return;
        }

        selectedRequestId = null;
        resultForm.reset();
        resultSection.hidden = true;
        await loadRequests();
        message.textContent = "Result saved successfully.";
    } catch (error) {
        console.error(error);
        resultMessage.textContent =
            "Could not save the result. Please try again.";
    } finally {
        saveResultButton.disabled = false;
    }
});