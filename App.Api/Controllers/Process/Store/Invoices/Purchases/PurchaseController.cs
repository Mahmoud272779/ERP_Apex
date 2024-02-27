using System.Linq;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Purchases.AddPurchases;
using App.Application.Handlers.Purchases.DeletePurchase;
using App.Application.Handlers.Purchases.GetAllPurchases;
using App.Application.Handlers.Purchases.GetEmailForSuppliers;
using App.Application.Handlers.Purchases.GetInvoiceForSuppliers;
using App.Application.Handlers.Purchases.GetPrintWithSave;
using App.Application.Handlers.Purchases.GetPurchaseById;
using App.Application.Handlers.Purchases.SendEmailForSuppliers;
using App.Application.Handlers.Purchases.UpdatePurchases;
using App.Application.Helpers;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Printing.PrintPermission;
using App.Application.Services.Process.Invoices.Purchase;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers.Process.Store.Invoices.Purchases
{
    public class PurchaseController : ApiStoreControllerBase
    {
        //private readonly IAddPurchaseService PurchaseService;
        //private readonly IGetInvoiceByIdService GetPurchaseServiceById;
        //private readonly IDeletePurchaseService DeletePurchaseService;
        //private readonly IUpdatePurchaseService UpdatePurchaseService;
        //private readonly IGetAllPurchasesService GetAllPurchaseService;
        //private readonly ISendEmailPurchases SendEmailPurchases;
        //private readonly IGetAllSalesServices _getAllSalesServices;
        //private readonly IRepositoryQuery<InvGeneralSettings> _settingService;
        //private readonly IPrintResponseService iPrintResponseService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
        //   private readonly IMultiInvoices MultiInvoices;

        private readonly Itestfiles testfilesService;
        public PurchaseController(
                                  iAuthorizationService iAuthorizationService,
                                  IActionResultResponseHandler ResponseHandler, IMediator mediator
, iUserInformation iUserInformation
, IPermissionsForPrint iPermmissionsForPrint
                                    //, IRepositoryQuery<InvGeneralSettings> settingService
                                    //IAddPurchaseService _PurchaseService,
                                    //IGetInvoiceByIdService _GetPurchaseServiceById,
                                    //IDeletePurchaseService _DeletePurchaseService,
                                    //IUpdatePurchaseService _UpdatePurchaseService,
                                    //IGetAllPurchasesService _GetAllPurchaseService,
                                    //ISendEmailPurchases _SendEmailPurchases,
                                    //IGetAllSalesServices getAllSalesServices,
                                    //IRepositoryQuery<InvGeneralSettings> settingService, IPrintResponseService iPrintResponseService,
                                    ) : base(ResponseHandler)
        {
            _iAuthorizationService = iAuthorizationService;
            //PurchaseService = _PurchaseService;
            //GetAllPurchaseService = _GetAllPurchaseService;
            //SendEmailPurchases = _SendEmailPurchases;
            //_getAllSalesServices = getAllSalesServices;
            //GetPurchaseServiceById = _GetPurchaseServiceById;
            //DeletePurchaseService = _DeletePurchaseService;
            //UpdatePurchaseService = _UpdatePurchaseService;
            //this.testfilesService = testfilesService;
            //this.settingService = settingService;
            //this.iPrintResponseService = iPrintResponseService;
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
            //_settingService = settingService;
        }
        [HttpPost(nameof(testFiles))]
        public async Task<bool> testFiles([FromForm] IFormFile[] AttachedFile)
        {
            var result = await testfilesService.testFiles(AttachedFile);

            return true;
        }
        [HttpPost(nameof(AddPurchase))]
        public async Task<IActionResult> AddPurchase([FromForm] AddPurchasesRequest parameter, [FromQuery]bool isArabic = true)
        {
            var isAuthorized = new ResponseResult();
            if (!parameter.IsPurchasesWithoutVat)
            {
                isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);
            }
            else
            {
                isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.PurchasesWithoutVat, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);
            }
            



            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {
                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }
            var InvPurchaseAdditionalCostsRelations_ = Request.Form["OtherAdditionList"];
            foreach (var item in InvPurchaseAdditionalCostsRelations_)
            {
                var resReport = JsonConvert.DeserializeObject<OtherAdditionList>(item);
                parameter.OtherAdditionList.Add(resReport);
            }
            var PaymentsMethods_ = Request.Form["PaymentsMethods"];
            foreach (var item in PaymentsMethods_)
            {
                var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                parameter.PaymentsMethods.Add(resReport);
            }

            var result = new ResponseResult();



            //result = await PurchaseService.AddPurchase(parameter);
            result = await _mediator.Send(parameter);

            //var print = _settingService.TableNoTracking.FirstOrDefault().Purchases_PrintWithSave;
            //var print = await _mediator.Send(new GetPrintWithSaveRequest());
            var print = result.isPrint;

            //var userInfo = _iUserInformation.GetUserInformation();

            if (print && result.Result == Result.Success)
            {
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Purchases, result.Code.ToString(), exportType.Print);
                try
                {
                    var permissions = await _iPermmissionsForPrint.GetPermisions(result.permissionListId/*userInfo.permissionListId*/, (int)SubFormsIds.Purchases);

                    if (permissions.IsPrint)
                    {
                        var printResponse = await _mediator.Send(new InvoicePrintRequest
                        {
                            invoiceId = result.Id.Value,
                            invoiceCode = result.Code.ToString(),
                            exportType = exportType.Print,
                            screenId = (int)SubFormsIds.Purchases,
                            isArabic = isArabic


                        });
                        printResponse.ResultForPrint = printResponse.Result;
                        printResponse.Result = result.Result;
                        return Ok(printResponse);
                    }
                    else
                    {
                        var reportsReponse = new ReportsReponse()
                        {
                            ResultForPrint = Result.UnAuthorized,
                            Result = result.Result

                        };
                        return Ok(reportsReponse);
                    }
                }
                catch (Exception ex)
                {
                    return Ok(ex.Message);
                }
            }

            return Ok(result);
        }
    
        [HttpPost(nameof(GetAllPurchase))]
        public async Task<ResponseResult> GetAllPurchase(GetAllPurchaseRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetAllPurchaseService.GetAllPurchase(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }

        [HttpPut(nameof(UpdatePurchase))]
        public async Task<IActionResult> UpdatePurchase([FromForm] updatePurchasesRequest parameter, [FromQuery] bool isArabic = true)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Edit);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            var InvoicesDetails_ = Request.Form["InvoiceDetails"];
            foreach (var item in InvoicesDetails_)
            {

                var resReport = JsonConvert.DeserializeObject<InvoiceDetailsRequest>(item);
                parameter.InvoiceDetails.Add(resReport);
            }

            var InvPurchaseAdditionalCostsRelations_ = Request.Form["OtherAdditionList"];
            foreach (var item in InvPurchaseAdditionalCostsRelations_)
            {

                var resReport = JsonConvert.DeserializeObject<OtherAdditionList>(item);
                parameter.OtherAdditionList.Add(resReport);
            }

            var PaymentsMethods_ = Request.Form["PaymentsMethods"];
            foreach (var item in PaymentsMethods_)
            {

                var resReport = JsonConvert.DeserializeObject<PaymentList>(item);
                parameter.PaymentsMethods.Add(resReport);
            }

            //var result = await UpdatePurchaseService.UpdatePurchase(parameter);
            var result = await _mediator.Send(parameter);

            var print = await _mediator.Send(new GetPrintWithSaveRequest());
            var userInfo = await _iUserInformation.GetUserInformation();

            if (print && result.Result == Result.Success)
            {
                
                //var printResponse = await iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.Purchases, result.Code.ToString(), exportType.Print);
                var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.Purchases);
                if (permissions.IsPrint)
                {
                    var printResponse = await _mediator.Send(new InvoicePrintRequest
                    {
                        invoiceId = result.Id.Value,
                        invoiceCode = result.Code.ToString(),
                        exportType = exportType.Print,
                        screenId = (int)SubFormsIds.Purchases,
                        isArabic = isArabic


                    });
                    printResponse.ResultForPrint = printResponse.Result;
                    printResponse.Result = result.Result;
                    return Ok(printResponse);
                }
                else
                {
                    var reportsReponse = new ReportsReponse()
                    {
                        ResultForPrint = Result.UnAuthorized,
                        Result = result.Result

                    };
                    return Ok(reportsReponse);
                }
            }
            return Ok(result);
        }

        [HttpDelete("DeletePurchase")]
        public async Task<ResponseResult> DeletePurchase([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;
            //SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            //{
            //    Ids = InvoiceIdsList
            //};
            //var result = await DeletePurchaseService.DeletePurchase(parameter);
            var result = await _mediator.Send(new DeletePurchaseRequest
            {
                Ids = InvoiceIdsList
            });
            return result;
        }

        [HttpGet("GetPurchaseById")]
        public async Task<ResponseResult> GetPurchaseById(int InvoiceId, bool isCopy)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await GetPurchaseServiceById.GetInvoiceById(InvoiceId, isCopy);
            var result = await _mediator.Send(new GetPurchaseByIdRequest
            {
                InvoiceId = InvoiceId,
                isCopy = isCopy
            });
            return result;
        }

        [HttpGet("GetEmailForSuppliers")]
        public async Task<ResponseResult> GetEmailForSuppliers(int InvoiceId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await SendEmailPurchases.GetEmailForSuppliers(InvoiceId);
            var result = await _mediator.Send(new GetEmailForSuppliersRequest { InvoiceId = InvoiceId });
            return result;
        }

        [HttpPost(nameof(SendEmailForSuppliers))]
        public async Task<ResponseResult> SendEmailForSuppliers([FromForm] SendEmailForSuppliersRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Purchases, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            //var result = await SendEmailPurchases.SendEmailForSuppliers(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }

        [HttpGet("GetInvoiceForSuppliers")]
        public async Task<ResponseResult> GetInvoiceForSuppliers(int InvoiceId)
        {
            //var result = await SendEmailPurchases.GetInvoiceForSuppliers(InvoiceId);
            var result = await _mediator.Send(new GetInvoiceForSuppliersRequest { InvoiceId = InvoiceId });
            return result;
        }

    }
}
