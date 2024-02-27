using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.GetItemsDropDown
{
    public class GetItemsDropDownRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public string? SearchCriteria { get; set; }
        public bool? isSearchByCode { get; set; }
        public int? itemType { get; set; }
        public int? invoiceTypeId { get; set; }
        public bool isInvoice { get; set; } = true;
    }
}
