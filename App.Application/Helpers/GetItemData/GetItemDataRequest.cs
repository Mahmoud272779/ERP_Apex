using App.Domain.Entities.Setup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.GetItemData
{
    public class GetItemDataRequest : IRequest<ItemAllData>
    {
        public int? UnitId { get; set; }
        public bool IsAdd { get; set; }
        public string ItemCode { get; set; }
        public IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository { get; set; }
        public IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery { get; set; }
    }
}
