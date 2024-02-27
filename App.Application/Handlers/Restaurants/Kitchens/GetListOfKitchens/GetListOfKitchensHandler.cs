using App.Application.Handlers.Restaurants.Kitchens.GetListOfKitchens;
using App.Domain.Entities.Process.Restaurants;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class GetListOfKitchensHandler : IRequestHandler<GetListOfKitchensRequest, ResponseResult>
    {
        
        private readonly IRepositoryQuery<Kitchens> kitchenQuery;

        public GetListOfKitchensHandler(IRepositoryQuery<Kitchens> kitchenQuery)
        {
            
            this.kitchenQuery = kitchenQuery;
        }
        public async Task<ResponseResult> Handle(GetListOfKitchensRequest parameters, CancellationToken cancellationToken)
        {
            var resData = kitchenQuery.TableNoTracking
            .Where(x => !string.IsNullOrEmpty(parameters.Name) ? x.ArabicName.Contains(parameters.Name) || x.LatinName.Contains(parameters.Name) || x.Code.ToString().Contains(parameters.Name) : true)
            .Where(x => parameters.Status != 0 ? x.Status == parameters.Status : true);
            if (string.IsNullOrEmpty(parameters.Name))
                resData = resData.OrderByDescending(x => x.Code);
            else
                resData = resData.OrderByDescending(x => x.Code);
            var count = resData.Count();

            if (parameters.PageSize > 0 && parameters.PageNumber > 0)
            {
                resData = resData.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize);
            }
            else
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };

            }

            return new ResponseResult() { Data = resData.ToList(), DataCount = count, Id = null, Result = resData.Any() ? Result.Success : Result.Failed };
        }
    }
}
