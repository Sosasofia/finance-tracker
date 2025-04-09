using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Server.Migrations
{
    /// <inheritdoc />
    public partial class DeleteFKFromTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReimburstmentId",
                table: "transaction");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "payment_method",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "payment_method");

            migrationBuilder.AddColumn<Guid>(
                name: "ReimburstmentId",
                table: "transaction",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
