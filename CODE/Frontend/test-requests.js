const apiUrl =
    "http://localhost:5297/api/test-requests";

const storedUser =
    sessionStorage.getItem(
        "reserveHealthUser"
    );

const message =
    document.getElementById(
        "requests-message"
    );

const table =
    document.getElementById(
        "requests-table"
    );

const tbody =
    document.getElementById(
        "requests-body"
    );

const resultSection =
    document.getElementById(
        "result-section"
    );

const resultHeading =
    document.getElementById(
        "result-heading"
    );

const resultForm =
    document.getElementById(
        "result-form"
    );

const resultInformation =
    document.getElementById(
        "result-information"
    );

const resultMessage =
    document.getElementById(
        "result-message"
    );

const selectedRequestText =
    document.getElementById(
        "selected-request"
    );

const saveResultButton =
    document.getElementById(
        "save-result-button"
    );

let selectedRequestId =
    null;

let editingExistingResult =
    false;

let user;


// ----------------------------------------------------
// LOGOUT
// ----------------------------------------------------

document
    .getElementById(
        "logout-button"
    )
    .addEventListener(
        "click",
        () => {

            sessionStorage.removeItem(
                "reserveHealthUser"
            );

            window.location.href =
                "login.html";
        }
    );


// ----------------------------------------------------
// READ LOGGED-IN USER
// ----------------------------------------------------

try {

    user =
        storedUser
            ? JSON.parse(
                storedUser
            )
            : null;

} catch {

    user =
        null;
}


// ----------------------------------------------------
// AUTHENTICATION
// ----------------------------------------------------

if (
    !user ||
    !user.token
) {

    sessionStorage.removeItem(
        "reserveHealthUser"
    );

    window.location.href =
        "login.html";

    throw new Error(
        "Authentication required."
    );
}


// ----------------------------------------------------
// ROLE-BASED ACCESS
// ----------------------------------------------------

if (
    user.role !==
    "Lab Technician"
) {

    window.location.href =
        "index.html";

    throw new Error(
        "Lab Technician role required."
    );
}


// ----------------------------------------------------
// LOAD TEST REQUESTS
// ----------------------------------------------------

async function loadRequests() {

    table.hidden =
        true;

    tbody.replaceChildren();

    message.textContent =
        "Loading test requests...";


    try {

        const response =
            await fetch(
                apiUrl,
                {
                    headers: {

                        Authorization:
                            `Bearer ${user.token}`
                    }
                }
            );


        if (
            response.status ===
            401
        ) {

            sessionStorage.removeItem(
                "reserveHealthUser"
            );

            window.location.href =
                "login.html";

            return;
        }


        if (
            response.status ===
            403
        ) {

            window.location.href =
                "index.html";

            return;
        }


        if (
            !response.ok
        ) {

            throw new Error(
                "Could not load test requests."
            );
        }


        const requests =
            await response.json();


        if (
            requests.length ===
            0
        ) {

            message.textContent =
                "No test requests yet.";

            return;
        }


        for (
            const request
            of requests
        ) {

            const row =
                document.createElement(
                    "tr"
                );


            for (
                const value
                of [
                    request.patientName,
                    request.testType,
                    request.status,
                    new Date(
                        request.requestedAt
                    ).toLocaleString(
                        "en-NZ"
                    )
                ]
            ) {

                const cell =
                    document.createElement(
                        "td"
                    );


                cell.textContent =
                    value ?? "";


                row.appendChild(
                    cell
                );
            }


            const actionCell =
                document.createElement(
                    "td"
                );


            if (
                request.status ===
                "Completed"
            ) {

                row.classList.add(
                    "completed"
                );


                const editButton =
                    document.createElement(
                        "button"
                    );


                editButton.type =
                    "button";


                editButton.textContent =
                    "Edit result";


                editButton.addEventListener(
                    "click",
                    () => {

                        selectedRequestId =
                            request.testRequestId;


                        editingExistingResult =
                            true;


                        resultHeading.textContent =
                            "Edit test result";


                        selectedRequestText.textContent =
                            `${request.patientName} — ${request.testType}`;


                        resultInformation.value =
                            request.resultInformation
                            ?? "";


                        resultMessage.textContent =
                            "";


                        saveResultButton.textContent =
                            "Update result";


                        resultSection.hidden =
                            false;


                        resultInformation.focus();
                    }
                );


                actionCell.appendChild(
                    editButton
                );

            } else {

                const enterButton =
                    document.createElement(
                        "button"
                    );


                enterButton.type =
                    "button";


                enterButton.textContent =
                    "Enter result";


                enterButton.addEventListener(
                    "click",
                    () => {

                        selectedRequestId =
                            request.testRequestId;


                        editingExistingResult =
                            false;


                        resultHeading.textContent =
                            "Enter test result";


                        selectedRequestText.textContent =
                            `${request.patientName} — ${request.testType}`;


                        resultInformation.value =
                            "";


                        resultMessage.textContent =
                            "";


                        saveResultButton.textContent =
                            "Save result";


                        resultSection.hidden =
                            false;


                        resultInformation.focus();
                    }
                );


                actionCell.appendChild(
                    enterButton
                );
            }


            row.appendChild(
                actionCell
            );


            tbody.appendChild(
                row
            );
        }


        table.hidden =
            false;


        message.textContent =
            "";

    } catch (error) {

        console.error(
            error
        );


        message.textContent =
            "Could not load test requests. Please try again.";
    }
}


// ----------------------------------------------------
// CANCEL RESULT ENTRY
// ----------------------------------------------------

document
    .getElementById(
        "cancel-result-button"
    )
    .addEventListener(
        "click",
        () => {

            selectedRequestId =
                null;


            editingExistingResult =
                false;


            resultForm.reset();


            resultMessage.textContent =
                "";


            resultHeading.textContent =
                "Enter test result";


            saveResultButton.textContent =
                "Save result";


            resultSection.hidden =
                true;
        }
    );


// ----------------------------------------------------
// SAVE / UPDATE RESULT
// ----------------------------------------------------

resultForm
    .addEventListener(
        "submit",
        async event => {

            event.preventDefault();


            const information =
                resultInformation
                    .value
                    .trim();


            if (
                selectedRequestId ===
                null
            ) {

                resultMessage.textContent =
                    "Select a test request first.";

                return;
            }


            if (
                !information
            ) {

                resultMessage.textContent =
                    "Result information is required.";


                resultInformation.focus();


                return;
            }


            saveResultButton.disabled =
                true;


            resultMessage.textContent =
                editingExistingResult
                    ? "Updating result..."
                    : "Saving result...";


            try {

                const method =
                    editingExistingResult
                        ? "PUT"
                        : "POST";


                const response =
                    await fetch(
                        `${apiUrl}/${selectedRequestId}/result`,
                        {
                            method:
                                method,

                            headers: {

                                Authorization:
                                    `Bearer ${user.token}`,

                                "Content-Type":
                                    "application/json"
                            },

                            body:
                                JSON.stringify(
                                    {
                                        resultInformation:
                                            information
                                    }
                                )
                        }
                    );


                if (
                    response.status ===
                    401
                ) {

                    sessionStorage.removeItem(
                        "reserveHealthUser"
                    );


                    window.location.href =
                        "login.html";


                    return;
                }


                if (
                    response.status ===
                    403
                ) {

                    window.location.href =
                        "index.html";


                    return;
                }


                if (
                    response.status ===
                    404
                ) {

                    resultMessage.textContent =
                        "The test request or result could not be found.";


                    return;
                }


                if (
                    response.status ===
                    409
                ) {

                    resultMessage.textContent =
                        "A result has already been entered for this request.";


                    await loadRequests();


                    return;
                }


                if (
                    response.status ===
                    400
                ) {

                    const errorText =
                        await response.text();


                    resultMessage.textContent =
                        errorText ||
                        "The result could not be saved.";


                    return;
                }


                if (
                    !response.ok
                ) {

                    resultMessage.textContent =
                        "Could not save the result. Please try again.";


                    return;
                }


                const wasEditing =
                    editingExistingResult;


                selectedRequestId =
                    null;


                editingExistingResult =
                    false;


                resultForm.reset();


                resultHeading.textContent =
                    "Enter test result";


                saveResultButton.textContent =
                    "Save result";


                resultSection.hidden =
                    true;


                await loadRequests();


                message.textContent =
                    wasEditing
                        ? "Result updated successfully."
                        : "Result saved successfully.";

            } catch (error) {

                console.error(
                    error
                );


                resultMessage.textContent =
                    "Could not save the result. Please try again.";

            } finally {

                saveResultButton.disabled =
                    false;
            }
        }
    );


// ----------------------------------------------------
// INITIAL PAGE LOAD
// ----------------------------------------------------

loadRequests();