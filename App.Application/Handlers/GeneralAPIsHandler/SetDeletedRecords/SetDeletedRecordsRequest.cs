using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.SetDeletedRecords
{
    public class SetDeletedRecordsRequest : IRequest<ResponseResult>
    {
        public List<int> _Ids { get; set; }
        public int _type { get; set; }
    }
}
