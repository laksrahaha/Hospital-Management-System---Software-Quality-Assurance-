// Referral API
const referralApi = "http://localhost:5297/api/referrals";

// Load referrals
async function loadReferrals() {
    const response = await fetch(referralApi);
    const referrals = await response.json();

    const referralList = document.getElementById("referralList");

    if (referrals.length === 0) {
        referralList.innerHTML = "<p>No referrals available.</p>";
        return;
    }

    referralList.innerHTML = "";

    referrals.forEach(referral => {
        const referralCard = document.createElement("div");
        referralCard.className = "referral-item";

        referralCard.innerHTML = `
            <p><strong>Reason:</strong> ${referral.reason}</p>
            <p><strong>Priority:</strong> ${referral.priority}</p>
            <p><strong>Status:</strong> ${referral.status}</p>
        `;

        referralList.appendChild(referralCard);
    });
}

// Create referral
document.getElementById("referralForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const reason = document.getElementById("reason").value.trim();
    const priority = document.getElementById("priority").value;
    const message = document.getElementById("referralMessage");

    // Check fields
    if (reason === "" || priority === "") {
        message.textContent = "Reason and priority are required.";
        return;
    }

    const referral = {
        reason: reason,
        priority: priority
    };

    // Send referral
    const response = await fetch(referralApi, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(referral)
    });

    if (response.ok) {
        message.textContent = "Referral created successfully.";

        document.getElementById("referralForm").reset();

        loadReferrals();
    } else {
        message.textContent = "Referral could not be created.";
    }
});

// Show referrals when page opens
loadReferrals();