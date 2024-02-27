using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices.Purchase.IPurchasesServices
{
    public interface ISendEmailPurchases
    {
        Task<ResponseResult> GetEmailForSuppliers(int InvoiceId);
        Task<ResponseResult> SendEmailForSuppliers(EmailRequest parameter);
        Task<ResponseResult> GetInvoiceForSuppliers(int InvoiceId);
    }
}
