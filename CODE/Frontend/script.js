const apiUrl = "http://localhost:5297/api/patient";

let selectedPatient = null;


async function loadPatients() {

    const tableBody =
        document.getElementById("patient-table-body");

    try {

        const response =
            await fetch(apiUrl);

        if (!response.ok) {
            throw new Error(
                "Could not load patient information."
            );
        }

        const patients =
            await response.json();

        tableBody.innerHTML = "";


        patients.forEach(patient => {

            const row =
                document.createElement("tr");

            const dateOfBirth =
                new Date(patient.dateOfBirth)
                    .toLocaleDateString("en-NZ");


            row.innerHTML = `
                <td>${patient.patientId}</td>

                <td>
                    ${patient.firstName}
                    ${patient.lastName}
                </td>

                <td>
                    ${dateOfBirth}
                </td>

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

        const response =
            await fetch(
                `${apiUrl}/${patientId}`
            );


        if (!response.ok) {

            throw new Error(
                "Patient could not be found."
            );

        }


        const patient =
            await response.json();


        document
            .getElementById("patient-empty")
            .classList.add("hidden");


        document
            .getElementById("patient-details")
            .classList.remove("hidden");


        document
            .getElementById("detail-patient-id")
            .textContent =
            `Patient ID: ${patient.patientId}`;


        document
            .getElementById("detail-patient-name")
            .textContent =
            `${patient.firstName} ${patient.lastName}`;


        document
            .getElementById("detail-patient-dob")
            .textContent =
            `DOB: ${
                new Date(patient.dateOfBirth)
                    .toLocaleDateString("en-NZ")
            }`;


        document
            .getElementById("detail-patient-status")
            .textContent =
            patient.status;


        document
            .getElementById("detail-medical-history")
            .textContent =
            patient.medicalHistorySummary
            || "No medical history available.";


        document
            .getElementById("detail-allergies")
            .textContent =
            patient.allergies
            || "No allergy information available.";


        selectedPatient = patient;

    }

    catch (error) {

        console.error(error);

        alert(
            "Patient information could not be loaded."
        );

    }
}



const patientSearch =
    document.getElementById("patient-search");


if (patientSearch) {

    patientSearch.addEventListener(
        "input",
        function () {

            const searchValue =
                this.value
                    .toLowerCase()
                    .trim();


            const rows =
                document.querySelectorAll(
                    "#patient-table-body tr"
                );


            rows.forEach(row => {

                const rowText =
                    row.textContent
                        .toLowerCase();


                if (
                    rowText.includes(
                        searchValue
                    )
                ) {

                    row.style.display = "";

                }

                else {

                    row.style.display =
                        "none";

                }

            });

        }
    );

}



const tabButtons =
    document.querySelectorAll(
        ".tab-button"
    );

const tabContents =
    document.querySelectorAll(
        ".tab-content"
    );


tabButtons.forEach(button => {

    button.addEventListener(
        "click",
        function () {

            tabButtons.forEach(tab => {

                tab.classList.remove(
                    "active"
                );

            });


            tabContents.forEach(content => {

                content.classList.remove(
                    "active-tab"
                );

            });


            this.classList.add(
                "active"
            );


            const tabName =
                this.dataset.tab;


            document
                .getElementById(tabName)
                .classList.add(
                    "active-tab"
                );

        }
    );

});



function logout() {

    sessionStorage.removeItem(
        "reserveHealthUser"
    );

    window.location.href =
        "login.html";

}



document
    .getElementById(
        "edit-patient-button"
    )
    .addEventListener(
        "click",
        function () {

            if (!selectedPatient) {

                alert(
                    "Please select a patient first."
                );

                return;

            }


            document
                .getElementById(
                    "edit-options"
                )
                .classList.remove(
                    "hidden"
                );

        }
    );



function closeEditOptions() {

    document
        .getElementById(
            "edit-options"
        )
        .classList.add(
            "hidden"
        );

}



function showEditSection(section) {

    closeEditOptions();


    if (section === "status") {

        document
            .getElementById(
                "edit-status"
            )
            .value =
            selectedPatient.status;


        document
            .getElementById(
                "edit-status-section"
            )
            .classList.remove(
                "hidden"
            );

    }


    if (section === "history") {

        document
            .getElementById(
                "edit-medical-history"
            )
            .value =
            selectedPatient
                .medicalHistorySummary
            || "";


        document
            .getElementById(
                "edit-history-section"
            )
            .classList.remove(
                "hidden"
            );

    }


    if (section === "allergies") {

        document
            .getElementById(
                "edit-allergies"
            )
            .value =
            selectedPatient.allergies
            || "";


        document
            .getElementById(
                "edit-allergies-section"
            )
            .classList.remove(
                "hidden"
            );

    }

}



function closeEditSection(section) {

    document
        .getElementById(
            `edit-${section}-section`
        )
        .classList.add(
            "hidden"
        );

}



async function savePatientField(field) {

    if (!selectedPatient) {

        alert(
            "Please select a patient first."
        );

        return;

    }


    const updatedPatient = {
        ...selectedPatient
    };


    if (field === "status") {

        updatedPatient.status =
            document
                .getElementById(
                    "edit-status"
                )
                .value;

    }


    if (field === "history") {

        updatedPatient
            .medicalHistorySummary =
            document
                .getElementById(
                    "edit-medical-history"
                )
                .value
                .trim();

    }


    if (field === "allergies") {

        updatedPatient.allergies =
            document
                .getElementById(
                    "edit-allergies"
                )
                .value
                .trim();

    }


    try {

        const response =
            await fetch(
                `${apiUrl}/${selectedPatient.patientId}`,
                {
                    method: "PUT",

                    headers: {
                        "Content-Type":
                            "application/json"
                    },

                    body:
                        JSON.stringify(
                            updatedPatient
                        )
                }
            );


        if (!response.ok) {

            const errorMessage =
                await response.text();

            throw new Error(
                errorMessage
                || "Update failed."
            );

        }


        await loadPatient(
            selectedPatient.patientId
        );

        await loadPatients();


        closeEditSection(field);


        alert(
            "Patient updated successfully."
        );

    }

    catch (error) {

        console.error(error);

        alert(
            `Patient could not be updated: ${error.message}`
        );

    }

}



const addPatientModal =
    document.getElementById(
        "add-patient-modal"
    );

const addPatientForm =
    document.getElementById(
        "add-patient-form"
    );

const addPatientMessage =
    document.getElementById(
        "add-patient-message"
    );

const saveNewPatientButton =
    document.getElementById(
        "save-new-patient-button"
    );



function showAddPatientMessage(
    message,
    type
) {

    addPatientMessage.textContent =
        message;

    addPatientMessage.className =
        `form-message ${type}`;

}



function clearAddPatientMessage() {

    addPatientMessage.textContent =
        "";

    addPatientMessage.className =
        "form-message hidden";

}



function clearAddPatientForm() {

    document
        .getElementById(
            "add-first-name"
        )
        .value = "";


    document
        .getElementById(
            "add-last-name"
        )
        .value = "";


    document
        .getElementById(
            "add-date-of-birth"
        )
        .value = "";


    document
        .getElementById(
            "add-status"
        )
        .value =
        "Admitted";


    document
        .getElementById(
            "add-medical-history"
        )
        .value = "";


    document
        .getElementById(
            "add-allergies"
        )
        .value = "";

}



function openAddPatientModal() {

    clearAddPatientMessage();

    addPatientModal
        .classList.remove(
            "hidden"
        );


    document
        .getElementById(
            "add-first-name"
        )
        .focus();

}



function closeAddPatientModal() {

    addPatientModal
        .classList.add(
            "hidden"
        );

    clearAddPatientMessage();

}



document
    .getElementById(
        "add-patient-button"
    )
    .addEventListener(
        "click",
        openAddPatientModal
    );



document
    .getElementById(
        "cancel-add-patient-button"
    )
    .addEventListener(
        "click",
        closeAddPatientModal
    );



document
    .getElementById(
        "close-add-patient-button"
    )
    .addEventListener(
        "click",
        closeAddPatientModal
    );



document.addEventListener(
    "keydown",
    function (event) {

        if (
            event.key === "Escape"
            &&
            !addPatientModal
                .classList.contains(
                    "hidden"
                )
        ) {

            closeAddPatientModal();

        }

    }
);



addPatientModal.addEventListener(
    "click",
    function (event) {

        if (
            event.target
            === addPatientModal
        ) {

            closeAddPatientModal();

        }

    }
);



addPatientForm.addEventListener(
    "submit",
    async function (event) {

        event.preventDefault();

        clearAddPatientMessage();


        const firstName =
            document
                .getElementById(
                    "add-first-name"
                )
                .value
                .trim();


        const lastName =
            document
                .getElementById(
                    "add-last-name"
                )
                .value
                .trim();


        const dateOfBirth =
            document
                .getElementById(
                    "add-date-of-birth"
                )
                .value;


        const status =
            document
                .getElementById(
                    "add-status"
                )
                .value;


        const medicalHistorySummary =
            document
                .getElementById(
                    "add-medical-history"
                )
                .value
                .trim();


        const allergies =
            document
                .getElementById(
                    "add-allergies"
                )
                .value
                .trim();


        if (!firstName) {

            showAddPatientMessage(
                "First name is required.",
                "error"
            );

            document
                .getElementById(
                    "add-first-name"
                )
                .focus();

            return;

        }


        if (!lastName) {

            showAddPatientMessage(
                "Last name is required.",
                "error"
            );

            document
                .getElementById(
                    "add-last-name"
                )
                .focus();

            return;

        }


        if (!dateOfBirth) {

            showAddPatientMessage(
                "Date of birth is required.",
                "error"
            );

            document
                .getElementById(
                    "add-date-of-birth"
                )
                .focus();

            return;

        }


        const selectedDate =
            new Date(
                `${dateOfBirth}T00:00:00`
            );

        const today =
            new Date();


        if (
            selectedDate > today
        ) {

            showAddPatientMessage(
                "Date of birth cannot be in the future.",
                "error"
            );

            document
                .getElementById(
                    "add-date-of-birth"
                )
                .focus();

            return;

        }


        const allowedStatuses = [
            "Admitted",
            "Active",
            "Follow-up"
        ];


        if (
            !allowedStatuses.includes(
                status
            )
        ) {

            showAddPatientMessage(
                "Please select a valid patient status.",
                "error"
            );

            return;

        }


        if (
            medicalHistorySummary.length
            > 1000
        ) {

            showAddPatientMessage(
                "Medical history cannot exceed 1000 characters.",
                "error"
            );

            return;

        }


        if (
            allergies.length > 500
        ) {

            showAddPatientMessage(
                "Allergies cannot exceed 500 characters.",
                "error"
            );

            return;

        }


        const newPatient = {

            firstName,

            lastName,

            dateOfBirth,

            status,

            medicalHistorySummary,

            allergies

        };


        saveNewPatientButton.disabled =
            true;

        saveNewPatientButton.textContent =
            "Saving...";


        try {

            const response =
                await fetch(
                    apiUrl,
                    {
                        method: "POST",

                        headers: {
                            "Content-Type":
                                "application/json"
                        },

                        body:
                            JSON.stringify(
                                newPatient
                            )
                    }
                );


            if (!response.ok) {

                const errorMessage =
                    await response.text();


                throw new Error(
                    errorMessage
                    || "Patient could not be added."
                );

            }


            const createdPatient =
                await response.json();


            showAddPatientMessage(
                "Patient added successfully.",
                "success"
            );


            await loadPatients();


            setTimeout(
                async function () {

                    closeAddPatientModal();

                    clearAddPatientForm();

                    await loadPatient(
                        createdPatient.patientId
                    );

                },
                500
            );

        }

        catch (error) {

            console.error(error);


            showAddPatientMessage(
                "Patient could not be added. Please check the information and try again.",
                "error"
            );

        }

        finally {

            saveNewPatientButton.disabled =
                false;

            saveNewPatientButton.textContent =
                "Save Patient";

        }

    }
);



loadPatients();