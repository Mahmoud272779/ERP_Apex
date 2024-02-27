using App.Domain.Models.Request.GeneralLedger;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.MultiCollectionReceipts.DeleteMultiCollectionReceipts
{
    public class DeleteMultiCollectionReceiptsRequest : Delete_MultiCollectionReceiptsRequestDTO,IRequest<ResponseResult>
    {
    }
}
