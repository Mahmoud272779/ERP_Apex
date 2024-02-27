using App.Application.Helpers;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Application.Services.Process.StoreServices.Invoices.OfferPrice.IOfferPriceService;
using App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.ISalesServices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.Sales.Sales.SalesServices
{
    public  class DeleteTempInvoiceService : BaseClass, IDeleteTempInvoiceService
    {
        private readonly IRepositoryCommand<OfferPriceMaster> OfferPriceMasterRepositoryCommand;
        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterRepositoryQuery;

        private readonly IRepositoryCommand<OfferPriceDetails> OfferPriceDetailsRepositoryCommand;
        private readonly IRepositoryQuery<OfferPriceDetails> OfferPriceDetailsRepositoryQuery;

        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly ISystemHistoryLogsService systemHistoryLogsService;

        // private SettingsOfInvoice SettingsOfInvoice;
        private readonly IHttpContextAccessor httpContext;
        public DeleteTempInvoiceService(
                              IRepositoryCommand<OfferPriceMaster> _OfferPriceMasterRepositoryCommand,
                              IRepositoryCommand<OfferPriceDetails> _OfferPriceDetailsRepositoryCommand,
                              IRepositoryQuery<OfferPriceDetails> _OfferPriceDetailsRepositoryQuery,
                              IHistoryInvoiceService _HistoryInvoiceService,
                              IGeneralAPIsService _GeneralAPIsService,
                              ISystemHistoryLogsService systemHistoryLogsService,
                              IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
                               IRepositoryQuery<OfferPriceMaster> OfferPriceMasterRepositoryQuery,
                              IHttpContextAccessor _httpContext ) : base(_httpContext)
        {
            OfferPriceMasterRepositoryCommand = _OfferPriceMasterRepositoryCommand;
            OfferPriceDetailsRepositoryCommand = _OfferPriceDetailsRepositoryCommand;
            OfferPriceDetailsRepositoryQuery = _OfferPriceDetailsRepositoryQuery;
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            httpContext = _httpContext;
            HistoryInvoiceService = _HistoryInvoiceService;
            GeneralAPIsService = _GeneralAPIsService;
            this.systemHistoryLogsService = systemHistoryLogsService;
            this.OfferPriceMasterRepositoryQuery = OfferPriceMasterRepositoryQuery;
        }
        public async Task<ResponseResult> DeleteTempInvoice(SharedRequestDTOs.Delete parameter , int invoiceTypeId , string invoiceType,int mainInvoiceTypeId)
        {

                var reasonOfNotDeletedAr = "";
                var reasonOfNotDeletedEn = "";


            var invoicesListWOtype = OfferPriceMasterRepositoryQuery.TableNoTracking.Include(a => a.OfferPriceDetails)//.ThenInclude(a => a.Items.Serials)
                       .Where(e => parameter.Ids.First() == e.InvoiceId).ToList();

            var invoicesList = invoicesListWOtype.Where(h => h.InvoiceTypeId == invoiceTypeId || h.InvoiceTypeId == mainInvoiceTypeId);
            if (invoicesListWOtype.Any() && invoicesList.Count() <= 0)
            {
                return new ResponseResult { Result = Result.CanNotBeDeleted, ErrorMessageAr = " لا يمكن الحذف المستند نظرا لاختلاف ال API ", ErrorMessageEn = "you can not delete this rec as  Api is different " };
            }
           
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

                // invoice ids from request
                var idS = invoicesList.Select(a => a.InvoiceId).ToList();
            
                   if (invoicesList.Count() > 0)
                   {
                       if (invoicesList.First().InvoiceSubTypesId == (int)SubType.OfferPriceAccridited)
                       {
                           reasonOfNotDeletedAr = ErrorMessagesAr.CantDeletedInvAccredited;
                           reasonOfNotDeletedEn = ErrorMessagesEn.CantDeletedInvAccredited;
                       }
                       else if (invoicesList.First().InvoiceSubTypesId == (int)SubType.OfferPriceDeleted)
                       {
                           reasonOfNotDeletedAr = ErrorMessagesAr.CantDeletedInvDeleted;
                           reasonOfNotDeletedEn = ErrorMessagesEn.CantDeletedInvDeleted;
                       }
                  
                       if (!string.IsNullOrEmpty(reasonOfNotDeletedAr))
                           return new ResponseResult()
                           {
                               Result = Result.CanNotBeUpdated,
                               ErrorMessageAr = reasonOfNotDeletedAr,
                               ErrorMessageEn = reasonOfNotDeletedEn
                           };
                   }
            
            List<int> invoiceCantDeleted = new List<int>();
               


                // update old invoice , change isdelete = true
                var invoiceWillDelete = invoicesList.Where(a => a.InvoiceSubTypesId==(int)SubType.OfferPriceUnAccepted).ToList();
            if (invoiceWillDelete.Count() == 0)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    ErrorMessageEn = (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedEn : ErrorMessagesEn.CanNotDeleteSomeInvoices),
                    ErrorMessageAr = (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedAr : ErrorMessagesAr.CanNotDeleteSomeInvoices)
                };

            invoiceWillDelete.Select(e => {
                    e.IsDeleted = true;
                    e.InvoiceSubTypesId = (int)SubType.OfferPriceDeleted;
                    return e;
                }).ToList();

                try
                {
                    var res = await OfferPriceMasterRepositoryCommand.UpdateAsyn(invoiceWillDelete);

                }
                catch (Exception e)
                {

                    throw;
                }
                try
                {
                
                    foreach (var item in invoiceWillDelete)
                    {

                        OfferPriceMasterRepositoryCommand.StartTransaction();

                        item.BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString();

                        // validation of total result
                        var invoice = new OfferPriceMaster()
                        {
                            BranchId = item.BranchId,
                            EmployeeId=item.EmployeeId,
                            Code = item.Code,
                            InvoiceType = item.BranchId + "-" + invoiceType + "-" + item.Code,
                            InvoiceTypeId = invoiceTypeId,
                            BookIndex = item.BookIndex.Trim(),
                            InvoiceDate = item.InvoiceDate,
                            //InvoiceId=item.InvoiceId,
                            StoreId = item.StoreId,
                            PersonId = item.PersonId,
                            Notes = item.Notes.Trim(),
                            DiscountType = item.DiscountType,
                            Net = item.Net,
                            ActiveDiscount = item.ActiveDiscount,
                            BrowserName = httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString(),
                            Paid = item.Paid,
                            VirualPaid = item.VirualPaid,
                            ApplyVat = item.ApplyVat,
                            TotalAfterDiscount = item.TotalAfterDiscount,
                            InvoiceSubTypesId = item.InvoiceSubTypesId,
                            PaymentType = item.PaymentType,
                            ParentInvoiceCode = item.InvoiceType,
                            PriceWithVat = item.PriceWithVat,
                            Remain = item.Remain,
                            TotalDiscountRatio = item.TotalDiscountRatio,
                            TotalDiscountValue = item.TotalDiscountValue,
                            TotalPrice = item.TotalPrice,
                            TotalVat = item.TotalVat,
                            IsDeleted = true,
                            SalesManId = item.SalesManId,
                            transferStatus = item.transferStatus
                        };
                        invoice.InvoiceTransferType = invoice.InvoiceType;
                        //purchase.ParentInvoiceCode = item.ParentInvoiceCode;
                        invoice.Serialize = Convert.ToDouble(GeneralAPIsService.CreateSerializeOfInvoice(invoiceTypeId, item.InvoiceId, invoice.BranchId).ToString());

                        var saved = await OfferPriceMasterRepositoryCommand.AddAsync(invoice);
                        //   var saved=  await OfferPriceMasterRepositoryCommand.SaveAsync();
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice master" + invoice.InvoiceType };

                        var details = OfferPriceDetailsRepositoryQuery.FindAll(q => q.InvoiceId == item.InvoiceId);
                        // save details of items in the invoice
                        var OfferPriceDetailsList = new List<OfferPriceDetails>();
                        foreach (var itemDetail in details)
                        {
                            var OfferPriceDetails = new OfferPriceDetails();
                            OfferPriceDetails.InvoiceId = invoice.InvoiceId;
                            OfferPriceDetails.ItemId = itemDetail.ItemId;
                            OfferPriceDetails.Price = itemDetail.Price;
                            OfferPriceDetails.Quantity = itemDetail.Quantity;
                            OfferPriceDetails.TotalWithSplitedDiscount = itemDetail.TotalWithSplitedDiscount;// OfferPriceDetails.PurchasePrice * OfferPriceDetails.Quantity;
                            OfferPriceDetails.UnitId = itemDetail.UnitId;
                            OfferPriceDetails.ItemTypeId = itemDetail.ItemTypeId;
                            OfferPriceDetails.Signal = GeneralAPIsService.GetSignal(invoice.InvoiceTypeId);
                            OfferPriceDetails.ConversionFactor = itemDetail.ConversionFactor;
                            OfferPriceDetails.VatRatio = itemDetail.VatRatio;
                            OfferPriceDetails.VatValue = itemDetail.VatValue;
                            OfferPriceDetails.indexOfItem = itemDetail.indexOfItem;
                           
                            OfferPriceDetailsList.Add(OfferPriceDetails);

                            // invoice.TotalPrice += OfferPriceDetails.Total;
                        }
                       


                        saved = await OfferPriceDetailsRepositoryCommand.AddAsync(OfferPriceDetailsList);
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };



                        //Add serialize 
                        saved = GeneralAPIsService.addSerialize(invoice.Serialize, item.InvoiceId, invoiceTypeId, invoice.BranchId);
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in add serialize of " + invoice.InvoiceType };

               

                        // to update TotalPrice after calculations
                        await OfferPriceMasterRepositoryCommand.UpdateAsyn(invoice);
                        var mainSerialize = Math.Truncate(invoice.Serialize);
                        OfferPriceMasterRepositoryCommand.CommitTransaction();
                     

                        //    HistoryInvoiceService.HistoryOfferPriceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, "A", null, invoice.BookIndex, invoice.Code
                        //, invoice.InvoiceDate, invoice.InvoiceId, invoice.InvoiceType, invoiceType, (int)SubType.Nothing, invoice.IsDeleted, invoice.ParentInvoiceCode, invoice.Serialize, invoice.StoreId, invoice.TotalPrice);


                        HistoryInvoiceService.HistoryInvoiceMaster(item.BranchId, item.Notes, item.BrowserName, "D", null, item.BookIndex, item.Code
                 , item.InvoiceDate, item.InvoiceId, item.InvoiceType, item.InvoiceTypeId, (int)SubType.Nothing, item.IsDeleted, item.InvoiceType, item.Serialize, item.StoreId, item.TotalPrice);

                    }
                }
                catch (Exception e)
                {


                    throw;
                }

                //            var setReceipt = await ReceiptsInvoice.DeleteInvoicesReceipts(Ids.ToList());

                SystemActionEnum systemActionEnum = new SystemActionEnum();
              
                if (invoiceTypeId == (int)DocumentType.OfferPrice)
                    systemActionEnum = SystemActionEnum.deleteOfferPrice;
                else if (invoiceTypeId == (int)DocumentType.PurchaseOrder)
                    systemActionEnum = SystemActionEnum.deletePurchaseOrder;
                if (systemActionEnum != null)
                    await systemHistoryLogsService.SystemHistoryLogsService(systemActionEnum);
                return new ResponseResult
                {
                    Result = Result.Success,
                    ErrorMessageEn = (invoiceCantDeleted.Count() > 0 ? (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedEn : ErrorMessagesEn.CanNotDeleteSomeInvoices) : ErrorMessagesEn.DeletedSuccessfully),
                    ErrorMessageAr = (invoiceCantDeleted.Count() > 0 ? (invoiceCantDeleted.Count() == 1 ? reasonOfNotDeletedAr : ErrorMessagesAr.CanNotDeleteSomeInvoices) : ErrorMessagesAr.DeletedSuccessfully)

                };

            }



        }
    }
