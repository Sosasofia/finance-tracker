using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorMoneyValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_installment_transaction_transaction_id",
                table: "installment");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "reimbursement",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "installment",
                newName: "amount");

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "reimbursement",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "currency",
                table: "installment",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: true);

            migrationBuilder.Sql("UPDATE [reimbursement] SET currency = 'ARS' WHERE currency IS NULL");
            migrationBuilder.Sql("UPDATE [installment] SET currency = 'ARS' WHERE currency IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "currency",
                table: "reimbursement",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "currency",
                table: "installment",
                nullable: false,
                oldNullable: true);

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

            migrationBuilder.DropColumn(
                name: "currency",
                table: "reimbursement");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "installment");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "reimbursement",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "installment",
                newName: "Amount");

            migrationBuilder.AddForeignKey(
                name: "FK_installment_transaction_transaction_id",
                table: "installment",
                column: "transaction_id",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
