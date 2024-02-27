using App.Application.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetStoresDropDownForReports
{
    public class GetStoresDropDownForReportsHandler : IRequestHandler<GetStoresDropDownForReportsRequest, ResponseResult>
    {
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvStpStores> storeQuery;

        public GetStoresDropDownForReportsHandler(iUserInformation userinformation, IRepositoryQuery<InvStpStores> storeQuery)
        {
            Userinformation = userinformation;
            this.storeQuery = storeQuery;
        }

        public async Task<ResponseResult> Handle(GetStoresDropDownForReportsRequest parm, CancellationToken cancellationToken)
        {
            var userInfo = await Userinformation.GetUserInformation();
            var stores = storeQuery.TableNoTracking.Include(a => a.StoreBranches)
                        .Where(x => userInfo.employeeBranches.Contains(x.StoreBranches.First().BranchId)).Select(x => new
                        {
                            x.Id,
                            x.ArabicName,
                            x.LatinName
                        });
            var res = stores.Skip((parm.pageNumber - 1) * parm.pageSize)
                                .Take(parm.pageSize)
                                .ToList();
            double MaxPageNumber = stores.ToList().Count() / Convert.ToDouble(parm.pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success,
                Note = (countofFilter == parm.pageNumber ? Actions.EndOfData : ""),
                TotalCount = stores.Count()
            };
        }
    }
}
