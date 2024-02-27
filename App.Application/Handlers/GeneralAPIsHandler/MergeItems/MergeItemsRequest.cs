using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.MergeItems
{
    public class MergeItemsRequest : IRequest<ResponseResult>
    {
        public List<InvoiceDetailsRequest> list { get; set; }
        public int invoiceType { get; set; }

    }
}
