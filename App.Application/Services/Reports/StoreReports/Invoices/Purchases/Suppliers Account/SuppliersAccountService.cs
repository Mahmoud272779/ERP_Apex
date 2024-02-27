using App.Application.Helpers.CashingService.GetFromCache;
using App.Application.Helpers.CashingService.SetInCash;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Store.Reports.Invoices.Purchases;
using App.Domain.Models.Security.Authentication.Response.Store.Reports.Purchases;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Reports.StoreReports.Invoices.Purchases.Suppliers_Account
{
    public class SuppliersAccountService : ISuppliersAccountService
    {
        private readonly IRepositoryQuery<GlReciepts> RecieptQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersons;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;
        private readonly IRoundNumbers _iRoundNumbers;
        private readonly IMemoryCache _memoryCache;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IMediator _mediator;

        public SuppliersAccountService(IRepositoryQuery<GlReciepts> RecieptQuery, iUserInformation iUserInformation, IGeneralPrint iGeneralPrint, IRoundNumbers iRoundNumbers, IRepositoryQuery<InvPersons> invPersons, IMemoryCache memoryCache, ISecurityIntegrationService securityIntegrationService, IMediator mediator)
        {
            this.RecieptQuery = RecieptQuery;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _iRoundNumbers = iRoundNumbers;
            _InvPersons = invPersons;
            _memoryCache = memoryCache;
            _securityIntegrationService = securityIntegrationService;
            _mediator = mediator;
        }

        public async Task<ResponseResult> GetSuppliersAccountData(SuppliersAccountRequest request, bool isSales, bool isPrint = false)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            //data in period
            List<int> branches = request.Branches.Split(',').Select(a => string.IsNullOrWhiteSpace(a) ? 0 : Convert.ToInt32(a.Trim())).Where(a => a != 0).ToList();
            var companyInfo = await _securityIntegrationService.getCompanyInformation();
            string cashKey = string.Empty;
            if (isSales)
            {
                cashKey = "CustomersBalance_" + companyInfo.companyLogin;
            }
            else
            {
                cashKey = "SuppliersBalance_" + companyInfo.companyLogin;
            }



            IEnumerable<SuppliersAccountList> allData = null;
            var cachedData = await _mediator.Send(new GetFromCacheRequest { key = cashKey });

            if ((request.PageNumber == 1 && !isPrint) || cachedData == null)
            {
                var persons = _InvPersons.TableNoTracking;
                var data = RecieptQuery.TableNoTracking
                                        .Where(x => isSales ? x.ParentTypeId == (int)DocumentType.POS || x.ParentTypeId == (int)DocumentType.ReturnPOS ? (!userInfo.otherSettings.posShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : (!userInfo.otherSettings.salesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true) : (!userInfo.otherSettings.purchasesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true))
                                        .Where(x => !userInfo.otherSettings.purchasesShowOtherPersonsInv ? x.UserId == userInfo.employeeId : true)
                                        .Where(a => !a.IsBlock)
                                        .Where(a => (request.IsSupplier ? a.person.IsSupplier == true : a.person.IsCustomer == true) && (branches.Contains(a.BranchId)));


                var _trans = data.Where(a =>
                                           a.RecieptDate.Date >= request.DateFrom.Date &&
                                           a.RecieptDate.Date <= request.DateTo.Date
                                            );
                var trans = _trans
                                          //.Where(c=> personPage.Contains(c.PersonId))
                                          .GroupBy(e => new { e.PersonId })
                                          .Select(a => new SuppliersAccountList()
                                          {
                                              supplierId = a.First().PersonId ?? 0,
                                              supplierName = persons.FirstOrDefault(c => c.Id == a.First().PersonId).ArabicName,
                                              supplierNameEn = persons.FirstOrDefault(c => c.Id == a.First().PersonId).LatinName,

                                              transDebtor = a.Sum(e => e.Debtor),
                                              transCreditor = a.Sum(e => e.Creditor)
                                          })
                                          .ToList();

                var _prev = data.Where(a => a.RecieptDate.Date < request.DateFrom.Date);
                var prev = _prev
                    //.Where(c => personPage.Contains(c.PersonId))
                    .GroupBy(e => new { e.PersonId })
                    .Select(a => new SuppliersAccountList()
                    {
                        supplierId = Convert.ToInt32(a.Key.PersonId),
                        prevDebtor = a.Sum(e => e.Debtor),
                        prevCreditor = a.Sum(e => e.Creditor)
                    }).ToList();

                allData = trans.Union(prev).OrderBy(a => a.supplierId).GroupBy(a => a.supplierId)
                    .Select(a => new SuppliersAccountList()
                    {
                        supplierId = a.First().supplierId,
                        supplierName = persons.Where(c => c.Id == a.First().supplierId).FirstOrDefault().ArabicName,//a.First().supplierName,
                        supplierNameEn = persons.Where(c => c.Id == a.First().supplierId).FirstOrDefault().LatinName,//a.First().supplierNameEn,

                        prevDebtor = _iRoundNumbers.GetRoundNumber(a.Sum(a => a.prevDebtor)),
                        prevCreditor = _iRoundNumbers.GetRoundNumber(a.Sum(a => a.prevCreditor)),

                        transDebtor = _iRoundNumbers.GetRoundNumber(a.Sum(a => a.transDebtor)),
                        transCreditor = _iRoundNumbers.GetRoundNumber(a.Sum(a => a.transCreditor)),


                        balanceDebtor = Math.Abs(_iRoundNumbers.GetRoundNumber((((a.Sum(a => a.prevCreditor) + a.Sum(a => a.transCreditor)) -
                                       (a.Sum(a => a.prevDebtor) + a.Sum(a => a.transDebtor))) > 0 ? 0 :
                                       ((a.Sum(a => a.prevCreditor) + a.Sum(a => a.transCreditor)) -
                                       (a.Sum(a => a.prevDebtor) + a.Sum(a => a.transDebtor)))))),
                        balanceCreditor = Math.Abs(_iRoundNumbers.GetRoundNumber((((a.Sum(a => a.prevCreditor) + a.Sum(a => a.transCreditor)) -
                                        (a.Sum(a => a.prevDebtor) + a.Sum(a => a.transDebtor))) > 0 ?
                                       ((a.Sum(a => a.prevCreditor) + a.Sum(a => a.transCreditor)) -
                                        (a.Sum(a => a.prevDebtor) + a.Sum(a => a.transDebtor))) : 0)))
                    })
                    .ToList();
                _mediator.Send(new SetInCashRequest
                {
                    data = allData,
                    expirationTime = TimeSpan.FromMinutes(10),
                    key = cashKey
                });
                if (!request.zeroBalances)
                    allData = allData.Where(x => x.balanceCreditor + x.balanceDebtor != 0);
            }
            else
            {
                allData = (List<SuppliersAccountList>)cachedData;
                if (!request.zeroBalances)
                    allData = allData.Where(x => x.balanceCreditor + x.balanceDebtor != 0);
            }









            var FinalResult = isPrint ? allData : allData.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
            //var FinalResult = allData;

            var totalData = new SuppliersAccountResponse();

            totalData.SuppliersAccountData = FinalResult.ToList();
            //الرصيد الفعلي
            totalData.totalPrevDebtor = _iRoundNumbers.GetRoundNumber(allData.Sum(a => a.prevDebtor));
            totalData.totalPrevCreditor = _iRoundNumbers.GetRoundNumber(allData.Sum(a => a.prevCreditor));


            totalData.totalTransDebtor   = _iRoundNumbers.GetRoundNumber(allData.Sum(a => a.transDebtor));
            totalData.totalTransCreditor = _iRoundNumbers.GetRoundNumber(allData.Sum(a => a.transCreditor));
            // الرصيد عن فترة

            //var Balance = _iRoundNumbers.GetRoundNumber(allData.Sum(a => a.balanceCreditor) - allData.Sum(a => a.balanceDebtor));
            var Balance = _iRoundNumbers.GetRoundNumber((totalData.totalPrevDebtor + totalData.totalTransDebtor) - (totalData.totalPrevCreditor + totalData.totalTransCreditor));

            if (Balance > 0)
                totalData.totalBalanceCreditor = _iRoundNumbers.GetRoundNumber(Math.Abs(Balance));
            else
                totalData.totalBalanceDebtor = _iRoundNumbers.GetRoundNumber(Math.Abs(Balance));
            
            return new ResponseResult { Data = totalData, Id = null, DataCount = allData.Count(), Result = Result.Success };

        }
        public async Task<WebReport> SuppliersCustomersBalancesReport(SuppliersAccountRequest request, bool isSales, exportType exportType, bool isArabic, int fileId = 0)
        {
            var data = await GetSuppliersAccountData(request, true,true);
            var mainData = (SuppliersAccountResponse)data.Data;

            var userInfo = await _iUserInformation.GetUserInformation();

            var otherData = ArabicEnglishDate.OtherDataWithDatesArEn(isArabic, request.DateFrom, request.DateTo);

            otherData.EmployeeName = userInfo.employeeNameAr.ToString();
            otherData.EmployeeNameEn = userInfo.employeeNameEn.ToString();

            //if (isArabic)
            //{
            //    otherdata.DateFrom = request.DateFrom.ToString("yyyy/MM/dd");
            //    otherdata.DateTo = request.DateTo.ToString("yyyy/MM/dd");
            //    otherdata.Date = DateTime.Now.ToString("yyyy/MM/dd");
            //}
            //else
            //{
            //    otherdata.DateFrom = request.DateFrom.ToString("dd/MM/yyyy");
            //    otherdata.DateTo = request.DateTo.ToString("dd/MM/yyyy");
            //    otherdata.Date = DateTime.Now.ToString("dd/MM/yyyy");
            //}
            var tablesNames = new TablesNames()
            {

                ObjectName = "SuplierCustomerAcountResponse",
                FirstListName = "SuplierCustomerAcountList"
            };
            int screenId = 0;
            if (request.IsSupplier)
            {
                screenId = (int)SubFormsIds.GetSuppliersAccountData_Purchases;
            }
            else
            {
                screenId = (int)SubFormsIds.CustomersBalances;

            }
            var report = await _iGeneralPrint.PrintReport<SuppliersAccountResponse, SuppliersAccountList, object>(mainData, mainData.SuppliersAccountData, null, tablesNames, otherData
             , screenId, exportType, isArabic,fileId);
            return report;


        }
    }
}
