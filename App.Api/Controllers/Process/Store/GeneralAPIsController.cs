using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.GeneralAPIsHandler.AddPrintFiles;
using App.Application.Handlers.GeneralAPIsHandler.checkDeleteOfInvoice;
using App.Application.Handlers.GeneralAPIsHandler.GeneralLedgerHomeData;
using App.Application.Handlers.GeneralAPIsHandler.GetBranchesForAllUsers;
using App.Application.Handlers.GeneralAPIsHandler.GetDeletedRecors;
using App.Application.Handlers.GeneralAPIsHandler.GetInvoiceById;
using App.Application.Handlers.GeneralAPIsHandler.GetItemsDropDownForReports;
using App.Application.Handlers.GeneralAPIsHandler.GetOfflineVersion;
using App.Application.Handlers.GeneralAPIsHandler.getPersonEmail;
using App.Application.Handlers.GeneralAPIsHandler.GetStoresDropDownForReports;
using App.Application.Handlers.GeneralAPIsHandler.GetSystemHistoryLogs;
using App.Application.Handlers.GeneralAPIsHandler.HomeData;
using App.Application.Handlers.GeneralAPIsHandler.NavigationStep;
using App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod;
using App.Application.Handlers.GeneralAPIsHandler.PrintSystemHistoryLogs;
using App.Application.Handlers.GeneralAPIsHandler.SendingEmail;
using App.Application.Handlers.GeneralAPIsHandler.setPOSstartup;
using App.Application.Handlers.GeneralAPIsHandler.updatedSelectedBranch;
using App.Application.Handlers.GeneralAPIsHandler.UpdatePrintFiles;
using App.Application.Handlers.InvoicesHelper.calcQyt;
using App.Application.Handlers.InvoicesHelper.calculatePaymentMethods;
using App.Application.Handlers.InvoicesHelper.CheckInvoiceExistance;
using App.Application.Handlers.InvoicesHelper.CheckIsCustomer;
using App.Application.Handlers.InvoicesHelper.getBranches;
using App.Application.Handlers.InvoicesHelper.GetItemsDropDown;
using App.Application.Handlers.InvoicesHelper.GetItemsInPartsDropDown;
using App.Application.Handlers.InvoicesHelper.GetItemUnitsDropDown;
using App.Application.Handlers.InvoicesHelper.GetReceiptBalanceForBenifit;
using App.Application.Handlers.InvoicesHelper.MergeItems;
using App.Application.Handlers.InvoicesHelper.Serials;
using App.Application.Handlers.InvoicesHelper.SetQuantityForExpiaryDate;
using App.Application.Handlers.InvoicesHelper.UpdateCanDeleteInItemUnits;
using App.Application.Handlers.Setup.ItemCard.Query.FillItemCardQuery;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.General_APIs;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Request.Store.Invoices;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Api.Controllers.Process
{
    public class GeneralAPIsController : ApiStoreControllerBase
    {
  

        private readonly iAuthorizationService iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IprintFileService _printFileService;
        private readonly iUserInformation Userinformation;

        public GeneralAPIsController(iAuthorizationService iAuthorizationService, IActionResultResponseHandler ResponseHandler, IMediator mediator, IConfiguration configuration, IprintFileService printFileService, iUserInformation userinformation) : base(ResponseHandler)
        {
            this.iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _configuration = configuration;
            _printFileService = printFileService;
            Userinformation = userinformation;
        }

        [HttpPost(nameof(CalculateItemQuantity))]
        public async Task<QuantityInStoreAndInvoice> CalculateItemQuantity(itemsRequest items)
        {
            if (items.ParentInvoiceType == null)
                items.ParentInvoiceType = "";

            //var result = await generalAPIsService.CalculateItemQuantity(items.ItemId, items.UnitId, items.StoreId, items.ParentInvoiceType,
            //           items.ExpiryDate, items.IsExpiared, items.invoiceId, items.invoiceDate, items.invoiceTypeId, items.currentItems);
            var result = await _mediator.Send(new calcItemQuantityRequest
            {
                ItemId = items.ItemId,
                UnitId = items.UnitId,
                StoreId = items.StoreId,
                ParentInvoiceType = items.ParentInvoiceType,
                ExpiryDate = items.ExpiryDate,
                IsExpiared = items.IsExpiared,
                invoiceId = items.invoiceId,
                invoiceDate = items.invoiceDate,
                invoiceTypeId = items.invoiceTypeId,
                items = items.currentItems,
                currentQuantity= items.currentQuantity,
                
            });
            return result;
        }
        [HttpPost(nameof(CheckSerial))]
        public async Task<serialsReponse> CheckSerial(serialsRequest request)
        {
            //var result = await generalAPIsService.CheckSerials(request);
            var result = await _mediator.Send(new CheckSerialRequest
            {
                toNumber = request.toNumber,
                stratPattern = request.stratPattern,
                serialsInTheSameInvoice = request.serialsInTheSameInvoice,
                endPattern = request.endPattern,
                fromNumber = request.fromNumber,
                invoiceType = request.invoiceType,
                isDiffNumbers = request.isDiffNumbers,
                newEnteredSerials = request.newEnteredSerials,
                serial = request.serial,
                serialRemovedInEdit = request.serialRemovedInEdit,
            });
            return result;
        }
        [HttpGet("GetItemsDropDown")]
        public async Task<ResponseResult> GetItemsDropDown(string? SearchCriteria, bool? isSearchByCode, int itemType, int PageSize, int PageNumber, int? invoiceTypeId)
        {
            //DropDownRequest request = new DropDownRequest()
            //{
            //    SearchCriteria = SearchCriteria,
            //    PageNumber = PageNumber,
            //    PageSize = PageSize,
            //    isSearchByCode = isSearchByCode,
            //    itemType = itemType,
            //    invoiceTypeId = invoiceTypeId
            //};

            //var result = await generalAPIsService.GetItemsDropDown(request);
            var result = await _mediator.Send(new GetItemsDropDownRequest
            {
                invoiceTypeId = invoiceTypeId,
                isSearchByCode = isSearchByCode,
                itemType = itemType,
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria = SearchCriteria
            });
            return result;
        }
        [HttpPut(nameof(UpdateCanDeleteInItemUnits))]
        public async Task<ResponseResult> UpdateCanDeleteInItemUnits(int itemId, int? unitId, bool delete)
        {
            //var result = await generalAPIsService.UpdateCanDeleteInItemUnits(itemId, unitId, delete);
            var result = await _mediator.Send(new UpdateCanDeleteInItemUnitsRequest
            {
                unitId = unitId,
                delete = delete,
                itemId = itemId
            });
            return result;
        }
        [HttpGet("GetItemUnitsDropDown")]
        public async Task<ResponseResult> GetItemUnitsDropDown(int itemId, string? barcode)
        {
            //var result = await generalAPIsService.GetItemUnitsDropDown(itemId, barcode);
            var result = await _mediator.Send(new GetItemUnitsDropDownRequest
            {
                itemId = itemId,
                barcode = barcode
            });
            return result;
        }
        [HttpGet("GetItemsInPartsDropDown")]
        public async Task<ResponseResult> GetItemsInPartsDropDown()
        {
            //var result = await generalAPIsService.GetItemsInPartsDropDown();
            var result = await _mediator.Send(new GetItemsInPartsDropDownRequest());
            return result;
        }
        [HttpPost(nameof(calculatePaymentMethods))]
        public async Task<ResponseResult> calculatePaymentMethods(calcPaymentMethodsRequest request)
        {
            //var result = await generalAPIsService.calculatePaymentMethods(request);
            var result = await _mediator.Send(new calculatePaymentMethodsRequest
            {
                net = request.net,
                values = request.values
            });
            return result;
        }
        [HttpPost(nameof(MergeItems))]
        public async Task<ResponseResult> MergeItems(List<InvoiceDetailsRequest> list, int invoiceType)
        {
            //var result = await generalAPIsService.mergeTotalItems(list, invoiceType);
            var result = await _mediator.Send(new MergeItemsRequest { list = list, invoiceType = invoiceType });
            return result;
        }
        [HttpPost(nameof(FillItemCardQuery))]
        public async Task<ResponseResult> FillItemCardQuery([FromBody] FillItemCardQueryRequest parm)
        {

            //var result = await generalAPIsService.FillItemCardQuery(request);
            var result = await _mediator.Send(parm);
            return result;
        }
        [HttpGet("CheckInvoiceExistance")]
        public async Task<ResponseResult> CheckInvoiceExistance(string invoiceType, int InvoiceTypeId)
        {

            //var result = await generalAPIsService.CheckInvoiceExistance(invoiceType, InvoiceTypeId);
            var result = await _mediator.Send(new CheckInvoiceExistanceRequest
            {
                invoiceType = invoiceType,
                InvoiceTypeId = InvoiceTypeId
            });
            return result;
        }
        [HttpPost(nameof(SetQuantityForExpiaryDate))]
        public async Task<ResponseResult> SetQuantityForExpiaryDate(setQuantityForExpiaryDateRequest request)
        {
            try
            {
                //var result = await generalAPIsService.SetQuantityForExpiaryDate(request);
                var result = await _mediator.Send(request);
                return result;
            }
            catch (Exception e)
            {
                return new ResponseResult { Data = e };
            }

        }
        [HttpGet("GetTotalAmountOfPerson/{personID}")]
        public async Task<ResponseResult> GetTotalAmountOfPerson(int personID)
        {

            var isCustomer = await _mediator.Send(new checkIsCustomerRequest { personId = personID });/*_personHelperService.checkIsCustomer(personID);*/
            //var totalAmount = await _receiptsService.GetReceiptBalanceForBenifit(isCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers, personID);
            var totalAmount = await _mediator.Send(new GetReceiptBalanceForBenifitRequest
            {
                AuthorityId = isCustomer ? AuthorityTypes.customers : AuthorityTypes.suppliers,
                BenefitID = personID
            });
            return new ResponseResult()
            {
                Data = totalAmount.Total
            };
        }
        /// <summary>
        /// stopped here 
        /// </summary>
        [Authorize]
        [HttpGet("getEmployeeBranchs")]
        public async Task<ResponseResult> getEmployeeBranchs()
        {
            //return await _branchsService.getBranches();
            return await _mediator.Send(new getBranchesRequest());
        }
        [HttpGet("getEmployeeBranchsForAllUsers")]
        public async Task<ResponseResult> getEmployeeBranchsForAllUsers(int pageNumber, int pageSize = 10)
        {
            GetBranchesForAllUsersRequest parameters = new GetBranchesForAllUsersRequest {
                PageNumber = pageNumber,
                PageSize = pageSize };
            return await _mediator.Send(parameters);
        }
        [HttpPost("updatedSelectedBranch/{branchId}")]
        public async Task<ResponseResult> updatedSelectedBranch(int branchId)
        {
            //return await _branchsService.updatedSelectedBranch(branchId);
            return await _mediator.Send(new updatedSelectedBranchRequest { branchId = branchId });
        }
        //[HttpGet(nameof(GetAllInvoiceIds))]
        //public async Task<ResponseResult> GetAllInvoiceIds(int invoiceTypeId)
        //{

        //    var isAuthorized = await iAuthorizationService.isAuthorized(mainFormCode: 7, 41, opretion: Opretion.Open);
        //    if (isAuthorized != null)
        //        return isAuthorized;
        //    var result = await GetInvoiceService.GetAllInvoiceIds(invoiceTypeId);
        //    return result;
        //}
        [HttpGet("setPOSstartup")]
        public async Task<ResponseResult> setPOSstartup(int invoiceTypeId)
        {
            ////var result = await GetInvoiceService.GetAllInvoiceIds(invoiceTypeId);
            ////return result;
            //ResponseResult result = await generalAPIsService.setPOSstartup(invoiceTypeId);
            ResponseResult result = await _mediator.Send(new setPOSstartupRequest { invoiceTypeId = invoiceTypeId });
            return result;
        }
        [HttpGet("NavigationStep")]
        public async Task<ResponseResult> NavigationStep(int invoiceTypeId, int invoiceID, bool nextCode)
        {
            //ResponseResult result = await generalAPIsService.NavigationStep(invoiceTypeId, invoiceID, nextCode);
            ResponseResult result = await _mediator.Send(new NavigationStepRequest { invoiceTypeId = invoiceTypeId, invoiceCode = invoiceID, NextCode = nextCode });
            return result;
        }
        [HttpGet("HomeData")]
        public async Task<ResponseResult> HomeData([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] int dashBoardType = 1)
        {
            //ResponseResult result = await generalAPIsService.HomeData(request);
            ResponseResult result = new ResponseResult();
            if(dashBoardType == 1)
                result = await _mediator.Send(new HomeDataRequest
                {
                    dateFrom = dateFrom,
                    dateTo = dateTo
                });
            else if(dashBoardType == 2)
                result = await _mediator.Send(new GeneralLedgerHomeDataRequest
                {
                    dateFrom = dateFrom,
                    dateTo = dateTo
                });
            return result;
        }
        [HttpGet("GetSystemHistoryLogs")]
        public async Task<ResponseResult> GetSystemHistoryLogs([FromQuery] GetSystemHistoryLogsRequest request)
        {
            //ResponseResult result = await generalAPIsService.GetSystemHistoryLogs(pageNumber, pageSize);
            ResponseResult result = await _mediator.Send(request);
            return result;
        }
        [HttpGet("getPersonEmail/{personId}")]
        public async Task<ResponseResult> getPersonEmail(int personId)
        {
            //ResponseResult result = await generalAPIsService.getPersonEmail(personId);
            ResponseResult result = await _mediator.Send(new getPersonEmailRequest { personId = personId});
            return result;
        }
        [HttpGet("checkDeleteOfInvoice")]
        public async Task<bool> checkDeleteOfInvoice(int InvoiceTypeId, bool IsAccredite, bool IsReturn, bool IsDeleted
                  , string InvoiceType, int StoreId, DateTime InvoiceDate, int InvoiceId, List<CalcQuantityRequest> items)
        {
            //bool result = await generalAPIsService.checkDeleteOfInvoice(InvoiceTypeId, IsAccredite, IsReturn, IsDeleted,InvoiceType, StoreId, InvoiceDate, InvoiceId);
            bool result = await _mediator.Send(new checkDeleteOfInvoiceRequest
            {
                InvoiceTypeId = InvoiceTypeId,
                IsAccredite = IsAccredite,
                InvoiceDate = InvoiceDate,
                InvoiceId = InvoiceId,
                InvoiceType = InvoiceType,
                IsDeleted = IsDeleted,
                IsReturn = IsReturn,
                items = items,
                StoreId = StoreId
            });
            return result;
        }
        [HttpGet("GetInvoiceById")]
        public async Task<ResponseResult> GetInvoiceById(int InvoiceId, bool isCopy, int invoiceTypeId,bool? ForIOS)
        {
            SubFormsIds subFormCode = new SubFormsIds();
            if (invoiceTypeId == (int)DocumentType.Purchase || invoiceTypeId == (int)DocumentType.DeletePurchase)
            {
                subFormCode = SubFormsIds.Purchases;
            }
            else if (invoiceTypeId == (int)DocumentType.AddPermission || invoiceTypeId == (int)DocumentType.DeleteAddPermission)
            {

                subFormCode = SubFormsIds.AddPermission_Repository;
            }
            else if (invoiceTypeId == (int)DocumentType.Sales || invoiceTypeId == (int)DocumentType.DeleteSales)
            {
                subFormCode = SubFormsIds.Sales;
            }
            else if (invoiceTypeId == (int)DocumentType.itemsFund || invoiceTypeId == (int)DocumentType.DeleteItemsFund)
            {
                subFormCode = SubFormsIds.items_Fund;
            }
            else if (invoiceTypeId == (int)DocumentType.ExtractPermission || invoiceTypeId == (int)DocumentType.DeleteExtractPermission)
            {
                subFormCode = SubFormsIds.pay_permission;
            }
            else if (invoiceTypeId == (int)DocumentType.POS || invoiceTypeId == (int)DocumentType.DeletePOS)
            {
                subFormCode = SubFormsIds.POS;
            }
            else if (invoiceTypeId == (int)DocumentType.IncomingTransfer)
            {
                subFormCode = SubFormsIds.IncomingTransfer;
            }
            else if (invoiceTypeId == (int)DocumentType.OutgoingTransfer || invoiceTypeId == (int)DocumentType.OutgoingTransfer)
            {
                subFormCode = SubFormsIds.OutgoingTransfer;
            }
            else if (invoiceTypeId == (int)DocumentType.ReturnPOS)
            {
                subFormCode = SubFormsIds.returnPOS;
            }
            else if (invoiceTypeId == (int)DocumentType.ReturnPurchase)
            {
                subFormCode = SubFormsIds.PurchasesReturn_Purchases;
            }
            else if (invoiceTypeId == (int)DocumentType.ReturnSales)
            {
                subFormCode = SubFormsIds.SalesReturn_Sales;
            }

            var isAuthorized = await iAuthorizationService.isAuthorized(0, (int)subFormCode, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

           
            var result = await _mediator.Send(new GetInvoiceByIdRequest { isCopy = isCopy, InvoiceId = InvoiceId ,ForIOS=ForIOS});
            return result;
        }
        [HttpPost("SendingEmail")]
        public async Task<ResponseResult> SendingEmail([FromForm] SendingEmailRequest parm)
        {

            //if (parm.isInvoice)
            //{
            //    var invoice = await GetInvoiceFile(new Domain.Models.Request.InvoiceDTO()
            //    {
            //        exportType = exportType.ExportToPdf,
            //        invoiceCode = parm.invoiceCode,
            //        invoiceId = parm.invoiceId.Value,
            //        isArabic = parm.isArabic,
            //        screenId = parm.screenId.Value,
            //    });
            //    var ms = new MemoryStream(invoice);
            //    await ms.FlushAsync();

            //    parm.invoice = ms.ToArray();
            //}
            //return await generalAPIsService.SendingEmail(parm);
            return await _mediator.Send(parm);
        }
        [AllowAnonymous]
        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string FileName)
        {
            var fullPath = System.IO.Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], FileName);
            Byte[] b = System.IO.File.ReadAllBytes(fullPath);
            var extension = System.IO.Path.GetExtension(fullPath);
            var applicationContant = string.Empty;
            switch (extension)
            {
                case ".docx":
                    applicationContant = " application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".xlsx":
                    applicationContant = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".svg":
                    applicationContant = "image/svg+xml";
                    break;
                case ".pdf":
                    applicationContant = "application/pdf";
                    break;
            }
  

            return File(b, applicationContant, $"{FileName}");
        }
        [HttpGet("GetItemsDropDownForReports")]
        public async Task<ResponseResult> GetItemsDropDownForReports([FromQuery] GetItemsDropDownForReportsRequest parm)
        {
            //return await generalAPIsService.GetItemsDropDownForReports(parm);
            return await _mediator.Send(parm);
        }
        [HttpGet("GetStoresDropDownForReports")]
        public async Task<ResponseResult> GetStoresDropDownForReports([FromQuery] GetStoresDropDownForReportsRequest parm)
        {
            //return await generalAPIsService.GetStoresDropDownForReports(parm);
            return await _mediator.Send(parm);
        }
        [HttpGet("AddPrintFiles")]
        public async Task<int> AddPrintFiles()
        {
            //return await generalAPIsService.AddPrintFiles();
            return await _mediator.Send(new AddPrintFilesRequest());
        }
        [HttpGet("UpdatePrintFiles")]
        public async Task<int> UpdatePrintFiles()
        {
            //return await generalAPIsService.UpdatePrintFiles();
            return await _mediator.Send(new UpdatePrintFilesRequest());
        }
        [HttpGet("GetDeletedRecords")]
        public async Task<ResponseResult> GetDeletedRecords(DateTime dateTime)
        {

            return await _mediator.Send(new GetDeletedRecordsRequest { date = dateTime });
        }

        [HttpGet("GetPersonBalance")]
        public async Task<double> GetPersonBalance(int SupplierId, int invoiceTypeId)
        {

            var result = await _mediator.Send(new GetPersonBalanceRequest { invoiceTypeId = invoiceTypeId, personId = SupplierId });
            return result;
        }

        [HttpGet("GetOfflineVersion")]
        
        public async Task<ResponseResult> GetOfflineVersion()
        {
            var result = await _mediator.Send(new GetOfflineVersionRequest());
            return result;
        }

        [HttpGet("PrintSystemHistoryLogs")]
        public async Task<IActionResult> PrintSystemHistoryLogs([FromQuery]PrintSystemHistoryLogsRequest request)
        {
            var isAuthorized = await iAuthorizationService.isAuthorized((int)MainFormsIds.Users, (int)SubFormsIds.userActions, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            //  return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
           //WebReport re = new WebReport();
           var  report =  await  _mediator.Send(request);
            return Ok(_printFileService.PrintFile(report, "UsersTransactions", request.exportType));

        }

        // front call it when select paymentmethodid =-1 -> PersonBalance
        [HttpGet("GetPersonBalanceForPaymentMethod")]
        public async Task<GetPersonBalanceForPaymentMethodResponse> GetPersonBalanceForPaymentMethod(int invoiceTypeId ,int personId, int? invoiceId)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var request = new GetPersonBalanceForPaymentMethodRequest() {invoiceId=invoiceId, 
                   invoiceTypeId = invoiceTypeId, personId = personId ,BranchId=userInfo.CurrentbranchId};
            var balance = await _mediator.Send(request);
            return balance;

        }
       
    }
}
