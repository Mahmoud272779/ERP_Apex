using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Domain;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static App.Domain.Enums.Enums;
using App.Infrastructure.Mapping;
using App.Application.Services.Process.StoreServices;
using Newtonsoft.Json;
using App.Application.Services.HelperService.authorizationServices;
using App.Domain.Enums;
using App.Domain.Entities;
using FastReport.Web;
using App.Application.Services.Printing.PrintFile;
using App.Application.Services.Process.GLServices.ReceiptBusiness.ReceiptsPaid;
using FastReport;

namespace App.Api.Controllers.Process
{
    public class ReceiptController : ApiGeneralLedgerControllerBase
    {
        private readonly IReceiptsService ReceiptsBusiness;
        private readonly ICollectionReceipts CollectionReceiptsBusiness;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IHistoryReceiptsService RecieptsHistory;
        private readonly IprintFileService _printFileService;

        public ReceiptController(IReceiptsService receiptsBusiness, iAuthorizationService iAuthorizationService,
          IHistoryReceiptsService recieptsHistory, IActionResultResponseHandler ResponseHandler, IprintFileService printFileService, ICollectionReceipts collectionReceiptsBusiness) :
            base(ResponseHandler)
        {
            ReceiptsBusiness = receiptsBusiness;
            _iAuthorizationService = iAuthorizationService;
            RecieptsHistory = recieptsHistory;
            _printFileService = printFileService;
            CollectionReceiptsBusiness = collectionReceiptsBusiness;
        }
        //create new bank with multiple branches

        [HttpPost(nameof(CreateNewReceipt))]
        public async Task<ActionResult> CreateNewReceipt([FromForm] MainRequestDataForAdd parameter)
        {
            SubFormsIds SubFormId = new SubFormsIds();
            MainFormsIds mainFormsId = new MainFormsIds();
            if (parameter.RecieptTypeId == (int)DocumentType.SafeCash)
            {
                SubFormId = SubFormsIds.CashReceiptForSafe;
                mainFormsId = MainFormsIds.safes;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.SafePayment)
            {
                SubFormId = SubFormsIds.PayReceiptForSafe;
                mainFormsId = MainFormsIds.safes;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.BankCash)
            {
                SubFormId = SubFormsIds.CashReceiptForBank;
                mainFormsId = MainFormsIds.banks;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.BankPayment)
            {
                SubFormId = SubFormsIds.PayReceiptForBank;
                mainFormsId = MainFormsIds.banks;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.SafeTransfer)
            {
                SubFormId = SubFormsIds.transferFromSafes;
                mainFormsId = MainFormsIds.safes;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.BankTransfer)
            {
                SubFormId = SubFormsIds.transferFromBanks;
                mainFormsId = MainFormsIds.banks;
            }
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)mainFormsId, (int)SubFormId, Opretion.Add);
            if (isAuthorized != null)
                return Ok(isAuthorized);

            var costcenter = Request.Form["costCenterReciepts"];
            foreach (var item in costcenter)
            {
                var resReport = JsonConvert.DeserializeObject<CostCenterReciepts>(item);
                if (resReport.Number <= 0 || resReport.Percentage <= 0)
                    return StatusCode(StatusCodes.Status422UnprocessableEntity, "Costcenter Number and Percentage Must be Greater than Zero");
                parameter.costCenterReciepts.Add(resReport);
            }
         
            var data= Mapping.Mapper.Map<MainRequestDataForAdd,RecieptsRequest>(parameter);
           // data.costCenterReciepts = costCenter;// Mapping.Mapper.Map<List<CostCenterReciepts>, List<CostCenterReciepts>>(parameter.costCenterReciepts, data.costCenterReciepts);
            var add = await ReceiptsBusiness.AddReceipt(data);
            if (add.Result == Result.Success) 
              return Ok(add);
            return StatusCode(StatusCodes.Status422UnprocessableEntity, add);

           

        }

        [HttpPut(nameof(UpdateReceipt))]
        public async Task<ActionResult> UpdateReceipt([FromForm] UpdateRecieptsRequest parameter)
        {
            SubFormsIds SubFormId = new SubFormsIds();
            MainFormsIds mainFormsId = new MainFormsIds();
            if (parameter.RecieptTypeId == (int)DocumentType.SafeCash)
            {
                SubFormId = SubFormsIds.CashReceiptForSafe;
                mainFormsId = MainFormsIds.safes;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.SafePayment)
            {
                SubFormId = SubFormsIds.PayReceiptForSafe;
                mainFormsId = MainFormsIds.safes;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.BankCash)
            {
                SubFormId = SubFormsIds.CashReceiptForBank;
                mainFormsId = MainFormsIds.banks;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.BankPayment)
            {
                SubFormId = SubFormsIds.PayReceiptForBank;
                mainFormsId = MainFormsIds.banks;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.BankTransfer)
            {
                SubFormId = SubFormsIds.transferFromBanks;
                mainFormsId = MainFormsIds.banks;
            }
            else if (parameter.RecieptTypeId == (int)DocumentType.SafeTransfer)
            {
                SubFormId = SubFormsIds.transferFromSafes;
                mainFormsId = MainFormsIds.banks;
            }
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)mainFormsId, (int)SubFormId, Opretion.Edit);
            if (isAuthorized != null)
                return Ok(isAuthorized);

            var costcenter = Request.Form["costCenterReciepts"];
            foreach (var item in costcenter)
            {
               
                var resReport = JsonConvert.DeserializeObject<UpdateCostCenterReciepts>(item);
               if( resReport.Number <=0 || resReport.Percentage<=0)
                    return  StatusCode(StatusCodes.Status422UnprocessableEntity, "Costcenter Number and Percentage Must be Greater than Zero");
                parameter.costCenterReciepts.Add(resReport);
            }
            var add = await ReceiptsBusiness.UpdateReceipt(parameter);
            if (add.Result == Result.Success)
                return Ok(add);
            return StatusCode(StatusCodes.Status422UnprocessableEntity, add);
        }

        [HttpGet("GetReceiptById")]
        public async Task<IActionResult> GetReceiptById(int ReceiptId, int RecieptsType)
        {
            var account = await ReceiptsBusiness.GetReceiptById(ReceiptId, RecieptsType,false);
            if(account.Result == Result.NoDataFound)
                return NotFound();
            return Ok(account);
        }
        [HttpGet("ReceiptPrint")]
        public async Task<IActionResult> ReceiptPrint(int ReceiptId, int RecieptsType,exportType exportType, bool isArabic,int fileId=0,int screenId = 0)
        {
            if (screenId != 0)
            {
                if (screenId == (int)SubFormsIds.PayReceiptForSafe)
                    RecieptsType = (int)DocumentType.SafePayment;
                else if(screenId == (int)SubFormsIds.PayReceiptForBank)
                    RecieptsType = (int)DocumentType.BankPayment;
                else if(screenId == (int)SubFormsIds.CashReceiptForSafe)
                    RecieptsType = (int)DocumentType.SafeCash;
                else if (screenId == (int)SubFormsIds.CashReceiptForBank)
                    RecieptsType = (int)DocumentType.BankCash;
            }
            WebReport report = new WebReport();
            report = await ReceiptsBusiness.ReceiptPrint(ReceiptId, RecieptsType,exportType,isArabic,fileId);
            string reportName = "";

            if (RecieptsType == (int)DocumentType.SafeCash)
            {
               
                reportName = "CashReceiptForSafe";

            }
            else if (RecieptsType == (int)DocumentType.SafePayment)
            {
                reportName = "PayReceiptForSafe";
            }
            else if (RecieptsType == (int)DocumentType.BankCash)
            {
               
                reportName = "CashReceiptForBank";

            }
            else if (RecieptsType == (int)DocumentType.BankPayment)
            {
                reportName = "PayReceiptForBank";

            }
            else if (RecieptsType == (int)DocumentType.SafeTransfer)
            {

                reportName = "TransferReceiptForSafe";

            }
            else if (RecieptsType == (int)DocumentType.BankTransfer)
            {
                reportName = "TransferReceiptForBank";

            }
            

            var file =  _printFileService.PrintFile(report, reportName, exportType);
            return Ok(file);

        }

        [HttpGet("MultiCollectionReceiptPrint")]
        public async Task<IActionResult> MultiCollectionReceiptPrint(int ReceiptId, int RecieptsType, exportType exportType, bool isArabic, int fileId = 0, int screenId = 0)
        {
            WebReport report = new WebReport();
            report = await ReceiptsBusiness.MultiCollectionReceptsPrint(ReceiptId, RecieptsType, exportType, isArabic, fileId);
            string  reportName = "";
              if (RecieptsType == (int)DocumentType.SafeMultiCollectionReceipts)
            {

                reportName = "SafeMultiCollectionReceipts";

            }
            else if (RecieptsType == (int)DocumentType.BankMultiCollectionReceipts)
            {
                reportName = "BankMultiCollectionReceipts";

            }
            var file = _printFileService.PrintFile(report, reportName, exportType);
            return Ok(file);
        }


            [HttpGet("GetReceiptsAuthortyDropDown")]
        public async Task<IActionResult> GetReceiptsAuthortyDropDown()
        {
            var account = await ReceiptsBusiness.GetReceiptsAuthortyDropDown();
            return Ok(account.Data);
        }



        [HttpDelete ("DeleteReciepts") ]
        public async Task<IActionResult> DeleteReciepts(List<int?> reciepts)
        {
          
            var isDEleted = await CollectionReceiptsBusiness.DeleteCollectionReceipts(reciepts);
            if (isDEleted.Result == Result.NotFound)
                return StatusCode(StatusCodes.Status422UnprocessableEntity,isDEleted.ErrorMessageAr); 
            if (isDEleted.Result == Result.Failed)
                return StatusCode(StatusCodes.Status422UnprocessableEntity);
            return Ok(isDEleted);
        }
        [HttpGet("GetAllReceipt")]
        public async Task<IActionResult> GetAllReceipt(int PageSize, int PageNumber, DateTime? dateFrom, DateTime? dateTo, string? Code, int SafeOrBankId, int BenefitId, int AuthotityId, int PaymentMethoudId, int RecieptsType)
        {
            SubFormsIds SubFormId = new SubFormsIds();
            MainFormsIds mainFormsId = new MainFormsIds();
            if (RecieptsType == (int)DocumentType.SafeCash)
            {
                SubFormId = SubFormsIds.CashReceiptForSafe;
                mainFormsId = MainFormsIds.safes;
            }
            else if (RecieptsType == (int)DocumentType.SafePayment)
            {
                SubFormId = SubFormsIds.PayReceiptForSafe;
                mainFormsId = MainFormsIds.safes;
            }
            else if (RecieptsType == (int)DocumentType.BankCash)
            {
                SubFormId = SubFormsIds.CashReceiptForBank;
                mainFormsId = MainFormsIds.banks;
            }
            else if (RecieptsType == (int)DocumentType.BankPayment)
            {
                SubFormId = SubFormsIds.PayReceiptForBank;
                mainFormsId = MainFormsIds.banks;
            }
            else if (RecieptsType == (int)DocumentType.BankTransfer)
            {
                SubFormId = SubFormsIds.transferFromBanks;
                mainFormsId = MainFormsIds.banks;
            }
            else if (RecieptsType == (int)DocumentType.SafeTransfer)
            {
                SubFormId = SubFormsIds.transferFromSafes;
                mainFormsId = MainFormsIds.safes;
            }
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)mainFormsId, (int)SubFormId, Opretion.Open);
            if (isAuthorized != null)
                return Ok(isAuthorized);




            GetRecieptsData data = new GetRecieptsData()
            {
                SafeOrBankId = SafeOrBankId,
                PageNumber = PageNumber,
                PageSize = PageSize,
                Code = Code,
                PaymentWays = PaymentMethoudId,
                BenefitId = BenefitId,
                AuthotityId = AuthotityId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ReceiptsType = RecieptsType,
               
            };
            
            var result = await ReceiptsBusiness.GetAllReceipts(data);
            if(result.Result== Result.RequiredData)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, result);
            return Ok(result);
        }

        [HttpGet("GetReceiptsHistory")]
        public async Task<IActionResult> GetReceiptsHistory(int ReceiptsID)
        {

            if(ReceiptsID==0)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "RecieptsId Must not be null or 0");
            var account = await RecieptsHistory.GetAllReceiptsHistory(ReceiptsID);
            return Ok(account.Data);
        }

        [HttpGet("GetReceiptCurrentFinancialBalance")]
        public async Task<IActionResult> GetReceiptCurrentFinancialBalance(int AuthorityId, int BenefitID)
        {
            if (AuthorityId <= 0|| BenefitID <= 0)
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Some Data Requierd");
            var account = await ReceiptsBusiness.GetReceiptBalanceForBenifit( AuthorityId,  BenefitID);
            if (account.Result == Result.Failed)
                return StatusCode(StatusCodes.Status422UnprocessableEntity,account);
            return Ok(account.Data);
        }

    }
}
