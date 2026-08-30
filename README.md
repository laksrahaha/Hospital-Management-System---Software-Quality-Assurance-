Reserve Health - How to Run the Project

1. Open the project folder in VS Code.

2. Start the Backend API

Open a terminal in VS Code and run:

cd CODE/Backend/ReserveHealth.Api
dotnet run

Wait until the terminal shows that the API is running.

The backend should run on:

http://localhost:5297

Keep this terminal open while using the system.

3. Start the Frontend

In the VS Code Explorer, go to:

CODE/Frontend/login.html

Right-click login.html and select:

Open with Live Server

Live Server may use port 5500, 5501, or another available local port.

4. Login or Create an Account

The system should always be started from login.html.

If you already have an account:
- Enter your email and password.
- Select Login.

If you do not have an account:
- Select Sign up.
- Enter your full name, email, password and confirm password.
- Create the account.
- Return to the login page.
- Login using the same details.

5. Use the Dashboard

After a successful login, the main dashboard will open.

From the dashboard you can access:

- Patient Management
- Referral Management

6. Patient Management

The patient section allows users to:

- View patient records
- Search patients
- Add patients
- Edit selected patient information
- Archive patients
- Restore archived patients
- View patient medical history and allergies

7. Referral Management

The referral section allows users to:

- Create referrals
- View referral records
- Edit referral priority
- Edit referral status

New referrals are automatically assigned a Pending status.

Referral statuses can later be changed to:

- Pending
- Accepted
- Returned for more information
- Rejected
- Completed

8. Logout

Use the Logout button to return to the login page.

9. Important Notes

- The backend API must be running while using the frontend.
- The frontend is run using Live Server.
- The backend and frontend run on separate local servers.
- The frontend may use different Live Server ports depending on what is available.
- Sample patient data is added automatically for testing and demonstration.
- Referral data is stored in the database.
- This system is a prototype and is not connected to real Auckland Hospital systems or real patient data.

10. GitHub Repository

https://github.com/laksrahaha/Hospital-Management-System---Software-Quality-Assurance-


