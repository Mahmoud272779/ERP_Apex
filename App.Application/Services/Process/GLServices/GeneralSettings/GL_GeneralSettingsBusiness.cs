using App.Application.Basic_Process;
using App.Application.Helpers;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using App.Infrastructure.Pagination;
using App.Infrastructure.Reposiotries.Configuration;
using Attendleave.Erp.Core.APIUtilities;
using Attendleave.Erp.ServiceLayer.Abstraction;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.GeneralSettings
{
    public class GL_GeneralSettingsBusiness : BusinessBase<GLGeneralSetting>, GL_IGeneralSettingsBusiness
    {
        private readonly IRepositoryQuery<GLGeneralSetting> GeneralSettingRepositoryQuery;
        private readonly IRepositoryCommand<GLGeneralSetting> GeneralSettingRepositoryCommand;
        //persons
        private readonly IRepositoryCommand<InvPersons> PersonsCommand;
        private readonly IRepositoryQuery<InvPersons> PersonsQuery;
        //safe
        private readonly IRepositoryCommand<GLSafe> SafeCommand;
        private readonly IRepositoryQuery<GLSafe> SafeQuery;
        //Bank
        private readonly IRepositoryCommand<GLBank> BankCommand;
        private readonly IRepositoryQuery<GLBank> BankQuery;
        //SalesMan
        private readonly IRepositoryCommand<InvSalesMan> SalesManCommand;
        private readonly IRepositoryQuery<InvSalesMan> SalesManQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _financialAccount;
        private readonly IRepositoryQuery<SubCodeLevels> _subCodeLevelsQuery;
        private readonly IRepositoryCommand<SubCodeLevels> _subCodeLevelsCommand;

        //otherauthorities
        private readonly IRepositoryCommand<GLOtherAuthorities> OtherAuthoritiesCommand;
        private readonly IRepositoryQuery<GLOtherAuthorities> OtherAuthoritiesQuery;
        //purchase and sales
        private readonly IRepositoryCommand<GLPurchasesAndSalesSettings> InvoiceCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> InvoiceQuery;
        private readonly IPagedList<GLGeneralSetting, GLGeneralSetting> pagedListGeneralSetting;
        private readonly IRepositoryQuery<InvFundsBanksSafesMaster> _invFundsBanksSafesMasterQuery;
        private readonly IRepositoryQuery<InvFundsCustomerSupplier> _invFundsCustomerSupplierQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLIntegrationSettings> _gLIntegrationSettingsQuery;
        private readonly IRepositoryCommand<GLIntegrationSettings> _gLIntegrationSettingsCommand;

        public IFinancialAccountBusiness _FinancialAccountBusiness { get; }

        public GL_GeneralSettingsBusiness(
                                        IRepositoryQuery<GLGeneralSetting> _GeneralSettingRepositoryQuery,
                                        //person
                                        IRepositoryQuery<InvPersons> personQuery,
                                        IRepositoryCommand<InvPersons> personsCommand,
                                        //safe
                                        IRepositoryCommand<GLSafe> safeCommand,
                                        IRepositoryQuery<GLSafe> safeQuery,
                                        //Bank
                                        IRepositoryCommand<GLBank> bankCommand,
                                        IRepositoryQuery<GLBank> bankQuery,
                                        //salesman
                                        IRepositoryCommand<InvSalesMan> salesManCommand,
                                        IRepositoryQuery<InvSalesMan> salesManQuery,
                                        //
                                        IRepositoryQuery<GLFinancialAccount> FinancialAccount,
                                        //SubCodeLevels
                                        IRepositoryQuery<SubCodeLevels> SubCodeLevelsQuery,
                                        IRepositoryCommand<SubCodeLevels> SubCodeLevelsCommand,

                                        //
                                        IRepositoryCommand<GLOtherAuthorities> otherAuthoritiesCommand,
                                        IRepositoryQuery<GLOtherAuthorities> otherAuthoritiesQuery,
                                        IRepositoryCommand<GLPurchasesAndSalesSettings> invoiceCommand,

                                        ISystemHistoryLogsService systemHistoryLogsService,
                                        IRepositoryQuery<GLPurchasesAndSalesSettings> invoiceQuery,
                                        IRepositoryCommand<GLGeneralSetting> _GeneralSettingRepositoryCommand,
                                        IPagedList<GLGeneralSetting, GLGeneralSetting> PagedListGeneralSetting,

                                        IRepositoryQuery<InvFundsBanksSafesMaster> InvFundsBanksSafesMasterQuery,
                                        IRepositoryQuery<InvFundsCustomerSupplier> InvFundsCustomerSupplierQuery,
                                        IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
                                        iUserInformation iUserInformation,
                                        IRepositoryQuery<GLIntegrationSettings> GLIntegrationSettingsQuery,
                                        IRepositoryCommand<GLIntegrationSettings> GLIntegrationSettingsCommand,
                                       IRepositoryActionResult repositoryActionResult, IFinancialAccountBusiness financialAccountBusiness) : base(repositoryActionResult)
        {
            GeneralSettingRepositoryQuery = _GeneralSettingRepositoryQuery;
            GeneralSettingRepositoryCommand = _GeneralSettingRepositoryCommand;
            pagedListGeneralSetting = PagedListGeneralSetting;
            _invFundsBanksSafesMasterQuery = InvFundsBanksSafesMasterQuery;
            _invFundsCustomerSupplierQuery = InvFundsCustomerSupplierQuery;
            _invoiceMasterQuery = InvoiceMasterQuery;
            _iUserInformation = iUserInformation;
            _gLIntegrationSettingsQuery = GLIntegrationSettingsQuery;
            _gLIntegrationSettingsCommand = GLIntegrationSettingsCommand;
            _FinancialAccountBusiness = financialAccountBusiness;
            PersonsQuery = personQuery;
            SafeCommand = safeCommand;
            BankCommand = bankCommand;
            SalesManCommand = salesManCommand;
            SafeQuery = safeQuery;
            PersonsCommand = personsCommand;
            BankQuery = bankQuery;
            SalesManQuery = salesManQuery;
            _financialAccount = FinancialAccount;
            _subCodeLevelsQuery = SubCodeLevelsQuery;
            _subCodeLevelsCommand = SubCodeLevelsCommand;
            OtherAuthoritiesQuery = otherAuthoritiesQuery;
            OtherAuthoritiesCommand = otherAuthoritiesCommand;
            InvoiceCommand = invoiceCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            InvoiceQuery = invoiceQuery;
        }



        //public async Task<ResponseResult> setGLsettingforAllAuthorites(AllAuthoritiesParameter parameter)
        //{


        //    var Data = await GeneralSettingRepositoryQuery.GetByAsync(a => a.Id == 1);
        //    List<InvPersons> lstUpdatedCust = new List<InvPersons>();
        //    List<InvPersons> lstUpdatedSupplier = new List<InvPersons>();
        //    List<GLBank> lstUpdatedBank = new List<GLBank>();
        //    List<GLSafe> lstUpdatedSafe = new List<GLSafe>();
        //    List<GLOtherAuthorities> lstUpdatedOtherAuthorities = new List<GLOtherAuthorities>();
        //    List<InvSalesMan> lstUpdatedSalesMan = new List<InvSalesMan>();

        //    //customer
        //    #region customer
        //    if(parameter.CustomersAcc!=null)
        //    if (parameter.CustomersAcc.useDefultAcc)
        //    {
        //        Data.useThisAcountCustomer = parameter.CustomersAcc.useThisAcount;
        //        Data.useUnderThisAcountCustomer = parameter.CustomersAcc.useUnderThisAcount;
        //        Data.FinancialAccountIdCustomer = parameter.CustomersAcc.financialAccId;
        //    }
        //    else
        //    {
        //        //var lstUpdatedCust=new List<InvPersons>();    
        //        var oldData = await PersonsQuery.GetAllAsyn(h => h.IsCustomer == true);
        //        foreach (var addCust in oldData)
        //        {
        //            int fID = parameter.CustomersAcc.lstCustomers.Where(h => h.Id == addCust.Id).Select(h => h.FinancialAccountId).FirstOrDefault(-1);
        //            if (addCust.FinancialAccountId != fID && fID != -1)
        //            {
        //                addCust.FinancialAccountId = fID;
        //                lstUpdatedCust.Add(addCust);
        //            }


        //        }


        //    }
        //    #endregion
        //    //supplier
        //    #region supplier
        //    if (parameter.SuppliersAcc != null)
        //        if (parameter.SuppliersAcc.useDefultAcc)
        //    {
        //        Data.useThisAcountSupplier = parameter.SuppliersAcc.useThisAcount;
        //        Data.useUnderThisAcountSupplier = parameter.SuppliersAcc.useUnderThisAcount;
        //        Data.FinancialAccountIdSupplier = parameter.SuppliersAcc.financialAccId;
        //    }
        //    else
        //    {
        //        //var lstUpdatedCust=new List<InvPersons>();    
        //        var oldData = await PersonsQuery.GetAllAsyn(h => h.IsSupplier == true);

        //        foreach (var item in oldData)
        //        {
        //            int fID = parameter.SuppliersAcc.lstSuppliers.Where(h => h.Id == item.Id).Select(h => h.FinancialAccountId).FirstOrDefault(-1);
        //            if (item.FinancialAccountId != fID && fID != -1)
        //            {
        //                item.FinancialAccountId = fID;
        //                lstUpdatedSupplier.Add(item);
        //            }


        //        }

        //    }
        //    #endregion
        //    //bank
        //    #region bank
        //    if (parameter.BanksAcc != null)
        //        if (parameter.BanksAcc.useDefultAcc)
        //    {
        //        Data.useThisAcountBank = parameter.BanksAcc.useThisAcount;
        //        Data.useUnderThisAcountBank = parameter.BanksAcc.useUnderThisAcount;
        //        Data.FinancialAccountIdBank = parameter.BanksAcc.financialAccId;
        //    }
        //    else
        //    {

        //        var oldData = await BankQuery.GetAllAsyn();


        //        foreach (var item in oldData)
        //        {
        //            int fID = parameter.BanksAcc.lstBanks.Where(h => h.Id == item.Id).Select(h => h.FinancialAccountId).FirstOrDefault(-1);
        //            if (item.FinancialAccountId != fID && fID != -1)
        //            {
        //                item.FinancialAccountId = fID;
        //                lstUpdatedBank.Add(item);
        //            }
        //        }

        //    }
        //    #endregion
        //    //safe
        //    #region safe
        //    if (parameter.SafesAcc != null)
        //        if (parameter.SafesAcc.useDefultAcc)
        //    {
        //        Data.useThisAcountSafe = parameter.SafesAcc.useThisAcount;
        //        Data.useUnderThisAcountSafe = parameter.SafesAcc.useUnderThisAcount;
        //        Data.FinancialAccountIdSafe = parameter.SafesAcc.financialAccId;
        //    }
        //    else
        //    {
        //        var oldData = await SafeQuery.GetAllAsyn();


        //        foreach (var item in oldData)
        //        {
        //            int fID = parameter.SafesAcc.lstSafes.Where(h => h.Id == item.Id).Select(h => h.FinancialAccountId).FirstOrDefault(-1);
        //            if (item.FinancialAccountId != fID && fID != -1)
        //            {
        //                item.FinancialAccountId = fID;
        //                lstUpdatedSafe.Add(item);
        //            }
        //        }

        //    }
        //    #endregion
        //    //otherauthorities
        //    #region otherauthorities
        //    if (parameter.OtherAuthoritiesAcc != null)
        //        if (parameter.OtherAuthoritiesAcc.useDefultAcc)
        //    {
        //        Data.useThisAcountOtherAuthorities = parameter.OtherAuthoritiesAcc.useThisAcount;
        //        Data.useUnderThisAcountOtherAuthorities = parameter.OtherAuthoritiesAcc.useUnderThisAcount;
        //        Data.FinancialAccountIdOtherAuthorities = parameter.OtherAuthoritiesAcc.financialAccId;
        //    }
        //    else
        //    {
        //        var oldData = await OtherAuthoritiesQuery.GetAllAsyn();

        //        foreach (var item in oldData)
        //        {
        //            int fID = parameter.OtherAuthoritiesAcc.lstOtherAuthorities.Where(h => h.Id == item.Id).Select(h => h.FinancialAccountId).FirstOrDefault(-1);
        //            if (item.FinancialAccountId != fID && fID != -1)
        //            {
        //                item.FinancialAccountId = fID;
        //                lstUpdatedOtherAuthorities.Add(item);
        //            }
        //        }

        //    }
        //    #endregion
        //    //salesMan
        //    #region salesMan
        //    if (parameter.SalesManAcc != null)
        //        if (parameter.SalesManAcc.useDefultAcc)
        //    {
        //        Data.useThisAcountSalesMan = parameter.SalesManAcc.useThisAcount;
        //        Data.useUnderThisAcountSalesMan = parameter.SalesManAcc.useUnderThisAcount;
        //        Data.FinancialAccountIdSalesMan = parameter.SalesManAcc.financialAccId;
        //    }
        //    else
        //    {
        //        var oldData = await SalesManQuery.GetAllAsyn();

        //        foreach (var item in oldData)
        //        {
        //            int fID = parameter.SalesManAcc.lstSalesMan.Where(h => h.Id == item.Id).Select(h => h.FinancialAccountId).FirstOrDefault(-1);
        //            if (item.FinancialAccountId != fID && fID != -1)
        //            {
        //                item.FinancialAccountId = fID;
        //                lstUpdatedSalesMan.Add(item);
        //            }
        //        }
        //    }
        //    #endregion

        //    // Update customer if have value
        //    if (lstUpdatedCust.Count >0)           
        //       await PersonsCommand.UpdateAsyn(lstUpdatedCust);

        //    // Update supplier if have value
        //    if (lstUpdatedCust.Count > 0)
        //        await PersonsCommand.UpdateAsyn(lstUpdatedSupplier);
        //    // Update banks if have value
        //    if (lstUpdatedBank.Count > 0)
        //        await BankCommand.UpdateAsyn(lstUpdatedBank);
        //    // Update safes if have value

        //    if (lstUpdatedSafe.Count > 0)
        //        await SafeCommand.UpdateAsyn(lstUpdatedSafe);
        //    // Update safes if have value

        //    if (lstUpdatedSalesMan.Count > 0)
        //        await SalesManCommand.UpdateAsyn(lstUpdatedSalesMan);
        //    // Update safes if have value

        //    if (lstUpdatedOtherAuthorities.Count > 0)
        //        await OtherAuthoritiesCommand.UpdateAsyn(lstUpdatedOtherAuthorities);

        //    var saved = await GeneralSettingRepositoryCommand.UpdateAsyn(Data);

        //    return new ResponseResult() { Data = null, Result = Result.Success };
        //}

        public async Task<IRepositoryActionResult> UpdateGeneralSettings(UpdateGeneralSettingsParameter parameter)
        {

            parameter.Id = 1;
            if (parameter.codingLevels.Length < 4)
                return repositoryActionResult.GetRepositoryActionResult(
                                                                    RepositoryActionStatus.Error,
                                                                    message: "Coding levels Cannot be less than 4s",
                                                                    messageAr: ErrorMessagesAr.CodeLevelCount,
                                                                    messageEn: ErrorMessagesEn.CodeLevelCount);
            if (parameter.evaluationMethodOfEndOfPeriodStockType != 1 && parameter.evaluationMethodOfEndOfPeriodStockType != 2 && parameter.evaluationMethodOfEndOfPeriodStockType != 3)
                return repositoryActionResult.GetRepositoryActionResult(
                                                                    RepositoryActionStatus.Error,
                                                                    message: "Coding levels Cannot be less than 1 or more than 3");
            var data = await GeneralSettingRepositoryQuery.GetByAsync(a => a.Id == 1);
            if (data == null)
                GeneralSettingRepositoryCommand.Add(data);
           

            GLGeneralSetting table = Mapping.Mapper.Map<UpdateGeneralSettingsParameter, GLGeneralSetting>(parameter, data);
            //update safe-bank-... with financialaccount
           // GLGeneralSetting table = await setGLsetting(parameter, tabledata);
            table.AutomaticCoding = parameter.isAutoCoding;

            var saved = await GeneralSettingRepositoryCommand.UpdateAsyn(table);
            if(saved)
            {
                var subLevels = _subCodeLevelsQuery.GetAll();
                _subCodeLevelsCommand.RemoveRange(subLevels);
                await _subCodeLevelsCommand.SaveAsync();
                _subCodeLevelsCommand.ClearTracking();
                var listOfSubLevels = new List<SubCodeLevels>();
                foreach (var item in parameter.codingLevels)
                {
                    if (item < 1)
                    {
                        return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Error, message: "value can not be less than 1",
                                                                                messageAr: ErrorMessagesAr.CodeLevelValue,
                                                                                messageEn: ErrorMessagesEn.CodeLevelValue);
                    }
                    SubCodeLevels subCodeLevels = new SubCodeLevels();
                    subCodeLevels.value = item;
                    subCodeLevels.GLGeneralSettingId = 1;
                    listOfSubLevels.Add(subCodeLevels);
                }
                _subCodeLevelsCommand.AddRange(listOfSubLevels);
                await _subCodeLevelsCommand.SaveAsync();

            }

            if (parameter.isAutoCoding)
            {
                //BackgroundJob.Enqueue(() => _FinancialAccountBusiness.RecodingFinancialAccount().Wait());
                await _FinancialAccountBusiness.RecodingFinancialAccount();
            }
            if (!saved)
                return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Error, message: "Error", messageAr: ErrorMessagesAr.ErrorSaving, messageEn: ErrorMessagesEn.ErrorSaving);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editGeneralLedgerSettings);
            return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Updated, message: "Updated successfully", messageAr: ErrorMessagesAr.Updatedsuccessfully, messageEn: ErrorMessagesEn.Updatedsuccessfully);

        }


        public async Task<IRepositoryActionResult> GetGLGeneralSettings()
        {
            try
            {
                var Data = GeneralSettingRepositoryQuery.GetAll()
                    .Select(x => new
                    {
                        x.isFundsClosed,
                        isAutoCoding = x.AutomaticCoding,
                        x.evaluationMethodOfEndOfPeriodStockType,
                        x.AddDiscountInJournalEntryForInvoics,
                        codingLevels = x.subCodeLevels.Select(x => x.value).ToArray()
                    })
                    .FirstOrDefault();

                return repositoryActionResult.GetRepositoryActionResult(Data, RepositoryActionStatus.Ok, message: "Ok");

            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Error, message: " Error");
            }

        }

        #region Get pagini
        public async Task<IRepositoryActionResult> GetGeneralSettings(PageParameter paramters)
        {
            try
            {
                var Data = GeneralSettingRepositoryQuery.GetAll().ToList();
                if (Data != null)
                {
                    var result = pagedListGeneralSetting.GetGenericPagination(Data, paramters.PageNumber, paramters.PageSize, Mapper);
                    return repositoryActionResult.GetRepositoryActionResult(result, RepositoryActionStatus.Ok, message: "Ok");
                }
                else
                {
                    return repositoryActionResult.GetRepositoryActionResult(null, RepositoryActionStatus.NotFound, message: "There is no data");
                }
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Error, message: " Error");
            }

        }
        #endregion
        #region add genral settings
        public async Task<IRepositoryActionResult> AddGeneralSettings(GeneralSettingsParameter parameter)
        {
            try
            {
                //var dataExist = GeneralSettingRepositoryQuery.GetFirstOrDefault(a => a.Id == 1);
                //if (dataExist != null)
                //    return repositoryActionResult.GetRepositoryActionResult(null , RepositoryActionStatus.ExistedBefore , message: " Existed before ");

                var data = Mapping.Mapper.Map<GeneralSettingsParameter, GLGeneralSetting>(parameter);
                GeneralSettingRepositoryCommand.Add(data);
                return repositoryActionResult.GetRepositoryActionResult(data.Id, RepositoryActionStatus.Created, message: "Saved successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Error, message: "Not saved");
            }

        }
        #endregion



        public async Task<ResponseResult> UpdatePurchaseGeneralSettings(GLsettingInvoicesParameter parameter,int MainType)
        {






            var lstupdatePurchaseSetting = new List<GLPurchasesAndSalesSettings>();
            if (parameter.invoicesSettings.Count <= 0)
                return new ResponseResult() { Data = null, Result = Result.NoDataFound };

            var allAccounts = await _financialAccount.GetAllAsyn();
            if (allAccounts.Where(x => parameter.invoicesSettings.Select(d => d.financialAccountId).Contains(x.Id) && x.IsMain == true).Any())
            {
                return new ResponseResult() { Note = "Some Financial Accounts are main", Result = Result.Failed };
            }

            var listOfInvoicesTypes = listOfInvoicesNames.listOfNames().Where(x=> x.MainType == MainType);
            var wrongFiniancalAccount = new List<GLPurchasesAndSalesSettings>();
            var checkMainTypeId = listOfInvoicesTypes.Where(x => parameter.invoicesSettings.Select(d => d.receiptType).Contains(x.invoiceTypeId)).Where(x => x.MainType != MainType);
            if(checkMainTypeId.Any())
                return new ResponseResult() { Note = Actions.SaveFailed, Result = Result.Failed };

            if(allAccounts.Where(d=> parameter.invoicesSettings.Select(c=> c.financialAccountId).Contains(d.Id)).FirstOrDefault().IsMain == true)
            {
                return new ResponseResult() { Note =Actions.SaveFailed,Result =Result.Failed};
                    
            }
            if(!listOfInvoicesTypes.Where(d=> parameter.invoicesSettings.Select(c=> c.receiptType).Contains(d.invoiceTypeId)).Any())
            {
                return new ResponseResult() { Note = Actions.SaveFailed, Result = Result.Failed };
                    
            }
            var userInfo = await _iUserInformation.GetUserInformation();
            var itemForDelte = await InvoiceQuery.GetAllAsyn(x => parameter.invoicesSettings.Select(c=> c.receiptType).Contains(x.RecieptsType) && x.branchId == userInfo.CurrentbranchId && parameter.invoicesSettings.Select(c=> c.ReceiptElemntID).Contains(x.ReceiptElemntID));

            if (parameter.invoicesSettings.Where(x => x.receiptType == (int)DocumentType.SafeFunds || x.receiptType == (int)DocumentType.BankFunds || x.receiptType == (int)DocumentType.itemsFund || x.receiptType == (int)DocumentType.CustomerFunds || x.receiptType == (int)DocumentType.SuplierFunds).Any())
            {
                var SafesAndBanksfund = _invFundsBanksSafesMasterQuery.TableNoTracking;
                var CustomersAndSuppliersFund = _invFundsCustomerSupplierQuery.TableNoTracking.Include(x=> x.Person).Where(x=> x.Credit > 0 || x.Debit > 0).Where(c=> c.branchId == userInfo.CurrentbranchId);
                var invoices = _invoiceMasterQuery.TableNoTracking.Where(x => x.InvoiceTypeId == (int)DocumentType.itemsFund);


                parameter.invoicesSettings.ForEach(x =>
                {
                    if (SafesAndBanksfund.Any())
                    {
                        if (x.receiptType == (int)DocumentType.SafeFunds && parameter.invoicesSettings.Where(c=> c.receiptType == (int)DocumentType.SafeFunds).Any())
                            x.financialAccountId = itemForDelte.Where(c => c.RecieptsType == (int)DocumentType.SafeFunds).FirstOrDefault().FinancialAccountId;
                        if(x.receiptType == 29 && parameter.invoicesSettings.Where(c => c.receiptType == (int)DocumentType.BankFunds /*29*/ ).Any())
                            x.financialAccountId = itemForDelte.Where(c => c.RecieptsType == (int)DocumentType.BankFunds /*29*/).FirstOrDefault().FinancialAccountId;
                    }
                    if (x.receiptType == (int)DocumentType.CustomerFunds /*32*/ && CustomersAndSuppliersFund.Where(x => x.Person.First().IsCustomer).Any() && parameter.invoicesSettings.Where(c => c.receiptType == (int)DocumentType.CustomerFunds/*32*/).Any())
                        x.financialAccountId = itemForDelte.Where(c => c.RecieptsType == (int)DocumentType.CustomerFunds /*32*/).FirstOrDefault().FinancialAccountId;

                    if (x.receiptType == (int)DocumentType.SuplierFunds /*33*/ && CustomersAndSuppliersFund.Where(x => x.Person.First().IsSupplier).Any() && parameter.invoicesSettings.Where(c => c.receiptType == (int)DocumentType.SuplierFunds /*33*/).Any())
                        x.financialAccountId = itemForDelte.Where(c => c.RecieptsType == (int)DocumentType.SuplierFunds /*33*/).FirstOrDefault().FinancialAccountId;

                    if(invoices.Any() && parameter.invoicesSettings.Where(c => c.receiptType == (int)DocumentType.itemsFund /*22*/ && c.ReceiptElemntID == (int)DebitoAndCredito.debit).Any())
                        x.financialAccountId = itemForDelte.Where(c => c.RecieptsType == (int)DocumentType.itemsFund /*22*/&& c.ReceiptElemntID == (int)DebitoAndCredito.debit).FirstOrDefault().FinancialAccountId;
                    if (invoices.Any() && parameter.invoicesSettings.Where(c => c.receiptType == (int)DocumentType.itemsFund /*22*/ && c.ReceiptElemntID == (int)DebitoAndCredito.creditor).Any())
                        x.financialAccountId = itemForDelte.Where(c => c.RecieptsType == (int)DocumentType.itemsFund /*22*/&& c.ReceiptElemntID == (int)DebitoAndCredito.creditor).FirstOrDefault().FinancialAccountId;
                });
            }
            itemForDelte.ToList().ForEach(x =>
            {
                var elem = parameter.invoicesSettings.Where(c => c.receiptType == x.RecieptsType && c.ReceiptElemntID == x.ReceiptElemntID);
                if(elem.Any())
                    x.FinancialAccountId = parameter.invoicesSettings.Where(c => c.receiptType == x.RecieptsType && c.ReceiptElemntID == x.ReceiptElemntID).LastOrDefault().financialAccountId;
            });

            bool isSaved = false;
            isSaved =  await InvoiceCommand.UpdateAsyn(itemForDelte);
            if(isSaved)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editGeneralLedgerSettings);
            return new ResponseResult() { Note = isSaved ?  Actions.SavedSuccessfully : Actions.SaveFailed,  Result = isSaved ?  Result.Success : Result.Failed};
        }

        public async Task<ResponseResult> GetPurchaseAndSalesData(int MainType)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var allF_A = _financialAccount.TableNoTracking;
            var _data =  InvoiceQuery.TableNoTracking.Where(h => h.MainType == MainType);
            var data = _data.Where(h => h.branchId == userInfo.CurrentbranchId);

            var DefultBranchSettingsC_S_Funds = _data.Where(h => h.branchId == userInfo.CurrentbranchId && (h.RecieptsType == (int)DocumentType.CustomerFunds || h.RecieptsType == (int)DocumentType.SuplierFunds));
            var SafesAndBanksfund = _invFundsBanksSafesMasterQuery.TableNoTracking.Where(c=> c.BranchId == userInfo.CurrentbranchId);
            var CustomersAndSuppliersFund = _invFundsCustomerSupplierQuery.TableNoTracking.Include(x => x.Person).Where(c=> c.branchId == userInfo.CurrentbranchId).Sum(c=> c.Credit + c.Debit);
            var invoices = _invoiceMasterQuery.TableNoTracking.Where(c=> c.BranchId == userInfo.CurrentbranchId).Where(x => x.InvoiceTypeId == 22);



            var response = data.Select(x => new
            {
                receiptType = x.RecieptsType,
                x.ReceiptElemntID,
                financialAccount = allF_A.Where(d => d.Id == x.FinancialAccountId).Select(c => new
                {
                    Id         = c.Id,
                    ArabicName = allF_A.Where(d => d.Id == x.FinancialAccountId).FirstOrDefault().ArabicName,
                    LatinName  = allF_A.Where(d => d.Id == x.FinancialAccountId).FirstOrDefault().LatinName,

                    isClosed   = (x.RecieptsType == (int)DocumentType.SafeFunds && SafesAndBanksfund.Where(t => t.IsBank).Any()) ||
                                 (x.RecieptsType == (int)DocumentType.BankFunds && SafesAndBanksfund.Where(t => t.IsSafe).Any()) ||
                                 (x.RecieptsType == (int)DocumentType.CustomerFunds && CustomersAndSuppliersFund > 0 ? true : false ) ||
                                 (x.RecieptsType == (int)DocumentType.SuplierFunds && CustomersAndSuppliersFund > 0 ? true : false) ||
                                 (x.RecieptsType == (int)DocumentType.itemsFund && invoices.Any())
                                 ?  true : false

                }).FirstOrDefault()
            }).ToList();


            return new ResponseResult() { Data = response, Result = Result.Success ,DataCount=data.Count() };
        }



        public async Task<ResponseResult> MainDataIntegration(updateFinancialAccountRelationSettings parameter, SubFormsIds subFormsIds)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var row = _gLIntegrationSettingsQuery.TableNoTracking.Where(x=> x.GLBranchId == userInfo.CurrentbranchId && x.screenId == (int)subFormsIds).FirstOrDefault();
            row.linkingMethodId = parameter.linkingMethodId;
            if(parameter.financialAccountId!=0)
                row.GLFinancialAccountId = parameter.financialAccountId;
            var saved = await _gLIntegrationSettingsCommand.UpdateAsyn(row);
            if (saved)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editGeneralLedgerSettings);
            return new ResponseResult() { Note =saved ? Actions.UpdatedSuccessfully : Actions.UpdateFailed, Result = saved ?  Result.Success : Result.Failed};


        }
        
        /// <summary>
        /// Forms:
        ///      - customer Settings = 1
        ///      - Suppliers Settings =2
        ///      - safes Settings = 3
        ///      - banks Settings = 4
        ///      - salesman Settings = 5
        ///      - Other Authorities Settings = 6
        ///      - employees Settings = 7
        /// </summary>
        public async Task<ResponseResult> getFinancialAccountRelationSettings(getFinancialAccountRelationRequest parameter)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var accounts = _financialAccount.TableNoTracking.Select(x=> new {x.Id,x.ArabicName,x.LatinName});
            var element = await GeneralSettingRepositoryQuery.GetByIdAsync(1);
            if (element.Id <= 0)
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };



            var settings = _gLIntegrationSettingsQuery.TableNoTracking.Where(x => x.GLBranchId == userInfo.CurrentbranchId);

            getFinancialAccountRelationResponse response;
            if (parameter.entryScreenSettings == 1)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x=> x.screenId == (int)SubFormsIds.Customers_Sales).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x=> x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.Customers_Sales).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else if (parameter.entryScreenSettings == 2)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x => x.screenId == (int)SubFormsIds.Suppliers_Purchases).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x => x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.Suppliers_Purchases).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else if (parameter.entryScreenSettings == 3)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x => x.screenId == (int)SubFormsIds.Safes_MainData).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x => x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.Safes_MainData).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else if (parameter.entryScreenSettings == 4)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x => x.screenId == (int)SubFormsIds.Banks_MainData).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x => x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.Banks_MainData).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else if (parameter.entryScreenSettings == 5)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x => x.screenId == (int)SubFormsIds.Salesmen_Sales).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x => x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.Salesmen_Sales).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else if (parameter.entryScreenSettings == 6)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x => x.screenId == (int)SubFormsIds.OtherAuthorities_MainData).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x => x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.OtherAuthorities_MainData).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else if (parameter.entryScreenSettings == 7)
                response = new getFinancialAccountRelationResponse()
                {
                    linkingMethodId = settings.Where(x => x.screenId == (int)SubFormsIds.Employees_MainUnits).FirstOrDefault().linkingMethodId,
                    financialAccount = accounts.Where(x => x.Id == settings.Where(x => x.screenId == (int)SubFormsIds.Employees_MainUnits).FirstOrDefault().GLFinancialAccountId).FirstOrDefault()
                };
            else
                return new ResponseResult()
                {
                    Note = "Form Id is Wrong",
                    Result = Result.Failed
                };


            #region old
            //getFinancialAccountRelationResponse response;
            //if (parameter.entryScreenSettings == 1)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccCustomer,
            //        financialAccount =element.DefultAccCustomer == 1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdCustomer).FirstOrDefault()
            //    };
            //else if (parameter.entryScreenSettings == 2)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccSupplier,
            //        financialAccount = element.DefultAccSupplier ==1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdSupplier).FirstOrDefault()
            //    };
            //else if (parameter.entryScreenSettings == 3)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccSafe,
            //        financialAccount = element.DefultAccSafe == 1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdSafe).FirstOrDefault()
            //    };
            //else if (parameter.entryScreenSettings == 4)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccBank,
            //        financialAccount = element.DefultAccBank == 1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdBank).FirstOrDefault()
            //    };
            //else if (parameter.entryScreenSettings == 5)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccSalesMan,
            //        financialAccount = element.DefultAccSalesMan == 1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdSalesMan).FirstOrDefault()
            //    };
            //else if (parameter.entryScreenSettings == 6)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccOtherAuthorities,
            //        financialAccount = element.DefultAccOtherAuthorities == 1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdOtherAuthorities).FirstOrDefault()
            //    };
            //else if (parameter.entryScreenSettings == 7)
            //    response = new getFinancialAccountRelationResponse()
            //    {
            //        linkingMethodId = element.DefultAccEmployee,
            //        financialAccount = element.DefultAccEmployee == 1 ? null : accounts.Where(d => d.Id == element.FinancialAccountIdEmployee).FirstOrDefault() 
            //    };
            //else
            //    return new ResponseResult()
            //    {
            //        Note = "Form Id is Wrong",
            //        Result = Result.Failed
            //    };
            #endregion
            return new ResponseResult()
            {
                Data = response,
                Note = Actions.Success,
                Result = Result.Success
            };

        }
    }




}
