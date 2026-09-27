const storedUser = sessionStorage.getItem("reserveHealthUser");
const message = document.getElementById("requests-message");
const table = document.getElementById("requests-table");
const tbody = document.getElementById("requests-body");

document.getElementById("logout-button").addEventListener("click", () => {
    sessionStorage.removeItem("reserveHealthUser");
    window.location.href = "login.html";
});

let user;

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
    loadRequests(user.token);
}

async function loadRequests(token) {
    try {
        const response = await fetch("http://localhost:5297/api/test-requests", {
            headers: { Authorization: `Bearer ${token}` }
        });

        if (response.status === 401) {
            sessionStorage.removeItem("reserveHealthUser");
            window.location.href = "login.html";
            return;
        }

        if (response.status === 403) {
            message.textContent = "You do not have access to laboratory requests.";
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
                new Date(request.requestedAt).toLocaleString()
            ]) {
                const cell = document.createElement("td");
                cell.textContent = value ?? "";
                row.appendChild(cell);
            }

            if (request.status === "Completed") {
                row.classList.add("completed");
            }

            tbody.appendChild(row);
        }

        table.hidden = false;
        message.textContent = "";
    } catch (error) {
        console.error(error);
        message.textContent = "Could not load test requests. Please try again.";
    }
}