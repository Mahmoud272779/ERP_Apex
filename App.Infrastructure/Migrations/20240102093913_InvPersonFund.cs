using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class InvPersonFund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvFundsCustomerSupplier_InvPersons_PersonId",
                table: "InvFundsCustomerSupplier");

            migrationBuilder.DropIndex(
                name: "IX_InvFundsCustomerSupplier_PersonId",
                table: "InvFundsCustomerSupplier");

            migrationBuilder.AddColumn<int>(
                name: "FundsCustomerSuppliersId",
                table: "InvPersons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "branchId",
                table: "InvFundsCustomerSupplier",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_InvPersons_FundsCustomerSuppliersId",
                table: "InvPersons",
                column: "FundsCustomerSuppliersId");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsCustomerSupplier_branchId",
                table: "InvFundsCustomerSupplier",
                column: "branchId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvFundsCustomerSupplier_GLBranch_branchId",
                table: "InvFundsCustomerSupplier",
                column: "branchId",
                principalTable: "GLBranch",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvPersons_InvFundsCustomerSupplier_FundsCustomerSuppliersId",
                table: "InvPersons",
                column: "FundsCustomerSuppliersId",
                principalTable: "InvFundsCustomerSupplier",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvFundsCustomerSupplier_GLBranch_branchId",
                table: "InvFundsCustomerSupplier");

            migrationBuilder.DropForeignKey(
                name: "FK_InvPersons_InvFundsCustomerSupplier_FundsCustomerSuppliersId",
                table: "InvPersons");

            migrationBuilder.DropIndex(
                name: "IX_InvPersons_FundsCustomerSuppliersId",
                table: "InvPersons");

            migrationBuilder.DropIndex(
                name: "IX_InvFundsCustomerSupplier_branchId",
                table: "InvFundsCustomerSupplier");

            migrationBuilder.DropColumn(
                name: "FundsCustomerSuppliersId",
                table: "InvPersons");

            migrationBuilder.DropColumn(
                name: "branchId",
                table: "InvFundsCustomerSupplier");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsCustomerSupplier_PersonId",
                table: "InvFundsCustomerSupplier",
                column: "PersonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvFundsCustomerSupplier_InvPersons_PersonId",
                table: "InvFundsCustomerSupplier",
                column: "PersonId",
                principalTable: "InvPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
