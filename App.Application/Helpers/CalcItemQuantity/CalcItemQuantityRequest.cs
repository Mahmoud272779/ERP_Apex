using App.Domain.Entities.Setup;
using App.Domain.Models;
using MediatR;

namespace App.Application.Helpers.CalcItemQuantity
{
    public class CalcItemQuantityRequest
    {
        public int? invoiceId { get; set; }
        public int ItemId { get; set; }
        public int? UnitId { get; set; }
        public int StoreId { get; set; }
        public string ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpiared { get; set; }
        public int? invoiceTypeId { get; set; }
        public DateTime invoiceDate { get; set; }
        public List<CalcQuantityRequest> items { get; set; }
        public int itemTypeId { get; set; }
        public double currentQuantity { get; set; }
        public IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository      { get; set; }
        public IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery         { get; set; }
        public IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery              { get; set; }
        public IRepositoryQuery<InvGeneralSettings> GeneralSettings             { get; set; }
        public IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery                 { get; set; }
        public IRepositoryQuery<InvoiceMaster> invoiceMasterQuery                       { get; set; }




    }
    public class CalcCompositeItemQuantityRequest
    {
        public int? invoiceId { get; set; }
        public int ItemId { get; set; }
        public int? UnitId { get; set; }
        public int StoreId { get; set; }
        public string ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpiared { get; set; }
        public int? invoiceTypeId { get; set; }
        public DateTime invoiceDate { get; set; }
        public List<CalcQuantityRequest> items { get; set; }
        public int itemTypeId { get; set; }
        public double  currentQuantity { get; set; }
        public IRepositoryQuery<InvStpItemCardMaster> itemCardMasterRepository { get; set; }
        public IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery { get; set; }
        public IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery { get; set; }
        public IRepositoryQuery<InvGeneralSettings> GeneralSettings { get; set; }
        public IRepositoryQuery<InvoiceMaster> invoiceMasterQuery { get; set; }
        public IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery { get; set; }


    }
    public class CalcItemQuantityNotCompositeRequest
    {
        public int? invoiceId { get; set; }
        public int ItemId { get; set; }
        public int? UnitId { get; set; }
        public int StoreId { get; set; }
        public string ParentInvoiceType { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsExpiared { get; set; }
        public int? invoiceTypeId { get; set; }
        public DateTime invoiceDate { get; set; }
        public List<CalcQuantityRequest> items { get; set; }
        public int itemTypeId { get; set; }
        public double currentQuantity { get; set; }
        public IRepositoryQuery<InvGeneralSettings> GeneralSettings { get; set; }
        public IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery { get; set; }
        public IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery { get; set; }
        public IRepositoryQuery<InvoiceMaster> invoiceMasterQuery { get; set; }
        public IRepositoryQuery<InvStpItemCardParts> itemCardPartsQuery { get; set; }
    }
}
