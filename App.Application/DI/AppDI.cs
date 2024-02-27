using System.Reflection;
using App.Application.Common.Behaviours;
using App.Application.Helpers;
using App.Application.Helpers.Service_helper.ApplicationSettingsHandler;
using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.History;
using App.Application.Helpers.Service_helper.Item_unit;
using App.Application.Services.Acquired_And_Premitted_Discount;
using App.Application.Services.HelperService;
using App.Application.Services.Process;
using App.Application.Services.Process.BalanceForLastPeriods;
using App.Application.Services.Process.BalanceReview;
using App.Application.Services.Process.BalanceReviewDetailed;
using App.Application.Services.Process.Bank;
using App.Application.Services.Process.BarCode;
using App.Application.Services.Process.Branches;
using App.Application.Services.Process.Category;
using App.Application.Services.Process.Color;
using App.Application.Services.Process.Commission_list;
using App.Application.Services.Process.Company_data;
using App.Application.Services.Process.CostCenters;
using App.Application.Services.Process.CostCentersReport;
using App.Application.Services.Process.Currancy;
using App.Application.Services.Process.Employee;
using App.Application.Services.Process.FinancialAccountCostCenters;
using App.Application.Services.Process.FinancialAccountForOpeningBalances;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.FinancialSettings;
using App.Application.Services.Process.Funds_Banks_and_Safes;
using App.Application.Services.Process.FundsCustomerSupplier;
using App.Application.Services.Process.GeneralSettings;
using App.Application.Services.Process.GLServices.ledger_Report;
using App.Application.Services.Process.Inv_General_Settings;
using App.Application.Services.Process.Invoices;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.Invoices.Purchase.IPurchasesServices;
using App.Application.Services.Process.Invoices.Purchase.PurchasesServices;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Application.Services.Process.Invoices.Return_Purchase.IReturnPurchaseService;
using App.Application.Services.Process.Invoices.Return_Purchase.ReturnPurchaseService;
using App.Application.Services.Process.Job;
//using App.Application.Services.Process.JournalEntries;
using App.Application.Services.Process.JournalEntryDrafts;
using App.Application.Services.Process.Payment_methods;
using App.Application.Services.Process.Persons;
using App.Application.Services.Process.PurchasesAdditionalCosts;
using App.Application.Services.Process.Safes;
using App.Application.Services.Process.Sales_Man;
using App.Application.Services.Process.Size;
using App.Application.Services.Process.Store;
using App.Application.Services.Process.Store_places;
using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.AdditionPermissionServices;
using App.Application.Services.Process.StoreServices.Invoices.AdditionPermission.IAdditionPermissionServices;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.IItemsFundsServices;
using App.Application.Services.Process.StoreServices.Invoices.ItemsFund.ItemsFundsServices;
using App.Application.Services.Process.Unit;
using App.Application.Services.Reports.Invoices.Purchases.Item_Purchases_For_Supplier;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.Invoices.Purchases.Items_Purchases_For_Supplier;
using App.Application.Services.Reports.Invoices.Purchases.Supplier_Account;
using App.Application.Services.Reports.Items_Prices;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Items_Purchases;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Purchases_transaction;
using App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Detailed_trans_of_item;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Item_balance_in_stores;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.Total_transactions_of_items;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using App.Infrastructure.Helpers.Progress;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.SalesServices;
using App.Application.Services.Process.StoreServices;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using Hangfire;
using App.Application.Services;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Reports;
using App.Application.Services.Process.StoreServices.Invoices.POS;
using App.Application.Services.Process.FileManger.ReportFileServices;
using App.Application.Services.HelperService.CookiesAppend;
using App.Application.Services.HelperService.EmailServices;
using App.Application.Services.Process.StoreServices.Invoices.Funds.ItemsFund.ItemFundGLIntegrationServices;
using App.Application.Services.HelperService.InvoicePDF;
using App.Application.Services.Reports.StoreReports.Invoices.Stores.DetailsOfSerialTransactions;
using App.Application.Services.Reports.StoreReports.Sales;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Reports.StoreReports.RPT_BanksSafesServices;
using App.Application.Services.Reports.StoreReports.MainData;
using App.Application.Helpers.UpdateSystem.Services;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Application.Services.Process.GLServices.ReceiptBusiness.CompinedReceiptsService;
using App.Application.Services.Process.StoreServices.Invoices.ItemsCard;
using App.Application.Handlers.Profit;
using App.Application.Services.Reports.StoreReports.salesProfit;
using App.Application.Helpers.DemandLimitNotificationSystem;
using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.OfferPriceService;
using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Application.Helpers.DataBasesMigraiton;
using App.Application.Services.Printing.OfferPriceLanding;
using App.Application.Services.Printing.UpdatePrintFiles;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Application.Services.Printing.PrintPermission;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Printing.PrintResponse;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Printing.CompanyData;
using App.Application.Services.Printing.GenralPrint;
using App.Application.Services.Process.GLServices.Prnters;
using App.Application.Services.Process.RestaurantsServices.KitchensServices;
using App.Application.Services.Process.GLServices.ReceiptBusiness.ReceiptsPaid;
using App.Application.Handlers.Reports.SalesReports.OfferPriceReport;
using App.Application.Services.Process.StoreServices.Dashboard;

namespace App.Application.DI
{
    public static class AppDI
    {
        public static int ProgressCount = 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
        public static IServiceCollection AddApplicationDI(this IServiceCollection services)
        {

       

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient<IRoundNumbers, RoundNumbers>();
            services.AddScoped<IHelperService, HelperService>();
            services.AddTransient<IActionResultResponseHandler, ActionResultResponseHandler>();
            services.AddTransient<IRepositoryActionResult, RepositoryActionResult>();
            services.AddTransient<IRepositoryResult, RepositoryResult>();
            services.AddTransient<iDBCreation, dBCreation>();
            services.AddTransient<iGLFinancialAccountRelation, gLFinancialAccountRelation>();
            services.AddTransient<iBranchsService, branchsService>();
            services.AddTransient<iUserInformation, UserInformation>();
            services.AddTransient<iDefultDataRelation, defultDataRelation>();
            services.AddTransient<ISecurityIntegrationService, SecurityIntegrationService>();
            services.AddTransient<iInvoicesIntegrationService, InvoicesIntegrationService>();
            services.AddTransient<IPersonHelperService, PersonHelperService>();
            services.AddTransient<IDeletedRecordsServices, DeletedRecordsServices>();
            services.AddTransient<IDashboardData, DashboardData>();
            
            #region General Services 
            services.AddTransient<iAuthService, authService>();
            services.AddTransient<iLoginService, loginService>();
            services.AddTransient<ICookiesService, cookiesService>();
            services.AddTransient<IEmailService, emailService>();
            services.AddTransient<iItemFundGLIntegrationService, itemFundGLIntegrationService>();
            services.AddTransient<IPrintService, PrintService>();
            services.AddTransient<IprintFileService, PrintFileService>();
            services.AddTransient<iUpdateService, updateService>(); 
            services.AddTransient<IBalanceBarcodeProcs, BalanceBarcodeProcs>();
            services.AddTransient<IGeneralPrint, GeneralPrint>();
            services.AddTransient<iUpdateMigrationForCompanies, updateMigrationForCompanies>();
            services.AddTransient<IPrinterService, PrinterService>();



            #endregion

            #region  Restaurants Services

            services.AddTransient<iKitchensServices, KitchensServices>();

            #endregion



            #region  Service
            #region General APIs
            services.AddTransient<iUserService, userService>();
            services.AddTransient<iPermissionService, permissionService>();
            services.AddTransient<iAuthorizationService, authorizationService>();
            services.AddTransient<ISystemHistoryLogsService, systemHistoryLogsService>();
            services.AddTransient<IPrepareDataForProfit, PrepareDataForProfit>();
            #endregion
            #region General Ledger Services
            services.AddTransient<ICurrencyBusiness, CurrencyBusiness>();
            services.AddTransient<ICostCentersBusiness, CostCentersBusiness>();
            services.AddTransient<IFinancialAccountBusiness, FinancialAccountBusiness>();
            services.AddTransient<IFinancialAccountForOpeningBalanceBusiness, FinancialAccountForOpeningBalanceBusiness>();
            services.AddTransient<IFinancialAccountCostCenterBusiness, FinancialAccountCostCenterBusiness>();
            //services.AddTransient<IJournalEntryBusiness, JournalEntryBusiness>();
            services.AddTransient<GL_IGeneralSettingsBusiness, GL_GeneralSettingsBusiness>();
            services.AddTransient<IJournalEntryDraftBusiness, JournalEntryDraftBusiness>();
            services.AddTransient<IBalanceReviewBusiness, BalanceReviewBusiness>();
            services.AddTransient<ISafesBusiness, SafesBusiness>();
            services.AddTransient<Services.Process.Bank.iBanksService, BanksBusiness>();
            services.AddTransient<IIncomeListBusiness, IncomeListBusiness>();
            services.AddTransient<IBalanceForLastPeriodBusiness, BalanceForLastPeriodBusiness>();
            services.AddTransient<IBalanceReviewDetailedBusiness, BalanceReviewDetailedBusiness>();
            services.AddTransient<ICostCentersReportBusiness, CostCentersReportBusiness>();
            services.AddTransient<IFinancialSettingBusiness, FinancialSettingBusiness>();
            services.AddTransient<IOtherAuthoritiesServices, OtherAuthoritiesServices>();
            services.AddTransient<ILedgerReportService, LedgerReportService>();
            services.AddTransient<IBudgetReportService, BudgetReportService>();
            services.AddTransient<IIncomeListAndBudget, IncomeListAndBudget>();
            services.AddTransient<ICostCenterReport, CostCenterReport>();
            services.AddTransient<ICollectionReceipts, CollectionReceipts>(); 
            services.AddTransient<IReceiptsService, ReceiptsService>(); 
            services.AddTransient<IHistoryReceiptsService, HistoryReceiptsService>(); 
            services.AddTransient<ICompinedReceiptsService, CompinedReceiptsService>();


            #endregion
            #region Store Services 
            services.AddTransient<IEditedItemsService,EditedItemsService>();
            services.AddTransient<ICalculateProfitService,CalculateProfitService>();
            services.AddTransient<IBranchesBusiness, BranchesBusiness>();
            services.AddTransient<IGeneralSettingsBusiness, GeneralSettingsBusiness>();
            services.AddTransient<IColorsService, ColorsService>();
            services.AddTransient<ISizesService, SizesService>();
            services.AddTransient<IStorePlacesService, StorePlacesService>();
            services.AddTransient<IJobsService, JobsService>();
            services.AddTransient<IUnitsService, UnitsService>();
            services.AddTransient<IStoresService, StoresService>();
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IEmployeeServices, EmployeeServices>();
            services.AddTransient<IInvGeneralSettingsService, InvGeneralSettingsService>();
            services.AddTransient<ICommistionListService, CommistionListService>();
            services.AddTransient<ISalesManService, SalesManService>();
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IPaymentMethodsService, PaymentMethodsService>();
            services.AddTransient<IDiscountService, DiscountService>();
            services.AddTransient<ICompanyDataService, CompanyDataService>();
            services.AddTransient<IAddPurchaseService, AddPurchaseService>();
            services.AddTransient<IGeneralAPIsService, GeneralAPIsService>();
            services.AddTransient<IBarCodeService, BarCodeService>();
            services.AddTransient<IFundsCustomerSupplierService, FundsCustomerSupplierService>();
            services.AddTransient<IFundsBankSafeService, FundsBankSafeService>();
            services.AddTransient<IPurchasesAdditionalCostsService, PurchasesAdditionalCostsService>();
            services.AddTransient<IGetInvoiceByIdService, GetInvoiceByIdService>();
            services.AddTransient<IGetAllPurchasesService, GetAllPurchasesService>();
            services.AddTransient<IUpdatePurchaseService, UpdatePurchaseService>();
            services.AddTransient<ICalculationSystemService, CalculationSystemService>();
            services.AddTransient<IDeletePurchaseService, DeletePurchaseService>();
            services.AddTransient<IHistoryInvoiceService, HistoryInvoiceService>();
            services.AddTransient<ISendEmailPurchases, SendEmailPurchases>();
            services.AddTransient<IAddInvoice, AddInvoice>();
            services.AddTransient<IGetPurchaseByCodeForReturn, GetPurchaseByCodeForReturn>();
            services.AddTransient<IUpdateInvoice, UpdateInvoice>();
            services.AddTransient<IDeleteInvoice, DeleteInvoice>();
            services.AddTransient<IGetInvoiceForReturn, GetInvoiceForReturn>();
            services.AddTransient<IGetAllInvoicesService, GetAllInvoicesService>();
            services.AddTransient<IAddReturnPurchaseService, AddReturnPurchaseService>();
            services.AddTransient<IGetAllReturnPurchaseService, GetAllReturnPurchaseService>();
            services.AddTransient<IAddRecieptsForInvoices, AddRecieptsForInvoices>();
            services.AddTransient<IAccrediteInvoice, AccrediteInvoice>();
            services.AddTransient<IDeleteSalesService, DeleteSalesService>();
            services.AddTransient<IReceiptsFromInvoices, ReceiptsFromInvoices>();

            //Shared Services
            services.AddTransient<IFileHandler, FileHandler>(); 
            services.AddTransient<IProgressService, ProgressService>(); 
            services.AddScoped<IInvGeneralSettingsHandler, InvGeneralSettingsHandler>();
            services.AddScoped<IFilesOfInvoices, FilesOfInvoices>();
            services.AddTransient<IPaymentMethodsForInvoiceService, PaymentMethodsForInvoiceService>();


            services.AddTransient<Itestfiles, testfiles>();
            //services.AddTransient<IAddHistory, AddHistory>();
            services.AddTransient(typeof(IHistory<>), typeof(History<>));
            services.AddTransient<IAddAdditionPermissionService, AddAdditionPermissionService>();
            services.AddTransient<IGetAllAdditionPermissionService, GetAllAdditionPermissionService>();
            services.AddTransient<IUpdateAdditionPermissionService, UpdateAdditionPermissionService>();
            services.AddTransient<IDeleteAdditionPermissionService, DeleteAdditionPermissionService>();
            services.AddTransient<IAddItemsFundService, AddItemsFundService>();
            services.AddTransient<IGetAllitemsFundService, GetAllitemsFundService>();
            services.AddTransient<IUpdateItemsFundService, UpdateItemsFundService>();
            services.AddTransient<IDeleteItemsFundService, DeleteItemsFundService>();   
            services.AddTransient<IAddSalesService, AddSalesService>();   
            services.AddTransient<IGetAllSalesServices, GetAllSalesServices>();
            services.AddTransient<IUpdateSalesService, UpdateSalesService>();
            services.AddTransient<ISerialsService, SerialsService>();   
            services.AddTransient<IAddReturnSalesService, AddReturnSalesService>();   
            services.AddTransient<IGetAllReturnSalesService, GetAllReturnSalesService>();   
            services.AddTransient<IGetSalesByCodeForReturn, GetSalesByCodeForReturn>();   
            services.AddTransient<IGetPOSInvoicesService, GetPOSInvoicesService>();
            services.AddTransient<IPOSInvSuspensionService, POSInvSuspensionService>();
            services.AddTransient<IAddSuspensionInvoice, AddSuspensionInvoice>();
            services.AddTransient<IGetInvSuspensionService, GetInvSuspensionService>();
            services.AddTransient<IAddPOSInvoice, AddPOSInvoice>();
            services.AddTransient<IAddExtractPermissionService, AddExtractPermissionService>();
            services.AddTransient<IGetAllExtractPermissionService, GetAllExtractPermissionService>();
            services.AddTransient<IUpdateExtractPermissionService, UpdateExtractPermissionService>();
            services.AddTransient<IDeleteExtractPermissionService, DeleteExtractPermissionService>();
            services.AddTransient<IRedefineInvoiceRequestService, RedefineInvoiceRequestService>();
            services.AddTransient<IPosService, PosService>();
            services.AddTransient<IPOSTouchService, POSTouchService>();
            services.AddTransient<IUpdateItemsPrices, UpdateItemsPrices>();
            services.AddTransient<IPersonLastPriceService, PersonLastPriceService>();
            services.AddTransient<IAddTempInvoiceService, AddTempInvoiceService>();
            services.AddTransient<IUpdateTempInvoiceService, UpdateTempInvoiceService>();
            services.AddTransient<IGetAllTempInvoicesServices, GetAllTempInvoicesServices>();
            services.AddTransient<IGetTempInvoiceByIdService, GetTempInvoiceByIdService>();
            services.AddTransient<IDeleteTempInvoiceService, DeleteTempInvoiceService>();
            services.AddTransient<ITransferToSalesService, TransferToSalesService>();
            #endregion

            #endregion


            #region Reports

            services.AddTransient<IFilesMangerService, FilesMangerService>(); 
            services.AddTransient<IReportFileService, ReportFileService>();
            services.AddTransient<IPrintResponseService, PrintResponseService>();
            services.AddTransient<IPermissionsForPrint, PermissionsForPrint>();
            services.AddTransient<IRpt_ItemsPrices, Rpt_ItemsPrices>();
            services.AddTransient<IItemPurchasesForSupplierService, ItemPurchasesForSupplierService>();
            services.AddTransient<IItemsPurchasesForSupplierService, ItemsPurchasesForSupplierService>();
            services.AddTransient<IPersonAccountService, PersonAccountService>();
            services.AddTransient<ISuppliersAccountService, SuppliersAccountService>();
            services.AddTransient<IpurchasesTransactionService, purchasesTransactionService>();
            services.AddTransient<IItemsPurchasesService, ItemsPurchasesService>();
            services.AddTransient<IVatStatementService, VatStatementService>();
            services.AddTransient<IDetailedTransOfItemService, DetailedTransOfItemService>();
            services.AddTransient<IitemBalanceInStoresService, itemBalanceInStoresService>();
            services.AddTransient<IitemUnitHelperServices, itemUnitHelperServices>();
            services.AddTransient<ItotalTransactionsOfItemsServices, totalTransactionsOfItemsServices>();
            services.AddTransient<IRpt_Store, Rpt_Store>();
            services.AddTransient<iRpt_PurchasesService, rpt_PurchasesService>();
            services.AddTransient<iInvoicePDFService, InvoicePDFService>();
            services.AddTransient<IDetailsOfSerialReportService, DetailsOfSerialReportService>();
            services.AddTransient<iRPT_Sales, RPT_Sales>();
            services.AddTransient<iSafeOrBankReportService, safeOrBankReportService>();
            services.AddTransient<iRPT_MainData, RPT_MainData>();
            services.AddTransient<IItemCardService, ItemCardService>();
            services.AddTransient<IRPT_SalesProfit, RPT_SalesProfit>();
            services.AddTransient<IDemandLimitNotificationService, DemandLimitNotification>();
            services.AddTransient<IOfferPriceLanding, OfferPriceLanding>();
            services.AddTransient<IUpdatePrintFiles, UpdatPrintFiles>();
            services.AddTransient<ICompanyDataForInvoicePrint,CompanyDataForInvoicePrint>();
            services.AddTransient<ICreateDataTable,CreateDataTable>();
            services.AddTransient<IPricesOffersPrint, PricesOffersPrint>();



            #endregion

            #region  PageList

            services.AddScoped<IPagedList<FinancialAccountDto, FinancialAccountDto>, PagedList<FinancialAccountDto, FinancialAccountDto>>();
            services.AddScoped<IPagedList<JournalEntryDto, JournalEntryDto>, PagedList<JournalEntryDto, JournalEntryDto>>();
            services.AddScoped<IPagedList<GLJournalEntry, GLJournalEntry>, PagedList<GLJournalEntry, GLJournalEntry>>();
            services.AddScoped<IPagedList<GLJournalEntryDetails, GLJournalEntryDetails>, PagedList<GLJournalEntryDetails, GLJournalEntryDetails>>();
            services.AddScoped<IPagedList<GLBranch, GLBranch>, PagedList<GLBranch, GLBranch>>();
            services.AddScoped<IPagedList<GLCurrency, GLCurrency>, PagedList<GLCurrency, GLCurrency>>();
            services.AddScoped<IPagedList<GLGeneralSetting, GLGeneralSetting>, PagedList<GLGeneralSetting, GLGeneralSetting>>();
            services.AddScoped<IPagedList<GLRecHistory, GLRecHistory>, PagedList<GLRecHistory, GLRecHistory>>();
            services.AddScoped<IPagedList<AllTreasuryDto, AllTreasuryDto>, PagedList<AllTreasuryDto, AllTreasuryDto>>();
            services.AddScoped<IPagedList<BankResponsesDTOs.GetAll, BankResponsesDTOs.GetAll>, PagedList<BankResponsesDTOs.GetAll, BankResponsesDTOs.GetAll>>();
            services.AddScoped<IPagedList<CostCenterDto, CostCenterDto>, PagedList<CostCenterDto, CostCenterDto>>();
            services.AddScoped<IPagedList<GenaricCostCenterDto, GenaricCostCenterDto>, PagedList<GenaricCostCenterDto, GenaricCostCenterDto>>();
            services.AddScoped<IPagedList<ReceiptsForPaymentVoucherBankDto, ReceiptsForPaymentVoucherBankDto>, PagedList<ReceiptsForPaymentVoucherBankDto, ReceiptsForPaymentVoucherBankDto>>();
            services.AddScoped<IPagedList<ReceiptsForPaymentVoucherTreasuryDto, ReceiptsForPaymentVoucherTreasuryDto>, PagedList<ReceiptsForPaymentVoucherTreasuryDto, ReceiptsForPaymentVoucherTreasuryDto>>();
            services.AddScoped<IPagedList<GLFinancialAccount, GLFinancialAccount>, PagedList<GLFinancialAccount, GLFinancialAccount>>();


            services.AddScoped<IPagedList<GLBranch, GLBranch>, PagedList<GLBranch, GLBranch>>();

            services.AddScoped<IPagedList<GLGeneralSetting, GLGeneralSetting>, PagedList<GLGeneralSetting, GLGeneralSetting>>();
            services.AddScoped<IPagedList<RecHistory, RecHistory>, PagedList<RecHistory, RecHistory>>();

            #endregion

            return services;
        }
    }
}
