
using App.Application.Basic_Process;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts.GetAllFinancialAccountDropDown2
{
    public class getAllFinancialAccountDropDownHandler : BusinessBase<GLFinancialAccount>,IRequestHandler<getAllFinancialAccountDropDownRequest, IRepositoryActionResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;

        public getAllFinancialAccountDropDownHandler(IRepositoryActionResult repositoryActionResult, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery) : base(repositoryActionResult)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
        }
        public async Task<IRepositoryActionResult> Handle(getAllFinancialAccountDropDownRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var parentData = financialAccountRepositoryQuery.GetAll().Where(q => q.IsBlock == false).Select(x => new
                {
                    x.Id,
                    x.ArabicName,
                    x.LatinName,
                    x.Credit,
                    x.Debit,
                    x.AccountCode
                }).ToList();
                var list = new List<CostCenterDto>();
                if (parentData != null)
                {
                    foreach (var item in parentData)
                    {
                        var CostCenterDto = new CostCenterDto();
                        CostCenterDto.Id = item.Id;
                        CostCenterDto.ArabicName = item.ArabicName;
                        CostCenterDto.LatinName = item.LatinName;
                        CostCenterDto.Code = item.AccountCode;
                        CostCenterDto.Credit = item.Credit;
                        CostCenterDto.Debit = item.Debit;
                        list.Add(CostCenterDto);
                    }
                }

                // var result = pagedListFinancialAccountDto.GetGenericPagination(list, paramters.PageNumber, paramters.PageSize, Mapper);
                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok, message: "Ok");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
    }
}
