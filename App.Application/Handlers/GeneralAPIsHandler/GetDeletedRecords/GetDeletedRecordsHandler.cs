using App.Application.Handlers.GeneralAPIsHandler.GetDeletedRecors;
using App.Application.Handlers.Units;
using App.Domain.Entities.Process.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetDeletedRecords
{
    public class GetDeletedRecordsHandler : IRequestHandler<GetDeletedRecordsRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<DeletedRecords> _deletedRecords;

        public GetDeletedRecordsHandler(IRepositoryQuery<DeletedRecords> deletedRecords)
        {
            _deletedRecords = deletedRecords;
        }

        public async Task<ResponseResult> Handle(GetDeletedRecordsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _deletedRecords.TableNoTracking.Where(q => q.DTime >= request.date).ToListAsync();
                return new ResponseResult() { Data = data, Id = null, Result = Result.Success };

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}
