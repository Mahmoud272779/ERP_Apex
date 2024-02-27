using App.Domain.Models.Shared;
using System;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices
{
    public interface IHistoryReceiptsService
    {
        void AddReceiptsHistory(int BranchId, int BenefitId,
            string ReceiptsAction, int PaymentMethodId,
             int UserId, int SafeIDOrBank, int Code
, DateTime RecieptDate, int ReceiptsId, string RecieptType,
            int RecieptTypeId, int Signal, bool IsBlocked,
            bool IsAccredit, double Serialize, int AuthorityType, double Amount,int subTypId, UserInformationModel userInfo);

         Task<ResponseResult> GetAllReceiptsHistory(int InvoiceId );
    }
}
