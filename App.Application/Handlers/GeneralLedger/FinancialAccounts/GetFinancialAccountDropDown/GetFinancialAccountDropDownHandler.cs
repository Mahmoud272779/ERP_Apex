using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetFinancialAccountDropDownHandler : IRequestHandler<GetFinancialAccountDropDownRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialCost> _GLFinancialCostQuery;
        public GetFinancialAccountDropDownHandler(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery, IRepositoryQuery<GLFinancialCost> gLFinancialCostQuery)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            _GLFinancialCostQuery = gLFinancialCostQuery;
        }
        public async Task<ResponseResult> Handle(GetFinancialAccountDropDownRequest request, CancellationToken cancellationToken)
        {
            // By Alaa
            // جعفور طلب يوم 24/3/2022 ان الليست لاضافة حساب جديد لازم تكون رئيسية وفي القيود فرعيه

            var parentDatas = financialAccountRepositoryQuery.FindAll(a => (request.isMain == null ? true : a.IsMain == request.isMain)
                         && (request.SearchCriteria != null ? (a.ArabicName.Contains(request.SearchCriteria) ||
                            a.LatinName.Contains(request.SearchCriteria) || a.AccountCode.Replace(".", string.Empty).StartsWith(request.SearchCriteria)) : true)).OrderBy(q => q.autoCoding).ToHashSet();
            
           
            if(request.costCenterId != 0)
            {
                var costCenterAccounts = _GLFinancialCostQuery.TableNoTracking.Where(c => c.CostCenterId == request.costCenterId).Select(c=> c.FinancialAccountId);
                if(costCenterAccounts.Any())
                {
                    parentDatas = parentDatas.Where(c => !costCenterAccounts.Contains(c.Id)).ToHashSet();
                }
            }

            int dataCount = parentDatas.Count();
            double MaxPageNumber = parentDatas.Count() / Convert.ToDouble(request.PageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);

            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                parentDatas = parentDatas.ToList().Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToHashSet();
            }


            var res = parentDatas.Select(a => new FinancialAccountDropDown
            {
                Id = a.Id,
                Code = a.AccountCode.Replace(".", string.Empty),
                ArabicName = a.ArabicName,
                LatinName = a.LatinName,
                fA_Nature = a.FA_Nature
            });
            return new ResponseResult() { Data = res, DataCount = dataCount, Note = countofFilter == request.PageNumber ? Actions.EndOfData : null, Result = Result.Success };
        }
    }
}
