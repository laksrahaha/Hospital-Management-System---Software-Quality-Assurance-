const apiUrl = "http://localhost:5297/api/patient";


async function loadPatients() {
    const tableBody = document.getElementById("patient-table-body");

    try {
        const response = await fetch(apiUrl);

        if (!response.ok) {
            throw new Error("Could not load patient information.");
        }

        const patients = await response.json();

        tableBody.innerHTML = "";

        patients.forEach(patient => {
            const row = document.createElement("tr");

            const dateOfBirth =
                new Date(patient.dateOfBirth)
                    .toLocaleDateString("en-NZ");

            row.innerHTML = `
                <td>${patient.patientId}</td>

                <td>
                    ${patient.firstName} ${patient.lastName}
                </td>

                <td>${dateOfBirth}</td>

                <td>
                    <span class="status-pill">
                        ${patient.status}
                    </span>
                </td>

                <td>
                    <button
                        class="view-button"
                        onclick="loadPatient(${patient.patientId})">
                        Open
                    </button>
                </td>
            `;

            tableBody.appendChild(row);
        });
    }

    catch (error) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="5">
                    Patient information could not be loaded.
                </td>
            </tr>
        `;

        console.error(error);
    }
}



async function loadPatient(patientId) {
    try {
        const response = await fetch(`${apiUrl}/${patientId}`);

        if (!response.ok) {
            throw new Error("Patient could not be found.");
        }

        const patient = await response.json();

        document
            .getElementById("patient-empty")
            .classList.add("hidden");

        document
            .getElementById("patient-details")
            .classList.remove("hidden");

        document.getElementById("detail-patient-id").textContent =
            `Patient ID: ${patient.patientId}`;

        document.getElementById("detail-patient-name").textContent =
            `${patient.firstName} ${patient.lastName}`;

        document.getElementById("detail-patient-dob").textContent =
            `DOB: ${new Date(patient.dateOfBirth).toLocaleDateString("en-NZ")}`;

        document.getElementById("detail-patient-status").textContent =
            patient.status;

        document.getElementById("detail-medical-history").textContent =
            patient.medicalHistorySummary || "No medical history available.";

        document.getElementById("detail-allergies").textContent =
            patient.allergies || "No allergy information available.";

    }

    catch (error) {
        console.error(error);

        alert("Patient information could not be loaded.");
    }
}

const patientSearch = document.getElementById("patient-search");

if (patientSearch) {
    patientSearch.addEventListener("input", function () {
        const searchValue = this.value.toLowerCase();

        const rows = document.querySelectorAll(
            "#patient-table-body tr"
        );

        rows.forEach(row => {
            const rowText = row.textContent.toLowerCase();

            if (rowText.includes(searchValue)) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        });
    });
}

const tabButtons = document.querySelectorAll(".tab-button");
const tabContents = document.querySelectorAll(".tab-content");

tabButtons.forEach(button => {

    button.addEventListener("click", function () {

        tabButtons.forEach(tab => {
            tab.classList.remove("active");
        });

        tabContents.forEach(content => {
            content.classList.remove("active-tab");
        });

        this.classList.add("active");

        const tabName = this.dataset.tab;

        document
            .getElementById(tabName)
            .classList.add("active-tab");

    });

});

function logout() {

    sessionStorage.removeItem(
        "reserveHealthUser"
    );

    window.location.href =
        "login.html";
}


loadPatients();