//using App.Domain.Models.Security.Authentication.Request;
//using App.Domain.Models.Security.Authentication.Response;
//using App.Domain.Models.Shared;
//using App.Infrastructure.Pagination;
//using Attendleave.Erp.Core.APIUtilities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace App.Application.Services.Process.JournalEntries
//{
//    public interface IJournalEntryBusiness
//    {
//        Task<IRepositoryActionResult> AddJournalEntry(JournalEntryParameter parameter);
//        Task<IRepositoryActionResult> addEntryFunds(addEntryFunds parameter,int docId = -1,int Fund_FAId = 0);
//        Task<IRepositoryActionResult> GetJournalEntry(PageParameterJournalEntries paramters);
//        Task<getAllJournalEntryResponse> getJournalEntryServ(PageParameterJournalEntries paramters, bool isPrint = false);
//        Task<IRepositoryActionResult> UpdateJournalEntry(UpdateJournalEntryParameter parameter);
//        Task<IRepositoryActionResult> BlockJournalEntry(BlockJournalEntry parameter);
//        Task<IRepositoryActionResult> AddJournalEntryFiles(JournalEntryFilesDto parameter);
//        Task<IRepositoryActionResult> GetJournalEntryFiles(int JournalEntryId, int JournalEntryDraftId);
//        Task<IRepositoryActionResult> RemoveJournalEntryFiles(FileTrans parameter);
//        Task<ResponseResult> GetJournalEntryHistory(PageParameter paramters);
//        Task<GetJournalEntryByID> JournalEntryById(int Id);
//        Task<IRepositoryActionResult> GetJournalEntryById(int Id);
//        Task<IRepositoryActionResult> GetJournalEntryByFinancialAccountCode(int financialId, int pageNumber, int pageSize);
//        Task<IRepositoryActionResult> UpdateJournalEntryTransfer(UpdateTransfer parameter);
//        Task<ResponseResult> GetAllJournalEntryHistory(int JournalEntryId);
//        Task<string> AddAutomaticCode();
//        Task<WebReport> JournalEntryPrint(string ids, exportType exportType, bool isArabic);



//        //
//        Task<bool> removeStoreFundFromJournalDetiales(int[] storeFundIds,int DocType);
        
//    }
//}
