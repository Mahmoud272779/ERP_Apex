using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.JournalEntryPrint
{
    public class JournalEntryPrintRequest : IRequest<WebReport>
    {
        public string ids { get; set; }
        public exportType exportType { get; set; }
        public bool isArabic { get; set; }
        public int fileId { get; set; }
    }
}
