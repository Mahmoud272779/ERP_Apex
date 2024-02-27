using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class isExpenses_modifySalesPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pos_ExceedPrices",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "Sales_ExceedPrices",
                table: "InvGeneralSettings");

            migrationBuilder.AddColumn<bool>(
                name: "isExpenses",
                table: "InvoiceMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Pos_ModifyPricesType",
                table: "InvGeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sales_ModifyPricesType",
                table: "InvGeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isExpenses",
                table: "InvoiceMaster");

            migrationBuilder.DropColumn(
                name: "Pos_ModifyPricesType",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "Sales_ModifyPricesType",
                table: "InvGeneralSettings");

            migrationBuilder.AddColumn<bool>(
                name: "Pos_ExceedPrices",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sales_ExceedPrices",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
