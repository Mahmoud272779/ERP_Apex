using App.Infrastructure;
using DocumentFormat.OpenXml.Office2010.Excel;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetAccountInformationHandler : IRequestHandler<GetAccountInformationRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLCurrency> currencyRepositoryQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchRepositoryQuery;

        public GetAccountInformationHandler(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLCurrency> currencyRepositoryQuery, IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery, IRepositoryQuery<GLBranch> branchRepositoryQuery)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            this.currencyRepositoryQuery = currencyRepositoryQuery;
            this.costCenterRepositoryQuery = costCenterRepositoryQuery;
            this.branchRepositoryQuery = branchRepositoryQuery;
        }
        public async Task<ResponseResult> Handle(GetAccountInformationRequest request, CancellationToken cancellationToken)
        {
            if (request.id == null)
                return new ResponseResult()
                {
                    ErrorMessageAr = ErrorMessagesAr.DataRequired,
                    ErrorMessageEn = ErrorMessagesEn.DataRequired,
                };
            var account = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.Id == request.id).Select(x => new
            {
                x.Id,
                accountCode = x.AccountCode.Replace(".", string.Empty),
                MainAccountNameAr = financialAccountRepositoryQuery.TableNoTracking.Where(x => x.Id == request.id).Select(x => x.ArabicName).FirstOrDefault(),
                CurrancyNameAr = currencyRepositoryQuery.TableNoTracking.Where(c => c.Id == x.CurrencyId).Select(x => x.ArabicName).FirstOrDefault(),
                x.FA_Nature,
                CostCenterNameAr = costCenterRepositoryQuery.TableNoTracking.Where(c => c.Id == x.HasCostCenter).Select(x => x.ArabicName).FirstOrDefault(),
                x.Notes,
                accountNameAr = x.ArabicName,
                x.FinalAccount,
                branchNameAr = branchRepositoryQuery.TableNoTracking.Where(c => c.Id == x.BranchId).Select(x => x.ArabicName).FirstOrDefault(),
                x.Status,
                x.IsMain
            });
            return new ResponseResult()
            {
                Data = account.Any() ? account.FirstOrDefault() : null,
                Result = Result.Success,
                Id = account.Any() ? account.FirstOrDefault().Id : null
            };
        }
    }
}
