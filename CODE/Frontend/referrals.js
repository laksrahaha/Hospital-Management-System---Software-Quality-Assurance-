// Referral page
// Loads patients and referrals from the API.
// Handles creating, filtering and editing referrals.

const referralApi = "http://localhost:5297/api/referrals";
const patientApi = "http://localhost:5297/api/patient";

let referrals = [];
let patients = [];

// Load patients into the patient dropdown
async function loadPatients() {
    const patientSelect = document.getElementById("patientId");
    const message = document.getElementById("referralMessage");

    try {
        const response = await fetch(patientApi);

        if (!response.ok) {
            throw new Error("Could not load patients.");
        }

        patients = await response.json();

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
        message.textContent = "Patients could not be loaded.";
    }
}

// Find the patient's name using their ID
function getPatientName(patientId) {
    const patient = patients.find(
        patient =>
            Number(patient.patientId) === Number(patientId)
    );

    if (patient) {
        return `${patient.firstName} ${patient.lastName}`;
    }

    return `Patient ${patientId}`;
}

// Load referrals from the API
async function loadReferrals() {
    const referralList =
        document.getElementById("referralList");

    try {
        const response = await fetch(referralApi);

        if (!response.ok) {
            throw new Error("Could not load referrals.");
        }

        referrals = await response.json();

        displayReferrals();

    } catch (error) {
        console.error("Error loading referrals:", error);

        referralList.innerHTML = `
            <tr>
                <td colspan="6">
                    Referrals could not be loaded.
                </td>
            </tr>
        `;
    }
}

// Display referrals using the selected filters
function displayReferrals() {
    const referralList =
        document.getElementById("referralList");

    const statusFilter =
        document.getElementById("statusFilter").value;

    const priorityFilter =
        document.getElementById("priorityFilter").value;

    const filteredReferrals =
        referrals.filter(referral => {
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
            <td>
                ${referral.referralId}
            </td>

            <td>
                ${getPatientName(referral.patientId)}
            </td>

            <td>
                ${referral.service}
            </td>

            <td>
                <span class="status-pill">
                    ${referral.priority}
                </span>
            </td>

            <td>
                ${referral.status}
            </td>

            <td>
                <button
                    type="button"
                    class="edit-referral-button"
                    onclick="openReferralEditor(
                        ${referral.referralId}
                    )">
                    Edit
                </button>
            </td>
        `;

        referralList.appendChild(row);
    });
}

// Open the edit form for one referral
function openReferralEditor(referralId) {
    const referral = referrals.find(
        referral =>
            Number(referral.referralId) ===
            Number(referralId)
    );

    if (!referral) {
        return;
    }

    const detailsPanel =
        document.getElementById("referralDetails");

    const detailsContent =
        document.getElementById("referralDetailsContent");

    const editReferralId =
        document.getElementById("editReferralId");

    const editPriority =
        document.getElementById("editPriority");

    const editStatus =
        document.getElementById("editStatus");

    const editStatusReason =
        document.getElementById("editStatusReason");

    const editMessage =
        document.getElementById("editReferralMessage");

    let createdDate = "Not available";

    if (referral.dateCreated) {
        createdDate =
            new Date(referral.dateCreated).toLocaleString();
    }

    detailsContent.innerHTML = `
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
            <strong>Referral reason:</strong>
            ${referral.reason}
        </p>

        <p>
            <strong>Date created:</strong>
            ${createdDate}
        </p>
    `;

    editReferralId.value = referral.referralId;
    editPriority.value = referral.priority;
    editStatus.value = referral.status;
    editStatusReason.value = referral.statusReason || "";
    editMessage.textContent = "";

    updateStatusReasonVisibility();

    detailsPanel.classList.remove("hidden");

    detailsPanel.scrollIntoView({
        behavior: "smooth",
        block: "start"
    });
}

// Show the reason field for returned or rejected referrals
function updateStatusReasonVisibility() {
    const editStatus =
        document.getElementById("editStatus").value;

    const statusReasonField =
        document.getElementById("statusReasonField");

    const editStatusReason =
        document.getElementById("editStatusReason");

    const reasonIsRequired =
        editStatus === "Returned for more information" ||
        editStatus === "Rejected";

    if (reasonIsRequired) {
        statusReasonField.classList.remove("hidden");
        editStatusReason.required = true;
    } else {
        statusReasonField.classList.add("hidden");
        editStatusReason.required = false;
        editStatusReason.value = "";
    }
}

// Close the edit form
function closeReferralEditor() {
    const detailsPanel =
        document.getElementById("referralDetails");

    const editForm =
        document.getElementById("editReferralForm");

    const editMessage =
        document.getElementById("editReferralMessage");

    detailsPanel.classList.add("hidden");
    editForm.reset();
    editMessage.textContent = "";

    updateStatusReasonVisibility();
}

// Create a new referral
document
    .getElementById("referralForm")
    .addEventListener(
        "submit",
        async function (event) {
            event.preventDefault();

            const patientId =
                Number(
                    document
                        .getElementById("patientId")
                        .value
                );

            const service =
                document
                    .getElementById("service")
                    .value;

            const reason =
                document
                    .getElementById("reason")
                    .value
                    .trim();

            const priority =
                document
                    .getElementById("priority")
                    .value;

            const message =
                document
                    .getElementById("referralMessage");

            message.textContent = "";

            if (
                !patientId ||
                service === "" ||
                reason === "" ||
                priority === ""
            ) {
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
                const response =
                    await fetch(
                        referralApi,
                        {
                            method: "POST",

                            headers: {
                                "Content-Type":
                                    "application/json"
                            },

                            body:
                                JSON.stringify(referral)
                        }
                    );

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
                        errorMessage ||
                        "Referral could not be created.";
                }

            } catch (error) {
                console.error(
                    "Error creating referral:",
                    error
                );

                message.textContent =
                    "Referral could not be created.";
            }
        }
    );

// Save changes made to a referral
document
    .getElementById("editReferralForm")
    .addEventListener(
        "submit",
        async function (event) {
            event.preventDefault();

            const referralId =
                Number(
                    document
                        .getElementById("editReferralId")
                        .value
                );

            const priority =
                document
                    .getElementById("editPriority")
                    .value;

            const status =
                document
                    .getElementById("editStatus")
                    .value;

            const statusReason =
                document
                    .getElementById("editStatusReason")
                    .value
                    .trim();

            const message =
                document
                    .getElementById("editReferralMessage");

            message.textContent = "";

            const reasonIsRequired =
                status === "Returned for more information" ||
                status === "Rejected";

            if (priority === "" || status === "") {
                message.textContent =
                    "Priority and status are required.";

                return;
            }

            if (reasonIsRequired && statusReason === "") {
                message.textContent =
                    "A reason is required when a referral is returned or rejected.";

                return;
            }

            const updatedReferral = {
                priority: priority,
                status: status,
                statusReason:
                    reasonIsRequired
                        ? statusReason
                        : null
            };

            try {
                const response =
                    await fetch(
                        `${referralApi}/${referralId}`,
                        {
                            method: "PUT",

                            headers: {
                                "Content-Type":
                                    "application/json"
                            },

                            body:
                                JSON.stringify(
                                    updatedReferral
                                )
                        }
                    );

                if (response.ok) {
                    message.textContent =
                        "Referral updated successfully.";

                    await loadReferrals();

                    setTimeout(
                        closeReferralEditor,
                        800
                    );

                } else {
                    const errorMessage =
                        await response.text();

                    message.textContent =
                        errorMessage ||
                        "Referral could not be updated.";
                }

            } catch (error) {
                console.error(
                    "Error updating referral:",
                    error
                );

                message.textContent =
                    "Referral could not be updated.";
            }
        }
    );

// Update the reason field when status changes
document
    .getElementById("editStatus")
    .addEventListener(
        "change",
        updateStatusReasonVisibility
    );

// Close the edit panel when Cancel is selected
document
    .getElementById("cancelEditButton")
    .addEventListener(
        "click",
        closeReferralEditor
    );

// Filter referrals by status
document
    .getElementById("statusFilter")
    .addEventListener(
        "change",
        displayReferrals
    );

// Filter referrals by priority
document
    .getElementById("priorityFilter")
    .addEventListener(
        "change",
        displayReferrals
    );

// Return the user to the login page
function logout() {

    sessionStorage.removeItem(
        "reserveHealthUser"
    );

    window.location.href =
        "login.html";
}

// Load patients and referrals when the page opens
async function initialiseReferralPage() {
    await loadPatients();
    await loadReferrals();
}

initialiseReferralPage();