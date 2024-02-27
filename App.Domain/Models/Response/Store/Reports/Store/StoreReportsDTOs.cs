using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace App.Domain.Models.Response.Store.Reports.Store
{
    public class DetailedMovementOfanItemResponse
    {
        public double? PreviousBalance { get; set; }
        public List<Data> data { get; set; }
    }
    public class Data 
    {
        public int id { get; set; }
        public string date { get; set; }
        public string TransactionTypeAr { get; set; }
        public string TransactionTypeEn { get; set; }
        public string rowClassName { get; set; }
        public string DocumentCode { get; set; }
        public double IncomingBalanc { get; set; }
        public double OutgoingBalanc { get; set; }
        public double Balanc { get; set; }
        public string notes { get; set; }
        public double Serialize { get; set; }

        public int invoiceTypeId { get; set; }
        public string parentType { get; set; }
        
        public int pageId { get; set; }
        public int pageEnum { get; set; }
        public int DocTypeId { get; set; }
    }


    public class InventoryValuationResponse
    {
        public double totalItemsPrice { get; set; }
        public List<InventoryValuationData> Items { get; set; }
        public string Note { get; set; }
        public int DataCount { get; set; }
        public int TotalCount { get; set; }

    }
    public class InventoryValuationData
    {
        public string itemCode { get; set; }
        public string ArabicName { get; set; }
        public string latinName { get; set; }
        public string unitNameAr { get; set; }
        public string unitNameEn { get; set; }
        public double Balance { get; set; }
        public double price { get; set; }
        public double totalPrice { get; set; }
    }
    public class SerialDetailsReport
    {
        public DateTime Date { get; set; }
        public string ArabicName { get; set; }
        public string latinName { get; set; }
        public string DocumentCode { get; set; } 
        public string DocumentTypeAr { get; set; }
        public string DocumentTypeEn { get; set; }
        public string ItemNameAr { get; set; }
        public string ItemNameEn { get; set; }
        //for print
        public string DocumentDate { get; set; }
        public string rowClassName { get; set; }
        public string storeNameAr { get; set; }
        public string storeNameEn { get; set; }


        public int? InvoiceId { get; set; }
        public int? InvoiceTypeId { get; set; }

        public int? pageId { get; set; }
        public int? pageEnum { get; set; }
        public int? DocTypeId { get; set; }


    }

    public class firstSelection
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public string TransactionTypeAr { get; set; }
        public string TransactionTypeEn { get; set; }
        public string rowClassName { get; set; }
        public string DocumentCode { get; set; }
        public double Qyt { get; set; }
        public string notes { get; set; }
        public int invoiceTypeId { get; set; }
        public string parentType { get; set; }
        public double Serialize { get; set; }
        public int pageId { get; set; }
        public int pageEnum { get; set; }
        public int DocTypeId { get; set; }


    }
    public class SerialStores
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
}
