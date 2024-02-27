using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using FastReport.Utils.Json.Serialization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.EmailServices
{
    public interface IEmailService
    {
        public Task<string> SendEmail(emailRequest emailRequest);
        public Task<string> ForgetPasswordSendEmail(ForgetPasswordEmail parm);
    }
    public class emailRequest : InvoiceDTO
    {
        public string ToEmail { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public IList<IFormFile> Attachments { get; set; }
        public bool isInvoice { get; set; } = false;
    }
    public class InvoiceDTO
    {
        [JsonIgnore]
        public byte[]? invoice { get; set; }
        public int? invoiceId { get; set; }
        public int? screenId { get; set; }
        public string? invoiceCode { get; set; }
        public bool isArabic { get; set; } = true;

    }

    public class ForgetPasswordEmail
    {
        public string Body { get; set; }
        public string toEmail { get; set; }
    }
}
