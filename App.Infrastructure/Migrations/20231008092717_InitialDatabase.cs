using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcodePrintFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    File = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodePrintFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeletedRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EditedItems",
                columns: table => new
                {
                    itemId = table.Column<int>(type: "int", nullable: false),
                    sizeId = table.Column<int>(type: "int", nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    serialize = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditedItems", x => new { x.itemId, x.sizeId, x.BranchID });
                });

            migrationBuilder.CreateTable(
                name: "GLBalanceForLastPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Balance = table.Column<double>(type: "float", nullable: false),
                    TotalIncomeList = table.Column<double>(type: "float", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLBalanceForLastPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLBranch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    ManagerPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLBranch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLCostCenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialBalance = table.Column<double>(type: "float", nullable: false),
                    CC_Nature = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutoCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLCostCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLCostCenter_GLCostCenter_ParentId",
                        column: x => x.ParentId,
                        principalTable: "GLCostCenter",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLCurrency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    CoinsAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoinsEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AbbrAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AbbrEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Factor = table.Column<double>(type: "float", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrancySymbol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLCurrency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialAccountForOpeningBalance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialAccountForOpeningBalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLGeneralSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isFundsClosed = table.Column<bool>(type: "bit", nullable: false),
                    AutomaticCoding = table.Column<bool>(type: "bit", nullable: false),
                    MainCode = table.Column<int>(type: "int", nullable: true),
                    SubCode = table.Column<int>(type: "int", nullable: true),
                    evaluationMethodOfEndOfPeriodStockType = table.Column<int>(type: "int", nullable: false),
                    DefultAccCustomer = table.Column<int>(type: "int", nullable: false),
                    DefultAccEmployee = table.Column<int>(type: "int", nullable: false),
                    DefultAccSalesMan = table.Column<int>(type: "int", nullable: false),
                    DefultAccOtherAuthorities = table.Column<int>(type: "int", nullable: false),
                    DefultAccBank = table.Column<int>(type: "int", nullable: false),
                    DefultAccSafe = table.Column<int>(type: "int", nullable: false),
                    DefultAccSupplier = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdCustomer = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdEmployee = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdSalesMan = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdOtherAuthorities = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdBank = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdSafe = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountIdSupplier = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLGeneralSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntryDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CreditTotal = table.Column<double>(type: "float", nullable: false),
                    DebitTotal = table.Column<double>(type: "float", nullable: false),
                    FTDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryDraft", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvBarcodeTemplate",
                columns: table => new
                {
                    BarcodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvBarcodeTemplate", x => x.BarcodeId);
                });

            migrationBuilder.CreateTable(
                name: "InvCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VatValue = table.Column<double>(type: "float", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UsedInSales = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvColors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvColors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvCommissionList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Ratio = table.Column<double>(type: "float", nullable: true),
                    Target = table.Column<double>(type: "float", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCommissionList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvCompanyData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommercialRegister = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    imageFile = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCompanyData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvGeneralSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Purchases_ModifyPrices = table.Column<bool>(type: "bit", nullable: false),
                    Purchases_PayTotalNet = table.Column<bool>(type: "bit", nullable: false),
                    Purchases_UseLastPrice = table.Column<bool>(type: "bit", nullable: false),
                    Purchases_PriceIncludeVat = table.Column<bool>(type: "bit", nullable: false),
                    Purchases_PrintWithSave = table.Column<bool>(type: "bit", nullable: false),
                    Purchases_ReturnWithoutQuantity = table.Column<bool>(type: "bit", nullable: false),
                    Purchases_ActiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    Purchase_UpdateItemsPricesAfterInvoice = table.Column<bool>(type: "bit", nullable: false),
                    Pos_ModifyPrices = table.Column<bool>(type: "bit", nullable: false),
                    Pos_ExceedPrices = table.Column<bool>(type: "bit", nullable: false),
                    Pos_ExceedDiscountRatio = table.Column<bool>(type: "bit", nullable: false),
                    Pos_UseLastPrice = table.Column<bool>(type: "bit", nullable: false),
                    Pos_ActivePricesList = table.Column<bool>(type: "bit", nullable: false),
                    Pos_ExtractWithoutQuantity = table.Column<bool>(type: "bit", nullable: false),
                    Pos_PriceIncludeVat = table.Column<bool>(type: "bit", nullable: false),
                    Pos_ActiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    Pos_DeferredSale = table.Column<bool>(type: "bit", nullable: false),
                    Pos_IndividualCoding = table.Column<bool>(type: "bit", nullable: false),
                    Pos_PreventEditingRecieptFlag = table.Column<bool>(type: "bit", nullable: false),
                    Pos_PreventEditingRecieptValue = table.Column<int>(type: "int", nullable: false),
                    Pos_ActiveCashierCustody = table.Column<bool>(type: "bit", nullable: false),
                    Pos_PrintPreview = table.Column<bool>(type: "bit", nullable: false),
                    Pos_PrintWithEnding = table.Column<bool>(type: "bit", nullable: false),
                    Pos_EditingOnDate = table.Column<bool>(type: "bit", nullable: false),
                    Sales_ModifyPrices = table.Column<bool>(type: "bit", nullable: false),
                    Sales_ExceedPrices = table.Column<bool>(type: "bit", nullable: false),
                    Sales_ExceedDiscountRatio = table.Column<bool>(type: "bit", nullable: false),
                    Sales_PayTotalNet = table.Column<bool>(type: "bit", nullable: false),
                    Sales_UseLastPrice = table.Column<bool>(type: "bit", nullable: false),
                    Sales_ExtractWithoutQuantity = table.Column<bool>(type: "bit", nullable: false),
                    Sales_PriceIncludeVat = table.Column<bool>(type: "bit", nullable: false),
                    Sales_PrintWithSave = table.Column<bool>(type: "bit", nullable: false),
                    Sales_ActiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    Sales_LinkRepresentCustomer = table.Column<bool>(type: "bit", nullable: false),
                    Sales_ActivePricesList = table.Column<bool>(type: "bit", nullable: false),
                    Other_MergeItems = table.Column<bool>(type: "bit", nullable: false),
                    otherMergeItemMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other_ItemsAutoCoding = table.Column<bool>(type: "bit", nullable: false),
                    Other_ZeroPricesInItems = table.Column<bool>(type: "bit", nullable: false),
                    Other_PrintSerials = table.Column<bool>(type: "bit", nullable: false),
                    Other_AutoExtractExpireDate = table.Column<bool>(type: "bit", nullable: false),
                    Other_ViewStorePlace = table.Column<bool>(type: "bit", nullable: false),
                    Other_ConfirmeSupplierPhone = table.Column<bool>(type: "bit", nullable: false),
                    Other_ConfirmeCustomerPhone = table.Column<bool>(type: "bit", nullable: false),
                    Other_DemandLimitNotification = table.Column<bool>(type: "bit", nullable: false),
                    Other_ExpireNotificationFlag = table.Column<bool>(type: "bit", nullable: false),
                    Other_ExpireNotificationValue = table.Column<int>(type: "int", nullable: false),
                    Other_Decimals = table.Column<int>(type: "int", nullable: false),
                    Other_ShowBalanceOfPerson = table.Column<bool>(type: "bit", nullable: false),
                    Other_UseRoundNumber = table.Column<bool>(type: "bit", nullable: false),
                    Funds_Items = table.Column<bool>(type: "bit", nullable: false),
                    Funds_Supplires = table.Column<bool>(type: "bit", nullable: false),
                    Funds_Supplires_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Funds_Customers = table.Column<bool>(type: "bit", nullable: false),
                    Funds_Customers_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Funds_Safes = table.Column<bool>(type: "bit", nullable: false),
                    Funds_Banks = table.Column<bool>(type: "bit", nullable: false),
                    barcodeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode_ItemCodestart = table.Column<bool>(type: "bit", nullable: false),
                    Vat_Active = table.Column<bool>(type: "bit", nullable: false),
                    Vat_DefaultValue = table.Column<double>(type: "float", nullable: false),
                    Accredite_StartPeriod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Accredite_EndPeriod = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerDisplay_Active = table.Column<bool>(type: "bit", nullable: false),
                    CustomerDisplay_PortNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerDisplay_Code = table.Column<int>(type: "int", nullable: false),
                    CustomerDisplay_LinesNumber = table.Column<int>(type: "int", nullable: false),
                    CustomerDisplay_CharNumber = table.Column<int>(type: "int", nullable: false),
                    CustomerDisplay_DefaultWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerDisplay_ScreenType = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailHost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailPort = table.Column<int>(type: "int", nullable: false),
                    secureSocketOptions = table.Column<int>(type: "int", nullable: false),
                    autoLogoutInMints = table.Column<int>(type: "int", nullable: false),
                    UpdateFilesNumber = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvGeneralSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceSerialize",
                columns: table => new
                {
                    InvoiceSerializeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    InvoiceCode = table.Column<int>(type: "int", nullable: false),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceSerialize", x => x.InvoiceSerializeId);
                });

            migrationBuilder.CreateTable(
                name: "InvPersonLastPrice",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    personId = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    unitId = table.Column<int>(type: "int", nullable: false),
                    invoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPersonLastPrice", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "InvPurchasesAdditionalCosts",
                columns: table => new
                {
                    PurchasesAdditionalCostsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalType = table.Column<int>(type: "int", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPurchasesAdditionalCosts", x => x.PurchasesAdditionalCostsId);
                });

            migrationBuilder.CreateTable(
                name: "InvSizes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSizes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvStorePlaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorePlaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvStpUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "permissionList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    arabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    latinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissionList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POS_OfflineDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    DeleteWaiting = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POS_OfflineDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POSDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "POSTouchSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PosTouch_FontSize = table.Column<double>(type: "float", nullable: false),
                    PosTouch_CategoryImgWidth = table.Column<double>(type: "float", nullable: false),
                    PosTouch_CategoryImgHeight = table.Column<double>(type: "float", nullable: false),
                    PosTouch_TableWidth = table.Column<double>(type: "float", nullable: false),
                    PosTouch_ItemsImgWidth = table.Column<double>(type: "float", nullable: false),
                    PosTouch_ItemsImgHeight = table.Column<double>(type: "float", nullable: false),
                    PosTouch_DisplayItemPrice = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSTouchSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MacAdress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistoryType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "reportFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportFileNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsArabic = table.Column<bool>(type: "bit", nullable: false),
                    IsReport = table.Column<int>(type: "int", nullable: false),
                    Files = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    uTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reportFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "screenNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreenNameAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScreenNameEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_screenNames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransferDataFromDeskTop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfTransfer = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferDataFromDeskTop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GLPrinter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IP = table.Column<string>(type: "char(15)", fixedLength: true, maxLength: 15, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLPrinter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLPrinter_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStpStores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GLBranchId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStpStores_GLBranch_GLBranchId",
                        column: x => x.GLBranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    autoCoding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainCode = table.Column<int>(type: "int", nullable: false),
                    SubCode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FA_Nature = table.Column<int>(type: "int", nullable: false),
                    FinalAccount = table.Column<int>(type: "int", nullable: false),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    OpenningCredit = table.Column<double>(type: "float", nullable: false),
                    OpenningDebit = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    HasCostCenter = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLFinancialAccount_GLCurrency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "GLCurrency",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GLFinancialAccount_GLFinancialAccount_ParentId",
                        column: x => x.ParentId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreditTotal = table.Column<double>(type: "float", nullable: false),
                    DebitTotal = table.Column<double>(type: "float", nullable: false),
                    FTDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    IsTransfer = table.Column<bool>(type: "bit", nullable: false),
                    IsAccredit = table.Column<bool>(type: "bit", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Auto = table.Column<bool>(type: "bit", nullable: false),
                    IsCompined = table.Column<bool>(type: "bit", nullable: false),
                    ReceiptsId = table.Column<int>(type: "int", nullable: true),
                    CompinedReceiptCode = table.Column<int>(type: "int", nullable: true),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    DocType = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntry_GLCurrency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "GLCurrency",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "subCodeLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<int>(type: "int", nullable: false),
                    GLGeneralSettingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subCodeLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subCodeLevels_GLGeneralSetting_GLGeneralSettingId",
                        column: x => x.GLGeneralSettingId,
                        principalTable: "GLGeneralSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntryDraftDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryDraftId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryDraftDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryDraftDetails_GLJournalEntryDraft_JournalEntryDraftId",
                        column: x => x.JournalEntryDraftId,
                        principalTable: "GLJournalEntryDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntryFilesDraft",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JournalEntryDraftId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryFilesDraft", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryFilesDraft_GLJournalEntryDraft_JournalEntryDraftId",
                        column: x => x.JournalEntryDraftId,
                        principalTable: "GLJournalEntryDraft",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvBarcodeItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BarcodeId = table.Column<int>(type: "int", nullable: false),
                    BarcodeItemType = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<double>(type: "float", nullable: false),
                    Height = table.Column<double>(type: "float", nullable: false),
                    X = table.Column<double>(type: "float", nullable: false),
                    Y = table.Column<double>(type: "float", nullable: false),
                    PositionX = table.Column<double>(type: "float", nullable: false),
                    PositionY = table.Column<double>(type: "float", nullable: false),
                    FontType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FontSize = table.Column<double>(type: "float", nullable: false),
                    Bold = table.Column<bool>(type: "bit", nullable: false),
                    Italic = table.Column<bool>(type: "bit", nullable: false),
                    UnderLine = table.Column<bool>(type: "bit", nullable: false),
                    AlignX = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlignY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dock = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsLogo = table.Column<bool>(type: "bit", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    TextType = table.Column<int>(type: "int", nullable: false),
                    TextContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VariableContent = table.Column<int>(type: "int", nullable: false),
                    BeginSplitter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndSplitter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BarcodeType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvBarcodeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvBarcodeItems_InvBarcodeTemplate_BarcodeId",
                        column: x => x.BarcodeId,
                        principalTable: "InvBarcodeTemplate",
                        principalColumn: "BarcodeId");
                });

            migrationBuilder.CreateTable(
                name: "InvCommissionSlides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommissionId = table.Column<int>(type: "int", nullable: false),
                    SlideNo = table.Column<int>(type: "int", nullable: false),
                    SlideFrom = table.Column<double>(type: "float", nullable: false),
                    SlideTo = table.Column<double>(type: "float", nullable: false),
                    Ratio = table.Column<double>(type: "float", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCommissionSlides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvCommissionSlides_InvCommissionList_CommissionId",
                        column: x => x.CommissionId,
                        principalTable: "InvCommissionList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NationalBarcode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    UsedInSales = table.Column<bool>(type: "bit", nullable: false),
                    DepositeUnit = table.Column<int>(type: "int", nullable: true),
                    WithdrawUnit = table.Column<int>(type: "int", nullable: true),
                    ReportUnit = table.Column<int>(type: "int", nullable: true),
                    VAT = table.Column<double>(type: "float", nullable: false),
                    ApplyVAT = table.Column<bool>(type: "bit", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultStoreId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    ImageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStpItemMaster_InvCategories_GroupId",
                        column: x => x.GroupId,
                        principalTable: "InvCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStpItemMaster_InvStorePlaces_DefaultStoreId",
                        column: x => x.DefaultStoreId,
                        principalTable: "InvStorePlaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    mainFormCode = table.Column<int>(type: "int", nullable: false),
                    subFormCode = table.Column<int>(type: "int", nullable: false),
                    applicationId = table.Column<int>(type: "int", nullable: false),
                    isVisible = table.Column<bool>(type: "bit", nullable: false),
                    arabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    latinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    permissionListId = table.Column<int>(type: "int", nullable: false),
                    isShow = table.Column<bool>(type: "bit", nullable: false),
                    isAdd = table.Column<bool>(type: "bit", nullable: false),
                    isEdit = table.Column<bool>(type: "bit", nullable: false),
                    isDelete = table.Column<bool>(type: "bit", nullable: false),
                    isPrint = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rules_permissionList_permissionListId",
                        column: x => x.permissionListId,
                        principalTable: "permissionList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportManger",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    screenId = table.Column<int>(type: "int", nullable: false),
                    ArabicFilenameId = table.Column<int>(type: "int", nullable: false),
                    IsArabic = table.Column<bool>(type: "bit", nullable: false),
                    Copies = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportManger", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportManger_reportFiles_ArabicFilenameId",
                        column: x => x.ArabicFilenameId,
                        principalTable: "reportFiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReportManger_screenNames_screenId",
                        column: x => x.screenId,
                        principalTable: "screenNames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvStoreBranch",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStoreBranch", x => new { x.StoreId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_InvStoreBranch_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStoreBranch_InvStpStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ArabicAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatinAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLBanks_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialAccountBranch",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialAccountBranch", x => new { x.BranchId, x.FinancialAccountId });
                    table.ForeignKey(
                        name: "FK_GLFinancialAccountBranch_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLFinancialAccountBranch_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialBranch",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    FinancialId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialBranch", x => new { x.BranchId, x.FinancialId });
                    table.ForeignKey(
                        name: "FK_GLFinancialBranch_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLFinancialBranch_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialCost",
                columns: table => new
                {
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialCost", x => new { x.CostCenterId, x.FinancialAccountId });
                    table.ForeignKey(
                        name: "FK_GLFinancialCost_GLCostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "GLCostCenter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLFinancialCost_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialSetting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsAssumption = table.Column<bool>(type: "bit", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    UseFinancialAccount = table.Column<bool>(type: "bit", nullable: false),
                    AddUnderFinancialAccount = table.Column<bool>(type: "bit", nullable: false),
                    IsBanks = table.Column<bool>(type: "bit", nullable: false),
                    IsOthorAuthorities = table.Column<bool>(type: "bit", nullable: false),
                    IsTreasuries = table.Column<bool>(type: "bit", nullable: false),
                    IsCustomers = table.Column<bool>(type: "bit", nullable: false),
                    IsSuppliers = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesRepresentatives = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLFinancialSetting_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLIntegrationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    linkingMethodId = table.Column<int>(type: "int", nullable: false),
                    screenId = table.Column<int>(type: "int", nullable: false),
                    GLFinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    GLBranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLIntegrationSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLIntegrationSettings_GLBranch_GLBranchId",
                        column: x => x.GLBranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLIntegrationSettings_GLFinancialAccount_GLFinancialAccountId",
                        column: x => x.GLFinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLPurchasesAndSalesSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptElemntID = table.Column<int>(type: "int", nullable: false),
                    RecieptsType = table.Column<int>(type: "int", nullable: false),
                    MainType = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    branchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLPurchasesAndSalesSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLPurchasesAndSalesSettings_GLBranch_branchId",
                        column: x => x.branchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLPurchasesAndSalesSettings_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLSafe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLSafe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLSafe_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GLSafe_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvEmployees_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvEmployees_InvJobs_JobId",
                        column: x => x.JobId,
                        principalTable: "InvJobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvSalesMan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplyToCommissions = table.Column<bool>(type: "bit", nullable: false),
                    CommissionListId = table.Column<int>(type: "int", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSalesMan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvSalesMan_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvSalesMan_InvCommissionList_CommissionListId",
                        column: x => x.CommissionListId,
                        principalTable: "InvCommissionList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OtherAuthorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherAuthorities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherAuthorities_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OtherAuthorities_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    FinancialCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isCostSales = table.Column<bool>(type: "bit", nullable: false),
                    isStoreFund = table.Column<bool>(type: "bit", nullable: false),
                    StoreFundId = table.Column<int>(type: "int", nullable: true),
                    DocType = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryDetails_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GLJournalEntryDetails_GLJournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "GLJournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLJournalEntryDetailsAccounts",
                columns: table => new
                {
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "GLJournalEntryFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    File = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLJournalEntryFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLJournalEntryFiles_GLJournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "GLJournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvSerialTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    AddedInvoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtractInvoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    indexOfSerialForAdd = table.Column<int>(type: "int", nullable: false),
                    indexOfSerialForExtract = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    IsAccridited = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TransferStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSerialTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvSerialTransaction_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvSerialTransaction_InvStpStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemCardParts",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PartId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemCardParts", x => new { x.ItemId, x.PartId });
                    table.ForeignKey(
                        name: "FK_InvStpItemCardParts_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStpItemCardParts_InvStpItemMaster_PartId",
                        column: x => x.PartId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvStpItemCardParts_InvStpUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "InvStpUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemCardSerials",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemCardSerials", x => new { x.ItemId, x.SerialNo });
                    table.UniqueConstraint("AK_InvStpItemCardSerials_SerialNo", x => x.SerialNo);
                    table.ForeignKey(
                        name: "FK_InvStpItemCardSerials_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemStores",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DemandLimit = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemStores", x => new { x.ItemId, x.StoreId });
                    table.ForeignKey(
                        name: "FK_InvStpItemStores_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStpItemStores_InvStpStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemUnit",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    ConversionFactor = table.Column<double>(type: "float", nullable: false),
                    PurchasePrice = table.Column<double>(type: "float", nullable: false),
                    SalePrice1 = table.Column<double>(type: "float", nullable: false),
                    SalePrice2 = table.Column<double>(type: "float", nullable: false),
                    SalePrice3 = table.Column<double>(type: "float", nullable: false),
                    SalePrice4 = table.Column<double>(type: "float", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WillDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemUnit", x => new { x.ItemId, x.UnitId });
                    table.ForeignKey(
                        name: "FK_InvStpItemUnit_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStpItemUnit_InvStpUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "InvStpUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLBankBranch",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLBankBranch", x => new { x.BankId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_GLBankBranch_GLBanks_BankId",
                        column: x => x.BankId,
                        principalTable: "GLBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GLBankBranch_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvFundsBanksSafesMaster",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    DocDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    SafeId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBank = table.Column<bool>(type: "bit", nullable: false),
                    IsSafe = table.Column<bool>(type: "bit", nullable: false),
                    isBlock = table.Column<bool>(type: "bit", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvFundsBanksSafesMaster", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_InvFundsBanksSafesMaster_GLBanks_BankId",
                        column: x => x.BankId,
                        principalTable: "GLBanks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvFundsBanksSafesMaster_GLSafe_SafeId",
                        column: x => x.SafeId,
                        principalTable: "GLSafe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvPaymentMethods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafeId = table.Column<int>(type: "int", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPaymentMethods", x => x.PaymentMethodId);
                    table.ForeignKey(
                        name: "FK_InvPaymentMethods_GLBanks_BankId",
                        column: x => x.BankId,
                        principalTable: "GLBanks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvPaymentMethods_GLSafe_SafeId",
                        column: x => x.SafeId,
                        principalTable: "GLSafe",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "chatGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    groupCreatorId = table.Column<int>(type: "int", nullable: false),
                    allowReply = table.Column<bool>(type: "bit", nullable: false),
                    groupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    groupImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isEnded = table.Column<bool>(type: "bit", nullable: false),
                    canExit = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chatGroups_InvEmployees_groupCreatorId",
                        column: x => x.groupCreatorId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLBanksHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AddressAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLBanksHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLBanksHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLBranchHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLBranchHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLBranchHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLCostCenterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
                    InitialBalance = table.Column<double>(type: "float", nullable: false),
                    CC_Nature = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLCostCenterHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLCostCenterHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLCurrencyHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuerrncyId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLCurrencyHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLCurrencyHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLFinancialAccountHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainCode = table.Column<int>(type: "int", nullable: false),
                    SubCode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FA_Nature = table.Column<int>(type: "int", nullable: false),
                    FinalAccount = table.Column<int>(type: "int", nullable: false),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    HasCostCenter = table.Column<int>(type: "int", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLFinancialAccountHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLFinancialAccountHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLOtherAuthoritiesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLOtherAuthoritiesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLOtherAuthoritiesHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLRecHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MacAdress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLRecHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLRecHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLRecieptsHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiptsId = table.Column<int>(type: "int", nullable: false),
                    SafeIDOrBank = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    RecieptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    AuthorityType = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    ChequeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieptTypeId = table.Column<int>(type: "int", nullable: false),
                    RecieptType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signal = table.Column<int>(type: "int", nullable: false),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsAccredit = table.Column<bool>(type: "bit", nullable: false),
                    BenefitId = table.Column<int>(type: "int", nullable: false),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    ReceiptsAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLRecieptsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLRecieptsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLSafeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreasuryId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLSafeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLSafeHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvBarcodeHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvBarcodeHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvBarcodeHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvCategoriesHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCategoriesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvCategoriesHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvColorsHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvColorsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvColorsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvCommissionListHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvCommissionListHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvCommissionListHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvDiscount_A_P_History",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvDiscount_A_P_History", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvDiscount_A_P_History_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvEmployeesBranches",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    current = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvEmployeesBranches", x => new { x.EmployeeId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_InvEmployeesBranches_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvEmployeesBranches_InvEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvEmployeesHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvEmployeesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvEmployeesHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvFundsBanksSafesHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvFundsBanksSafesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvFundsBanksSafesHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvJobsHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvJobsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvJobsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceMasterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    InvoiceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    ParentInvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    SubType = table.Column<int>(type: "int", nullable: false),
                    LastAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUserAr = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceMasterHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceMasterHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvPaymentMethodsHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPaymentMethodsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvPaymentMethodsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvPersonsHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPersonsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvPersonsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvPurchasesAdditionalCostsHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPurchasesAdditionalCostsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvPurchasesAdditionalCostsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvSalesManHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSalesManHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvSalesManHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvSizesHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSizesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvSizesHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStorePlacesHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStorePlacesHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStorePlacesHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStoresHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStoresHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStoresHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemCardHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemCardHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvStpItemCardHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvUnitsHistory",
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
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvUnitsHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvUnitsHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationsMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Desc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    titleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isSystem = table.Column<bool>(type: "bit", nullable: false),
                    pageId = table.Column<int>(type: "int", nullable: true),
                    specialUserId = table.Column<int>(type: "int", nullable: true),
                    cDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    routeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insertedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationsMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationsMaster_InvEmployees_insertedById",
                        column: x => x.insertedById,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationsMaster_InvEmployees_specialUserId",
                        column: x => x.specialUserId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "POSSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sessionCode = table.Column<int>(type: "int", nullable: false),
                    employeeId = table.Column<int>(type: "int", nullable: false),
                    start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end = table.Column<DateTime>(type: "datetime2", nullable: true),
                    sessionClosedById = table.Column<int>(type: "int", nullable: true),
                    sessionStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POSSession_InvEmployees_employeeId",
                        column: x => x.employeeId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_POSSession_InvEmployees_sessionClosedById",
                        column: x => x.sessionClosedById,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "signalR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    connectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvEmployeesId = table.Column<int>(type: "int", nullable: false),
                    isOnline = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signalR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_signalR_InvEmployees_InvEmployeesId",
                        column: x => x.InvEmployeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemHistoryLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    employeesId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ActionArabicName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActionLatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemHistoryLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemHistoryLogs_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemHistoryLogs_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "userAccount",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    employeesId = table.Column<int>(type: "int", nullable: false),
                    permissionListId = table.Column<int>(type: "int", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userAccount", x => x.id);
                    table.ForeignKey(
                        name: "FK_userAccount_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userAccount_permissionList_permissionListId",
                        column: x => x.permissionListId,
                        principalTable: "permissionList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvPersons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LatinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SalesManId = table.Column<int>(type: "int", nullable: true),
                    ResponsibleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponsibleEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditLimit = table.Column<double>(type: "float", nullable: true),
                    CreditPeriod = table.Column<int>(type: "int", nullable: true),
                    DiscountRatio = table.Column<double>(type: "float", nullable: true),
                    SalesPriceId = table.Column<int>(type: "int", nullable: true),
                    InvEmployeesId = table.Column<int>(type: "int", nullable: true),
                    LessSalesPriceId = table.Column<int>(type: "int", nullable: true),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsSupplier = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    AddToAnotherList = table.Column<bool>(type: "bit", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CodeT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuildingNumber = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    StreetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Neighborhood = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    City = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    PostalNumber = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvPersons_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvPersons_InvEmployees_InvEmployeesId",
                        column: x => x.InvEmployeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvPersons_InvSalesMan_SalesManId",
                        column: x => x.SalesManId,
                        principalTable: "InvSalesMan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvSalesMan_Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    SalesManId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvSalesMan_Branches", x => new { x.BranchId, x.SalesManId });
                    table.ForeignKey(
                        name: "FK_InvSalesMan_Branches_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvSalesMan_Branches_InvSalesMan_SalesManId",
                        column: x => x.SalesManId,
                        principalTable: "InvSalesMan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvStpItemColorSize",
                columns: table => new
                {
                    ColorId = table.Column<int>(type: "int", nullable: false),
                    SizeId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    PurchasePrice = table.Column<double>(type: "float", nullable: false),
                    SalePrice1 = table.Column<double>(type: "float", nullable: false),
                    SalePrice2 = table.Column<double>(type: "float", nullable: false),
                    SalePrice3 = table.Column<double>(type: "float", nullable: false),
                    SalePrice4 = table.Column<double>(type: "float", nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WillDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvStpItemColorSize", x => new { x.ItemId, x.UnitId, x.ColorId, x.SizeId });
                    table.ForeignKey(
                        name: "FK_InvStpItemColorSize_InvColors_ColorId",
                        column: x => x.ColorId,
                        principalTable: "InvColors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvStpItemColorSize_InvSizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "InvSizes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvStpItemColorSize_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvStpItemColorSize_InvStpItemUnit_ItemId_UnitId",
                        columns: x => new { x.ItemId, x.UnitId },
                        principalTable: "InvStpItemUnit",
                        principalColumns: new[] { "ItemId", "UnitId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvStpItemColorSize_InvStpUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "InvStpUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvFundsBanksSafesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    Debtor = table.Column<double>(type: "float", nullable: false),
                    Creditor = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvFundsBanksSafesDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvFundsBanksSafesDetails_InvFundsBanksSafesMaster_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "InvFundsBanksSafesMaster",
                        principalColumn: "DocumentId");
                });

            migrationBuilder.CreateTable(
                name: "chatGroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    groupId = table.Column<int>(type: "int", nullable: false),
                    memberId = table.Column<int>(type: "int", nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chatGroupMembers_chatGroups_groupId",
                        column: x => x.groupId,
                        principalTable: "chatGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chatGroupMembers_InvEmployees_memberId",
                        column: x => x.memberId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "chatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fromId = table.Column<int>(type: "int", nullable: false),
                    toId = table.Column<int>(type: "int", nullable: true),
                    groupId = table.Column<int>(type: "int", nullable: true),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    seen = table.Column<bool>(type: "bit", nullable: false),
                    seenDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    deleteDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isPrivate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chatMessages_chatGroups_groupId",
                        column: x => x.groupId,
                        principalTable: "chatGroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chatMessages_InvEmployees_fromId",
                        column: x => x.fromId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_chatMessages_InvEmployees_toId",
                        column: x => x.toId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationSeen",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationsMasterId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSeen", x => x.id);
                    table.ForeignKey(
                        name: "FK_NotificationSeen_InvEmployees_UserId",
                        column: x => x.UserId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificationSeen_NotificationsMaster_NotificationsMasterId",
                        column: x => x.NotificationsMasterId,
                        principalTable: "NotificationsMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "POSSessionHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    POSSessionId = table.Column<int>(type: "int", nullable: false),
                    actionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    actionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    employeesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSSessionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POSSessionHistory_InvEmployees_employeesId",
                        column: x => x.employeesId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSSessionHistory_POSSession_POSSessionId",
                        column: x => x.POSSessionId,
                        principalTable: "POSSession",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "otherSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    posAddDiscount = table.Column<bool>(type: "bit", nullable: false),
                    posAllowCreditSales = table.Column<bool>(type: "bit", nullable: false),
                    posEditOtherPersonsInv = table.Column<bool>(type: "bit", nullable: false),
                    posShowOtherPersonsInv = table.Column<bool>(type: "bit", nullable: false),
                    posShowReportsOfOtherPersons = table.Column<bool>(type: "bit", nullable: false),
                    allowCloseCloudPOSSession = table.Column<bool>(type: "bit", nullable: false),
                    canShowAllPOSSessions = table.Column<bool>(type: "bit", nullable: false),
                    posCashPayment = table.Column<bool>(type: "bit", nullable: false),
                    posNetPayment = table.Column<bool>(type: "bit", nullable: false),
                    posOtherPayment = table.Column<bool>(type: "bit", nullable: false),
                    salesAddDiscount = table.Column<bool>(type: "bit", nullable: false),
                    salesAllowCreditSales = table.Column<bool>(type: "bit", nullable: false),
                    salesEditOtherPersonsInv = table.Column<bool>(type: "bit", nullable: false),
                    salesShowOtherPersonsInv = table.Column<bool>(type: "bit", nullable: false),
                    salesShowReportsOfOtherPersons = table.Column<bool>(type: "bit", nullable: false),
                    salesCashPayment = table.Column<bool>(type: "bit", nullable: false),
                    salesNetPayment = table.Column<bool>(type: "bit", nullable: false),
                    salesOtherPayment = table.Column<bool>(type: "bit", nullable: false),
                    salesShowBalanceOfPerson = table.Column<bool>(type: "bit", nullable: false),
                    purchasesAddDiscount = table.Column<bool>(type: "bit", nullable: false),
                    purchasesAllowCreditSales = table.Column<bool>(type: "bit", nullable: false),
                    purchasesEditOtherPersonsInv = table.Column<bool>(type: "bit", nullable: false),
                    purchasesShowOtherPersonsInv = table.Column<bool>(type: "bit", nullable: false),
                    purchasesShowReportsOfOtherPersons = table.Column<bool>(type: "bit", nullable: false),
                    purchaseShowBalanceOfPerson = table.Column<bool>(type: "bit", nullable: false),
                    PurchasesCashPayment = table.Column<bool>(type: "bit", nullable: false),
                    PurchasesNetPayment = table.Column<bool>(type: "bit", nullable: false),
                    PurchasesOtherPayment = table.Column<bool>(type: "bit", nullable: false),
                    showHistory = table.Column<bool>(type: "bit", nullable: false),
                    accredditForAllUsers = table.Column<bool>(type: "bit", nullable: false),
                    showCustomersOfOtherUsers = table.Column<bool>(type: "bit", nullable: false),
                    showOfferPricesOfOtherUser = table.Column<bool>(type: "bit", nullable: false),
                    showDashboardForAllUsers = table.Column<bool>(type: "bit", nullable: false),
                    AllowPrintBarcode = table.Column<bool>(type: "bit", nullable: false),
                    showAllBranchesInCustomerInfo = table.Column<bool>(type: "bit", nullable: false),
                    showAllBranchesInSuppliersInfo = table.Column<bool>(type: "bit", nullable: false),
                    userAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_otherSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_otherSettings_userAccount_userAccountId",
                        column: x => x.userAccountId,
                        principalTable: "userAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "signinLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userAccountid = table.Column<int>(type: "int", nullable: false),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    signinDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    lastActionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isLogout = table.Column<bool>(type: "bit", nullable: false),
                    isLocked = table.Column<bool>(type: "bit", nullable: false),
                    logoutDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    stayLoggedin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signinLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_signinLogs_userAccount_userAccountid",
                        column: x => x.userAccountid,
                        principalTable: "userAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAndPermission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userAccountId = table.Column<int>(type: "int", nullable: false),
                    permissionListId = table.Column<int>(type: "int", nullable: false),
                    UTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAndPermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAndPermission_permissionList_permissionListId",
                        column: x => x.permissionListId,
                        principalTable: "permissionList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAndPermission_userAccount_userAccountId",
                        column: x => x.userAccountId,
                        principalTable: "userAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userAccountId = table.Column<int>(type: "int", nullable: false),
                    GLBranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userBranches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userBranches_GLBranch_GLBranchId",
                        column: x => x.GLBranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_userBranches_userAccount_userAccountId",
                        column: x => x.userAccountId,
                        principalTable: "userAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usersForgetPassword",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usersForgetPassword", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usersForgetPassword_userAccount_userId",
                        column: x => x.userId,
                        principalTable: "userAccount",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GlReciepts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SafeID = table.Column<int>(type: "int", nullable: true),
                    BankId = table.Column<int>(type: "int", nullable: true),
                    EntryFundId = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    PaperNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecieptDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Authority = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    ChequeNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChequeBankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChequeDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecieptTypeId = table.Column<int>(type: "int", nullable: false),
                    RecieptType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Signal = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ParentTypeId = table.Column<int>(type: "int", nullable: true),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsAccredit = table.Column<bool>(type: "bit", nullable: false),
                    IsCompined = table.Column<bool>(type: "bit", nullable: false),
                    CompinedParentId = table.Column<int>(type: "int", nullable: true),
                    NoteAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoteEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creditor = table.Column<double>(type: "float", nullable: false),
                    Debtor = table.Column<double>(type: "float", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    otherSalesManId = table.Column<int>(type: "int", nullable: false),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    IsIncludeVat = table.Column<bool>(type: "bit", nullable: false),
                    BenefitId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    SalesManId = table.Column<int>(type: "int", nullable: true),
                    OtherAuthorityId = table.Column<int>(type: "int", nullable: true),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    SubTypeId = table.Column<int>(type: "int", nullable: false),
                    CollectionCode = table.Column<int>(type: "int", nullable: false),
                    Deferre = table.Column<bool>(type: "bit", nullable: false),
                    RectypeWithPayment = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    isPartialPaid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlReciepts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GlReciepts_GLBanks_BankId",
                        column: x => x.BankId,
                        principalTable: "GLBanks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlReciepts_GLFinancialAccount_FinancialAccountId",
                        column: x => x.FinancialAccountId,
                        principalTable: "GLFinancialAccount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlReciepts_GLSafe_SafeID",
                        column: x => x.SafeID,
                        principalTable: "GLSafe",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlReciepts_InvPaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "InvPaymentMethods",
                        principalColumn: "PaymentMethodId");
                    table.ForeignKey(
                        name: "FK_GlReciepts_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlReciepts_InvSalesMan_SalesManId",
                        column: x => x.SalesManId,
                        principalTable: "InvSalesMan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GlReciepts_OtherAuthorities_OtherAuthorityId",
                        column: x => x.OtherAuthorityId,
                        principalTable: "OtherAuthorities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvDiscount_A_P",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    DocNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaperNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Creditor = table.Column<double>(type: "float", nullable: false),
                    Debtor = table.Column<double>(type: "float", nullable: false),
                    DocType = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Refrience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    recieptsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvDiscount_A_P", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvDiscount_A_P_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvFundsCustomerSupplier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Credit = table.Column<double>(type: "float", nullable: false),
                    Debit = table.Column<double>(type: "float", nullable: false),
                    LastTransactionAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeleteTransactionDate = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvFundsCustomerSupplier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvFundsCustomerSupplier_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceMaster",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    InvoiceType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    ParentInvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    StoreIdTo = table.Column<int>(type: "int", nullable: true),
                    transferStatus = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferNotesAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferNotesEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceSubTypesId = table.Column<int>(type: "int", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAccredite = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    TotalDiscountValue = table.Column<double>(type: "float", nullable: false),
                    TotalDiscountRatio = table.Column<double>(type: "float", nullable: false),
                    Net = table.Column<double>(type: "float", nullable: false),
                    Paid = table.Column<double>(type: "float", nullable: false),
                    Remain = table.Column<double>(type: "float", nullable: false),
                    VirualPaid = table.Column<double>(type: "float", nullable: false),
                    TotalAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    TotalVat = table.Column<double>(type: "float", nullable: false),
                    ApplyVat = table.Column<bool>(type: "bit", nullable: false),
                    PriceWithVat = table.Column<bool>(type: "bit", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    ActiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    IsReturn = table.Column<bool>(type: "bit", nullable: false),
                    SalesManId = table.Column<int>(type: "int", nullable: true),
                    PriceListId = table.Column<int>(type: "int", nullable: true),
                    InvoiceTransferType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    POSSessionId = table.Column<int>(type: "int", nullable: true),
                    ActualNet = table.Column<double>(type: "float", nullable: false),
                    CodeOfflinePOS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalOtherAdditions = table.Column<double>(type: "float", nullable: false),
                    IsCollectionReceipt = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceMaster", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_InvEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_InvSalesMan_SalesManId",
                        column: x => x.SalesManId,
                        principalTable: "InvSalesMan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_InvStpStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceMaster_InvStpStores_StoreIdTo",
                        column: x => x.StoreIdTo,
                        principalTable: "InvStpStores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvPersons_Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPersons_Branches", x => new { x.BranchId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_InvPersons_Branches_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvPersons_Branches_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OfferPriceMaster",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    InvoiceType = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    ParentInvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    transferStatus = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferNotesAR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferNotesEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceSubTypesId = table.Column<int>(type: "int", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAccredite = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    TotalDiscountValue = table.Column<double>(type: "float", nullable: false),
                    TotalDiscountRatio = table.Column<double>(type: "float", nullable: false),
                    Net = table.Column<double>(type: "float", nullable: false),
                    Paid = table.Column<double>(type: "float", nullable: false),
                    Remain = table.Column<double>(type: "float", nullable: false),
                    VirualPaid = table.Column<double>(type: "float", nullable: false),
                    TotalAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    TotalVat = table.Column<double>(type: "float", nullable: false),
                    ApplyVat = table.Column<bool>(type: "bit", nullable: false),
                    PriceWithVat = table.Column<bool>(type: "bit", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    ActiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    IsReturn = table.Column<bool>(type: "bit", nullable: false),
                    SalesManId = table.Column<int>(type: "int", nullable: true),
                    PriceListId = table.Column<int>(type: "int", nullable: true),
                    InvoiceTransferType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoundNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferPriceMaster", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_OfferPriceMaster_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceMaster_InvEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceMaster_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceMaster_InvSalesMan_SalesManId",
                        column: x => x.SalesManId,
                        principalTable: "InvSalesMan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceMaster_InvStpStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "POSInvoiceSuspension",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    InvoiceType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Serialize = table.Column<double>(type: "float", nullable: false),
                    ParentInvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BookIndex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<double>(type: "float", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceTypeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceSubTypesId = table.Column<int>(type: "int", nullable: false),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAccredite = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    TotalDiscountValue = table.Column<double>(type: "float", nullable: false),
                    TotalDiscountRatio = table.Column<double>(type: "float", nullable: false),
                    Net = table.Column<double>(type: "float", nullable: false),
                    Paid = table.Column<double>(type: "float", nullable: false),
                    Remain = table.Column<double>(type: "float", nullable: false),
                    VirualPaid = table.Column<double>(type: "float", nullable: false),
                    TotalAfterDiscount = table.Column<double>(type: "float", nullable: false),
                    TotalVat = table.Column<double>(type: "float", nullable: false),
                    ApplyVat = table.Column<bool>(type: "bit", nullable: false),
                    PriceWithVat = table.Column<bool>(type: "bit", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    ActiveDiscount = table.Column<bool>(type: "bit", nullable: false),
                    IsReturn = table.Column<bool>(type: "bit", nullable: false),
                    SalesManId = table.Column<int>(type: "int", nullable: true),
                    PriceListId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSInvoiceSuspension", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_POSInvoiceSuspension_GLBranch_BranchId",
                        column: x => x.BranchId,
                        principalTable: "GLBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvoiceSuspension_InvEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "InvEmployees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvoiceSuspension_InvPersons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "InvPersons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvoiceSuspension_InvSalesMan_SalesManId",
                        column: x => x.SalesManId,
                        principalTable: "InvSalesMan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvoiceSuspension_InvStpStores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OtherSettingsBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gLBankId = table.Column<int>(type: "int", nullable: false),
                    otherSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherSettingsBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherSettingsBanks_GLBanks_gLBankId",
                        column: x => x.gLBankId,
                        principalTable: "GLBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OtherSettingsBanks_otherSettings_otherSettingsId",
                        column: x => x.otherSettingsId,
                        principalTable: "otherSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherSettingsSafes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    gLSafeId = table.Column<int>(type: "int", nullable: false),
                    otherSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherSettingsSafes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherSettingsSafes_GLSafe_gLSafeId",
                        column: x => x.gLSafeId,
                        principalTable: "GLSafe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OtherSettingsSafes_otherSettings_otherSettingsId",
                        column: x => x.otherSettingsId,
                        principalTable: "otherSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OtherSettingsStores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvStpStoresId = table.Column<int>(type: "int", nullable: false),
                    otherSettingsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherSettingsStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherSettingsStores_InvStpStores_InvStpStoresId",
                        column: x => x.InvStpStoresId,
                        principalTable: "InvStpStores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OtherSettingsStores_otherSettings_otherSettingsId",
                        column: x => x.otherSettingsId,
                        principalTable: "otherSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GLRecieptCostCenter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
                    SupportId = table.Column<int>(type: "int", nullable: false),
                    CostCenteCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CostCenteName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percentage = table.Column<double>(type: "float", nullable: false),
                    Number = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLRecieptCostCenter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLRecieptCostCenter_GLCostCenter_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "GLCostCenter",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GLRecieptCostCenter_GlReciepts_SupportId",
                        column: x => x.SupportId,
                        principalTable: "GlReciepts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GLRecieptFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecieptId = table.Column<int>(type: "int", nullable: false),
                    FileLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtensions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GLRecieptFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GLRecieptFiles_GlReciepts_RecieptId",
                        column: x => x.RecieptId,
                        principalTable: "GlReciepts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    SizeId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    TotalWithSplitedDiscount = table.Column<double>(type: "float", nullable: false),
                    TotalWithOutSplitedDiscount = table.Column<double>(type: "float", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "date", nullable: true),
                    Signal = table.Column<int>(type: "int", nullable: false),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<double>(type: "float", nullable: false),
                    DiscountRatio = table.Column<double>(type: "float", nullable: false),
                    VatRatio = table.Column<double>(type: "float", nullable: false),
                    VatValue = table.Column<double>(type: "float", nullable: false),
                    TransQuantity = table.Column<double>(type: "float", nullable: false),
                    ReturnQuantity = table.Column<double>(type: "float", nullable: false),
                    StatusOfTrans = table.Column<int>(type: "int", nullable: false),
                    SplitedDiscountValue = table.Column<double>(type: "float", nullable: false),
                    SplitedDiscountRatio = table.Column<double>(type: "float", nullable: false),
                    AvgPrice = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    AutoDiscount = table.Column<double>(type: "float", nullable: false),
                    PriceList = table.Column<int>(type: "int", nullable: false),
                    MinimumPrice = table.Column<double>(type: "float", nullable: false),
                    ConversionFactor = table.Column<double>(type: "float", nullable: false),
                    indexOfSerialNo = table.Column<int>(type: "int", nullable: false),
                    indexOfItem = table.Column<int>(type: "int", nullable: false),
                    balanceBarcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    parentItemId = table.Column<int>(type: "int", nullable: true),
                    OtherAdditions = table.Column<double>(type: "float", nullable: false),
                    CodeOfflinePOS = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_InvoiceMaster_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_InvSizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "InvSizes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_InvStpUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "InvStpUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvoiceFiles",
                columns: table => new
                {
                    InvoiceFileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    FileLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileExtensions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceFiles", x => x.InvoiceFileId);
                    table.ForeignKey(
                        name: "FK_InvoiceFiles_InvoiceMaster_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceId");
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentsMethods",
                columns: table => new
                {
                    InvoicePaymentsMethodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    Cheque = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CodeOfflinePOS = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentsMethods", x => x.InvoicePaymentsMethodId);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentsMethods_InvoiceMaster_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_InvoicePaymentsMethods_InvPaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "InvPaymentMethods",
                        principalColumn: "PaymentMethodId");
                });

            migrationBuilder.CreateTable(
                name: "InvPurchaseAdditionalCostsRelation",
                columns: table => new
                {
                    PurchaseAdditionalCostsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    AddtionalCostId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    CodeOfflinePOS = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvPurchaseAdditionalCostsRelation", x => x.PurchaseAdditionalCostsId);
                    table.ForeignKey(
                        name: "FK_InvPurchaseAdditionalCostsRelation_InvoiceMaster_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "InvoiceMaster",
                        principalColumn: "InvoiceId");
                    table.ForeignKey(
                        name: "FK_InvPurchaseAdditionalCostsRelation_InvPurchasesAdditionalCosts_AddtionalCostId",
                        column: x => x.AddtionalCostId,
                        principalTable: "InvPurchasesAdditionalCosts",
                        principalColumn: "PurchasesAdditionalCostsId");
                });

            migrationBuilder.CreateTable(
                name: "OfferPriceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    SizeId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    TotalWithSplitedDiscount = table.Column<double>(type: "float", nullable: false),
                    TotalWithOutSplitedDiscount = table.Column<double>(type: "float", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "date", nullable: true),
                    Signal = table.Column<int>(type: "int", nullable: false),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<double>(type: "float", nullable: false),
                    DiscountRatio = table.Column<double>(type: "float", nullable: false),
                    VatRatio = table.Column<double>(type: "float", nullable: false),
                    VatValue = table.Column<double>(type: "float", nullable: false),
                    TransQuantity = table.Column<double>(type: "float", nullable: false),
                    ReturnQuantity = table.Column<double>(type: "float", nullable: false),
                    StatusOfTrans = table.Column<int>(type: "int", nullable: false),
                    SplitedDiscountValue = table.Column<double>(type: "float", nullable: false),
                    SplitedDiscountRatio = table.Column<double>(type: "float", nullable: false),
                    AvgPrice = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    AutoDiscount = table.Column<double>(type: "float", nullable: false),
                    PriceList = table.Column<int>(type: "int", nullable: false),
                    MinimumPrice = table.Column<double>(type: "float", nullable: false),
                    ConversionFactor = table.Column<double>(type: "float", nullable: false),
                    indexOfSerialNo = table.Column<int>(type: "int", nullable: false),
                    indexOfItem = table.Column<int>(type: "int", nullable: false),
                    balanceBarcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    parentItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferPriceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferPriceDetails_InvSizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "InvSizes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceDetails_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceDetails_InvStpUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "InvStpUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OfferPriceDetails_OfferPriceMaster_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "OfferPriceMaster",
                        principalColumn: "InvoiceId");
                });

            migrationBuilder.CreateTable(
                name: "POSInvSuspensionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: true),
                    SizeId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Total = table.Column<double>(type: "float", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "date", nullable: true),
                    Signal = table.Column<int>(type: "int", nullable: false),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    DiscountValue = table.Column<double>(type: "float", nullable: false),
                    DiscountRatio = table.Column<double>(type: "float", nullable: false),
                    VatRatio = table.Column<double>(type: "float", nullable: false),
                    VatValue = table.Column<double>(type: "float", nullable: false),
                    TransQuantity = table.Column<double>(type: "float", nullable: false),
                    ReturnQuantity = table.Column<double>(type: "float", nullable: false),
                    StatusOfTrans = table.Column<int>(type: "int", nullable: false),
                    SplitedDiscountValue = table.Column<double>(type: "float", nullable: false),
                    SplitedDiscountRatio = table.Column<double>(type: "float", nullable: false),
                    AvgPrice = table.Column<double>(type: "float", nullable: false),
                    Cost = table.Column<double>(type: "float", nullable: false),
                    AutoDiscount = table.Column<double>(type: "float", nullable: false),
                    PriceList = table.Column<int>(type: "int", nullable: false),
                    MinimumPrice = table.Column<double>(type: "float", nullable: false),
                    ConversionFactor = table.Column<double>(type: "float", nullable: false),
                    SerialTexts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    indexOfSerialNo = table.Column<int>(type: "int", nullable: false),
                    indexOfItem = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POSInvSuspensionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POSInvSuspensionDetails_InvSizes_SizeId",
                        column: x => x.SizeId,
                        principalTable: "InvSizes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvSuspensionDetails_InvStpItemMaster_ItemId",
                        column: x => x.ItemId,
                        principalTable: "InvStpItemMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvSuspensionDetails_InvStpUnits_UnitId",
                        column: x => x.UnitId,
                        principalTable: "InvStpUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_POSInvSuspensionDetails_POSInvoiceSuspension_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "POSInvoiceSuspension",
                        principalColumn: "InvoiceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_chatGroupMembers_groupId",
                table: "chatGroupMembers",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "IX_chatGroupMembers_memberId",
                table: "chatGroupMembers",
                column: "memberId");

            migrationBuilder.CreateIndex(
                name: "IX_chatGroups_groupCreatorId",
                table: "chatGroups",
                column: "groupCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_fromId",
                table: "chatMessages",
                column: "fromId");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_groupId",
                table: "chatMessages",
                column: "groupId");

            migrationBuilder.CreateIndex(
                name: "IX_chatMessages_toId",
                table: "chatMessages",
                column: "toId");

            migrationBuilder.CreateIndex(
                name: "IX_GLBankBranch_BranchId",
                table: "GLBankBranch",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_GLBanks_FinancialAccountId",
                table: "GLBanks",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLBanksHistory_employeesId",
                table: "GLBanksHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLBranchHistory_employeesId",
                table: "GLBranchHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLCostCenter_ParentId",
                table: "GLCostCenter",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GLCostCenterHistory_employeesId",
                table: "GLCostCenterHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLCurrencyHistory_employeesId",
                table: "GLCurrencyHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialAccount_CurrencyId",
                table: "GLFinancialAccount",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialAccount_ParentId",
                table: "GLFinancialAccount",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialAccountBranch_FinancialAccountId",
                table: "GLFinancialAccountBranch",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialAccountHistory_employeesId",
                table: "GLFinancialAccountHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialBranch_FinancialAccountId",
                table: "GLFinancialBranch",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialCost_FinancialAccountId",
                table: "GLFinancialCost",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLFinancialSetting_FinancialAccountId",
                table: "GLFinancialSetting",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLIntegrationSettings_GLBranchId",
                table: "GLIntegrationSettings",
                column: "GLBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_GLIntegrationSettings_GLFinancialAccountId",
                table: "GLIntegrationSettings",
                column: "GLFinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntry_CurrencyId",
                table: "GLJournalEntry",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryDetails_FinancialAccountId",
                table: "GLJournalEntryDetails",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryDetails_JournalEntryId",
                table: "GLJournalEntryDetails",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryDetailsAccounts_FinancialAccountId",
                table: "GLJournalEntryDetailsAccounts",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryDraftDetails_JournalEntryDraftId",
                table: "GLJournalEntryDraftDetails",
                column: "JournalEntryDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryFiles_JournalEntryId",
                table: "GLJournalEntryFiles",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_GLJournalEntryFilesDraft_JournalEntryDraftId",
                table: "GLJournalEntryFilesDraft",
                column: "JournalEntryDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_GLOtherAuthoritiesHistory_employeesId",
                table: "GLOtherAuthoritiesHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLPrinter_BranchId",
                table: "GLPrinter",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_GLPurchasesAndSalesSettings_branchId",
                table: "GLPurchasesAndSalesSettings",
                column: "branchId");

            migrationBuilder.CreateIndex(
                name: "IX_GLPurchasesAndSalesSettings_FinancialAccountId",
                table: "GLPurchasesAndSalesSettings",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLRecHistory_employeesId",
                table: "GLRecHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLRecieptCostCenter_CostCenterId",
                table: "GLRecieptCostCenter",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_GLRecieptCostCenter_SupportId",
                table: "GLRecieptCostCenter",
                column: "SupportId");

            migrationBuilder.CreateIndex(
                name: "IX_GLRecieptFiles_RecieptId",
                table: "GLRecieptFiles",
                column: "RecieptId");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_BankId",
                table: "GlReciepts",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_FinancialAccountId",
                table: "GlReciepts",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_OtherAuthorityId",
                table: "GlReciepts",
                column: "OtherAuthorityId");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_PaymentMethodId",
                table: "GlReciepts",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_PersonId",
                table: "GlReciepts",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_RectypeWithPayment_CollectionCode_BranchId",
                table: "GlReciepts",
                columns: new[] { "RectypeWithPayment", "CollectionCode", "BranchId" },
                unique: true,
                filter: "[RectypeWithPayment] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_SafeID",
                table: "GlReciepts",
                column: "SafeID");

            migrationBuilder.CreateIndex(
                name: "IX_GlReciepts_SalesManId",
                table: "GlReciepts",
                column: "SalesManId");

            migrationBuilder.CreateIndex(
                name: "IX_GLRecieptsHistory_employeesId",
                table: "GLRecieptsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_GLSafe_BranchId",
                table: "GLSafe",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_GLSafe_FinancialAccountId",
                table: "GLSafe",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GLSafeHistory_employeesId",
                table: "GLSafeHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvBarcodeHistory_employeesId",
                table: "InvBarcodeHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvBarcodeItems_BarcodeId",
                table: "InvBarcodeItems",
                column: "BarcodeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvCategories_Code",
                table: "InvCategories",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvCategoriesHistory_employeesId",
                table: "InvCategoriesHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvColors_Code",
                table: "InvColors",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvColorsHistory_employeesId",
                table: "InvColorsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvCommissionList_Code",
                table: "InvCommissionList",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvCommissionListHistory_employeesId",
                table: "InvCommissionListHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvCommissionSlides_CommissionId",
                table: "InvCommissionSlides",
                column: "CommissionId");

            migrationBuilder.CreateIndex(
                name: "IX_InvDiscount_A_P_PersonId",
                table: "InvDiscount_A_P",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_InvDiscount_A_P_History_employeesId",
                table: "InvDiscount_A_P_History",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvEmployees_Code",
                table: "InvEmployees",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvEmployees_FinancialAccountId",
                table: "InvEmployees",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvEmployees_JobId",
                table: "InvEmployees",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_InvEmployeesBranches_BranchId",
                table: "InvEmployeesBranches",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InvEmployeesHistory_employeesId",
                table: "InvEmployeesHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsBanksSafesDetails_DocumentId",
                table: "InvFundsBanksSafesDetails",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsBanksSafesHistory_employeesId",
                table: "InvFundsBanksSafesHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsBanksSafesMaster_BankId",
                table: "InvFundsBanksSafesMaster",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsBanksSafesMaster_SafeId",
                table: "InvFundsBanksSafesMaster",
                column: "SafeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvFundsCustomerSupplier_PersonId",
                table: "InvFundsCustomerSupplier",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvJobs_Code",
                table: "InvJobs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvJobsHistory_employeesId",
                table: "InvJobsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_ItemId",
                table: "InvoiceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_SizeId",
                table: "InvoiceDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_UnitId",
                table: "InvoiceDetails",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceFiles_InvoiceId",
                table: "InvoiceFiles",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_BranchId",
                table: "InvoiceMaster",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_EmployeeId",
                table: "InvoiceMaster",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_InvoiceType_BranchId",
                table: "InvoiceMaster",
                columns: new[] { "InvoiceType", "BranchId" },
                unique: true,
                filter: "[InvoiceType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_PersonId",
                table: "InvoiceMaster",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_SalesManId",
                table: "InvoiceMaster",
                column: "SalesManId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_StoreId",
                table: "InvoiceMaster",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMaster_StoreIdTo",
                table: "InvoiceMaster",
                column: "StoreIdTo");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceMasterHistory_employeesId",
                table: "InvoiceMasterHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentsMethods_InvoiceId",
                table: "InvoicePaymentsMethods",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentsMethods_PaymentMethodId",
                table: "InvoicePaymentsMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPaymentMethods_BankId",
                table: "InvPaymentMethods",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPaymentMethods_Code",
                table: "InvPaymentMethods",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvPaymentMethods_SafeId",
                table: "InvPaymentMethods",
                column: "SafeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPaymentMethodsHistory_employeesId",
                table: "InvPaymentMethodsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPersons_FinancialAccountId",
                table: "InvPersons",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPersons_InvEmployeesId",
                table: "InvPersons",
                column: "InvEmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPersons_SalesManId",
                table: "InvPersons",
                column: "SalesManId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPersons_Branches_PersonId",
                table: "InvPersons_Branches",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPersonsHistory_employeesId",
                table: "InvPersonsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPurchaseAdditionalCostsRelation_AddtionalCostId",
                table: "InvPurchaseAdditionalCostsRelation",
                column: "AddtionalCostId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPurchaseAdditionalCostsRelation_InvoiceId",
                table: "InvPurchaseAdditionalCostsRelation",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvPurchasesAdditionalCosts_Code",
                table: "InvPurchasesAdditionalCosts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvPurchasesAdditionalCostsHistory_employeesId",
                table: "InvPurchasesAdditionalCostsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesMan_Code",
                table: "InvSalesMan",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesMan_CommissionListId",
                table: "InvSalesMan",
                column: "CommissionListId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesMan_FinancialAccountId",
                table: "InvSalesMan",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesMan_Branches_SalesManId",
                table: "InvSalesMan_Branches",
                column: "SalesManId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSalesManHistory_employeesId",
                table: "InvSalesManHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSerialTransaction_ItemId",
                table: "InvSerialTransaction",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSerialTransaction_StoreId",
                table: "InvSerialTransaction",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InvSizes_Code",
                table: "InvSizes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvSizesHistory_employeesId",
                table: "InvSizesHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStoreBranch_BranchId",
                table: "InvStoreBranch",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStorePlaces_Code",
                table: "InvStorePlaces",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvStorePlacesHistory_employeesId",
                table: "InvStorePlacesHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStoresHistory_employeesId",
                table: "InvStoresHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemCardHistory_employeesId",
                table: "InvStpItemCardHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemCardParts_PartId",
                table: "InvStpItemCardParts",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemCardParts_UnitId",
                table: "InvStpItemCardParts",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemColorSize_ColorId",
                table: "InvStpItemColorSize",
                column: "ColorId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemColorSize_SizeId",
                table: "InvStpItemColorSize",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemColorSize_UnitId",
                table: "InvStpItemColorSize",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemMaster_DefaultStoreId",
                table: "InvStpItemMaster",
                column: "DefaultStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemMaster_GroupId",
                table: "InvStpItemMaster",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemMaster_ItemCode",
                table: "InvStpItemMaster",
                column: "ItemCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemMaster_NationalBarcode",
                table: "InvStpItemMaster",
                column: "NationalBarcode",
                unique: true,
                filter: "[NationalBarcode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemStores_StoreId",
                table: "InvStpItemStores",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpItemUnit_UnitId",
                table: "InvStpItemUnit",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpStores_Code",
                table: "InvStpStores",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvStpStores_GLBranchId",
                table: "InvStpStores",
                column: "GLBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_InvStpUnits_Code",
                table: "InvStpUnits",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvUnitsHistory_employeesId",
                table: "InvUnitsHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeen_NotificationsMasterId",
                table: "NotificationSeen",
                column: "NotificationsMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSeen_UserId",
                table: "NotificationSeen",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsMaster_insertedById",
                table: "NotificationsMaster",
                column: "insertedById");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationsMaster_specialUserId",
                table: "NotificationsMaster",
                column: "specialUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceDetails_InvoiceId",
                table: "OfferPriceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceDetails_ItemId",
                table: "OfferPriceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceDetails_SizeId",
                table: "OfferPriceDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceDetails_UnitId",
                table: "OfferPriceDetails",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceMaster_BranchId",
                table: "OfferPriceMaster",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceMaster_EmployeeId",
                table: "OfferPriceMaster",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceMaster_InvoiceType",
                table: "OfferPriceMaster",
                column: "InvoiceType",
                unique: true,
                filter: "[InvoiceType] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceMaster_PersonId",
                table: "OfferPriceMaster",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceMaster_SalesManId",
                table: "OfferPriceMaster",
                column: "SalesManId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferPriceMaster_StoreId",
                table: "OfferPriceMaster",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherAuthorities_BranchId",
                table: "OtherAuthorities",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherAuthorities_FinancialAccountId",
                table: "OtherAuthorities",
                column: "FinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_otherSettings_userAccountId",
                table: "otherSettings",
                column: "userAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSettingsBanks_gLBankId",
                table: "OtherSettingsBanks",
                column: "gLBankId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSettingsBanks_otherSettingsId",
                table: "OtherSettingsBanks",
                column: "otherSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSettingsSafes_gLSafeId",
                table: "OtherSettingsSafes",
                column: "gLSafeId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSettingsSafes_otherSettingsId",
                table: "OtherSettingsSafes",
                column: "otherSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSettingsStores_InvStpStoresId",
                table: "OtherSettingsStores",
                column: "InvStpStoresId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherSettingsStores_otherSettingsId",
                table: "OtherSettingsStores",
                column: "otherSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvoiceSuspension_BranchId",
                table: "POSInvoiceSuspension",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvoiceSuspension_EmployeeId",
                table: "POSInvoiceSuspension",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvoiceSuspension_PersonId",
                table: "POSInvoiceSuspension",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvoiceSuspension_SalesManId",
                table: "POSInvoiceSuspension",
                column: "SalesManId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvoiceSuspension_StoreId",
                table: "POSInvoiceSuspension",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvSuspensionDetails_InvoiceId",
                table: "POSInvSuspensionDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvSuspensionDetails_ItemId",
                table: "POSInvSuspensionDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvSuspensionDetails_SizeId",
                table: "POSInvSuspensionDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_POSInvSuspensionDetails_UnitId",
                table: "POSInvSuspensionDetails",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_POSSession_employeeId",
                table: "POSSession",
                column: "employeeId");

            migrationBuilder.CreateIndex(
                name: "IX_POSSession_sessionClosedById",
                table: "POSSession",
                column: "sessionClosedById");

            migrationBuilder.CreateIndex(
                name: "IX_POSSession_sessionCode",
                table: "POSSession",
                column: "sessionCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POSSessionHistory_employeesId",
                table: "POSSessionHistory",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_POSSessionHistory_POSSessionId",
                table: "POSSessionHistory",
                column: "POSSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportManger_ArabicFilenameId",
                table: "ReportManger",
                column: "ArabicFilenameId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportManger_screenId",
                table: "ReportManger",
                column: "screenId");

            migrationBuilder.CreateIndex(
                name: "IX_rules_permissionListId",
                table: "rules",
                column: "permissionListId");

            migrationBuilder.CreateIndex(
                name: "IX_signalR_InvEmployeesId",
                table: "signalR",
                column: "InvEmployeesId");

            migrationBuilder.CreateIndex(
                name: "IX_signinLogs_userAccountid",
                table: "signinLogs",
                column: "userAccountid");

            migrationBuilder.CreateIndex(
                name: "IX_subCodeLevels_GLGeneralSettingId",
                table: "subCodeLevels",
                column: "GLGeneralSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemHistoryLogs_BranchId",
                table: "SystemHistoryLogs",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemHistoryLogs_employeesId",
                table: "SystemHistoryLogs",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_userAccount_employeesId",
                table: "userAccount",
                column: "employeesId");

            migrationBuilder.CreateIndex(
                name: "IX_userAccount_permissionListId",
                table: "userAccount",
                column: "permissionListId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAndPermission_permissionListId",
                table: "UserAndPermission",
                column: "permissionListId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAndPermission_userAccountId",
                table: "UserAndPermission",
                column: "userAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_userBranches_GLBranchId",
                table: "userBranches",
                column: "GLBranchId");

            migrationBuilder.CreateIndex(
                name: "IX_userBranches_userAccountId",
                table: "userBranches",
                column: "userAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_usersForgetPassword_userId",
                table: "usersForgetPassword",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodePrintFiles");

            migrationBuilder.DropTable(
                name: "chatGroupMembers");

            migrationBuilder.DropTable(
                name: "chatMessages");

            migrationBuilder.DropTable(
                name: "DeletedRecords");

            migrationBuilder.DropTable(
                name: "EditedItems");

            migrationBuilder.DropTable(
                name: "GLBalanceForLastPeriod");

            migrationBuilder.DropTable(
                name: "GLBankBranch");

            migrationBuilder.DropTable(
                name: "GLBanksHistory");

            migrationBuilder.DropTable(
                name: "GLBranchHistory");

            migrationBuilder.DropTable(
                name: "GLCostCenterHistory");

            migrationBuilder.DropTable(
                name: "GLCurrencyHistory");

            migrationBuilder.DropTable(
                name: "GLFinancialAccountBranch");

            migrationBuilder.DropTable(
                name: "GLFinancialAccountForOpeningBalance");

            migrationBuilder.DropTable(
                name: "GLFinancialAccountHistory");

            migrationBuilder.DropTable(
                name: "GLFinancialBranch");

            migrationBuilder.DropTable(
                name: "GLFinancialCost");

            migrationBuilder.DropTable(
                name: "GLFinancialSetting");

            migrationBuilder.DropTable(
                name: "GLIntegrationSettings");

            migrationBuilder.DropTable(
                name: "GLJournalEntryDetails");

            migrationBuilder.DropTable(
                name: "GLJournalEntryDetailsAccounts");

            migrationBuilder.DropTable(
                name: "GLJournalEntryDraftDetails");

            migrationBuilder.DropTable(
                name: "GLJournalEntryFiles");

            migrationBuilder.DropTable(
                name: "GLJournalEntryFilesDraft");

            migrationBuilder.DropTable(
                name: "GLOtherAuthoritiesHistory");

            migrationBuilder.DropTable(
                name: "GLPrinter");

            migrationBuilder.DropTable(
                name: "GLPurchasesAndSalesSettings");

            migrationBuilder.DropTable(
                name: "GLRecHistory");

            migrationBuilder.DropTable(
                name: "GLRecieptCostCenter");

            migrationBuilder.DropTable(
                name: "GLRecieptFiles");

            migrationBuilder.DropTable(
                name: "GLRecieptsHistory");

            migrationBuilder.DropTable(
                name: "GLSafeHistory");

            migrationBuilder.DropTable(
                name: "InvBarcodeHistory");

            migrationBuilder.DropTable(
                name: "InvBarcodeItems");

            migrationBuilder.DropTable(
                name: "InvCategoriesHistory");

            migrationBuilder.DropTable(
                name: "InvColorsHistory");

            migrationBuilder.DropTable(
                name: "InvCommissionListHistory");

            migrationBuilder.DropTable(
                name: "InvCommissionSlides");

            migrationBuilder.DropTable(
                name: "InvCompanyData");

            migrationBuilder.DropTable(
                name: "InvDiscount_A_P");

            migrationBuilder.DropTable(
                name: "InvDiscount_A_P_History");

            migrationBuilder.DropTable(
                name: "InvEmployeesBranches");

            migrationBuilder.DropTable(
                name: "InvEmployeesHistory");

            migrationBuilder.DropTable(
                name: "InvFundsBanksSafesDetails");

            migrationBuilder.DropTable(
                name: "InvFundsBanksSafesHistory");

            migrationBuilder.DropTable(
                name: "InvFundsCustomerSupplier");

            migrationBuilder.DropTable(
                name: "InvGeneralSettings");

            migrationBuilder.DropTable(
                name: "InvJobsHistory");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "InvoiceFiles");

            migrationBuilder.DropTable(
                name: "InvoiceMasterHistory");

            migrationBuilder.DropTable(
                name: "InvoicePaymentsMethods");

            migrationBuilder.DropTable(
                name: "InvoiceSerialize");

            migrationBuilder.DropTable(
                name: "InvPaymentMethodsHistory");

            migrationBuilder.DropTable(
                name: "InvPersonLastPrice");

            migrationBuilder.DropTable(
                name: "InvPersons_Branches");

            migrationBuilder.DropTable(
                name: "InvPersonsHistory");

            migrationBuilder.DropTable(
                name: "InvPurchaseAdditionalCostsRelation");

            migrationBuilder.DropTable(
                name: "InvPurchasesAdditionalCostsHistory");

            migrationBuilder.DropTable(
                name: "InvSalesMan_Branches");

            migrationBuilder.DropTable(
                name: "InvSalesManHistory");

            migrationBuilder.DropTable(
                name: "InvSerialTransaction");

            migrationBuilder.DropTable(
                name: "InvSizesHistory");

            migrationBuilder.DropTable(
                name: "InvStoreBranch");

            migrationBuilder.DropTable(
                name: "InvStorePlacesHistory");

            migrationBuilder.DropTable(
                name: "InvStoresHistory");

            migrationBuilder.DropTable(
                name: "InvStpItemCardHistory");

            migrationBuilder.DropTable(
                name: "InvStpItemCardParts");

            migrationBuilder.DropTable(
                name: "InvStpItemCardSerials");

            migrationBuilder.DropTable(
                name: "InvStpItemColorSize");

            migrationBuilder.DropTable(
                name: "InvStpItemStores");

            migrationBuilder.DropTable(
                name: "InvUnitsHistory");

            migrationBuilder.DropTable(
                name: "NotificationSeen");

            migrationBuilder.DropTable(
                name: "OfferPriceDetails");

            migrationBuilder.DropTable(
                name: "OtherSettingsBanks");

            migrationBuilder.DropTable(
                name: "OtherSettingsSafes");

            migrationBuilder.DropTable(
                name: "OtherSettingsStores");

            migrationBuilder.DropTable(
                name: "POS_OfflineDevices");

            migrationBuilder.DropTable(
                name: "POSDevices");

            migrationBuilder.DropTable(
                name: "POSInvSuspensionDetails");

            migrationBuilder.DropTable(
                name: "POSSessionHistory");

            migrationBuilder.DropTable(
                name: "POSTouchSettings");

            migrationBuilder.DropTable(
                name: "RecHistory");

            migrationBuilder.DropTable(
                name: "ReportManger");

            migrationBuilder.DropTable(
                name: "rules");

            migrationBuilder.DropTable(
                name: "signalR");

            migrationBuilder.DropTable(
                name: "signinLogs");

            migrationBuilder.DropTable(
                name: "subCodeLevels");

            migrationBuilder.DropTable(
                name: "SystemHistoryLogs");

            migrationBuilder.DropTable(
                name: "TransferDataFromDeskTop");

            migrationBuilder.DropTable(
                name: "UserAndPermission");

            migrationBuilder.DropTable(
                name: "userBranches");

            migrationBuilder.DropTable(
                name: "usersForgetPassword");

            migrationBuilder.DropTable(
                name: "chatGroups");

            migrationBuilder.DropTable(
                name: "GLJournalEntry");

            migrationBuilder.DropTable(
                name: "GLJournalEntryDraft");

            migrationBuilder.DropTable(
                name: "GLCostCenter");

            migrationBuilder.DropTable(
                name: "GlReciepts");

            migrationBuilder.DropTable(
                name: "InvBarcodeTemplate");

            migrationBuilder.DropTable(
                name: "InvFundsBanksSafesMaster");

            migrationBuilder.DropTable(
                name: "InvoiceMaster");

            migrationBuilder.DropTable(
                name: "InvPurchasesAdditionalCosts");

            migrationBuilder.DropTable(
                name: "InvColors");

            migrationBuilder.DropTable(
                name: "InvStpItemUnit");

            migrationBuilder.DropTable(
                name: "NotificationsMaster");

            migrationBuilder.DropTable(
                name: "OfferPriceMaster");

            migrationBuilder.DropTable(
                name: "otherSettings");

            migrationBuilder.DropTable(
                name: "InvSizes");

            migrationBuilder.DropTable(
                name: "POSInvoiceSuspension");

            migrationBuilder.DropTable(
                name: "POSSession");

            migrationBuilder.DropTable(
                name: "reportFiles");

            migrationBuilder.DropTable(
                name: "screenNames");

            migrationBuilder.DropTable(
                name: "GLGeneralSetting");

            migrationBuilder.DropTable(
                name: "InvPaymentMethods");

            migrationBuilder.DropTable(
                name: "OtherAuthorities");

            migrationBuilder.DropTable(
                name: "InvStpItemMaster");

            migrationBuilder.DropTable(
                name: "InvStpUnits");

            migrationBuilder.DropTable(
                name: "userAccount");

            migrationBuilder.DropTable(
                name: "InvPersons");

            migrationBuilder.DropTable(
                name: "InvStpStores");

            migrationBuilder.DropTable(
                name: "GLBanks");

            migrationBuilder.DropTable(
                name: "GLSafe");

            migrationBuilder.DropTable(
                name: "InvCategories");

            migrationBuilder.DropTable(
                name: "InvStorePlaces");

            migrationBuilder.DropTable(
                name: "permissionList");

            migrationBuilder.DropTable(
                name: "InvEmployees");

            migrationBuilder.DropTable(
                name: "InvSalesMan");

            migrationBuilder.DropTable(
                name: "GLBranch");

            migrationBuilder.DropTable(
                name: "InvJobs");

            migrationBuilder.DropTable(
                name: "GLFinancialAccount");

            migrationBuilder.DropTable(
                name: "InvCommissionList");

            migrationBuilder.DropTable(
                name: "GLCurrency");
        }
    }
}
