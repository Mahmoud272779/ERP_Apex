using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.InvCollectionReceipt
{
    public  class UpdateinvoiceForCollectionReceiptRequest  : IRequest<ResponseResult>
    {
        public int signal { get; set; } // لو اضافه هيزود قيمة المسدد لو حذف هيطرحها
        public List<UpdateinvoiceForCollectionReceiptRequestList> invoicesList { get; set; } = new List<UpdateinvoiceForCollectionReceiptRequestList>();
    }

    public class UpdateinvoiceForCollectionReceiptRequestList
    {
        public double paid { get; set; }
        public int invoiceId { get; set; }
        public int branchId { get; set; }
        public int signal { get; set; } 
        public List<PaymentMethods> CollectionPaymentMethods { get; set; } = new List<PaymentMethods>();

    }

    public class CollectionPaymentMethods : PaymentMethods
    {
        public int invoiceId { get; set; }
        public int   signal { get; set; }
        public int branchId { get; set; }
    }
}
