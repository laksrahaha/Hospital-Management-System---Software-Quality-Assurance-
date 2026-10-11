const auditApi =
    "http://localhost:5297/api/audit";

const currentUser =
    JSON.parse(
        sessionStorage.getItem(
            "reserveHealthUser"
        )
    );

if (!currentUser ||
    !currentUser.token) {

    window.location.href =
        "login.html";

    throw new Error(
        "Authentication required."
    );
}

async function authenticatedFetch(
    url,
    options = {}
) {
    options.headers = {
        ...(options.headers || {}),

        "Authorization":
            `Bearer ${currentUser.token}`
    };

    const response =
        await fetch(
            url,
            options
        );

    if (response.status === 401) {
        sessionStorage.removeItem(
            "reserveHealthUser"
        );

        window.location.href =
            "login.html";
    }

    return response;
}

async function loadAuditLogs() {
    const auditList =
        document.getElementById(
            "auditList"
        );

    const auditMessage =
        document.getElementById(
            "auditMessage"
        );

    auditMessage.textContent = "";

    try {
        const response =
            await authenticatedFetch(
                auditApi
            );

        if (response.status === 403) {
            auditMessage.textContent =
                "You are not authorised to view audit records.";

            return;
        }

        if (!response.ok) {
            throw new Error(
                "Could not load audit records."
            );
        }

        const auditLogs =
            await response.json();

        auditList.innerHTML = "";

        if (auditLogs.length === 0) {
            auditList.innerHTML = `
                <tr>
                    <td colspan="8">
                        No audit records available.
                    </td>
                </tr>
            `;

            return;
        }

        auditLogs.forEach(log => {
            const row =
                document.createElement(
                    "tr"
                );

            const timestamp =
                log.timestamp
                    ? new Date(
                        log.timestamp
                    ).toLocaleString()
                    : "Unavailable";

            row.innerHTML = `
                <td>
                    ${log.auditLogId}
                </td>

                <td>
                    ${log.userId}
                </td>

                <td class="action-label">
                    ${log.actionType}
                </td>

                <td>
                    ${log.entityType}
                </td>

                <td>
                    ${log.entityId}
                </td>

                <td>
                    ${log.oldValue ?? "-"}
                </td>

                <td>
                    ${log.newValue ?? "-"}
                </td>

                <td>
                    ${timestamp}
                </td>
            `;

            auditList.appendChild(
                row
            );
        });

    } catch (error) {
        console.error(
            "Error loading audit records:",
            error
        );

        auditMessage.textContent =
            "Audit records could not be loaded.";
    }
}

loadAuditLogs();