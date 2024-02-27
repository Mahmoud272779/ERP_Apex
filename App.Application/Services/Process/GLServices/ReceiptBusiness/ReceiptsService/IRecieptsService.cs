using App.Domain.Models.Shared;
using App.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Response.General;
using App.Domain.Enums;
using FastReport.Web;
using App.Application.Handlers.Persons;
using App.Domain.Models.Security.Authentication.Response.Store;
using static App.Application.Services.Process.GLServices.ReceiptBusiness.ReceiptsService;

namespace App.Application.Services.Process.GLServices.ReceiptBusiness
{
    public interface IReceiptsService
    {
        Task<int> Autocode(int RecieptTypeId, int BranchId, bool isCompined);
        Task<ResponseResult> AddReceipt(RecieptsRequest parameter);
        Task<ResponseResult> GetReceiptById(int ReceiptId, int RecieptsType, bool isPrint = false);
        Task<ResponseResult> GetAllReceipts(GetRecieptsData parameter);
        Task<ResponseResult> UpdateReceipt(UpdateRecieptsRequest parameter,bool isFund = false);
        Task<ResponseResult> GetReceiptsAuthortyDropDown();
        Task<ResponseResult> DeleteReciepts(List<int?> Ids);
        Task<ResponseResult> GetReceiptCurrentFinancialBalance(int AuthorityId, int BenefitID);
        Task<ResponseResult> GetReceiptBalanceForBenifit(int AuthorityId, int BenefitID);
        Task<WebReport> ReceiptPrint(int ReceiptId, int RecieptsType, exportType exportType, bool isArabic, int fileId = 0);
        Task<WebReport> MultiCollectionReceptsPrint(int ReceiptId, int RecieptsType, exportType exportType, bool isArabic, int fileId = 0);
        ReceiptInfo SetRecieptTypeAndDirectoryAndNotes(int recieptTypeId, int? parentTypeId);

        public ValidationData receiptsValidattion(RecieptsRequest parameter, UserInformationModel userInfo, bool isFund = false);
        Task<bool> AddRecieptsInJournalEntry(RecieptsRequest parameter, GlReciepts data, int? financialIdOfSafeOfBank, financialData financialForBenfiteUser);
        Task<bool> updateRecieptsInJournalEntry(UpdateRecieptsRequest parameter, GlReciepts data, int? financialIdOfSafeOfBank, financialData financialForBenfiteUser);
        Task<financialData> getFinantialAccIdForAuthorty(int type, int ID, GlReciepts RData);

    }
}
