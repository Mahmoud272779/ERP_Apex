using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class publish_21112023 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                table: "NotificationSeen");

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "SystemHistoryLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "POSSessionHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CollectionReceipts",
                table: "otherSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDiscountRatio",
                table: "OfferPriceMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "VatValueForPrint",
                table: "OfferPriceDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isDiscountRatioItem",
                table: "OfferPriceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvUnitsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvStpItemCardHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvStoresHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvStorePlacesHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvSizesHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvSalesManHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvPurchasesAdditionalCostsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvPersonsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvPaymentMethodsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CollectionMainCode",
                table: "InvoiceMasterHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvoiceMasterHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "eInvoiceCode",
                table: "InvoiceMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "eInvoiceReported",
                table: "InvoiceMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDiscountRatio",
                table: "InvoiceMaster",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "VatValueForPrint",
                table: "InvoiceDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "isDiscountRatioItem",
                table: "InvoiceDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvJobsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ActiveElectronicInvoice",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "InvGeneralSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SystemUpdateNumber",
                table: "InvGeneralSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvGeneralSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvFundsBanksSafesHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvEmployeesHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvDiscount_A_P_History",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvCommissionListHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvColorsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvCategoriesHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CategoryType",
                table: "InvCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "kitchenId",
                table: "InvCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "InvBarcodeHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLSafeHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CollectionMainCode",
                table: "GLRecieptsHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubTypeId",
                table: "GLRecieptsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLRecieptsHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CollectionMainCode",
                table: "GlReciepts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNotes",
                table: "GlReciepts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLRecHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLOtherAuthoritiesHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLJournalEntryDraft",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptsMainCode",
                table: "GLJournalEntryDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLJournalEntry",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLGeneralSetting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLFinancialSetting",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLFinancialAccountHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLFinancialAccountForOpeningBalance",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLFinancialAccount",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLCurrencyHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLCostCenterHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLCostCenter",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLBranchHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLBanksHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isTechnicalSupport",
                table: "GLBalanceForLastPeriod",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GLPrinterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isTechnicalSupport = table.Column<bool>(type: "bit", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLPrinterHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLPrinterHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kitchens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kitchens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KitchensHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isTechnicalSupport = table.Column<bool>(type: "bit", nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitchensHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KitchensHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RstCategoriesPrinters",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PrinterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RstCategoriesPrinters", x => new { x.CategoryId, x.PrinterId });
                    table.ForeignKey(
                        name: "FK_RstCategoriesPrinters_GLPrinter_PrinterId",
                        column: x => x.PrinterId,
                        principalTable: "GLPrinter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RstCategoriesPrinters_InvCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InvCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvCategories_kitchenId",
                table: "InvCategories",
                column: "kitchenId");

            migrationBuilder.CreateIndex(
                name: "IX_GLPrinterHistory_employeesId",
                table: "GLPrinterHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_KitchensHistory_employeesId",
                table: "KitchensHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_RstCategoriesPrinters_PrinterId",
                table: "RstCategoriesPrinters",
                column: "PrinterId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvCategories_Kitchens_kitchenId",
                table: "InvCategories",
                column: "kitchenId",
                principalTable: "Kitchens",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                table: "NotificationSeen",
                column: "NotificationsMasterId",
                principalTable: "NotificationsMaster",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvCategories_Kitchens_kitchenId",
                table: "InvCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                table: "NotificationSeen");

            migrationBuilder.DropTable(
                name: "GLPrinterHistory");

            migrationBuilder.DropTable(
                name: "Kitchens");

            migrationBuilder.DropTable(
                name: "KitchensHistory");

            migrationBuilder.DropTable(
                name: "RstCategoriesPrinters");

            migrationBuilder.DropIndex(
                name: "IX_InvCategories_kitchenId",
                table: "InvCategories");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "SystemHistoryLogs");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "POSSessionHistory");

            migrationBuilder.DropColumn(
                name: "CollectionReceipts",
                table: "otherSettings");

            migrationBuilder.DropColumn(
                name: "isDiscountRatio",
                table: "OfferPriceMaster");

            migrationBuilder.DropColumn(
                name: "VatValueForPrint",
                table: "OfferPriceDetails");

            migrationBuilder.DropColumn(
                name: "isDiscountRatioItem",
                table: "OfferPriceDetails");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvUnitsHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvStpItemCardHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvStoresHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvStorePlacesHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvSizesHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvSalesManHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvPurchasesAdditionalCostsHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvPersonsHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvPaymentMethodsHistory");

            migrationBuilder.DropColumn(
                name: "CollectionMainCode",
                table: "InvoiceMasterHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvoiceMasterHistory");

            migrationBuilder.DropColumn(
                name: "eInvoiceCode",
                table: "InvoiceMaster");

            migrationBuilder.DropColumn(
                name: "eInvoiceReported",
                table: "InvoiceMaster");

            migrationBuilder.DropColumn(
                name: "isDiscountRatio",
                table: "InvoiceMaster");

            migrationBuilder.DropColumn(
                name: "VatValueForPrint",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "isDiscountRatioItem",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvJobsHistory");

            migrationBuilder.DropColumn(
                name: "ActiveElectronicInvoice",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "OTP",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "SystemUpdateNumber",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvGeneralSettings");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvFundsBanksSafesHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvEmployeesHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvDiscount_A_P_History");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvCommissionListHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvColorsHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvCategoriesHistory");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "InvCategories");

            migrationBuilder.DropColumn(
                name: "kitchenId",
                table: "InvCategories");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "InvBarcodeHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLSafeHistory");

            migrationBuilder.DropColumn(
                name: "CollectionMainCode",
                table: "GLRecieptsHistory");

            migrationBuilder.DropColumn(
                name: "SubTypeId",
                table: "GLRecieptsHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLRecieptsHistory");

            migrationBuilder.DropColumn(
                name: "CollectionMainCode",
                table: "GlReciepts");

            migrationBuilder.DropColumn(
                name: "InvoiceNotes",
                table: "GlReciepts");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLRecHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLOtherAuthoritiesHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLJournalEntryDraft");

            migrationBuilder.DropColumn(
                name: "ReceiptsMainCode",
                table: "GLJournalEntryDetails");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLJournalEntry");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLGeneralSetting");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLFinancialSetting");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLFinancialAccountHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLFinancialAccountForOpeningBalance");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLFinancialAccount");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLCurrencyHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLCostCenterHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLCostCenter");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLBranchHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLBanksHistory");

            migrationBuilder.DropColumn(
                name: "isTechnicalSupport",
                table: "GLBalanceForLastPeriod");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                table: "NotificationSeen",
                column: "NotificationsMasterId",
                principalTable: "NotificationsMaster",
                principalColumn: "Id");
        }
    }
}
