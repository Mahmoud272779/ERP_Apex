using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using App.Domain.Models.Security.Authentication.Request;

namespace App.Application.Handlers.GeneralLedger.JournalEntry
{
    public class BlockJournalEntryReqeust :  IRequest<IRepositoryActionResult>
    {
        public int[] Ids { get; set; }
    }
}
