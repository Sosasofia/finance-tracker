using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Server.Migrations
{
    /// <inheritdoc />
    public partial class RenameBusinessAndReimbursement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reimburstment");

            migrationBuilder.RenameColumn(
                name: "BussinessName",
                table: "transaction",
                newName: "BusinessName");

            migrationBuilder.CreateTable(
                name: "reimbursement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reimbursement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reimbursement_transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_reimbursement_TransactionId",
                table: "reimbursement",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reimbursement");

            migrationBuilder.RenameColumn(
                name: "BusinessName",
                table: "transaction",
                newName: "BussinessName");

            migrationBuilder.CreateTable(
                name: "reimburstment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TransactionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reason = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reimburstment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reimburstment_transaction_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transaction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_reimburstment_TransactionId",
                table: "reimburstment",
                column: "TransactionId",
                unique: true);
        }
    }
}
