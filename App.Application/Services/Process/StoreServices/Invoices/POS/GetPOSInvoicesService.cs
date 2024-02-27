using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application
{
    public class GetPOSInvoicesService : BaseClass, IGetPOSInvoicesService
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IGetAllInvoicesService GetAllInvoicesService;
        private SettingsOfInvoice SettingsOfInvoice;
        private IAddInvoice generalProcess;
        private iUserInformation Userinformation;
        private IGetInvoiceByIdService _GetInvoiceByIdService;
        private readonly IGetInvoiceForReturn GeneralProcessGetInvoiceForReturnService;
        private readonly IGetInvoiceByIdService GetServiceById;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;
        private IGeneralAPIsService generalAPIsService;

        public GetPOSInvoicesService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IGetInvoiceByIdService getInvoiceByIdService,
                               IGeneralAPIsService generalAPIsService, iUserInformation userinformation,
                               IGetAllInvoicesService _GetAllInvoicesService,
                              IGetInvoiceForReturn _GeneralProcessGetInvoiceForReturnService,
                              IRepositoryCommand<InvoiceMaster> _InvoiceMasterRepositoryCommand,
                              IGetInvoiceByIdService _GetServiceById,
                              IHttpContextAccessor _httpContext, IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsRepositoryQuery) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            this.generalProcess = generalProcess;
            Userinformation = userinformation;
            GetAllInvoicesService = _GetAllInvoicesService;
            _GetInvoiceByIdService = getInvoiceByIdService;
            GeneralProcessGetInvoiceForReturnService = _GeneralProcessGetInvoiceForReturnService;
            InvoiceMasterRepositoryCommand = _InvoiceMasterRepositoryCommand;
            GetServiceById = _GetServiceById;
            this.itemMasterQuery = itemMasterQuery;
            InvoiceDetailsRepositoryQuery = invoiceDetailsRepositoryQuery;
        }


        public async Task<ResponseResult>Navegation(int invoiceTypeId)
        {

          

            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            bool showOthoerInvoice = userInfo.otherSettings.posShowOtherPersonsInv;
            int invoiceTypeDel = (invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            int Code = InvoiceMasterRepositoryQuery.GetMaxCode(a=> a.Code,q =>
                q.BranchId == userInfo.CurrentbranchId &&(showOthoerInvoice?1==1: q.EmployeeId==userInfo.employeeId) 
                && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));



            //var Data = InvoiceMasterRepositoryQuery.TableNoTracking

            //    .Where(q =>
            //    q.BranchId == userInfo.CurrentbranchId
            //    && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel)).Select(h => h.InvoiceId);

           

            return new ResponseResult() { Data = Code, Result = Result.Success };

        }
        private bool showOthorInv(int invoiceTypeId, UserInformationModel userInfo)
        {
            if (invoiceTypeId == (int)DocumentType.POS)
                return userInfo.otherSettings.posShowOtherPersonsInv;
            if (invoiceTypeId == (int)DocumentType.Sales)
                return userInfo.otherSettings.salesShowOtherPersonsInv;

            if (invoiceTypeId == (int)DocumentType.Purchase)
                return userInfo.otherSettings.purchasesShowOtherPersonsInv;

            return false;
        }
        public async Task<ResponseResult> POSNavigationStep(int invoiceTypeId, int invoiceId, int stepType, int branchId)
        {

            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            bool showOthoerInvoice = showOthorInv(invoiceTypeId, userInfo);
            int NavInvoiceId = 0;
            int invoiceTypeDel = (invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            if (stepType == (int)StepType.last || stepType == (int)StepType.back)
            {
                NavInvoiceId = InvoiceMasterRepositoryQuery.GetMaxCode(a => a.InvoiceId, q =>
                q.BranchId == branchId
                && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                && (stepType == (int)StepType.last ? 1 == 1 : q.InvoiceId < invoiceId)
                && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));
            }

            else if (stepType == (int)StepType.first || stepType == (int)StepType.next)
            {
                NavInvoiceId = InvoiceMasterRepositoryQuery.GetMinCode(a=> a.InvoiceId,q =>
                    q.BranchId == branchId
                    && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                    && (stepType == (int)StepType.first ? 1 == 1 : q.InvoiceId > invoiceId)

                && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));
            }

            if (NavInvoiceId == 0)
                return new ResponseResult() { Data = 0, Result = Result.Success, Note = "No Data Found" };

            var result= await _GetInvoiceByIdService.GetInvoiceById(NavInvoiceId, false);

            result.DataCount = InvoiceMasterRepositoryQuery.Count(q =>
                q.BranchId == branchId
                && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
                && (stepType == (int)StepType.last ? 1 == 1 : q.InvoiceId < invoiceId)
                && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel));

            return result;

           
        }

        // get invoice by index 
        public async Task<ResponseResult> POSNavigationStepIndex(int invoiceTypeId, int IndexId, int branchId)
        {
            
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
            bool showOthoerInvoice = showOthorInv(invoiceTypeId, userInfo);
            int NavInvoiceId = 0;
            int invoiceTypeDel = (invoiceTypeId == (int)DocumentType.Purchase ? (int)DocumentType.DeletePurchase : invoiceTypeId == (int)DocumentType.Sales ? (int)DocumentType.DeleteSales : invoiceTypeId == (int)DocumentType.POS ? (int)DocumentType.POS : 0);

            var NavInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(q =>
            q.BranchId == branchId
            && (showOthoerInvoice ? 1 == 1 : q.EmployeeId == userInfo.employeeId)
            && ((q.InvoiceTypeId == invoiceTypeId && q.IsDeleted == false) || q.InvoiceTypeId == invoiceTypeDel)).OrderBy(a => a.Code);
            if (NavInvoice == null)
                return new ResponseResult() { Data = 0, Result = Result.Success, Note = "No Data Found" };

            int count = NavInvoice.Count();
            var NavInvoicePage = NavInvoice.Skip((IndexId - 1) ).Take(1).ToList();
            if(NavInvoicePage !=null)
               NavInvoiceId= NavInvoicePage.Select(a=>a.InvoiceId).LastOrDefault();

            if (NavInvoiceId==0)
                return new ResponseResult() { Data = 0, Result = Result.Success, Note = "No Data Found" };
            var data = await _GetInvoiceByIdService.GetInvoiceById(NavInvoiceId, false  );
            data.TotalCount = count;
            return data;
        }



        public async Task<ResponseResult> GetAllPOSInvoices(POSReturnInvoiceSearchDTO request)
        {

            if (request.InvoiceTypeId == 0)
                return new ResponseResult() { 
                    Data = null, Result = Result.RequiredData, ErrorMessageAr ="يجب ادخال نوع الفاتوره",ErrorMessageEn = "InvoiceTypeId must be entered"
                };

                UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var treeData = InvoiceMasterRepositoryQuery.TableNoTracking.Include(x => x.store).Include(x => x.Person)
                .Where(q => q.BranchId == userInfo.CurrentbranchId);

            if(!userInfo.otherSettings.posShowOtherPersonsInv)
                treeData = treeData.Where(q => q.EmployeeId == userInfo.employeeId);


            if (request.InvoiceTypeId == (int)DocumentType.ReturnPOS)
                treeData = treeData.Where(q => ((q.InvoiceTypeId == (int)DocumentType.ReturnPOS)));
            else
            {
                treeData = treeData.Where(q => ((q.InvoiceTypeId == (int)DocumentType.POS && q.IsDeleted == false ||
                                                          q.InvoiceTypeId == (int)DocumentType.DeletePOS) &&
                                                          (request.IsReturn ? q.InvoiceSubTypesId != (int)SubType.TotalReturn : true)));

            }

            var totalCount = treeData.Count();

            if (request.SessionId > 0)
                treeData = treeData.Where(a => a.POSSessionId == request.SessionId);

            if (request.PersonId != null && request.PersonId != 0)
                treeData = treeData.Where(a => a.PersonId == request.PersonId);

            if (request.StoreId != null && request.StoreId != 0)
                treeData = treeData.Where(a => a.StoreId == request.StoreId);


            if (request.InvoiceCode != null && request.InvoiceCode != 0)
                    treeData = treeData.Where(q => q.Code == request.InvoiceCode );
            else
            {
                 if (!string.IsNullOrEmpty(request.invoiceType))
                 {
                    var inv = treeData.Where(q => q.InvoiceType == request.invoiceType || (request.InvoiceTypeId == (int)DocumentType.POS ? q.CodeOfflinePOS == request.invoiceType : false));
                    treeData = inv.Any() ? inv : treeData.Where(q => q.InvoiceType.Contains(request.invoiceType)).OrderBy(a => a.Code);
                 }
                 else
                    treeData = treeData.OrderByDescending(x => x.Code);
           
            }

          
            if (request.Searches != null)
            {
                var searchCretiera = request.Searches.SearchCriteria;

                if (!string.IsNullOrEmpty(searchCretiera))
                {
                    treeData = treeData.Where
                                         (x =>
                                                 x.Code.ToString().Contains(searchCretiera) ||
                                                 x.InvoiceType == searchCretiera ||
                                                 x.Person.ArabicName.ToLower().Contains(searchCretiera) ||
                                                 x.Person.LatinName.ToLower().Contains(searchCretiera) ||
                                                x.Person.Phone==searchCretiera ||
                                                 //   x.Person.Phone.Contains(searchCretiera) ||
                                                 x.BookIndex.Contains(searchCretiera)
                                         ).OrderBy(a => a.Code);
                }
                else
                    treeData = treeData.OrderByDescending(x => x.Code);


                if (request.Searches.PaymentType.Count() > 0)
                {
                    treeData = treeData.Where(q => request.Searches.PaymentType.Contains(q.PaymentType));
                }
                if (request.Searches.SubType.Any() && request.InvoiceTypeId == (int)DocumentType.ReturnPOS)
                {

                    treeData = treeData.Where(q => request.Searches.SubType.Contains(q.InvoiceSubTypesId));
                }
                if (request.Searches.StoreId.Count() > 0)
                {
                    treeData = treeData.Where(q => request.Searches.StoreId.Contains(q.StoreId));
                }
                if (request.Searches.PersonId.Count() > 0)
                {
                    treeData = treeData.Where(q => request.Searches.PersonId.Contains(q.PersonId));
                }
                if (request.Searches.InvoiceDateFrom != null)
                    treeData = treeData.Where(q => q.InvoiceDate.Date >= request.Searches.InvoiceDateFrom.Value.Date).OrderByDescending(a => a.InvoiceDate);
                if (request.Searches.InvoiceDateTo != null) 
                    treeData = treeData.Where(q => q.InvoiceDate.Date <= request.Searches.InvoiceDateTo.Value).OrderByDescending(a => a.InvoiceDate);
               
                if (request.Searches.itemId > 0)
                {

                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => a.ItemId == request.Searches.itemId)
                               .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));

                }
                if (request.Searches.categoryId > 0)
                {

                    var items = itemMasterQuery.TableNoTracking.Where(a => a.GroupId == request.Searches.categoryId)
                               .Select(a => a.Id);
                    var invoiceIds = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => items.Contains(a.ItemId))
                             .Select(a => a.InvoiceId);
                    treeData = treeData.Where(a => invoiceIds.Contains(a.InvoiceId));
                }

            }

            if (request.InvoiceDate != null)
                treeData = treeData.Where(q => q.InvoiceDate.Date == request.InvoiceDate.Value.Date).OrderByDescending(a => a.InvoiceDate);
            else
            {
                if (request.DateFrom != null)
                    treeData = treeData.Where(q => q.InvoiceDate.Date >= request.DateFrom.Value.Date).OrderByDescending(a => a.InvoiceDate);
                if (request.DateTo != null)
                    treeData = treeData.Where(q => q.InvoiceDate.Date <= request.DateTo.Value.Date).OrderByDescending(a => a.InvoiceDate);
            }

            var dataCount = treeData.Count();
            List<InvoiceMaster> response = new List<InvoiceMaster>();
            response = treeData.ToList();
            var list2 = new List<AllInvoiceDto>();

            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                response = treeData.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            }



            GetAllInvoicesService.GetAllInvoices(response, list2);
            List<InvoiceInfoDTO> res = new List<InvoiceInfoDTO>();
            foreach(var inv in list2)
            {
                res.Add(new InvoiceInfoDTO()
                {
                    InvoiceId = inv.InvoiceId,
                    InvoiceDate = inv.InvoiceDate,
                    InvoiceType = inv.InvoiceType,
                    TotalPrice = inv.TotalPrice,
                    Discount = inv.Discount,
                    InvoiceSubTypesId = inv.InvoiceSubTypesId,
                    PaymentType = inv.PaymentType,
                    PersonNameAr = inv.PersonNameAr,
                    PersonNameEn = inv.PersonNameEn,
                    InvoiceTypeId = inv.InvoiceTypeId,
                    paid= inv.paid
                });
            }
            return new ResponseResult() { Id = null, Data = res, DataCount = dataCount, Result = res.Count > 0 ? Result.Success : Result.NoDataFound, Note = "", TotalCount = totalCount };


        }

        public async Task<ResponseResult> GetPOSReturnInvoice(string InvoiceCode)
        {
            return await GetPOSReturnInvoice(  InvoiceCode, null);
        }
        public async Task<ResponseResult> GetPOSReturnInvoice(string InvoiceCode,   bool? ForIOS)
        {
            return await GeneralProcessGetInvoiceForReturnService.GetMainInvoiceForReturn(InvoiceCode, (int)DocumentType.POS , ForIOS);
        }

        public async Task<ResponseResult> GetPOSInvoiceById(int? InvoiceId, string? InvoiceCode,bool? ForIOS)
        {
            if(InvoiceCode == null && InvoiceId == null)
                return new ResponseResult()
                {
                    Data = null,
                    Id = null,
                    Result = Result.RequiredData,
                    ErrorMessageAr = "You must enter an InvoiceId or InvoiceCode",
                    ErrorMessageEn = "You must enter an InvoiceId or InvoiceCode"
                };

            int Id = 0;

            if (InvoiceId != null)
                Id = (int)InvoiceId;
            else
            {
                var treeData = await InvoiceMasterRepositoryCommand.GetByAsync(a => a.InvoiceType == InvoiceCode);
                if (treeData == null)
                    return new ResponseResult()
                    {
                        Data = null,
                        Id = null,
                        Result = Result.NoDataFound,
                        ErrorMessageAr = "No data found",
                        ErrorMessageEn = "No data found"
                    };
                Id = treeData.InvoiceId;
            }
            
            var result = await GetServiceById.GetInvoiceById(Id, false, false, ForIOS);
            return result;
        }
    }
}
