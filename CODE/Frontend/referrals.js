// Referral page
// Connects the referral form to the patient and referral APIs.
// Handles loading patients, creating referrals and displaying referral information.

const referralApi = "http://localhost:5297/api/referrals";
const patientApi = "http://localhost:5297/api/patient";

let referrals = [];
let patients = [];

// Load patients from the patient API
async function loadPatients() {
    try {
        const response = await fetch(patientApi);

        if (!response.ok) {
            throw new Error("Could not load patients.");
        }

        patients = await response.json();

        const patientSelect = document.getElementById("patientId");

        patientSelect.innerHTML =
            '<option value="">Select Patient</option>';

        patients.forEach(patient => {
            const option = document.createElement("option");

            option.value = patient.patientId;
            option.textContent =
                `${patient.firstName} ${patient.lastName}`;

            patientSelect.appendChild(option);
        });
    } catch (error) {
        console.error("Error loading patients:", error);

        document.getElementById("referralMessage").textContent =
            "Patients could not be loaded.";
    }
}


// Get the patient name using the Patient ID
function getPatientName(patientId) {
    const patient = patients.find(
        patient => patient.patientId === patientId
    );

    if (patient) {
        return `${patient.firstName} ${patient.lastName}`;
    }

    return `Patient ${patientId}`;
}


// Load referrals
async function loadReferrals() {
    try {
        const response = await fetch(referralApi);

        if (!response.ok) {
            throw new Error("Could not load referrals.");
        }

        referrals = await response.json();

        displayReferrals();
    } catch (error) {
        console.error("Error loading referrals:", error);

        const referralList =
            document.getElementById("referralList");

        referralList.innerHTML = `
            <tr>
                <td colspan="6">
                    Referrals could not be loaded.
                </td>
            </tr>
        `;
    }
}


// Display referrals
function displayReferrals() {
    const referralList =
        document.getElementById("referralList");

    const statusFilter =
        document.getElementById("statusFilter").value;

    const priorityFilter =
        document.getElementById("priorityFilter").value;

    const filteredReferrals = referrals.filter(referral => {
        const statusMatches =
            statusFilter === "All" ||
            referral.status === statusFilter;

        const priorityMatches =
            priorityFilter === "All" ||
            referral.priority === priorityFilter;

        return statusMatches && priorityMatches;
    });


    if (filteredReferrals.length === 0) {
        referralList.innerHTML = `
            <tr>
                <td colspan="6">
                    No referrals available.
                </td>
            </tr>
        `;

        return;
    }


    referralList.innerHTML = "";


    filteredReferrals.forEach(referral => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${referral.referralId}</td>

            <td>
                ${getPatientName(referral.patientId)}
            </td>

            <td>${referral.service}</td>

            <td>${referral.priority}</td>

            <td>${referral.status}</td>

            <td>
                <button
                    type="button"
                    onclick="showReferralDetails(${referral.referralId})">
                    View
                </button>
            </td>
        `;

        referralList.appendChild(row);
    });
}


// Show the selected referral
function showReferralDetails(referralId) {
    const referral = referrals.find(
        referral => referral.referralId === referralId
    );

    if (!referral) {
        return;
    }

    const details =
        document.getElementById("referralDetails");

    const content =
        document.getElementById("referralDetailsContent");


    content.innerHTML = `
        <p>
            <strong>Referral ID:</strong>
            ${referral.referralId}
        </p>

        <p>
            <strong>Patient:</strong>
            ${getPatientName(referral.patientId)}
        </p>

        <p>
            <strong>Service:</strong>
            ${referral.service}
        </p>

        <p>
            <strong>Reason:</strong>
            ${referral.reason}
        </p>

        <p>
            <strong>Priority:</strong>
            ${referral.priority}
        </p>

        <p>
            <strong>Status:</strong>
            ${referral.status}
        </p>

        <p>
            <strong>Date Created:</strong>
            ${new Date(referral.dateCreated).toLocaleString()}
        </p>

        ${referral.statusReason
            ? `
                <p>
                    <strong>Status Reason:</strong>
                    ${referral.statusReason}
                </p>
              `
            : ""
        }
    `;

    details.classList.remove("hidden");
}


// Create referral
document
    .getElementById("referralForm")
    .addEventListener("submit", async function (event) {

        event.preventDefault();

        const patientId =
            Number(document.getElementById("patientId").value);

        const service =
            document.getElementById("service").value;

        const reason =
            document.getElementById("reason").value.trim();

        const priority =
            document.getElementById("priority").value;

        const message =
            document.getElementById("referralMessage");


        // Check required fields
        if (!patientId || service === "" ||
            reason === "" || priority === "") {

            message.textContent =
                "Patient, service, reason and priority are required.";

            return;
        }


        const referral = {
            patientId: patientId,
            service: service,
            reason: reason,
            priority: priority
        };


        try {
            const response = await fetch(referralApi, {
                method: "POST",

                headers: {
                    "Content-Type": "application/json"
                },

                body: JSON.stringify(referral)
            });


            if (response.ok) {
                message.textContent =
                    "Referral created successfully.";

                document
                    .getElementById("referralForm")
                    .reset();

                await loadReferrals();
            } else {
                const errorMessage =
                    await response.text();

                message.textContent =
                    errorMessage || "Referral could not be created.";
            }

        } catch (error) {
            console.error("Error creating referral:", error);

            message.textContent =
                "Referral could not be created.";
        }
    });


// Filter referrals when status changes
document
    .getElementById("statusFilter")
    .addEventListener("change", displayReferrals);


// Filter referrals when priority changes
document
    .getElementById("priorityFilter")
    .addEventListener("change", displayReferrals);


// Load page information
async function initialiseReferralPage() {
    await loadPatients();
    await loadReferrals();
}

initialiseReferralPage();