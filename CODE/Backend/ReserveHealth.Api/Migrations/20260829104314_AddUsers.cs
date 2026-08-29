using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReserveHealth.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Referrals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Referrals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Service",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Referrals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "Service",
                table: "Referrals");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Referrals");
        }
    }
}
