using App.Application.Services.Process.Invoices.Purchase.IPurchasesServices;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.EmailService;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase.PurchasesServices
{
    public class SendEmailPurchases : BaseClass, ISendEmailPurchases
    {
        
        private readonly IHttpContextAccessor httpContext;
        private readonly IMailKite mailKite;
        private readonly IEmailSender EmailSender;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvPersons> PersonsRepositoryQuery;
        private readonly IHostingEnvironment _hostingEnvironment;

        public SendEmailPurchases(IMailKite _mailKite,
                                   IHttpContextAccessor _httpContext,
                                   IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                                   IRepositoryQuery<InvoiceDetails> _InvoiceDetailsRepositoryQuery,
                                   IRepositoryQuery<InvPersons> _PersonsRepositoryQuery,
                                   IHostingEnvironment hostingEnvironment,
                                   IEmailSender _EmailSender) : base(_httpContext)
        {
            httpContext = _httpContext;
            mailKite = _mailKite;
            _hostingEnvironment = hostingEnvironment;
            InvoiceDetailsRepositoryQuery = _InvoiceDetailsRepositoryQuery;
            EmailSender = _EmailSender;
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            PersonsRepositoryQuery = _PersonsRepositoryQuery;
        }
        //api to send emails for suppliers depend on invoice id 
        public async Task<ResponseResult> SendEmailForSuppliers(EmailRequest parameter)
        {
            var invoiceDetails =await  InvoiceMasterRepositoryQuery.GetByAsync(q=>q.InvoiceId==parameter.InvoiceId);
           // var InvoiceSupplier = await PersonsRepositoryQuery.GetByAsync(q=>q.Id==invoiceDetails.PersonId);
            if (parameter.Email == null ||parameter.Email=="string") // if supplier doesnt have email
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFoundEmail };
            }
            else
            {
                var files = parameter.Files;
                string filesList = "";
                if (files != null) // upload files as array of files
                {
                    foreach (var item in files)
                    {
                        string FileUpload;
                        var path = _hostingEnvironment.WebRootPath;
                        string filePath = Path.Combine("Images\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + item.FileName.Replace(" ", ""));
                        string actulePath = Path.Combine(path, filePath);
                        using (var fileStream = new FileStream(actulePath, FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        FileUpload = "";
                        FileUpload = Constants.LocalServer+filePath;
                        // <img src='" + FileUpload + @"' alt='logo' width='300px' height='100%' /><br>
                        string body =  @"    <br> 
                                             <a href='" + FileUpload + @"' download='" + FileUpload + @"'> '" + item.FileName + @"' </a>";
                        filesList +=  body+ "             ";

                    }
        
                }
                //send email service contains to files and body and subject
                await EmailSender.SendEmailAsync(parameter.Email, parameter.Subject,string.Concat( parameter.Body , "\r\n\r\n" , filesList));

            }
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }
        //api to get data which will shown for user and will send with body foe email
        public async Task<ResponseResult> GetEmailForSuppliers(int InvoiceId)
        {
            var invoiceDetails =await  InvoiceMasterRepositoryQuery.GetByAsync(q=>q.InvoiceId==InvoiceId);
            var InvoiceSupplier = await PersonsRepositoryQuery.GetByAsync(q=>q.Id==invoiceDetails.PersonId);
            if (InvoiceSupplier.Email == null)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFoundEmail };
            }
            else
            {
                var messageDetails = new EmailDetailsDto()
                {
                    SupplierEmail = InvoiceSupplier.Email,
                    SupplierName = InvoiceSupplier.ArabicName,
                    InvoiceDate = invoiceDetails.InvoiceDate,
                    InvoiceNetPrice=invoiceDetails.Net,
                    InvoiceTotalPrice=invoiceDetails.TotalPrice,
                    InvoiceType=invoiceDetails.InvoiceType
                };
                return new ResponseResult() { Data = messageDetails, Id = null, Result = Result.Success };
            }

        }
        public async Task<ResponseResult> GetInvoiceForSuppliers(int InvoiceId)
        {
            var invoiceDetails = await InvoiceMasterRepositoryQuery.SingleOrDefault(q => q.InvoiceId == InvoiceId,(a=>a.InvoicesDetails));
            var InvoiceSupplier = await PersonsRepositoryQuery.GetByAsync(q => q.Id == invoiceDetails.PersonId);
            var messageDetailsList = new List<InvoiceForSupplierDto>();
            var messageDetails = new InvoiceForSupplierDto()
            {
                SupplierEmail = InvoiceSupplier.Email,
                Phone= InvoiceSupplier.Phone,
                AddressAr= InvoiceSupplier.AddressAr,
                SupplierName = InvoiceSupplier.ArabicName,
                InvoiceDate = invoiceDetails.InvoiceDate,
                InvoiceRemainPrice = invoiceDetails.Remain,
                InvoiceTotalPrice = invoiceDetails.TotalPrice,
                InvoiceType = invoiceDetails.InvoiceType,
            };
            var itemDetails = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(q=>q.InvoiceId==invoiceDetails.InvoiceId).Include(s=>s.Items);
            foreach (var detail in itemDetails)
            {
                var InvoiceDetailsForSupplierDto = new InvoiceDetailsForSupplier();
                InvoiceDetailsForSupplierDto.ItemId = detail.ItemId;
                InvoiceDetailsForSupplierDto.Quantity = detail.Quantity;
                InvoiceDetailsForSupplierDto.ItemName = detail.Items.ArabicName;
                InvoiceDetailsForSupplierDto.Price = detail.Price;
                InvoiceDetailsForSupplierDto.Total = detail.TotalWithSplitedDiscount;
                messageDetails.invoiceDetailsForSuppliers.Add(InvoiceDetailsForSupplierDto);
            }
            messageDetailsList.Add(messageDetails);
            return new ResponseResult() { Data = messageDetailsList, Id = null, Result = Result.Success };

        }
    }
}
