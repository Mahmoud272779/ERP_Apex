using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.RecieptsWithInvoices
{
    public interface IAddRecieptsForInvoices
    {
       Task<bool> deleteInvoiceReceipt(int receiptId);
        Task<bool> blockReceipt(int[] receiptId);
        Task<double> GetTotalAmount(int personId);
        Task<bool> updateReceipt(GlReciepts gLReciept);
    }
}
