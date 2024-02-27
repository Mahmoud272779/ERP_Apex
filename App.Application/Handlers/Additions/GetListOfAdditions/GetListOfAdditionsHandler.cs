
using MediatR;
using System.Threading;

namespace App.Application.Handlers
{
    public class GetListOfAdditionsHandler : IRequestHandler<GetListOfAdditionsRequest,ResponseResult>
    {
        private readonly IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery;
        private readonly IRepositoryQuery<InvPurchaseAdditionalCostsRelation> InvPurchaseAdditionalCostsRelationQuery;
        public GetListOfAdditionsHandler(IRepositoryQuery<InvPurchasesAdditionalCosts> additionsQuery, IRepositoryQuery<InvPurchaseAdditionalCostsRelation> invPurchaseAdditionalCostsRelationQuery)
        {
            this.additionsQuery = additionsQuery;
            InvPurchaseAdditionalCostsRelationQuery = invPurchaseAdditionalCostsRelationQuery;
        }

        public async Task<ResponseResult> Handle(GetListOfAdditionsRequest request, CancellationToken cancellationToken)
        {
            var purchaseAdditions = InvPurchaseAdditionalCostsRelationQuery.TableNoTracking;
            var resData = additionsQuery.TableNoTracking
          .Where(x => !string.IsNullOrEmpty(request.Name) ? x.ArabicName.Contains(request.Name) || x.LatinName.Contains(request.Name) || x.Code.ToString().Contains(request.Name) : true)
          .Where(x => request.Status != 0 ? x.Status == request.Status : true);

            resData = resData.OrderByDescending(x => x.Code);

            var count = resData.Count();

            if (request.PageSize > 0 && request.PageNumber > 0)
            {
                resData = resData.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize);
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            }
            resData.Where(a => a.InvPurchaseAdditionalCostsRelations.Count() == 0 );
            List<PurchasesAdditionalCostsDto>  additionList = new List<PurchasesAdditionalCostsDto>();
            Mapping.Mapper.Map(resData, additionList);

            additionList.ToList().ForEach(x => x.CanDelete = additionsHelper.isCanDeleteAdditions(purchaseAdditions, x.PurchasesAdditionalCostsId));

            return new ResponseResult() { Data = additionList, DataCount = count, Id = null, Result = additionList.Any() ? Result.Success : Result.Failed };

        }

    }
}
