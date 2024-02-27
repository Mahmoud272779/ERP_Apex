using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Request
{
    public class ModifyListOfItemsStatusRequest : IRequest<ResponseResult>
    {
        public List<int> Id { get; set; }
        public int Status{ get; set; }
    }
}
