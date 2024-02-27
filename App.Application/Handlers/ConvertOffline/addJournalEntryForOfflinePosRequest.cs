using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ConvertOffline
{
    public  class addJournalEntryForOfflinePosRequest :  IRequest<bool>
    {
        public List<PurchasesJournalEntryIntegrationDTO> data { get; set; }
    
   
    }
  
}
