using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.GetJournalEntryByFinancialAccountCode
{
    public class GetJournalEntryByFinancialAccountCodeRequest : IRequest<IRepositoryActionResult>
    {
        public int financialId { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }
}
