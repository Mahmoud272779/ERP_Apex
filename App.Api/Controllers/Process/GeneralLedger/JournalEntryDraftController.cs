using App.Api.Controllers.BaseController;
using App.Application.Services.Process.JournalEntryDrafts;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Pagination;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process
{
    public class JournalEntryDraftController : ApiGeneralLedgerControllerBase
    {
        private readonly IJournalEntryDraftBusiness journalEntryDraftBusiness;
        public JournalEntryDraftController(IJournalEntryDraftBusiness JournalEntryDraftBusiness
            , IActionResultResponseHandler ResponseHandler) :
            base(ResponseHandler)
        {
            journalEntryDraftBusiness = JournalEntryDraftBusiness;
        }
        
        [HttpGet("GenerateJournalAutomaticCodeDraft")]
        public async Task<string> GenerateJournalAutomaticCodeDraft()
        {
            var add = await journalEntryDraftBusiness.AddAutomaticCodeDraft();
            return add;
        }
        
        [HttpPost(nameof(CreatJournalEntryDraft))]
        public async Task<IRepositoryResult> CreatJournalEntryDraft([FromForm] JournalEntryParameter parameter)
        {
            var re = Request.Form["JournalEntryDetails"];
            foreach (var item in re)
            {

                var resReport = JsonConvert.DeserializeObject<JournalEntryDetail>(item);
                parameter.JournalEntryDetails.Add(resReport);
            }

            var add = await journalEntryDraftBusiness.AddJournalEntryDraft(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }

        [HttpGet("GetAllJournalEntryDraft")]
        
        public async Task<IRepositoryResult> GetAllJournalEntryDraft()
        {
            var account = await journalEntryDraftBusiness.GetJournalEntryDraft();
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        
        [HttpPut(nameof(UpdatJournalEntryDraft))]
        public async Task<IRepositoryResult> UpdatJournalEntryDraft([FromBody] UpdateJournalEntryParameter parameter)
        {
            var add = await journalEntryDraftBusiness.UpdateJournalEntryDraft(parameter);
            var result = ResponseHandler.GetResult(add);
            return result;
        }
        [HttpGet("GetJournalEntryDraftById/{id}")]
        public async Task<IRepositoryResult> GetJournalEntryDraftById(int id)
        {
            var account = await journalEntryDraftBusiness.GetJournalEntryDraftById(id);
            var result = ResponseHandler.GetResult(account);
            return result;
        }
        [HttpDelete("BlockJournalEntryDraft/{id}")]
        public async Task<IRepositoryResult> BlockJournalEntryDraft(int id)
        {
            var res = await journalEntryDraftBusiness.BlockJournalEntry(id,true);
            var result = ResponseHandler.GetResult(res);
            return result;
        }
    }
}
