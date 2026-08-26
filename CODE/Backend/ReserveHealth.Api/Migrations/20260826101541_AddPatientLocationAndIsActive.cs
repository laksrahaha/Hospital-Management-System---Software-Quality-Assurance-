using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReserveHealth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientLocationAndIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Referrals_Discharges_DischargeId",
                table: "Referrals");

            migrationBuilder.DropIndex(
                name: "IX_Referrals_DischargeId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "DischargeId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "FollowUpDate",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Organisation",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "ReferralDate",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "ReferralType",
                table: "Referrals");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Patients",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Patients");

            migrationBuilder.AddColumn<int>(
                name: "DischargeId",
                table: "Referrals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowUpDate",
                table: "Referrals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Organisation",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReferralDate",
                table: "Referrals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferralType",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Referrals_DischargeId",
                table: "Referrals",
                column: "DischargeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Referrals_Discharges_DischargeId",
                table: "Referrals",
                column: "DischargeId",
                principalTable: "Discharges",
                principalColumn: "DischargeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
