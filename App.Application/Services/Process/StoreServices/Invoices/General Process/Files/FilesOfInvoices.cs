using App.Application.Helpers.Service_helper.FileHandler;
using App.Domain.Entities.Process;
using App.Infrastructure.EmailService;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Reposiotries.Configuration;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App.Application
{
    public class FilesOfInvoices : IFilesOfInvoices
    {
        private readonly IRepositoryCommand<InvoiceFiles> invoiceFilesCommand;
        private readonly IRepositoryQuery<InvoiceFiles> invoiceFilesQuery;
        private readonly IRepositoryCommand<GLRecieptFiles> ReceiptsFilesCommand;
        private readonly IRepositoryQuery<GLRecieptFiles> ReceiptsFilesQuery;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IFileHandler fileHandler;

        public FilesOfInvoices(
                                    IRepositoryCommand<InvoiceFiles> invoiceFilesCommand, 
                                    IFileHandler fileHandler,
                                    IRepositoryQuery<InvoiceFiles> invoiceFilesQuery, 
                                    IRepositoryCommand<GLRecieptFiles> receiptsFilesCommand,
                                    IRepositoryQuery<GLRecieptFiles> receiptFilesQuery,
                                    Microsoft.Extensions.Configuration.IConfiguration configuration, 
                                    IHttpContextAccessor httpContext)
        {
            this.invoiceFilesCommand = invoiceFilesCommand;
            this.fileHandler = fileHandler;
            this.invoiceFilesQuery = invoiceFilesQuery;
            ReceiptsFilesQuery = receiptFilesQuery;
            _configuration = configuration;
            _httpContext = httpContext;
            ReceiptsFilesCommand = receiptsFilesCommand;
        }

        public async Task<bool> saveFilesOfInvoices(IFormFile[] AttachedFile, int BranchId, string fileDirectory, int InvoiceId, bool isUpdate, List<int>? FileId, bool isReceipt)
        {

            var localServerURL =  _configuration["ApplicationSetting:FileURL"];
            if (isUpdate && !isReceipt)
                RemoveOldFilesForUpdate(InvoiceId, FileId);
            else if(isUpdate&&isReceipt)
                RemoveOldFilesForUpdateReciepts(InvoiceId, FileId);


            if (AttachedFile != null)
            {
                if (AttachedFile.Count() > 0)
                {

                    var invoiceFileList = new List<InvoiceFiles>();
                    var receiptFileList = new List<GLRecieptFiles>();

                    var path = Path.Combine(new[] { BranchId.ToString(), fileDirectory, InvoiceId.ToString() });
                    var finalPath = fileHandler.folderExist(path);



                    foreach (var item in AttachedFile)
                    {

                        string filePath = Path.Combine(finalPath.Item2, DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + item.FileName.Replace(" ", ""));                  
                        string actulePath = Path.Combine(finalPath.Item1, filePath);
                       // string actulePath = Path.Combine(actulePath1, filePath);


                        using (var stream = new FileStream(path: actulePath, FileMode.Create))
                        {
                            await item.CopyToAsync(stream);
                        }

                        var invoiceFile = new InvoiceFiles();
                        var receiptFile = new GLRecieptFiles();
                        if (isReceipt)
                        {
                            receiptFile.RecieptId = InvoiceId;
                            receiptFile.FileLink = localServerURL + filePath;
                            receiptFile.FileName = item.FileName.Split(".")[0];
                            receiptFile.FileExtensions = item.FileName.Split(".")[1];
                            receiptFileList.Add(receiptFile);
                        }
                        invoiceFile.InvoiceId = InvoiceId;
                        invoiceFile.FileLink = localServerURL + filePath;
                        invoiceFile.FileName = item.FileName.Split(".")[0];
                        invoiceFile.FileExtensions = item.FileName.Split(".")[1];
                        invoiceFileList.Add(invoiceFile);
                    }
                    bool saved;


                    if (isReceipt)
                        saved = await ReceiptsFilesCommand.AddAsync(receiptFileList);
                    else
                        saved = await invoiceFilesCommand.AddAsync(invoiceFileList);

                    return saved;
                }
            }
            return  true;

        }

        public void RemoveOldFilesForUpdate(int InvoiceId, List<int>? FileId)
        {

            var OldinvoiceFile = invoiceFilesQuery.FindAll(q => q.InvoiceId == InvoiceId).ToList();
           
            if (FileId != null && FileId.Count() > 0)
            {
                var filesWillDeleted = OldinvoiceFile.Where(a => !FileId.Contains(a.InvoiceFileId));
                var PathOfFilesList = filesWillDeleted.Select(a => a.FileLink).ToList();
                invoiceFilesCommand.RemoveRange(filesWillDeleted);
                fileHandler.DeleteImage(PathOfFilesList);
            }
            else
            {
                if (OldinvoiceFile == null || OldinvoiceFile.Count == 0)
                    return;

                var PathOfFilesList = OldinvoiceFile.Select(a => a.FileLink).ToList();
                fileHandler.DeleteImage(PathOfFilesList);
                invoiceFilesCommand.RemoveRange(OldinvoiceFile);
            }


        }
        //for reciepts only 
        public void RemoveOldFilesForUpdateReciepts(int InvoiceId, List<int>? FileId)
        {

            var OldinvoiceFile = ReceiptsFilesQuery.FindAll(q => q.RecieptId == InvoiceId).ToList();
            if (FileId != null && FileId.Count() > 0)
            {
                var filesWillDeleted = OldinvoiceFile.Where(a => !FileId.Contains(a.Id));
                var PathOfFilesList = filesWillDeleted.Select(a => a.FileLink).ToList();
                ReceiptsFilesCommand.RemoveRange(filesWillDeleted);
                fileHandler.DeleteImage(PathOfFilesList);
            }
            else
            {
                if (OldinvoiceFile == null && OldinvoiceFile.Count == 0)
                    return;

                var PathOfFilesList = OldinvoiceFile.Select(a => a.FileLink).ToList();
                fileHandler.DeleteImage(PathOfFilesList);
                ReceiptsFilesCommand.RemoveRange(OldinvoiceFile);
            }

        }
    }
   
}
