using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class publish_25122023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GLJournalEntryDetailsAccounts");

            migrationBuilder.DropColumn(
                name: "Pos_UseLastPrice",
                table: "InvGeneralSettings");

            

            migrationBuilder.RenameColumn(
                name: "Sales_UseLastPrice",
                table: "InvGeneralSettings",
                newName: "other_ActivatePriceLists");

            migrationBuilder.AddColumn<int>(
                name: "SalesPriceId",
                table: "InvSalesMan",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceListType",
                table: "InvGeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SalesPriceId",
                table: "InvEmployees",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesPriceId",
                table: "GLBranch",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalesPriceId",
                table: "InvSalesMan");

            migrationBuilder.DropColumn(
                name: "PriceListType",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "SalesPriceId",
                table: "InvEmployees");

            migrationBuilder.DropColumn(
                name: "SalesPriceId",
                table: "GLBranch");

            migrationBuilder.RenameColumn(
                name: "other_ActivatePriceLists",
                table: "InvGeneralSettings",
                newName: "Sales_UseLastPrice");

            migrationBuilder.AddColumn<bool>(
                name: "Pos_UseLastPrice",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            

            migrationBuilder.CreateTable(
                name: "GLJournalEntryDetailsAccounts",
                columns: table => new
                {
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryDetailsAccounts", x => new { x.JournalEntryId, x.FinancialAccountId });
                    table.ForeignKey(
                        name: "FK_GLJournalEntryDetailsAccounts_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryDetailsAccounts_GLJournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "GLJournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryDetailsAccounts_FinancialAccountId",
                table: "GLJournalEntryDetailsAccounts",
                column: "FinancialAccountId");
        }
    }
}
