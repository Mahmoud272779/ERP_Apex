using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.ItemCard
{
    public class GetItemsByDateRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public DateTime _date { get; set; }
        public GetItemsByDateRequest(DateTime date, int pageNumber, int pageSize) {

            _date = date;
            PageNumber = pageNumber;
            PageSize = pageSize;

        }

    }
}
