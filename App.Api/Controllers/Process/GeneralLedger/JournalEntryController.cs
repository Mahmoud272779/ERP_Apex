using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application.Handlers.GeneralLedger;
using App.Application.Handlers.GeneralLedger.FinancialAccounts;
using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Handlers.GeneralLedger.JournalEntry.GetJournalEntryByFinancialAccountCode;
using App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryPrint;
using App.Application.Handlers.GeneralLedger.JournalEntry.UpdateJournalEntryTransfer;
using App.Application.Services.HelperService.authorizationServices;
using App.Application.Services.Printing.PrintFile;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using FastReport.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static App.Domain.Enums.Enums;

namespace App.Api.Controllers
{
    public class JournalEntryController : ApiGeneralLedgerControllerBase
    {
        //private readonly IJournalEntryBusiness journalEntryBusiness;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IprintFileService _printFileService;
        private readonly IMediator _mediator;

        public JournalEntryController(/*IJournalEntryBusiness JournalEntryBusiness, */iAuthorizationService iAuthorizationService
            , IActionResultResponseHandler ResponseHandler, IprintFileService printFileService, IMediator mediator) :
            base(ResponseHandler)
        {
            //journalEntryBusiness = JournalEntryBusiness;
            _iAuthorizationService = iAuthorizationService;
            _printFileService = printFileService;
            _mediator = mediator;
        }
        [HttpGet("GenerateJournalAutomaticCode")]
        public async Task<string> GenerateJournalAutomaticCode()
        {
            var add = await _mediator.Send(new Application.Handlers.GeneralLedger.FinancialAccounts.AddAutomaticCodeRequest());
            return add;
        }



        [HttpPost(nameof(CreateNewJournalEntry))]
        public async Task<IRepositoryResult> CreateNewJournalEntry([FromForm] AddJournalEntryRequest parameter)
                {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Add);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            var re = Request.Form["JournalEntryDetails"];
            foreach (var item in re)
            {
                var resReport = JsonConvert.DeserializeObject<JournalEntryDetail>(item);
                parameter.JournalEntryDetails.Add(resReport);
            }
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }




        [HttpGet("GetAllJournalEntry")]
        public async Task<IRepositoryResult> GetAllJournalEntry(int? Code, int PageNumber,
            int PageSize, int CurrencyId, DateTime? From, DateTime? To, int? IsTransfer,string? branches,journalEntryTypes type = journalEntryTypes.all)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            GetJournalEntryRequest paramters = new GetJournalEntryRequest()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                Search = new Domain.Models.Security.Authentication.Response.JournalEntriesSearchParameter
                {
                    branches = branches,
                    type = type
                }

            };
            if (Code != null)
            {
                paramters.Search.Code = Code;
            }

            paramters.Search.From = From;
            paramters.Search.IsTransfer = IsTransfer;
            paramters.Search.To = To;
            var account = await _mediator.Send(paramters);
            var result = ResponseHandler.GetResult(account);
            return result;
        }




        [HttpPut(nameof(UpdatJournalEntry))]
        public async Task<IRepositoryResult> UpdatJournalEntry([FromForm] UpdateJournalEntryRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Edit);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            var re = Request.Form["JournalEntryDetails"];
            foreach (var item in re)
            {

                var resReport = JsonConvert.DeserializeObject<JournalEntryDetail>(item);
                parameter.journalEntryDetails.Add(resReport);
            }

            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }



        [HttpPut(nameof(UpdatJournalEntryTransfer))]
        public async Task<IRepositoryResult> UpdatJournalEntryTransfer([FromBody] UpdateJournalEntryTransferRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Edit);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }




        [HttpDelete("BlockJournalEntry")]
        public async Task<IRepositoryResult> BlockJournalEntry([FromBody] int[] Ids)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Delete);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            BlockJournalEntryReqeust parameter = new BlockJournalEntryReqeust()
            {
                Ids = Ids
            };
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }




        [HttpPost(nameof(AddJournalEntryFiles))]
        public async Task<IRepositoryResult> AddJournalEntryFiles([FromForm] AddJournalEntryFilesRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Add);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }





        [HttpGet(nameof(GetAllJournalEntryFiles))]
        public async Task<IRepositoryResult> GetAllJournalEntryFiles(int JournalEntryId, int JournalEntryDraftId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await _mediator.Send(new GetJournalEntryFilesRequest { JournalEntryDraftId = JournalEntryDraftId ,JournalEntryId = JournalEntryId });
            var result = ResponseHandler.GetResult(account);
            return result;
        }




        [HttpDelete("DeleteJournalEntryFiles")]
        public async Task<IRepositoryResult> DeleteJournalEntryFiles(int Id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Delete);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            RemoveJournalEntryFilesRequest parameter = new RemoveJournalEntryFilesRequest()
            {
                Id = Id
            };
            var add = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }





        [HttpGet("GetAllJournalEntryHistory")]
        public async Task<ResponseResult> GetAllJournalEntryHistory(int PageSize, int PageNumber, string? SearchCriteria)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            // return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            GetJournalEntryHistoryRequest parameters = new GetJournalEntryHistoryRequest()
            {
                PageNumber = PageNumber,
                PageSize = PageSize,
                SearchCriteria = SearchCriteria
            };

            var account = await _mediator.Send(parameters);
            //var result = ResponseHandler.GetResult(account);
            return account;
        }




        [HttpGet("GetJournalEntryById/{id}")]
        public async Task<IRepositoryResult> GetJournalEntryById(int id)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, id == -1 ? (int)SubFormsIds.OpeningBalance_GL : (int)SubFormsIds.AccountingEntries_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            var account = await _mediator.Send(new GetJournalEntryByIdRequest { Id = id});
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpGet("GetJournalEntryByDocId")]
        public async Task<IRepositoryResult> GetJournalEntryByDocId([FromQuery] int docType , [FromQuery] int docId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized(0,(int)SubFormsIds.AccountingEntries_GL, Opretion.Open);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));

            var account = await _mediator.Send(new GetJournalEntryByIdRequest 
            {
                Id = 0 ,
                docId = docId,
                docType = docType
            });
            var result = ResponseHandler.GetResult(account);
            return result;
        }




        [HttpGet("JournalEntryPrint")]
        public async Task<IActionResult> JournalEntryPrint(string ids,exportType exportType,bool isArabic,int fileId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers,   (int)SubFormsIds.AccountingEntries_GL, Opretion.Print);
            if (isAuthorized != null)
                return Ok(isAuthorized);
            //  return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            WebReport report = new WebReport();
            report = await _mediator.Send(new JournalEntryPrintRequest { ids = ids,exportType = exportType,isArabic = isArabic,fileId=fileId });
            return Ok(_printFileService.PrintFile(report, "AccountingEntries", exportType));
            
        }




        [HttpGet("GetJournalEntryByFinancialAccountCode/{financialId}")]
        public async Task<IRepositoryResult> GetJournalEntryByFinancialAccountCode(int financialId, [FromQuery] int pageNumber, [FromQuery] int pageSize)
        {
            //return Ok(accountTree.GetById(id).Result);
            var account = await _mediator.Send(new GetJournalEntryByFinancialAccountCodeRequest { financialId = financialId, pageNumber = pageNumber, pageSize = pageSize });
            var result = ResponseHandler.GetResult(account);
            return result;
        }




        [HttpGet("GetAllJournalEntryHistory/{JournalEntryId}")]
        public async Task<ResponseResult> GetAllJournalEntryHistory(int JournalEntryId)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.AccountingEntries_GL, Opretion.Open);
            if (isAuthorized != null)
                return isAuthorized;
            // return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var account = await _mediator.Send(new GetAllJournalEntryHistoryRequest { JournalEntryId = JournalEntryId });
            //  var result = ResponseHandler.GetResult(account);
            return account;
        }
        /// <summary>
        /// To get the entry funds use GetJournalEntryById api with Id -1
        /// </summary>



        [HttpPut("addEntryFunds")]
        public async Task<IRepositoryResult> addEntryFunds([FromBody] addEntryFundsRequest parameter)
        {
            var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.GeneralLedgers, (int)SubFormsIds.OpeningBalance_GL, Opretion.Add);
            if (isAuthorized != null)
                return ResponseHandler.GetResult(new RepositoryActionResult(result: isAuthorized));
            var res = await _mediator.Send(parameter);
            var result = ResponseHandler.GetResult(res);
            return result;
        }
    }
}
