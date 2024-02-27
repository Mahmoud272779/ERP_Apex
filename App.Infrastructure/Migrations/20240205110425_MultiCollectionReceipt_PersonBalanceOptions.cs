using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class MultiCollectionReceipt_PersonBalanceOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PurchasesPaymentFromInsideInvoice",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "purchaseShowAllPersonBalance",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "purchaseShowCreditorPersonBaalance",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "purchaseShowDebitorPersonBalance",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "salesPaymentFromInsideInvoice",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "salesShowAllPersonBalance",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "salesShowCreditorPersonBaalance",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "salesShowDebitorPersonBalance",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MultiCollectionReceiptsId",
                table: "InvoiceMasterHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "other_AlertMessageForCustomerSupplierBalance",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "other_AutoExtractFromCustomerSupplierBalance",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "other_DoNothingForCustomerSupplierBalance",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "MultiCollectionReceiptParentId",
                table: "GlReciepts",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchasesPaymentFromInsideInvoice",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "purchaseShowAllPersonBalance",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "purchaseShowCreditorPersonBaalance",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "purchaseShowDebitorPersonBalance",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "salesPaymentFromInsideInvoice",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "salesShowAllPersonBalance",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "salesShowCreditorPersonBaalance",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "salesShowDebitorPersonBalance",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "MultiCollectionReceiptsId",
                table: "InvoiceMasterHistory");

            migrationBuilder.DropColumn(
                name: "other_AlertMessageForCustomerSupplierBalance",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "other_AutoExtractFromCustomerSupplierBalance",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "other_DoNothingForCustomerSupplierBalance",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "MultiCollectionReceiptParentId",
                table: "GlReciepts");
        }
    }
}
