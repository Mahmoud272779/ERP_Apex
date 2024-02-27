using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store.Reports.Store;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System.Threading;

namespace App.Application.Handlers.Units
{
    public class GetUnitsByDateHandler : IRequestHandler<GetUnitsByDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvStpUnits> UnitsRepositoryQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetUnitsByDateHandler(IRepositoryQuery<InvStpUnits> unitsRepositoryQuery , IGeneralAPIsService generalAPIsService)
        {
            UnitsRepositoryQuery = unitsRepositoryQuery;
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<ResponseResult> Handle(GetUnitsByDateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var resData = await UnitsRepositoryQuery.TableNoTracking.Where(q => q.UTime >= request.date).ToListAsync();

                return await generalAPIsService.Pagination(resData, request.PageNumber, request.PageSize);


            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}