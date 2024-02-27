using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using System.Text.Json.Serialization;

namespace App.Application.Handlers.GeneralLedger
{
    public class addEntryFundsRequest : IRequest<IRepositoryActionResult>
    {
        public List<EntryFunds> EntryFunds { get; set; }
        public DateTime date { get; set; }
        public string? note { get; set; }
        [JsonIgnore]
        public bool isFund { get; set; } = false;
        public int docId {get;set;} = -1;
        public int Fund_FAId { get; set; } = 0;
    }
}
