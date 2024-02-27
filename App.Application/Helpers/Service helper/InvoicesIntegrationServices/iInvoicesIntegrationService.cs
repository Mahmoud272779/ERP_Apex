using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Helpers.Service_helper.InvoicesIntegrationServices
{
    public interface iInvoicesIntegrationService
    {
        public Task<ResponseResult> InvoiceJournalEntryIntegration(PurchasesJournalEntryIntegrationDTO parm);
        public Task<ResponseResult> SalesInvoiceCostJournalEntryIntegration(InvoiceCostJournalEntryIntegrationDTO parm);
        public Task<ResponseResult> invoicePaymentJournalEntryIntegration(invoicePaymentJournalEntryDTO parm);
        public List<JournalEntryDetail> SetJournalEntryDetails(PurchasesJournalEntryIntegrationDTO request, GenralData generalData, int JEnteryId);


    }
    public class PurchasesJournalEntryIntegrationDTO
    {
        public double total { get; set; }
        public double serialize { get; set; }
        public double discount { get; set; }
        public double VAT { get; set; }
        public double net { get; set; }
        public int? personId { get; set; }
        public DateTime invDate { get; set; }
        public string InvoiceCode{ get; set; }
        public bool isIncludeVAT { get; set; }
        public bool isAllowedVAT { get; set; }
        public DocumentType DocType { get; set; }
        public int invoiceId { get; set; }
        public bool isUpdate { get; set; } = false;
        public bool isDelete { get; set; } = false;
        public List<InvoiceDetails> InvDetails { get; set; } = new List<InvoiceDetails>();
        public int? branchId { get; set; }
        public int? journalEntryId { get; set; }
        public int? employeeId { get; set; }
        public string note { get; set; }

    }
    public class InvoiceCostJournalEntryIntegrationDTO
    {
        public double Cost { get; set; }
        public DocumentType DocType { get; set; }
        public string InvoiceCode { get; set; }
        public int invoiceId { get; set; }
    }
    public class invoicePaymentJournalEntryDTO
    {
        public int paymentMethodId { get; set; }
        public DocumentType DocType { get; set; }
        public int personId { get; set; }
        public int RecNumber { get; set; }
        public int invoiceId { get; set; }
        public int invoiceCode { get; set; }
        public double net { get; set; }
    }
    public class itemFundJournalEntryIntegrationDTO
    {
        public int invoiceId { get; set; }
        public string documentCode { get; set; }
        public double total { get; set; }
        public bool isUpdate { get; set; }
    }
}
