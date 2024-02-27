using DocumentFormat.OpenXml.Office2013.Drawing.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.StoreServices.Invoices.Funds.ItemsFund.ItemFundGLIntegrationServices
{
    public interface iItemFundGLIntegrationService
    {
        public Task AddItemFundJournalEntry(ItemFundJournalEntryDTO param);
    }
    public class ItemFundJournalEntryDTO
    {
        public int documentId { get; set; }
        public double totalAmount { get; set; }
        public bool isUpdate { get; set; } = false;
        public bool isDelete { get; set; } = false;
        public DateTime date { get; set; }
    }
}
