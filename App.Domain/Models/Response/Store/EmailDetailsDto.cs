using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class EmailDetailsDto
    {
        public string SupplierEmail { get; set; } // supplier email which will get for user epend on invoice
        public string InvoiceType { get; set; } // invoice code which will added for email body 
        public DateTime InvoiceDate { get; set; } // invoice date which will added for email body
        public double InvoiceTotalPrice { get; set; } // invoice total price which will added for email body
        public double InvoiceNetPrice { get; set; } // invoice remain price which will added for email body
        public string SupplierName { get; set; } // supplier name which will added for email body
    }
    public class EmailRequest
    {
        public int InvoiceId { get; set; } // this invoice which selected to send email
        public string Subject { get; set; } // subject which will send with email
        public string Body { get; set; }// body which will send to supplier and this body will get from GetEmailForSuppliers api 
        public string Email { get; set; } 
        public IFormFile[] Files { get; set; } // array of files which will upload to send with email
    }
    public class InvoiceForSupplierDto
    {
        public InvoiceForSupplierDto()
        {
            invoiceDetailsForSuppliers = new List<InvoiceDetailsForSupplier>();
        }
        public string SupplierEmail { get; set; } // supplier email which will get for user epend on invoice
        public string Phone { get; set; }
        public string AddressAr { get; set; }
        public string InvoiceType { get; set; } // invoice code which will added for email body 
        public DateTime InvoiceDate { get; set; } // invoice date which will added for email body
        public double InvoiceTotalPrice { get; set; } // invoice total price which will added for email body
        public double InvoiceRemainPrice { get; set; } // invoice remain price which will added for email body
        public string SupplierName { get; set; } // supplier name which will added for email body
        public List<InvoiceDetailsForSupplier> invoiceDetailsForSuppliers { get; set; }
    }
    public class InvoiceDetailsForSupplier
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }//سعر الشراء
        public double Total { get; set; }
    }
}
