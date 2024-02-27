using App.Application.Handlers.GeneralAPIsHandler.GetDeletedRecors;
using App.Domain.Entities.Process.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.SetDeletedRecords
{
    public class SetDeletedRecordsHandler : IRequestHandler<SetDeletedRecordsRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<DeletedRecords> _deletedRecords;

        public SetDeletedRecordsHandler(IRepositoryCommand<DeletedRecords> deletedRecords)
        {
            _deletedRecords = deletedRecords;
        }

        public async Task<ResponseResult> Handle(SetDeletedRecordsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                //Fill The DeletedRecordTable
                var listRecords = new List<DeletedRecords>();
                foreach (var item in request._Ids.ToList())
                {
                    var deleted = new DeletedRecords
                    {
                        Type = request._type,
                        DTime = DateTime.Now,
                        RecordID = item
                    };
                    listRecords.Add(deleted);
                }
                _deletedRecords.AddRangeAsync(listRecords);

                return new ResponseResult() {Result = Result.Success };

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Result = Result.NotFound };


            }
        }
    }
}
