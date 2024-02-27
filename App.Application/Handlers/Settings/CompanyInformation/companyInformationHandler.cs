using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities.POS;
using App.Domain.Entities.Setup;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using MediatR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Settings
{
    public class companyInformationHandler : IRequestHandler<companyInformationRequest, ResponseResult>
    {
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IRepositoryQuery<userAccount> _userAccountQuery;
        private readonly IRepositoryQuery<POS_OfflineDevices> _POS_OfflineDevicesQuery;
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly IRepositoryQuery<InvEmployees> _InvEmployeesQuery;
        private readonly IRepositoryQuery<InvStpStores> _InvStpStoresQuery;
        private readonly IRepositoryQuery<GLBranch> _GLBranchQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _InvStpItemCardMasterQuery;
        private readonly IConfiguration _configuration;
        public companyInformationHandler(ISecurityIntegrationService securityIntegrationService, IRepositoryQuery<userAccount> userAccountQuery, IRepositoryQuery<POSSession> pOSSessionQuery, IRepositoryQuery<InvEmployees> invEmployeesQuery, IRepositoryQuery<InvStpStores> invStpStoresQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery, IRepositoryQuery<GLBranch> gLBranchQuery, IConfiguration configuration, IRepositoryQuery<POS_OfflineDevices> pOS_OfflineDevicesQuery)
        {
            _securityIntegrationService = securityIntegrationService;
            _userAccountQuery = userAccountQuery;
            _POSSessionQuery = pOSSessionQuery;
            _InvEmployeesQuery = invEmployeesQuery;
            _InvStpStoresQuery = invStpStoresQuery;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _InvPersonsQuery = invPersonsQuery;
            _InvStpItemCardMasterQuery = invStpItemCardMasterQuery;
            _GLBranchQuery = gLBranchQuery;
            _configuration = configuration;
            _POS_OfflineDevicesQuery = pOS_OfflineDevicesQuery;
        }

        public async Task<ResponseResult> Handle(companyInformationRequest request, CancellationToken cancellationToken)
        {
            ERP_UsersManagerContext _userManagementcontext = new ERP_UsersManagerContext(_configuration);


            var companyInfo = await _securityIntegrationService.getCompanyInformation();



            var companyBundleInfo = _userManagementcontext.UserApplicationCashes.Where(x => x.Id == companyInfo.requestId).FirstOrDefault();




            var AdditionalPriceSubscriptions = _userManagementcontext.AdditionalPriceSubscriptions.Where(x => x.SubRequestId == companyInfo.requestId);
            var CountOfUsers = _userAccountQuery.TableNoTracking.Count();
            var CountOfOpenedPOSSessions = _POSSessionQuery.TableNoTracking.Where(x => x.sessionStatus != (int)POSSessionStatus.closed).Count();
            var countOfEmployees = _InvEmployeesQuery.TableNoTracking.Count();
            var countOfStores = _InvStpStoresQuery.TableNoTracking.Count();
            var countOfBranches = _GLBranchQuery.TableNoTracking.Count();
            var countOfInvoices = _InvoiceMasterQuery.TableNoTracking.Where(x => Lists.MainInvoiceForReturn.Contains(x.InvoiceTypeId)).Count();
            var countOfCustomers = _InvPersonsQuery.TableNoTracking.Where(x => x.IsCustomer).Count();
            var countOfSuppliers = _InvPersonsQuery.TableNoTracking.Where(x => x.IsSupplier).Count();
            var countOfItems = _InvStpItemCardMasterQuery.TableNoTracking.Count();
            var countOfOfflinePOS = _POS_OfflineDevicesQuery.TableNoTracking.Count();


            var bundleInformation = new bundleInformation()
            {
                NumberOfUser = companyBundleInfo.IsInfinityNumbersOfUsers ? -1 : companyBundleInfo.AllowedNumberOfUsersOfBundle,
                NumberOfPOS = companyBundleInfo.IsInfinityNumbersOfPos ? -1 : companyBundleInfo.AllowedNumberOfPosofBundle,
                NumberOfEmployee = companyBundleInfo.IsInfinityNumbersOfEmployees ? -1 : companyBundleInfo.AllowedNumberOfEmployeesOfBundle,
                NumberOfStore = companyBundleInfo.IsInfinityNumbersOfStores ? -1 : companyBundleInfo.AllowedNumberOfStoresOfBundle,
                NumberOfBranchs = companyBundleInfo.IsInfinityNumbersOfBranchs ? -1 : companyBundleInfo.AllowedNumberOfBranchs,
                NumberOfInvoices = companyBundleInfo.IsInfinityNumbersOfInvoices ? -1 : companyBundleInfo.AllowedNumberOfInvoices,
                NumberOfSuppliers = companyBundleInfo.IsInfinityNumbersOfSuppliers ? -1 : companyBundleInfo.AllowedNumberOfSuppliers,
                NumberOfCustomers = companyBundleInfo.IsInfinityNumbersOfCustomers ? -1 : companyBundleInfo.AllowedNumberOfCustomers,
                NumberOfItems = companyBundleInfo.IsInfinityItems == true ? -1 : companyBundleInfo.AllowedNumberOfItems,
            };
            var AdditionalIformation = new AdditionalIformation()
            {
                NumberOfUser = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraUsers).FirstOrDefault()?.Count ?? 0,
                NumberOfPOS = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraPOS).FirstOrDefault()?.Count ?? 0,
                NumberOfEmployee = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraEmployees).FirstOrDefault()?.Count ?? 0,
                NumberOfStore = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraStores).FirstOrDefault()?.Count ?? 0,
                NumberOfBranchs = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraBranches).FirstOrDefault()?.Count ?? 0,
                NumberOfInvoices = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraInvoices).FirstOrDefault()?.Count ?? 0,
                NumberOfSuppliers = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraSuppliers).FirstOrDefault()?.Count ?? 0,
                NumberOfCustomers = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraCustomers).FirstOrDefault()?.Count ?? 0,
                NumberOfItems = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraItems).FirstOrDefault()?.Count * 1000 ?? 0,
                NumberOfOfflinePOS = AdditionalPriceSubscriptions.Where(x => x.AdditionalPriceId == (int)SecurityApplicationAdditionalPriceIndexs.extraOfflinePOS).FirstOrDefault()?.Count ?? 0
            };

            var bundleConsumption = new bundleConsumption()
            {
                //Users
                NumberOfUser = !companyBundleInfo.IsInfinityNumbersOfUsers ? bundleInformation.NumberOfUser + AdditionalIformation.NumberOfUser : -1,
                ConsumptionNumberOfUser = CountOfUsers,
                //remainingNumberOfUser = (bundleInformation.NumberOfUser + AdditionalIformation.NumberOfUser) == 0 ? 0 : (bundleInformation.NumberOfUser + AdditionalIformation.NumberOfUser) - CountOfUsers,

                //POS
                NumberOfPOS = !companyBundleInfo.IsInfinityNumbersOfPos ? bundleInformation.NumberOfPOS + AdditionalIformation.NumberOfPOS : -1,
                ConsumptionNumberOfPOS = CountOfOpenedPOSSessions,
                //remainingNumberOfPOS = (bundleInformation.NumberOfPOS + AdditionalIformation.NumberOfPOS) == 0 ? 0 : (bundleInformation.NumberOfPOS + AdditionalIformation.NumberOfPOS) - CountOfOpenedPOSSessions,

                //Employees
                NumberOfEmployee = !companyBundleInfo.IsInfinityNumbersOfEmployees ? bundleInformation.NumberOfEmployee + AdditionalIformation.NumberOfEmployee : -1,
                ConsumptionNumberOfEmployee = countOfEmployees,
                //remainingNumberOfEmployee = (bundleInformation.NumberOfEmployee + AdditionalIformation.NumberOfEmployee) == 0 ? 0 : (bundleInformation.NumberOfEmployee + AdditionalIformation.NumberOfEmployee) - countOfEmployees,

                //stores
                NumberOfStore = !companyBundleInfo.IsInfinityNumbersOfStores ? bundleInformation.NumberOfStore + AdditionalIformation.NumberOfStore : -1,
                ConsumptionNumberOfStore = countOfStores,
                //remainingNumberOfStore = (bundleInformation.NumberOfStore + AdditionalIformation.NumberOfStore) == 0 ? 0 : (bundleInformation.NumberOfStore + AdditionalIformation.NumberOfStore) - countOfStores,

                //branches 
                NumberOfBranchs = !companyBundleInfo.IsInfinityNumbersOfBranchs ? bundleInformation.NumberOfBranchs + AdditionalIformation.NumberOfBranchs : -1,
                ConsumptionNumberOfBranchs = countOfBranches,
                //remainingNumberOfBranchs = (bundleInformation.NumberOfBranchs + AdditionalIformation.NumberOfBranchs) == 0 ? 0 : (bundleInformation.NumberOfBranchs + AdditionalIformation.NumberOfBranchs) - countOfBranches,

                //Invoices 
                NumberOfInvoices = !companyBundleInfo.IsInfinityNumbersOfInvoices ? bundleInformation.NumberOfInvoices + AdditionalIformation.NumberOfInvoices : -1,
                ConsumptionNumberOfInvoices = countOfInvoices,
                //remainingNumberOfInvoices = (bundleInformation.NumberOfInvoices + AdditionalIformation.NumberOfInvoices) == 0 ? 0 : (bundleInformation.NumberOfInvoices + AdditionalIformation.NumberOfInvoices) - countOfInvoices,

                //suplters 
                NumberOfSuppliers = !companyBundleInfo.IsInfinityNumbersOfSuppliers ? bundleInformation.NumberOfSuppliers + AdditionalIformation.NumberOfSuppliers : -1,
                ConsumptionNumberOfSuppliers = countOfSuppliers,
                //remainingNumberOfSuppliers = (bundleInformation.NumberOfSuppliers + AdditionalIformation.NumberOfSuppliers) == 0 ? 0 : (bundleInformation.NumberOfSuppliers + AdditionalIformation.NumberOfSuppliers) - countOfSuppliers,

                //customers
                NumberOfCustomers = !companyBundleInfo.IsInfinityNumbersOfCustomers ? bundleInformation.NumberOfCustomers + AdditionalIformation.NumberOfCustomers : -1,
                ConsumptionNumberOfCustomers = countOfCustomers,
                //remainingNumberOfCustomers = (bundleInformation.NumberOfCustomers + AdditionalIformation.NumberOfCustomers) == 0 ? 0 : (bundleInformation.NumberOfCustomers + AdditionalIformation.NumberOfCustomers) - countOfCustomers,

                //Items 
                NumberOfItems = companyBundleInfo.IsInfinityItems == false ? bundleInformation.NumberOfItems + AdditionalIformation.NumberOfItems : -1,
                ConsumptionNumberOfItems = countOfItems,
                //remainingNumberOfItems = (bundleInformation.NumberOfItems + AdditionalIformation.NumberOfItems) == 0 ? 0 : (bundleInformation.NumberOfItems + AdditionalIformation.NumberOfItems) - countOfItems

                NumberOfOfflinePOS = AdditionalIformation.NumberOfOfflinePOS,
                ConsumptionNumberOfflinePOS = countOfOfflinePOS

            };


            var res = new subscriptionInformationResponse()
            {
                bundleId = companyInfo.bundleId,
                apps = companyInfo.apps,
                AdditionalPrices = companyInfo.AdditionalPrices,
                companyId = companyInfo.companyId,
                periodStart = companyInfo.startPeriod,
                periodEnd = companyInfo.endPeriod,
                remainingDays = Math.Ceiling((companyInfo.endPeriod - DateTime.Now.Date).TotalDays),
                arabicName = companyInfo.bundleAr,
                latinName = companyInfo.bundleEn,
                bundleTime = companyInfo.Months,
                periodType = new periodType
                {
                    Id = companyInfo.periodType,
                    arabicName = companyInfo.periodType == (int)Enums.periodTypes.monthly ? "شهور" : "سنه",
                    latinName = companyInfo.periodType == (int)Enums.periodTypes.monthly ? "Months" : "Years"
                },
                bundleInformation = bundleInformation,
                bundleConsumption = bundleConsumption,
                AdditionalIformation = AdditionalIformation,
                subscriptionId = companyInfo.requestId
            };





            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success
            };
        }
    }
}
