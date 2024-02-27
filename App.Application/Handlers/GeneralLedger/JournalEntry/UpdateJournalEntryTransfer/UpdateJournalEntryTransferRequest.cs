using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.JournalEntry.UpdateJournalEntryTransfer
{
    public class UpdateJournalEntryTransferRequest : UpdateTransfer, IRequest<IRepositoryActionResult>
    {
    }
}
