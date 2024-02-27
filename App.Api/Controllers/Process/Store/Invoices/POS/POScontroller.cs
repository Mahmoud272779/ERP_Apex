using App.Api.Controllers.BaseController;
using App.Application.Handlers.POS.ClosePOSSession;
using App.Application.Handlers.POS.OpenPOSSeassons;
using App.Application.Services.HelperService.authorizationServices;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;
using App.Application.Handlers.Invoices.POS.GetAllPOSInvoices;
using App.Application.Handlers.Invoices.POS.GetPOSInvoiceById;
using App.Application.Handlers.Invoices.POS.GetReturnPOS;
using App.Application.Handlers.Invoices.POS.GetAllPOSReturnInvoices;
using App.Application.Handlers.Invoices.POS.AddPOSReturnInvoice;
using App.Application.Handlers.Invoices.POS.PosPrintWithEnding;
using App.Application.Handlers.Helper.InvoicePrint;
using App.Application.Handlers.Invoices.POS.AddPOSTotalReturnInvoice;
using App.Application.Handlers.Invoices.POS.GetItemQuantity;
using App.Application.Handlers.Invoices.POS.AddPOSInvoice;
using App.Application.Handlers.Invoices.POS.UpdatePOSInvoice;
using App.Application.Handlers.Invoices.POS.DeletePOSInvoice;
using App.Application.Handlers.Invoices.POS.PosNavigation;
using App.Application.Handlers.Invoices.POS.getInvoiceByIndex;
using App.Application.Handlers.Invoices.POS.AddSuspensionInvoicePOS;
using App.Application.Handlers.Invoices.POS.GetSuspensionInvoicePOSById;
using App.Application.Handlers.Invoices.POS.GetSuspensionInvoicePOS;
using App.Application.Handlers.Invoices.POS.GetItemUnitsForPOS;
using App.Domain.Models.Response.General;
using App.Application.Helpers;
using FastReport.Utils;
using System.Drawing.Printing;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Application.Services.Printing.PrintPermission;
using App.Application.Handlers.OfflinePOSControlling.DeviceRegistrationService;
using App.Infrastructure.UserManagementDB;
using App.Application.Handlers.OfflinePOSControlling.GetRegisteredDevicesService;

namespace App.Api
{
    public class POScontroller : ApiStoreControllerBase
    {
        //private readonly IGetPOSInvoicesService GetPOSInvoiceService;
        //private readonly IPOSInvSuspensionService InvSuspensionService;
        //private readonly IAddSalesService addSalesService;
        //private readonly IGetAllSalesServices getAllSalesServices;
        //private readonly IUpdateSalesService updateSalesService;
        //private readonly IDeleteSalesService deleteSalesInvoice;
        //private readonly IGetInvoiceByIdService GetServiceById;
        //private readonly IGeneralAPIsService generalAPIsService;
        //private readonly IAddReturnSalesService AddReturnSalesService;
        //private readonly IAddPOSInvoice AddPOSInvoiceService;
        //private readonly IPosService posService;
        //private readonly IFilesMangerService _filesMangerService;
        //private readonly IRepositoryQuery<InvGeneralSettings> settingService;
        //private readonly IPrintResponseService _iPrintResponseService;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IPermissionsForPrint _iPermmissionsForPrint;
        public POScontroller(
            //IGetPOSInvoicesService _GetPOSInvoiceService,
            //IPOSInvSuspensionService _InvSuspensionService,
            //IGetAllSalesServices _getAllSalesServices,
            //IAddSalesService _addSalesService,
            //IUpdateSalesService _updateSalesService,
            //IDeleteSalesService _deleteSalesInvoice,
            //IGetInvoiceByIdService _GetServiceById,
            //IGeneralAPIsService _generalAPIsService,
            //IAddPOSInvoice _AddPOSInvoiceService,
            //IAddReturnSalesService addReturnSalesService, 
            //IPosService posService,
            //IFilesMangerService filesMangerService, 
            //IRepositoryQuery<InvGeneralSettings> settingService, 
            //IPrintResponseService iPrintResponseService,

            iAuthorizationService iAuthorizationService,
            IMediator mediator,
            IActionResultResponseHandler ResponseHandler
,
            iUserInformation iUserInformation,
            IPermissionsForPrint iPermmissionsForPrint) : base(ResponseHandler)
        {
            //GetPOSInvoiceService = _GetPOSInvoiceService;
            //InvSuspensionService = _InvSuspensionService;
            //addSalesService = _addSalesService;
            //updateSalesService = _updateSalesService;
            //getAllSalesServices = _getAllSalesServices;
            //deleteSalesInvoice = _deleteSalesInvoice;
            //GetServiceById = _GetServiceById;
            //generalAPIsService = _generalAPIsService;
            //AddReturnSalesService = addReturnSalesService;
            //AddPOSInvoiceService = _AddPOSInvoiceService;
            //this.posService = posService;
            //_filesMangerService = filesMangerService;
            //this.settingService = settingService;
            //_iPrintResponseService = iPrintResponseService;

            _iAuthorizationService = iAuthorizationService;
            _mediator = mediator;
            _iUserInformation = iUserInformation;
            _iPermmissionsForPrint = iPermmissionsForPrint;
        }


        [HttpPost(nameof(GetAllPOSInvoices))]
        public async Task<ResponseResult> GetAllPOSInvoices(POSInvoiceSearchDTO parameter)
        {
            var subFormId = (int)SubFormsIds.POS;
            if (parameter.InvoiceTypeId == (int)DocumentType.ReturnPOS)
            {
                subFormId = (int)SubFormsIds.returnPOS;
            }
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, subFormId, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            GetAllPOSInvoicesRequest req = new GetAllPOSInvoicesRequest()
            {
                DateFrom = parameter.DateFrom,
                DateTo = parameter.DateTo,
                InvoiceCode = parameter.InvoiceCode,
                InvoiceDate = parameter.InvoiceDate,
                invoiceType = parameter.invoiceType,
                InvoiceTypeId = parameter.InvoiceTypeId,
                PageNumber = parameter.PageNumber,
                PageSize = parameter.PageSize,
                PersonId = parameter.PersonId,
                StoreId = parameter.StoreId,
                Searches = null,
                IsReturn = parameter.IsReturn,

            };




            //var result = await GetPOSInvoiceService.GetAllPOSInvoices(req);
            var result = await _mediator.Send(req);
            return result;
        }


        


        [HttpGet("GetPOSInvoiceById")]
        public async Task<ResponseResult> GetPOSInvoiceById(int? InvoiceId, string? InvoiceCode,bool? ForIOS)
        {

            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.POS, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            //var result = await GetPOSInvoiceService.GetPOSInvoiceById(InvoiceId, InvoiceCode);
            var result = await _mediator.Send(new GetPOSInvoiceByIdRequest { InvoiceCode = InvoiceCode, InvoiceId = InvoiceId , ForIOS = ForIOS });
            return result;
        }




        [HttpGet("GetReturnPOS")]
        public async Task<ResponseResult> GetReturnPOS(string InvoiceCode, bool? ForIOS)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.returnPOS, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            //var result = await GetPOSInvoiceService.GetPOSReturnInvoice(InvoiceCode);
            var result = await _mediator.Send(new GetReturnPOSRequest { InvoiceCode = InvoiceCode  , ForIOS=ForIOS});
            return result;
        }




        [HttpPost(nameof(GetAllPOSReturnInvoices))]
        public async Task<ResponseResult> GetAllPOSReturnInvoices(GetAllPOSReturnInvoicesRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.returnPOS, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;

            //var result = await GetPOSInvoiceService.GetAllPOSInvoices(parameter);
            var result = await _mediator.Send(parameter);
            return result;
        }




        [HttpPost(nameof(AddPOSReturnInvoice))]
        public async Task<IActionResult> AddPOSReturnInvoice(AddPOSReturnInvoiceRequest Parameter,[FromQuery] bool isArabic=true)
        {
            try
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.returnPOS, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);


                //var result = await AddPOSInvoiceService.AddPOSReturnInvoice(Parameter);
                var result = await _mediator.Send(Parameter);
                var userInfo = await _iUserInformation.GetUserInformation();

                //var print = settingService.TableNoTracking.FirstOrDefault().Pos_PrintWithEnding;
                var print = await _mediator.Send(new PosPrintWithEndingRequest());

                if (print && result.Result == Result.Success && userInfo.isPOSDesktop != "1")
                {
                    var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId,(int) SubFormsIds.returnPOS);
                    if (permissions.IsPrint)
                    {
                        var printResponse = await _mediator.Send(new InvoicePrintRequest
                        {
                            invoiceId = result.Id.Value,
                            invoiceCode = result.Code.ToString(),
                            exportType = exportType.Print,
                            screenId = (int)SubFormsIds.returnPOS,
                            isPOS = true,
                            isArabic=isArabic
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
                //  return Ok(result);
                return Ok(result);
            }



            catch (Exception e)
            {

                throw;
            }

        }





        [HttpPost(nameof(AddPOSTotalReturnInvoice))]
        public async Task<IActionResult> AddPOSTotalReturnInvoice(int Id,[FromQuery] bool isArabic=true)
        {
            try
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.returnPOS, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);

                //var result = await AddPOSInvoiceService.AddPOSTotalReturnInvoice(Id);
                var result = await _mediator.Send(new AddPOSTotalReturnInvoiceRequest { Id = Id });

                var print = await _mediator.Send(new PosPrintWithEndingRequest());

                if (print && result.Result == Result.Success)
                {
                    //var printResponse = await _iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.returnPOS, result.Code.ToString(), exportType.Print, true);
                    var printResponse = await _mediator.Send(new InvoicePrintRequest
                    {
                        invoiceId = result.Id.Value,
                        invoiceCode = result.Code.ToString(),
                        exportType = exportType.Print,
                        screenId = (int)SubFormsIds.returnPOS,
                        isPOS = true,
                        isArabic = isArabic

                    });
                    return Ok(printResponse);

                }
                //  return Ok(result);
                return Ok(result);
            }



            catch (Exception e)
            {

                throw;
            }

        }




        [HttpGet("GetItemQuantity")]
        public async Task<ResponseResult> GetItemQuantity(int ItemId, int UnitId, int StoreId, string? ParentInvoiceType, DateTime? ExpiryDate, bool IsExpiared, int invoiceId, DateTime invoiceDate)
        {
            if (ParentInvoiceType == null)
                ParentInvoiceType = "";

            if (ItemId == 0 || StoreId == 0 || UnitId == 0)
                return new ResponseResult { Result = Result.Failed, Note = "this data is required" };

            //var result = await generalAPIsService.CalculateItemQuantity(ItemId, UnitId, StoreId, ParentInvoiceType, ExpiryDate, IsExpiared, invoiceId, invoiceDate, (int)DocumentType.POS, null);
            var result = await _mediator.Send(new GetItemQuantityRequest
            {
                ItemId = ItemId,
                UnitId = UnitId,
                StoreId = StoreId,
                ParentInvoiceType = ParentInvoiceType,
                ExpiryDate = ExpiryDate,
                IsExpiared = IsExpiared,
                invoiceId = invoiceId,
                invoiceDate = invoiceDate,
            });

            if (result == null || (result.InvoiceQuantity == 0 && result.StoreQuantity == 0 && result.StoreQuantityWithOutInvoice == 0))
                return new ResponseResult { Result = Result.Failed };

            return new ResponseResult { Result = Result.Success, Data = result };
        }




        [HttpPost(nameof(AddPOSInvoice))]
        public async Task<IActionResult> AddPOSInvoice([FromBody] AddPOSInvoiceRequest parameter, [FromQuery] bool isArabic = true)
        {
            try
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.POS, Opretion.Add);
                if (isAuthorized != null)
                    return Ok(isAuthorized);
                //  return Ok(isAuthorized);

                var req = Request;

                //var result = await addSalesService.AddInvoiceForPOS(parameter);
                var result = await _mediator.Send(parameter);
                var userInfo = await _iUserInformation.GetUserInformation();

                var print = await _mediator.Send(new PosPrintWithEndingRequest());

                if (print && result.Result == Result.Success && userInfo.isPOSDesktop != "1")
                {
                    var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.POS);
                    if (permissions.IsPrint)
                    {
                        var printResponse = await _mediator.Send(new InvoicePrintRequest
                        {
                            invoiceId = result.Id.Value,
                            invoiceCode = result.Code.ToString(),
                            exportType = exportType.Print,
                            screenId = (int)SubFormsIds.POS,
                            isPOS = true,
                            isArabic = isArabic

                        });
                        printResponse.ResultForPrint = printResponse.Result;
                        InvoiceDto data = (InvoiceDto)result.Data;
                        printResponse.data = new InvoiceData() { net = data.Net, remain = data.Remain, virualPaid = data.VirualPaid };
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
                    //var printResponse = await _iPrintResponseService.Print(result.Id.Value, (int)SubFormsIds.POS, result.Code.ToString(), exportType.Print, true);
                    
                }
                //  return Ok(result);
                return Ok(value: result);

            }
            catch (Exception e)
            {

                throw;
            }
        }




        [HttpPut(nameof(UpdatePOSInvoice))]
        public async Task<IActionResult> UpdatePOSInvoice([FromBody] UpdatePOSInvoiceRequest parameter, [FromHeader] int? posSessionId, [FromQuery] bool isArabic=true)
        {
            try
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.POS, Opretion.Edit);
                if (isAuthorized != null)
                    return Ok(isAuthorized);
                //var result = await updateSalesService.UpdateInvoiceForPOS(parameter);
                parameter.posSessionId = posSessionId;
                var result = await _mediator.Send(parameter);
                var print = await _mediator.Send(new PosPrintWithEndingRequest());
                var userInfo = await _iUserInformation.GetUserInformation();

                if (print && result.Result == Result.Success)
                {
                    var permissions = await _iPermmissionsForPrint.GetPermisions(userInfo.permissionListId, (int)SubFormsIds.POS);
                    if (permissions.IsPrint)
                    {
                        var printResponse = await _mediator.Send(new InvoicePrintRequest
                        {
                            invoiceId = result.Id.Value,
                            invoiceCode = result.Code.ToString(),
                            exportType = exportType.Print,
                            screenId = (int)SubFormsIds.POS,
                            isPOS = true,
                            isArabic = isArabic

                        });
                        InvoiceDto data = (InvoiceDto)result.Data;
                        printResponse.data = new InvoiceData() { net = data.Net, remain = data.Remain, virualPaid = data.VirualPaid };

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
                //  return Ok(result);
                return Ok(result);
            }

            catch (Exception e)
            {

                throw;
            }

        }




        [HttpDelete("DeletePOSInvoice")]
        public async Task<ResponseResult> DeletePOSInvoice([FromBody] int[] InvoiceIdsList)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.pos, (int)SubFormsIds.POS, Opretion.Delete);
            if (isAuthorized != null)
                return isAuthorized;

            //SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
            //{
            //    Ids = InvoiceIdsList
            //};
            //var result = await deleteSalesInvoice.DeleteInvoiceForPOS(parameter);
            var result = await _mediator.Send(new DeletePOSInvoiceRequest { Ids = InvoiceIdsList });
            return result;
        }



        [HttpGet(nameof(PosNavigation))]
        public async Task<ResponseResult> PosNavigation(int invoiceTypeId, int invoiceId, int stepType, int branchId)
        {
            #region auth
            //MainFormsIds mainFormCode = new MainFormsIds();
            //SubFormsIds subFormCode = new SubFormsIds();
            //if (invoiceTypeId == (int)DocumentType.Purchase)
            //{
            //    mainFormCode = MainFormsIds.Purchases;
            //    subFormCode = SubFormsIds.Purchases;
            //}
            //else if (invoiceTypeId == (int)DocumentType.AddPermission)
            //{

            //    mainFormCode = MainFormsIds.Repository;
            //    subFormCode = SubFormsIds.AddPermission_Repository;
            //}
            //else if (invoiceTypeId == (int)DocumentType.Sales)
            //{
            //    mainFormCode = MainFormsIds.Sales;
            //    subFormCode = SubFormsIds.Sales;
            //}
            //else if (invoiceTypeId == (int)DocumentType.itemsFund)
            //{
            //    mainFormCode = MainFormsIds.ItemsFund;
            //    subFormCode = SubFormsIds.items_Fund;
            //}
            //else if (invoiceTypeId == (int)DocumentType.ExtractPermission)
            //{
            //    // mainFormCode = MainFormsIds.pos;
            //    //subFormCode = ;
            //}
            //else if (invoiceTypeId == (int)DocumentType.POS)
            //{
            //    //mainFormCode = ;
            //    //subFormCode = ;
            //}
            //else if (invoiceTypeId == (int)DocumentType.IncomingTransfer)
            //{
            //    //mainFormCode = ;
            //    //subFormCode = ;
            //}
            //else if (invoiceTypeId == (int)DocumentType.OutgoingTransfer)
            //{
            //    //mainFormCode = ;
            //    //subFormCode = ;
            //}
            //else if (invoiceTypeId == (int)DocumentType.ReturnPOS)
            //{
            //    //mainFormCode = ;
            //    //subFormCode = ;
            //}
            //else if (invoiceTypeId == (int)DocumentType.ReturnPurchase)
            //{
            //    //mainFormCode = ;
            //    //subFormCode = ;
            //}
            //else if (invoiceTypeId == (int)DocumentType.ReturnSales)
            //{
            //    //mainFormCode = ;
            //    //subFormCode = ;
            //}




            //var isAuthorized = await iAuthorizationService.isAuthorized((int)mainFormCode, (int)subFormCode, Opretion.Open);
            //if (isAuthorized != null)
            //    return isAuthorized;
            #endregion

            //var result = await GetPOSInvoiceService.POSNavigationStep(invoiceTypeId, invoiceId, stepType, branchId);
            var result = await _mediator.Send(new PosNavigationRequest
            {
                branchId = branchId,
                invoiceId = invoiceId,
                invoiceTypeId = invoiceTypeId,
                stepType = stepType
            });
            return result;
        }



        [HttpGet(nameof(getInvoiceByIndex))]
        public async Task<ResponseResult> getInvoiceByIndex(int invoiceTypeId, int index, int branchId)
        {


            //var result = await GetPOSInvoiceService.POSNavigationStepIndex(invoiceTypeId, index, branchId);
            var result = await _mediator.Send(new getInvoiceByIndexRequest { branchId = branchId, invoiceTypeId = invoiceTypeId, index = index });
            return result;
        }




        [HttpPost(nameof(AddSuspensionInvoicePOS))]
        public async Task<ResponseResult> AddSuspensionInvoicePOS(AddSuspensionInvoicePOSRequest parameter)
        {
            try
            {
                var req = Request;

                //var result = await InvSuspensionService.AddSuspensionInvoice(parameter);
                var result = await _mediator.Send(parameter);
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }




        [HttpGet(nameof(GetSuspensionInvoicePOSById))]
        public async Task<ResponseResult> GetSuspensionInvoicePOSById(int Id)
        {
            try
            {
                var req = Request;

                //var result = await InvSuspensionService.GetSuspensionInvoiceById(Id);
                var result = await _mediator.Send(new GetSuspensionInvoicePOSByIdRequest { Id = Id });
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }




        [HttpGet(nameof(GetSuspensionInvoicePOS))]
        public async Task<ResponseResult> GetSuspensionInvoicePOS(int? PageNumber, int? PageSize)
        {
            try
            {
                //var result = await InvSuspensionService.GetSuspensionInvoice(PageNumber, PageSize);
                var result = await _mediator.Send(new GetSuspensionInvoicePOSRequest { PageNumber = PageNumber, PageSize = PageSize });
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }




        [HttpGet(nameof(GetItemUnitsForPOS))]
        public async Task<ResponseResult> GetItemUnitsForPOS(int itemId)
        {
            try
            {
                //var result = await posService.getItemUnitsForPOS(itemId);
                var result = await _mediator.Send(new GetItemUnitsForPOSRequest { itemId =new List<int> { itemId } });
                return result;
            }
            catch (Exception e)
            {

                throw;
            }
        }




        [HttpPost("OpenPOSSeassion")]
        public async Task<ResponseResult> OpenPOSSeassion()
        {
            var openSeasson = await _mediator.Send(new openPOSSessionRequest());
            return openSeasson;
        }




        [HttpPost("ClosePOSSeassion/sessionId")]
        public async Task<ResponseResult> ClosePOSSeassion(int sessionId)
        {
            var openSeasson = await _mediator.Send(new closePOSSessionRequest() { sessionId = sessionId });
            return openSeasson;
        }




        [HttpPost(nameof(ActiveDevice))]
        public async Task<ResponseResult> ActiveDevice([FromBody] DeviceRegistrationRequest request)
        {
            var res = await _mediator.Send(request);
            return res;
        }

        [HttpGet(nameof(getOfflinePOSActivation) + "/{serverId}")]
        public async Task<ResponseResult> getOfflinePOSActivation(int serverId)
        {
            var res = await _mediator.Send(new GetRegisteredDevicesRequest { serverId = serverId});
            return res;
        }
    }
}

