using App.Domain.Models.Request.GeneralLedger;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.MultiCollectionReceipts.EditMultiCollectionReceipts
{
    public class EditMultiCollectionReceiptsRequest : Edit_MultiCollectionReceiptsRequestDTO, IRequest<ResponseResult>
    {
    }
}
