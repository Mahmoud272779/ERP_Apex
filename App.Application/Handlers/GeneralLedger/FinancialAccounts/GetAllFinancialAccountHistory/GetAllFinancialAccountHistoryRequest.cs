using MediatR;
namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetAllFinancialAccountHistoryRequest : IRequest<ResponseResult>
    {
        public int id { get; set; } 
    }
}
