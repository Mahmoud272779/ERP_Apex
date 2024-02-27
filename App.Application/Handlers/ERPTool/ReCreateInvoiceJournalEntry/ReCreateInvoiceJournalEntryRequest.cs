using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ERPTool.ReCreateInvoiceJournalEntry
{
    public class ReCreateInvoiceJournalEntryRequest : IRequest<ResponseResult>
    {
        public ERPTool_CreateJournalEntry type { get; set; }
    }
}
