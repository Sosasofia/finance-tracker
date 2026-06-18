using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.Infrastructure.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class EnforceDescriptionMaxLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_category_user_UserId",
                table: "category");

            migrationBuilder.DropForeignKey(
                name: "FK_installment_transaction_transaction_id",
                table: "installment");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_category_CategoryId",
                table: "transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_payment_method_PaymentMethodId",
                table: "transaction");

            migrationBuilder.DropIndex(
                name: "IX_installment_transaction_id",
                table: "installment");

            migrationBuilder.DropIndex(
                name: "IX_category_UserId",
                table: "category");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "installment");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "transaction",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "transaction",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_category_CategoryId",
                table: "transaction",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_payment_method_PaymentMethodId",
                table: "transaction",
                column: "PaymentMethodId",
                principalTable: "payment_method",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transaction_category_CategoryId",
                table: "transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_transaction_payment_method_PaymentMethodId",
                table: "transaction");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "transaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "transaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
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

            migrationBuilder.CreateIndex(
                name: "IX_category_UserId",
                table: "category",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_category_user_UserId",
                table: "category",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_installment_transaction_transaction_id",
                table: "installment",
                column: "transaction_id",
                principalTable: "transaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_category_CategoryId",
                table: "transaction",
                column: "CategoryId",
                principalTable: "category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transaction_payment_method_PaymentMethodId",
                table: "transaction",
                column: "PaymentMethodId",
                principalTable: "payment_method",
                principalColumn: "Id");
        }
    }
}
