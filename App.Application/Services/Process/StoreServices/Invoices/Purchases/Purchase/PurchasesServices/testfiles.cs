
using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.RecieptsWithInvoices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class testfiles : BaseClass, Itestfiles
    {
       // private SettingsOfInvoice SettingsOfInvoice;
        IRepositoryCommand<InvoiceFiles> InvoiceFilesRepositoryCommand;
        private IAddInvoice generalProcess;
        IHostingEnvironment _hostingEnvironment;
        public testfiles(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IAddInvoice generalProcess,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                              IRepositoryCommand<InvoiceFiles> InvoiceFilesRepositoryCommand,
                              IHostingEnvironment _hostingEnvironment,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            this.InvoiceFilesRepositoryCommand = InvoiceFilesRepositoryCommand;
            this._hostingEnvironment = _hostingEnvironment;

            this.generalProcess = generalProcess;
        }
        public async Task<bool> testFiles(IFormFile[] attachedFiles)
        {
            if (attachedFiles != null)// || parameter.InvoiceFiles.Count() > 0)
            {
                if (attachedFiles.Count() > 0)
                {
                    var invoiceFileList = new List<InvoiceFiles>();

                    foreach (var item in attachedFiles)
                    {
                        var path = _hostingEnvironment.WebRootPath;
                        if (!(Directory.Exists(path + "FilesOfPurchases")))
                        {
                            Directory.CreateDirectory(path + "FilesOfPurchases");
                        }
                        string filePath = Path.Combine("FilesOfPurchases\\", DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + item.FileName.Replace(" ", ""));
                        string actulePath = Path.Combine(path, filePath);
                        using (var stream = new FileStream(actulePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);

                        }

                        var invoiceFile = new InvoiceFiles();

                        invoiceFile.InvoiceId = 1145;

                        invoiceFile.FileLink = filePath;
                        invoiceFileList.Add(invoiceFile);
                    }
                    InvoiceFilesRepositoryCommand.AddRange(invoiceFileList);
                    await InvoiceFilesRepositoryCommand.SaveChanges();

                    }

                }
                return true;
            }
        
     
    }
}
