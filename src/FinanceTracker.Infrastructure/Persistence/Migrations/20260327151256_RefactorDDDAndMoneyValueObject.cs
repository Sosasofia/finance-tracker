using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDDDAndMoneyValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "transaction",
                newName: "amount");

            migrationBuilder.Sql("UPDATE [transaction] SET CreatedAt = GETUTCDATE() WHERE CreatedAt IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "transaction",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "transaction",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.Sql("UPDATE [transaction] SET currency = 'ARS' WHERE currency IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "currency",
                table: "transaction",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "transaction_id",
                table: "installment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_installment_transaction_id",
                table: "installment",
                column: "transaction_id");

            migrationBuilder.AddForeignKey(
                name: "FK_installment_transaction_transaction_id",
                table: "installment",
                column: "transaction_id",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_installment_transaction_transaction_id",
                table: "installment");

            migrationBuilder.DropIndex(
                name: "IX_installment_transaction_id",
                table: "installment");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "installment");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "transaction",
                newName: "Amount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "transaction",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
