using App.Application.Services.Printing;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Domain.Models.Shared;
using App.Domain;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Infrastructure.Persistence.Context;
using System.IO;
using App.Application.DataSet;
using App.Domain.Models.Request.ReportFile;
using App.Domain.Entities.Process.Store.Barcode;

namespace App.Application.Helpers.UpdateSystem.Updates
{
    public static class ReportFilesUpdate
    {
        public static async Task UpdatePrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            try
            {
                var fileNamesFromDb = dbContext.reportFiles.ToList();
                var arFilesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Reports\\ar");
                var arfiles = from file in
                Directory.EnumerateFiles(arFilesPath)
                              select file;

                var enFilesPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "Reports\\en");
                var enfiles = from file in
                Directory.EnumerateFiles(enFilesPath)
                              select file;
                string fileName = "";
                foreach (var item in fileNamesFromDb)
                {
                    //int screenId = dbContext.reportMangers.Where(m => m.ArabicFilenameId == item.Id).FirstOrDefault().screenId;
                    //var fileManger = GetListOfFile.ReportFilesList();
                    //string filename = fileManger.Where(r => r.screenId == screenId).FirstOrDefault().reportName;
                    //item.ReportFileName = fileName;
                    //if(item.ReportFileName== "itemPurchases")
                    //{
                    //    dbContext.reportMangers.Where(m => m.ArabicFilenameId == item.Id).FirstOrDefault().screenId = 55;

                    //    item.ReportFileName = "ItemsPurchases";
                    //}
                    if (item.IsArabic == true)
                    {
                        foreach (var file in arfiles)
                        {
                            fileName = Path.GetFileNameWithoutExtension(file);
                            if (item.ReportFileName == fileName)
                            {
                                // var fileName = Path.GetFileName(file); 
                                byte[] arrbytes = File.ReadAllBytes(file);
                                item.Files = arrbytes;
                                item.IsDefault = true;
                               
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var file in enfiles)
                        {
                            fileName = Path.GetFileNameWithoutExtension(file);

                            if (item.ReportFileName == fileName)
                            {
                                byte[] arrbytes = File.ReadAllBytes(file);
                                item.Files = arrbytes;
                                item.IsDefault = true;

                                break;
                            }
                        }
                    }

                    
                    
                }
                dbContext.reportFiles.UpdateRange(fileNamesFromDb);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {
                
            }
        }
        public static async Task AddPrintFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            try
            {
                var allFileNames = GetListOfFile.ReportFilesList();
                var fileNamesFromDb = dbContext.reportFiles;
                    var fileNamesToAdd = new List<ReportFiles>();
                if (allFileNames.Count > 0 && fileNamesFromDb.Count() > 0)
                {
                    foreach (var item in fileNamesFromDb)
                    {

                        foreach (var file in allFileNames)
                        {

                            if (item.ReportFileName == file.reportName)
                            {
                                allFileNames.Remove(file);
                                break;
                            }
                        }
                    }
                }
                if (allFileNames.Count > 0)
                {
                    foreach (var file in allFileNames)
                    {

                        fileNamesToAdd.Add(new ReportFiles
                        {
                            IsArabic = true,
                            IsReport = file.isReport?1:0,
                            ReportFileName = file.reportName,
                            ReportFileNameAr=file.reportNameAr,
                            Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, true)

                        });
                        fileNamesToAdd.Add(new ReportFiles
                        {
                            IsArabic = false,
                            IsReport = file.isReport ? 1 : 0,
                            ReportFileName = file.reportName,
                            ReportFileNameAr = file.reportNameAr,

                            Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, false)

                        });

                    }

                    dbContext.reportFiles.AddRange(fileNamesToAdd);
                    dbContext.SaveChanges();
                    var ReportFileManagerToAdd = new List<ReportManger>();
                    foreach (var item in fileNamesToAdd)
                    {
                        ReportFileManagerToAdd.Add(new ReportManger
                        {
                            IsArabic = item.IsArabic,
                            Copies = 1,
                            ArabicFilenameId = item.Id,
                            screenId = allFileNames.Where(x => x.reportName == item.ReportFileName).FirstOrDefault().screenId
                        });
                    }

                    dbContext.reportMangers.AddRange(ReportFileManagerToAdd);
                    dbContext.SaveChanges();
                }

            }
            catch (Exception)
            {
            }



        }

        public static async Task AddBarcodeFiles(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment)
        {
            try
            {
                var allFileNames = GetBarcodeFiles.GetBarcodePrintFiles();
                var fileNamesToAdd = new List<BarcodePrintFiles>();
                foreach (var file in allFileNames)
                {

                    fileNamesToAdd.Add(new BarcodePrintFiles
                    {
                        ArabicName = file.ArabicName,
                        LatineName = file.LatineName,
                        IsDefault = file.IsDefault,
                        File = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.LatineName, true, true)

                    });
                }
                dbContext.BarcodePrintFiles.AddRange(fileNamesToAdd);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
           
        }

        public static async Task AddPrintFilesForUpdate(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment,int updateNumber)
        {
           
                using (var sqlTransation = dbContext.Database.BeginTransaction()) {
                try
                {

                    var allUpdatedFiles = GetListOfFile.ReportFilesList().Where(a => a.updateNumber == updateNumber).ToList();

                    var fileNamesToAdd = new List<ReportFiles>();
                    
                    if (allUpdatedFiles.Count > 0)
                    {
                        foreach (var file in allUpdatedFiles)
                        {

                            fileNamesToAdd.Add(new ReportFiles
                            {
                                IsArabic = true,
                                IsReport = file.isReport ? 1 : 0,
                                ReportFileName = file.reportName,
                                ReportFileNameAr = file.reportNameAr,
                                IsDefault=true,
                                Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, true)

                            });
                            fileNamesToAdd.Add(new ReportFiles
                            {
                                IsArabic = false,
                                IsReport = file.isReport ? 1 : 0,
                                ReportFileName = file.reportName,
                                ReportFileNameAr = file.reportNameAr,
                                IsDefault = true,


                                Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.reportName, false)

                            });

                        }

                        dbContext.reportFiles.AddRange(fileNamesToAdd);
                        dbContext.SaveChanges();
                        var ReportFileManagerToAdd = new List<ReportManger>();
                        foreach (var item in fileNamesToAdd)
                        {
                            ReportFileManagerToAdd.Add(new ReportManger
                            {
                                IsArabic = item.IsArabic,
                                Copies = 1,
                                ArabicFilenameId = item.Id,
                                screenId = allUpdatedFiles.Where(x => x.reportName == item.ReportFileName).FirstOrDefault().screenId
                            });
                        }

                        dbContext.reportMangers.AddRange(ReportFileManagerToAdd);
                        //dbContext.SaveChanges();
                        sqlTransation.Commit();
                    }
                }
                catch (Exception e)
                {
                    sqlTransation.Rollback();
                }


            }

                

            
            



        }
        public static async Task UpdateSpecificFile(ClientSqlDbContext dbContext, IWebHostEnvironment _webHostEnvironment, int screenId)
        {
            var filesToUpdate = GetListOfFile.ReportFilesList().Where(a => a.screenId == screenId).FirstOrDefault();

            var fileNamesToUpdate = dbContext.reportMangers.Include(r => r.Files).Where(r => r.screenId == screenId
            && r.Files.ReportFileName == filesToUpdate.reportName).Select(r => r.Files).ToList();
            foreach (var file in fileNamesToUpdate)
            {
                if (file.IsArabic == true)
                {
                    file.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.ReportFileName, true);
                }
                else
                {
                    file.Files = ConvertReportToBytes.ConvertReport(_webHostEnvironment, file.ReportFileName, false);
                }
            }
            dbContext.reportFiles.UpdateRange(fileNamesToUpdate);
        }
    }
}
