using AutoMapper;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Entities.Setup;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Setup.ItemCard.ViewModels;
using System.Collections.Generic;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Entities.Process.Barcode;
using App.Domain.Models.Security.Authentication.Response.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.Store;
using System.Linq;
using App.Domain.Models.Security.Authentication.Response.GeneralLedger;
using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain;
using App.Domain.Entities;
using App.Domain.Models.Request.General;
using App.Domain.Entities;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Models.Response.Store.Invoices;

using static App.Domain.POSTouchResponse;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Request.print;
using App.Domain.Entities.POS;
using App.Domain.Models.Response.General;
using System.DirectoryServices;
using App.Domain.Models.Response.Store.Reports;
using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Models.Request.Restaurants;
using App.Domain.Entities.Process.Restaurants;

namespace App.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            RegionsMapping();
        }

        private void RegionsMapping()
        {
            #region GL Mapping Profile
            CreateMap<BranchRequestsDTOs.Add, GLBranch>().ReverseMap();
            CreateMap<BranchRequestsDTOs.Update, GLBranch>().ReverseMap();

            CreateMap<FinancialAccountParameter, GLFinancialAccount>();
            CreateMap<UpdateFinancialAccountParameter, GLFinancialAccount>();

            CreateMap<CostCenterParameter, GLCostCenter>();

            CreateMap<CurrencyParameter, GLCurrency>();
            CreateMap<GLCurrency, GLCurrency>();

            CreateMap<GeneralSettingsParameter, GLGeneralSetting>();

            CreateMap<UpdateGeneralSettingsParameter, GLGeneralSetting>();

            CreateMap<FinancialAccountDto, FinancialAccountDto>();
            CreateMap<JournalEntryDto, JournalEntryDto>();


            CreateMap<GLGeneralSetting, GLGeneralSetting>();
            CreateMap<GLRecHistory, GLRecHistory>();
            CreateMap<GenaricCostCenterDto, GenaricCostCenterDto>();

            CreateMap<UpdateCostCenterParameter, GLCostCenter>();
            CreateMap<UpdateCurrencyParameter, GLCurrency>();
            CreateMap<UpdateJournalEntryParameter, GLJournalEntry>();
            CreateMap<UpdateJournalEntryParameter, GLJournalEntryDraft>();
            CreateMap<JournalEntryParameter, GLJournalEntryDraft>();
            CreateMap<JournalEntryParameter, GLJournalEntry>();
            CreateMap<CostCenterDto, CostCenterDto>();
            CreateMap<TreasuryParameter, GLSafe>();
            CreateMap<UpdateTreasuryParameter, GLSafe>();
            CreateMap<AllTreasuryDto, AllTreasuryDto>();
            CreateMap<BankRequestsDTOs.Add, GLBank>();
            CreateMap<BankRequestsDTOs.Update, GLBank>();
           // CreateMap<GlReciepts, GlReciepts>();
            CreateMap< OtherAuthoritiesParameter,GLOtherAuthorities>();
            CreateMap<UpdateOtherAuthoritiesParameter, GLOtherAuthorities>();
            CreateMap<GLOtherAuthorities, TreasuryDto>();
            CreateMap<EntryFunds, GLJournalEntryDetails>();

            CreateMap<GLFinancialAccount, FinancialAccountDto>();
            CreateMap<CostCenterReciepts, GLRecieptCostCenter>();
            CreateMap<UpdateCostCenterReciepts, GLRecieptCostCenter>();
            CreateMap<UpdateRecieptsRequest, GlReciepts>();
            CreateMap<MainRequestDataForAdd, RecieptsRequest>();
            CreateMap<CompinedReciept, GlReciepts>();


            CreateMap<GLBank, BankResponsesDTOs.GetAll>()
                .ForMember(dest => dest.AddressAr, opt => opt.MapFrom(src => src.ArabicAddress))
                .ForMember(dest => dest.AddressEn, opt => opt.MapFrom(src => src.LatinAddress))
                .ForMember(dest => dest.FinancialAccountId, opt => opt.MapFrom(src => src.FinancialAccountId != null ? src.FinancialAccountId : null))
                .ForMember(dest => dest.FinancialName, opt => opt.MapFrom(src => src.FinancialAccountId != null ? src.FinancialAccount.ArabicName : null))
                .ForMember(dest => dest.FinancialAccountCode, opt => opt.MapFrom(src => src.FinancialAccountId != null ? src.FinancialAccount.AccountCode : null))
                .ForMember(dest => dest.BranchesId, opt => opt.MapFrom(src => src.BankBranches.Select(e => e.BranchId).ToArray()))
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => src.Id == 1 || src.reciept.Count > 0 || src.BankBranches.Count > 0 || src.FundsBanksSafes.Count > 0 ? false : true))
                ;



            CreateMap<BankResponsesDTOs.GetAll, BankResponsesDTOs.GetAll>();
            CreateMap<UpdateFinancialAccountForOpeningBalanceParameter, GLFinancialAccountForOpeningBalance>();
            CreateMap<FinancialAccountForOpeningBalanceParameter, GLFinancialAccountForOpeningBalance>();
            CreateMap<ReceiptsForPaymentVoucherBankDto, ReceiptsForPaymentVoucherBankDto>();
            CreateMap<ReceiptsForPaymentVoucherTreasuryDto, ReceiptsForPaymentVoucherTreasuryDto>();
            CreateMap<GLBalanceForLastPeriod, BalanceForLastPeriodDto>();
            
            CreateMap<GLCostCenter, CostCenterDropDown>();
            CreateMap<GLCostCenter, CostCenterDto>();
            CreateMap<GLCurrency, CurrencyDto>();
            CreateMap<GLSafe, TreasurySettingDto>()
                  .ForMember(dest => dest.FinancialAccountCode, opt => opt.MapFrom(src => src.financialAccount.AccountCode))
                  .ForMember(dest => dest.FinancialAccountName, opt => opt.MapFrom(src => src.financialAccount.ArabicName));
            CreateMap<GLSafe, TreasuryDto>();
            CreateMap<GLSafe, AllTreasuryDto>()
                  .ForMember(dest => dest.FinancialName, opt => opt.MapFrom(src => src.financialAccount.ArabicName))
                  .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.ArabicName));
         
            CreateMap<GLBank, BankResponsesDTOs.BankSettingDto>()
                 .ForMember(dest => dest.FinancialAccountName, opt => opt.MapFrom(src => src.FinancialAccount.ArabicName))
                 .ForMember(dest => dest.FinancialAccountCode, opt => opt.MapFrom(src => src.FinancialAccount.AccountCode))
                ;
            CreateMap<GLFinancialAccount, FinancialAccountDropDown>();
          
            CreateMap<GLBranch, BranchResponsesDTOs.GetAll>();

            CreateMap<GLBranchHistory, HistoryDto>();
            CreateMap <InvUnitsHistory, HistoryDto>();
            CreateMap <InvBarcodeHistory, HistoryDto>();
            CreateMap <InvCategoriesHistory, HistoryDto>();
            CreateMap <InvColorsHistory, HistoryDto>();
            CreateMap <InvCommissionListHistory, HistoryDto>();
            CreateMap <InvDiscount_A_P_History, HistoryDto>();
            CreateMap <InvEmployeesHistory, HistoryDto>();
            CreateMap <InvFundsBanksSafesHistory, HistoryDto>();
            CreateMap <InvJobsHistory, HistoryDto>();
            CreateMap <InvPaymentMethodsHistory, HistoryDto>();
            CreateMap <InvPersonsHistory, HistoryDto>();
            CreateMap <InvSalesManHistory, HistoryDto>();
            CreateMap <InvSizesHistory, HistoryDto>();
            CreateMap <InvStorePlacesHistory, HistoryDto>();
            CreateMap <InvStoresHistory, HistoryDto>();
            CreateMap <InvStpItemCardHistory, HistoryDto>();
            CreateMap <InvUnitsHistory, HistoryDto>();
            CreateMap<HistoryResponceDto, HistoryDto>().ReverseMap();


            CreateMap<HistoryParameter, GLBranchHistory>();
            CreateMap<HistoryParameter, InvUnitsHistory>();
            CreateMap<HistoryParameter, InvBarcodeHistory>();
            CreateMap<HistoryParameter, InvCategoriesHistory>();
            CreateMap<HistoryParameter, InvColorsHistory>();
            CreateMap<HistoryParameter, InvCommissionListHistory>();
            CreateMap<HistoryParameter, InvDiscount_A_P_History>();
            CreateMap<HistoryParameter, InvEmployeesHistory>();
            CreateMap<HistoryParameter, InvFundsBanksSafesHistory>();
            CreateMap<HistoryParameter, InvJobsHistory>();
            CreateMap<HistoryParameter, InvPaymentMethodsHistory>();
            CreateMap<HistoryParameter, InvPersonsHistory>();
            CreateMap<HistoryParameter, InvSalesManHistory>();
            CreateMap<HistoryParameter, InvSizesHistory>();
            CreateMap<HistoryParameter, InvStorePlacesHistory>();
            CreateMap<HistoryParameter, InvStoresHistory>();
            CreateMap<HistoryParameter, InvStpItemCardHistory>();
            CreateMap<HistoryParameter, InvUnitsHistory>();
            CreateMap<HistoryParameter, InvPurchasesAdditionalCostsHistory>();

            CreateMap<InvPurchasesAdditionalCostsHistory, HistoryDto>();

            CreateMap<GlReciepts, GlRecieptsResopnseDTO>();

            CreateMap<PrinterRequestsDTOs.Add, GLPrinter>().ReverseMap();
            CreateMap<PrinterRequestsDTOs.Update, GLPrinter>().ReverseMap();

            CreateMap<KitchensParameter, Kitchens>();
            CreateMap<UpdateKitchensParameter, Kitchens>();


            //CreateMap<BranchRequestsDTOs.Update, GLBranch>().ReverseMap();

            //  CreateMap<GLBranchHistory, HistoryDto>();
            #endregion

            #region Store Mapping Profile

            CreateMap<GeneralSettingsParameter, GLGeneralSetting>();
            
            CreateMap<GLGeneralSetting, GLGeneralSetting>();
            CreateMap<RecHistory, RecHistory>();
            

            CreateMap<ColorsParameter, InvColors>();
            CreateMap<UpdateColorParameter, InvColors>().ReverseMap();
            CreateMap<InvColors, InvColors>();

            CreateMap<SizesParameter, InvSizes>();
            CreateMap<UpdateSizesParameter, InvSizes>();
            CreateMap<InvSizes, InvSizes>();
            CreateMap<StorePlacesParameter, InvStorePlaces>();
            CreateMap<UpdateStorePlacesParameter, InvStorePlaces>();
            CreateMap<InvStorePlaces, InvStorePlaces>();


            CreateMap<JobsParameter, InvJobs>();
            CreateMap<UpdateJobsParameter, InvJobs>();
            CreateMap<InvJobs, InvJobs>();



            CreateMap<AddItemRequest, InvStpItemCardMaster>().ReverseMap();
            CreateMap<UpdateItemRequest, InvStpItemCardMaster>().ReverseMap();
            CreateMap<ModifyListOfItemsStatusRequest, InvStpItemCardMaster>().ReverseMap();
            CreateMap<DeleteItemRequest, InvStpItemCardMaster>().ReverseMap();
            CreateMap<GetAllItemCardRequest, InvStpItemCardMaster>().ReverseMap();
            CreateMap<GetItemCardRequest, InvStpItemCardMaster>().ReverseMap();
            //CreateMap<GetAllItemsResponse, InvStpItemCardMaster>().ForMember(dest => dest.Category, opt => opt.MapFrom(e => e.GroupData)).ReverseMap();
            CreateMap<InvStpItemCardUnit, ItemUnitVM>().ReverseMap();
            CreateMap<InvStpItemColorSize, ItemColorSizeVM>().ReverseMap();
            CreateMap<InvStpItemCardStores, ItemStoreVM>().ReverseMap();
            CreateMap<InvStpItemCardParts, ItemPartVM>().ReverseMap();
            CreateMap<InvStpItemCardSerials, ItemSerialVM>().ReverseMap();
            CreateMap<GetAllItemsResponse, InvStpItemCardMaster>().ReverseMap();
            CreateMap<AddItemPartRequest, InvStpItemCardParts>().ReverseMap();
            CreateMap<UpdateItemPartRequest, InvStpItemCardParts>().ReverseMap();
            CreateMap<GetAllItemPartResponse, InvStpItemCardParts>().ReverseMap();
            CreateMap<PartVM, InvStpItemCardMaster>().ReverseMap();
            CreateMap<UnitVM, InvStpUnits>().ReverseMap();
            CreateMap<GetItemCardResponse, InvStpItemCardMaster>().ReverseMap();
            CreateMap<CategoriesVM, InvCategories>().ReverseMap();
            CreateMap<FillItemCardResponse, InvStpItemCardUnit>().ReverseMap();
            CreateMap<AddItemUnitRequest, InvStpItemCardUnit>().ReverseMap();
            CreateMap<UpdateItemUnitRequest, InvStpItemCardUnit>().ReverseMap();
            CreateMap<DeleteItemUnitRequest, InvStpItemCardUnit>().ReverseMap();
            CreateMap<GetAllItemStoreResponse, InvStpItemCardStores>().ReverseMap();
            CreateMap<UnitsParameter, Domain.Entities.Process.InvStpUnits>();
            CreateMap<UpdateUnitsParameter, Domain.Entities.Process.InvStpUnits>();
            CreateMap<Domain.Entities.Process.InvStpUnits, Domain.Entities.Process.InvStpUnits>();
            CreateMap<AddItemStoreRequest, InvStpItemCardStores>().ReverseMap();
            CreateMap<UpdateItemStoreRequest, InvStpItemCardStores>().ReverseMap();
            CreateMap<InvStpStores, InvStpStores>();
            CreateMap<StoresParameter, InvStpStores>();
            CreateMap<listParameter, GLBank>();

            CreateMap<InvStpStores, AllStoresDto>()
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.StoreBranches.ToList().Select(e => e.BranchId)));
            CreateMap<GLBranch, BranchesDto>().ReverseMap();
            CreateMap<UpdateStoresParameter, InvStpStores>();
            CreateMap<GetAllItemUnitResponse, InvStpItemCardUnit>().ReverseMap();
            CreateMap<InvCategories, InvCategories>();
            CreateMap<CategoriesParameter, InvCategories>();
            CreateMap<UpdateCategoryParameter, InvCategories>();

            CreateMap<InvGeneralSettings, InvGeneralSettings>();
            CreateMap<PurchasesRequest, InvGeneralSettings>();
            CreateMap<POSRequest, InvGeneralSettings>();
            CreateMap<SalesRequest, InvGeneralSettings>();
            CreateMap<OtherRequest, InvGeneralSettings>();
            CreateMap<Decimals, InvGeneralSettings>();
            CreateMap<FundsRequest, InvGeneralSettings>();
            CreateMap<BarcodeRequest, InvGeneralSettings>();
            CreateMap<VATRequest, InvGeneralSettings>();
            CreateMap<AccrediteRequest, InvGeneralSettings>();
            CreateMap<CustomerDisplayRequest, InvGeneralSettings>();

            CreateMap<UpdateSalesManRequest, InvSalesMan>();
            CreateMap<PersonRequest, InvPersons>();
            CreateMap<UpdatePersonRequest, InvPersons>();

            CreateMap<PaymentMethodsRequest, InvPaymentMethods>();
            CreateMap<UpdatePaymentMethodsRequest, InvPaymentMethods>();

            CreateMap<UpdateCompanyDataRequest, InvCompanyData>();

            CreateMap<FundsBankSafeRequest, InvFundsBanksSafesMaster>();
            CreateMap<PurchasesAdditionalCostsParameter, InvPurchasesAdditionalCosts>().ReverseMap();
            CreateMap<UpdatePurchasesAdditionalCostsParameter, InvPurchasesAdditionalCosts>().ReverseMap();
            CreateMap<PurchasesAdditionalCostsDto, InvPurchasesAdditionalCosts>().ReverseMap();
            CreateMap<AllInvoiceDto, InvoiceMaster>().ReverseMap();
            CreateMap<InvoiceDetailsRequest, InvoiceDetails>().ReverseMap();
            CreateMap<InvoiceMaster, InvoiceMaster>();
            CreateMap<InvoiceMaster, InvoiceMasterRequest>().ReverseMap();
            CreateMap<InvCategories, CategoryWithImgDto>().ReverseMap();
            CreateMap<InvEmployees, EmployeeResponsesDTOs.GetAll>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId))
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.EmployeeBranches.ToList().Select(e => e.BranchId)))
                .ForMember(dest => dest.JobNameAr, opt => opt.MapFrom(src => src.Job.ArabicName))
                .ForMember(dest => dest.JobNameEn, opt => opt.MapFrom(src => src.Job.LatinName))
                .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => src.Job.Status))
                .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => (src.Job != null || src.EmployeeBranches.Count == 0) ? src.CanDelete == true : src.CanDelete == false)).ReverseMap();

            //CreateMap<InvFundsCustomerSupplier, FundsCustomerandSuppliersDto>();
            CreateMap<InvPersons, FundsCustomerandSuppliersDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.FundsCustomerSuppliers.Id))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ArabicName, opt => opt.MapFrom(src => src.ArabicName))
                .ForMember(dest => dest.LatinName, opt => opt.MapFrom(src => src.LatinName))
                .ForMember(dest => dest.Credit, opt => opt.MapFrom(src => src.FundsCustomerSuppliers.Credit))
                .ForMember(dest => dest.Debit, opt => opt.MapFrom(src => src.FundsCustomerSuppliers.Debit))
                ;


            CreateMap<InvoiceDetails, ItemPurchasesForSupplierResponse>();
            CreateMap<InvGeneralSettings, InvGeneralSettingsResponse>();
            CreateMap<InvPersons, personsForBalanceDto>().ReverseMap();


            CreateMap<InvPersons, SupplierResponse>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.CodeT + "-" + src.Code))
                .ForMember(dest => dest.SalesmanId, opt => opt.MapFrom(src => src.SalesManId != null ? src.SalesManId : null))
                .ForMember(dest => dest.SalesmanAr, opt => opt.MapFrom(src => src.SalesManId != null ? src.SalesMan.ArabicName : null))
                .ForMember(dest => dest.SalesmanEn, opt => opt.MapFrom(src => src.SalesManId != null ? src.SalesMan.LatinName : null))
                .ForMember(dest => dest.branches, opt => opt.MapFrom(src => src.PersonBranch.Select(e => e.BranchId).ToArray()))
                // .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => (src.Id == 1 || src.Id == 2 || src.InvoiceMaster.Count > 0 || src.Discount_A_P.Count > 0 || src.reciept.Count > 0 || (src.FundsCustomerSuppliers.Credit > 0 && src.FundsCustomerSuppliers.Debit > 0)) ? false : true))
                //.ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => (src.Id == 1 || src.Id == 2 || src.InvoiceMaster.Count > 0 || src.Discount_A_P.Count > 0 || src.reciept.Count > 0||src.FundsCustomerSuppliers.Count>0) ? false : true))
                ;

            CreateMap<InvPersons, CustomerResponse>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.CodeT + "-" + src.Code))
                .ForMember(dest => dest.SalesmanId, opt => opt.MapFrom(src => src.SalesManId != null ? src.SalesManId : null))
                .ForMember(dest => dest.SalesmanAr, opt => opt.MapFrom(src => src.SalesManId != null ? src.SalesMan.ArabicName : null))
                .ForMember(dest => dest.SalesmanEn, opt => opt.MapFrom(src => src.SalesManId != null ? src.SalesMan.LatinName : null))
                //.ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => (src.Id == 1 || src.Id == 2 || src.InvoiceMaster.Count > 0 || src.Discount_A_P.Count > 0 || src.reciept.Count > 0 || (src.FundsCustomerSuppliers.Credit > 0 && src.FundsCustomerSuppliers.Debit > 0)) ? false : true))
                .ForMember(dest => dest.branches, opt => opt.MapFrom(src => src.PersonBranch.Select(e => e.BranchId).ToArray()))
                // .ForMember(dest => dest.CanDelete, opt => opt.MapFrom(src => (src.Id == 1 || src.Id == 2 || src.InvoiceMaster.Count > 0 || src.Discount_A_P.Count > 0 || src.reciept.Count > 0) ? false : true))
                ;
            CreateMap<InvoiceDetailsRequest, InvoiceDetailsAttributes>().ReverseMap();
             
            CreateMap<InvoiceMasterRequest, InvoiceResultCalculateDto>().ReverseMap();

            CreateMap<InvoiceDetailsRequest, ItemsTotalList>()
               .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => src.Total)).ReverseMap();
            CreateMap<InvoiceMasterRequest, UpdateInvoiceMasterRequest>().ReverseMap();
            CreateMap<InvoiceDetailsRequest, POSInvSuspensionDetails>().ReverseMap(); 
            CreateMap<OfferPriceMaster, InvoiceMaster>().ReverseMap();

            CreateMap<ReceiptsOfflinePos, GlReciepts>().ReverseMap();
            CreateMap<ElectronicInvoiceRequest, InvGeneralSettings>().ReverseMap();
            #endregion

            #region General Mapping profile
            CreateMap<addUsersDto, userAccount>();  
            CreateMap<editUsersDto, userAccount>();  
            CreateMap<OtherSettingsDto, otherSettings>();  
            CreateMap<addPermissionRequestDto, permissionList>();  
            CreateMap<editPermissionRequestDto, permissionList>();
            CreateMap<otherSettings, currentUserInformation>();

            CreateMap<updatedRules, rules >();
            //CreateMap<UserInformationModel, InvEmployees>();





            #endregion
            // CreateMap<PurchaseMasterRequest, InvoiceMaster>();
            //CreateMap<InvBarcodeTemplate, BarCodeDto>()
            //  .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.BarcodeItems.))
            // .ForMember(dest => dest.TotalHours, opt => opt.MapFrom(src => TimeSpan.FromHours((double)src.TotalHours).ToString("h\\:mm")))


            #region Restaurants
            CreateMap<HistoryParameter, KitchensHistory>();
            CreateMap<KitchensHistory, HistoryDto>();

            #endregion

            CreateMap<InvStpUnits, GetAllUnitsDTO>().ReverseMap();
            CreateMap<InvSalesMan, SalesManDto>()
                .ForMember(dest => dest.Branches, opt => opt.MapFrom(src => src.SalesManBranch.ToList().Select(e => e.BranchId)))
                .ForMember(dest => dest.CommissionListNameAr , opt=>opt.MapFrom(src=>src.CommissionListId != null ? src.CommissionList.ArabicName:null))
                .ForMember(dest => dest.CommissionListNameEn , opt=>opt.MapFrom(src=>src.CommissionListId != null ? src.CommissionList.LatinName:null));
            CreateMap<RecieptsRequest, GlReciepts>();
            CreateMap<InvPersons, ExportPersonModel>();
            CreateMap<InvPersons, PersonsReponseDto>().ReverseMap();

            //CreateMap<InvoiceDetailsRequest, InvoiceDetailsRequest>();
            CreateMap<ReportFiles, ReportFileRequest>().ReverseMap();
            CreateMap<InvCategories, CategoriesPOSTouchResponse>().ReverseMap();
            CreateMap<InvoiceMasterRequest, InvoiceMasterRequest>().ReverseMap();
            CreateMap<InvStpItemCardMaster, ItemCardMainData>()
                .ForMember(dest => dest.CategoryNameAr, opt => opt.MapFrom(src => src.Category.ArabicName))
                .ForMember(dest => dest.CatogeryNameEn, opt => opt.MapFrom(src => src.Category.LatinName))
                .ForMember(des=>des.StorePlaceAr,opt=>opt.MapFrom(src=>src.StorePlace.ArabicName))
                .ForMember(des=>des.StorePlaceEn,opt=>opt.MapFrom(src=>src.StorePlace.LatinName))
                .ForMember(des=>des.StorePlaceStatus,opt=>opt.MapFrom(src=>src.StorePlace.Status))
                ;

            CreateMap<totalBranchTransaction, SalesPurchasesTotals>().ReverseMap();

        }


    }
}
