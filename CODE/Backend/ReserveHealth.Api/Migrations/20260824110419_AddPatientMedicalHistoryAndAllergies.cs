using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReserveHealth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientMedicalHistoryAndAllergies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discharges_DischargeTasks_DischargeTaskId",
                table: "Discharges");

            migrationBuilder.DropForeignKey(
                name: "FK_Discharges_Users_StaffinchargeID",
                table: "Discharges");

            migrationBuilder.DropIndex(
                name: "IX_Discharges_DischargeTaskId",
                table: "Discharges");

            migrationBuilder.DropColumn(
                name: "DischargeTaskId",
                table: "Discharges");

            migrationBuilder.RenameColumn(
                name: "StaffinchargeID",
                table: "Discharges",
                newName: "StaffInChargeId");

            migrationBuilder.RenameIndex(
                name: "IX_Discharges_StaffinchargeID",
                table: "Discharges",
                newName: "IX_Discharges_StaffInChargeId");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicalHistorySummary",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "Discharges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FollowUpPlan",
                table: "Discharges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationChanges",
                table: "Discharges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Discharges_Users_StaffInChargeId",
                table: "Discharges",
                column: "StaffInChargeId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discharges_Users_StaffInChargeId",
                table: "Discharges");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicalHistorySummary",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "Discharges");

            migrationBuilder.DropColumn(
                name: "FollowUpPlan",
                table: "Discharges");

            migrationBuilder.DropColumn(
                name: "MedicationChanges",
                table: "Discharges");

            migrationBuilder.RenameColumn(
                name: "StaffInChargeId",
                table: "Discharges",
                newName: "StaffinchargeID");

            migrationBuilder.RenameIndex(
                name: "IX_Discharges_StaffInChargeId",
                table: "Discharges",
                newName: "IX_Discharges_StaffinchargeID");

            migrationBuilder.AddColumn<int>(
                name: "DischargeTaskId",
                table: "Discharges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discharges_DischargeTaskId",
                table: "Discharges",
                column: "DischargeTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discharges_DischargeTasks_DischargeTaskId",
                table: "Discharges",
                column: "DischargeTaskId",
                principalTable: "DischargeTasks",
                principalColumn: "DischargeTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discharges_Users_StaffinchargeID",
                table: "Discharges",
                column: "StaffinchargeID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
