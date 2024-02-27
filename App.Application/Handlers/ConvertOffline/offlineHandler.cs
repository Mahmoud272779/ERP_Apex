using App.Application.Handlers.Invoices.OfferPrice.AddOfferPrice;
using App.Application.Handlers.Test._1;
using App.Application.Services.Reports.StoreReports.Sales;
using Castle.Core.Configuration;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using App.Application.Helpers.Service_helper.offlinePOS;
using App.Application.Helpers.Service_helper.FileHandler;
using static App.Domain.Enums.Enums;
using App.Domain.Models.Request.Store.Reports.Purchases;
using DocumentFormat.OpenXml.Drawing;
using Path = System.IO.Path;
using App.Domain.Models.Security.Authentication.Response.Store;
using System.Security.Policy;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Aliases = App.Application.Helpers.Aliases;
using DocumentType = App.Domain.Enums.Enums.DocumentType;
using SkiaSharp;
using Microsoft.Extensions.Configuration;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using App.Domain.Entities.Setup;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using App.Application.Handlers.Transfer;
using Dapper;
using App.Application.Services.Process;
using System.Collections.Immutable;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Domain.Models.Security.Authentication.Request.Reports;

namespace App.Application.Handlers.ConvertOffline
{
    public class offlineHandler : IRequestHandler<offlineRequest, Tuple<bool, List<string>>>
    {
     //   int invoiceTypeId = (int)DocumentType.POS;
        public string invoiceMaster = "invoiceMaster.txt";
        public string invoiceDetails = "invoiceDetails.txt";
        public string PaymentsMethods = "PaymentsMethods.txt";
        public DataTable invoiceMasterDT = new DataTable();
        public DataTable invoiceDetailsDT = new DataTable();
        public DataTable PaymentsMethodsDT = new DataTable();
        public List<int> storeList = new List<int>();
        public List<int> itemList = new List<int>();
        public List<int> itemUnitsList = new List<int>();
        public List<int> branchList = new List<int>();
        public List<string> invoicesCantBeSaved = new List<string>();
        public List<string> savedOfflineCode = new List<string>();
        List<InvoiceMaster> allMaster = new List<InvoiceMaster>();
        List<InvoicePaymentsMethods> invoicePaymentMethodsList = new List<InvoicePaymentsMethods>();
        List<PurchasesJournalEntryIntegrationDTO> journalEntryList = new List<PurchasesJournalEntryIntegrationDTO>();

        private readonly IEditedItemsService editedItemsService;

        private readonly IFileHandler fileHandler;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterRepositoryCommand;
        private readonly IRepositoryCommand<InvoiceDetails> InvoiceDetailsRepositoryCommand;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IRepositoryCommand<InvoicePaymentsMethods> PaymentsMethodsCommand;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryCommand<InvoiceMasterHistory> _InvoiceMasterHistoryRepositoryCommand;
        private readonly IConfiguration configuration;
        private readonly IRepositoryQuery<InvStpStores> storesQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;
        private readonly IRepositoryQuery<GLBranch> branchQuery;
        private readonly IMediator mediator;


        public offlineHandler(IFileHandler fileHandler, IRepositoryQuery<InvoiceMaster> invoiceMasterRepositoryQuery,
            IGeneralAPIsService generalAPIsService, IRepositoryCommand<InvoiceMaster> invoiceMasterRepositoryCommand,
            IRepositoryCommand<InvoiceDetails> invoiceDetailsRepositoryCommand
           , IRepositoryCommand<InvoicePaymentsMethods> paymentsMethodsCommand,
            iUserInformation iUserInformation, IRepositoryCommand<InvoiceMasterHistory> invoiceMasterHistoryRepositoryCommand,
            IConfiguration configuration, IRepositoryQuery<InvStpStores> storesQuery,
            IRepositoryQuery<InvStpItemCardMaster> itemsQuery, IRepositoryQuery<GLBranch> branchQuery,
            IMediator mediator, IEditedItemsService editedItemsService, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery)
        {
            this.fileHandler = fileHandler;
            InvoiceMasterRepositoryQuery = invoiceMasterRepositoryQuery;
            GeneralAPIsService = generalAPIsService;
            InvoiceMasterRepositoryCommand = invoiceMasterRepositoryCommand;
            InvoiceDetailsRepositoryCommand = invoiceDetailsRepositoryCommand;
            PaymentsMethodsCommand = paymentsMethodsCommand;
            _iUserInformation = iUserInformation;
            _InvoiceMasterHistoryRepositoryCommand = invoiceMasterHistoryRepositoryCommand;
            this.configuration = configuration;
            this.storesQuery = storesQuery;
            this.itemsQuery = itemsQuery;
            this.branchQuery = branchQuery;
            this.mediator = mediator;
            this.editedItemsService = editedItemsService;
            this.itemUnitsQuery = itemUnitsQuery;
        }

        /*      public async Task<string> Handle(offlineRequest request, CancellationToken cancellationToken)
                  {
                      // 1. extract files
                      // 2.  Add invoice master
                      //     2.1  read invoiceMaster
                      //     2.2  convert file to datatable
                      //     2.3  create code and add to db row by row

                      //extract files
                      string[] files =await  extractFiles(request);
                      if(files.Count()>0)
                      {
                          List<string> OfflineCodePosList = new List<string>();
                           // select invoicemaster file
                          string file = files.Where(a => a.Contains(invoiceMaster)).FirstOrDefault();

                          if(!string.IsNullOrEmpty( file))
                          {

                                 // read invoiceMaster
                                  string fileData = readFile(file);
                                  // convert file to datatable
                                  DataTable dt = new DataTable();
                                  dt = getInvoiceDT(fileData);
                          //DataView view = new DataView(dt);
                          //DataTable distinctValues = view.ToTable(true, "StoreId");

                          //  create code and add to db row by row
                          try
                          {
                              OfflineCodePosList = await addInvoicesMaster(dt);
                              if (OfflineCodePosList.Count() == 0)
                                  return "Faild in invoice master";
                          }
                          catch (Exception e)
                          {

                              throw e;
                          }

                          }

                          if(OfflineCodePosList.Count>0)
                          {

                              file = files.Where(a => a.Contains(invoiceDetails)).FirstOrDefault();
                              if (!string.IsNullOrEmpty(file))
                              {
                                  // read invoiceMaster
                                  string fileData = readFile(file);
                                  // convert file to datatable
                                  DataTable dt = new DataTable();
                                  dt = getInvoiceDT(fileData);
                              //  create code and add to db row by row
                              try
                              {
                                  bool done = await addInvoiceDetails(dt, OfflineCodePosList );
                                  if (!done)
                                      return "Faild in invoice details";
                              }
                              catch (Exception e)
                              {
                                  throw e;
                              }



                              }
                              file = files.Where(a => a.Contains(PaymentsMethods)).FirstOrDefault();
                              if (!string.IsNullOrEmpty(file))
                              {
                                  // read invoiceMaster
                                  string fileData = readFile(file);
                                  // convert file to datatable
                                  DataTable dt = new DataTable();
                                  dt = getInvoiceDT(fileData);
                              //  create code and add to db row by row
                              try
                              {
                                  bool done = await addPaymentMethods(dt, OfflineCodePosList);
                                  if (!done)
                                      return "Faild in payment methods";
                              }
                              catch (Exception e)
                              {

                                  throw e;
                              }


                              }
                          try
                          {
                              var res = mediator.Send(new addReceiptsForOfflinePosRequest()
                              { invoiceMasters = allMaster, invoicesPaymentMethods = invoicePaymentMethodsList });

                          }
                          catch (Exception e)
                          {

                              throw e;
                          }

                          }


                      }


                      return "success";
                  }

      */
        public void ConvertFiles(string[] files)
        {
            foreach(var file in files)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    string fileData = readFile(file);
                    // convert file to datatable
                    var dt = getInvoiceDT(fileData , true );
                    if (file.Contains(invoiceMaster))
                        invoiceMasterDT = dt;
                    else if (file.Contains(invoiceDetails))
                        invoiceDetailsDT = dt;
                    else if (file.Contains(PaymentsMethods))
                        PaymentsMethodsDT = dt;
                }
            }
        }
        public async Task<List<string>> GetInvoicesCantBeSaved()
        {
            List<string> invoices = new List<string>();

            //   3.1 Get  distinct Lists of all
            storeList = storeList.Distinct().ToList();
            itemList = itemList.Distinct().ToList();
            itemUnitsList = itemUnitsList.Distinct().ToList();
            branchList = branchList.Distinct().ToList();
            //var list = invoiceMasterDT.Rows.OfType<DataRow>()
            //                                     .Select(dr => dr.Field<int>("StoreId")).ToList();

            //   3.2 Select not exist in db 
            var storesExist = storesQuery.TableNoTracking.Select(a => a.Id).Where(a => storeList.Contains(a)).ToList();
            var storesNotExist = storeList.Except(storesExist);
            storeList = storesNotExist.ToList();

            var itemsExist = itemsQuery.TableNoTracking.Select(a => a.Id).Where(a => itemList.Contains(a)).ToList();

            var itemsNotExist = itemList.Except(itemsExist);

            var itemUnitsExist = itemUnitsQuery.TableNoTracking.Select(a => new{a.ItemId, a.UnitId} )
                                 .Where(a => itemList.Contains(a.ItemId)).ToList();

            var itemUnitsInvoice = invoiceDetailsDT.Rows.OfType<DataRow>().Select(a =>
                                    new
                                    {
                                        ItemId = int.Parse(a.Field<string>("ItemId").ToString()),
                                        UnitId = int.Parse(a.Field<string>("UnitId").ToString()),
                                        invoiceCode = a.Field<string>("invoiceCode")
                                    });
            var itemsUnitNotExist = itemUnitsInvoice.Select(a=> new { a.ItemId,a.UnitId}).Except(itemUnitsExist).ToList();
            if (itemsUnitNotExist.Count() > 0)
            {
                foreach (var item in itemsUnitNotExist)
                {
                    var invoiceCode = itemUnitsInvoice.Where (a=> a.ItemId==item.ItemId && a.UnitId==item.UnitId).Select(a=>a.invoiceCode);
                    invoices.AddRange(invoiceCode);
                }
            }

            //var itemUnitsNotExist = invoiceDetailsDT.Rows.OfType<DataRow>()
            //                 .Where(dr => itemUnitsExist.Where(a=>a.ItemId==int.Parse(dr.Field<string>("ItemId").ToString())
            //                                              && a.UnitId == int.Parse(dr.Field<string>("UnitId").ToString())).Any())
            //                //|| branchList.Contains(int.Parse(dr.Field<string>("UnitId").ToString())))
            //                .Select(dr =>new { invoiceCode = dr.Field<string>("invoiceCode") 
            //                     , ItemId = int.Parse(dr.Field<string>("ItemId").ToString()) ,
            //                    UnitId = int.Parse(dr.Field<string>("UnitId").ToString())
            //                } ).ToList();
            //   invoices.AddRange(itemUnitsNotExist);

            //  itemUnitsList = itemUnitsNotExist.ToList();
            itemList = itemsNotExist.ToList();

            var branchExist = branchQuery.TableNoTracking.Select(a => a.Id).Where(a => branchList.Contains(a)).ToList();
            var branchNotExist = branchList.Except(branchExist);
            branchList = branchNotExist.ToList();

            //   3.3 Get invoiceCode that not exist
            // var invoiceNotSaved = new List<string>();
            var list = invoiceMasterDT.Rows.OfType<DataRow>()
                              .Where(dr=> storeList.Contains(int.Parse(dr.Field<string>("StoreId").ToString()))
                                     || branchList.Contains(int.Parse(dr.Field<string>("BranchID").ToString())))
                             .Select(dr => dr.Field<string>("invoiceCode")).ToList();
            invoices.AddRange(list);
              list = invoiceDetailsDT.Rows.OfType<DataRow>()
                             .Where(dr => itemList.Contains(int.Parse(dr.Field<string>("ItemId").ToString())))
                             //|| branchList.Contains(int.Parse(dr.Field<string>("UnitId").ToString())))
                            .Select(dr => dr.Field<string>("invoiceCode")).ToList();
            invoices.AddRange(list);

          

             return invoices.Distinct().ToList();
        }
       public async Task<Tuple<bool, List<string>>> Handle(offlineRequest request, CancellationToken cancellationToken)
        {
            // 1. extract files
            // 2. Convert files to datatable
            // 3. get distinct data in  of stores , categories , items ,units and paymentMethods t
            //   3.1 Get  distinct Lists of all
            //   3.2 Select not exist in db 
            //   3.3 Get invoiceCode that not exist
            // 4. add invoices


            // 1. extract files
            string[] files = await extractFiles(request);
            if (files.Count() > 0)
            {
                // 2. Convert files to datatable
                ConvertFiles(files);
                //  3.get distinct data
                 invoicesCantBeSaved =await GetInvoicesCantBeSaved();
                List<string> OfflineCodePosList = new List<string>();

                  savedOfflineCode = InvoiceMasterRepositoryQuery.TableNoTracking
                                       .Where(a => !string.IsNullOrEmpty(a.CodeOfflinePOS)).Select(a => a.CodeOfflinePOS).ToList();

                //  create code and add to db row by row
                OfflineCodePosList = await addInvoicesMaster(invoiceMasterDT);
                if(invoicesCantBeSaved.Count()>0 && OfflineCodePosList.Count() == 0)
                    return new Tuple<bool, List<string>>(true, invoicesCantBeSaved);

                //if (OfflineCodePosList.Count() == 0)
                //        return new Tuple<bool, List<string>>(false,null);
           

                if (OfflineCodePosList.Count > 0)
                {

                   
                        bool done = await addInvoiceDetails(invoiceDetailsDT, OfflineCodePosList);
                        if (!done)
                            return new Tuple<bool, List<string>>(false,null);

 
                          done = await addPaymentMethods(PaymentsMethodsDT, OfflineCodePosList);
                        if (!done)
                            return new Tuple<bool, List<string>>(false, null);


                    try
                    {
                        var res = mediator.Send(new addReceiptsForOfflinePosRequest()
                        { invoiceMasters = allMaster, invoicesPaymentMethods = invoicePaymentMethodsList });

                    }
                    catch (Exception e)
                    {

                        throw e;
                    }

                }


            }

            //string note = "";
            //if (invoicesCantBeSaved.Count() > 0)
            //    note = string.Join(",",invoicesCantBeSaved) + " can not be saved as some data has been deleted";
            return new Tuple<bool, List<string>>( true , invoicesCantBeSaved);
        }
        
        public async Task<bool> addPaymentMethods(DataTable dt, List<string> OfflineCodePosList)
        {
            var invoicesIdList = InvoiceMasterRepositoryQuery.TableNoTracking
                             .Where(a => OfflineCodePosList.Contains(a.CodeOfflinePOS))
                             .Select(a => new { a.CodeOfflinePOS, a.InvoiceId });
            bool done = false;
            var paymentMethod = new InvoicePaymentsMethods();

            for (int a = 0; a < dt.Rows.Count; a++)
            {
                if (dt.Rows[a]["invoiceCode"].ToString() == "")
                    continue;
                if (invoicesCantBeSaved.Contains(dt.Rows[a]["invoiceCode"].ToString()) || savedOfflineCode.Contains(dt.Rows[a]["invoiceCode"].ToString()))
                    continue;

                    paymentMethod = new InvoicePaymentsMethods();
               
                 paymentMethod.CodeOfflinePOS = dt.Rows[a]["invoiceCode"].ToString();
                string[] offlineCode = paymentMethod.CodeOfflinePOS.Split('-');
                paymentMethod.BranchId = int.Parse(offlineCode[1]);
                paymentMethod.InvoiceId = invoicesIdList.Where(a => a.CodeOfflinePOS == paymentMethod.CodeOfflinePOS).First().InvoiceId;
                paymentMethod.Cheque= dt.Rows[a]["Cheque"].ToString();
                paymentMethod.Value=double.Parse( dt.Rows[a]["Value"].ToString());
                paymentMethod.PaymentMethodId= int.Parse( dt.Rows[a]["PaymentMethodId"].ToString());
                invoicePaymentMethodsList.Add(paymentMethod);
            }

              PaymentsMethodsCommand.AddRangeAsync(invoicePaymentMethodsList);

              //PaymentsMethodsCommand.SaveChanges();

            return true ;
        }


        //public InvoiceMasterHistory addInvoiceHistory(int employeeId, int branchId
        //     , int Code , DateTime InvoiceDate , int InvoiceId , string InvoiceType ,int InvoiceTypeId
        //    , string ParentInvoiceCode,double Serialize ,int StoreId , double TotalPrice , int invoiceSubType )
        public InvoiceMasterHistory addInvoiceHistory(int employeeId , InvoiceMaster invoice)
         {
            var history = new InvoiceMasterHistory()
            {
                employeesId = employeeId,
                LastDate = invoice.InvoiceDate,
                LastAction = "A",
                LastTransactionAction = "A",
                BranchId = invoice.BranchId,
                Code = invoice.Code,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceId = invoice.InvoiceId,
                InvoiceType = invoice.InvoiceType,
                InvoiceTypeId = invoice.InvoiceTypeId,
                 ParentInvoiceCode = invoice.ParentInvoiceCode,
                Serialize = invoice.Serialize,
                StoreId = invoice.StoreId,
                TotalPrice = invoice.TotalPrice,
                BrowserName = "Offline POS",
                SubType = invoice.InvoiceSubTypesId
                // employees=userInfo.em

            };

            return history;
        }
        public async Task<bool> addInvoiceDetails(DataTable dt,List<string> OfflineCodePosList )
        {
            var invoicesIdList = InvoiceMasterRepositoryQuery.TableNoTracking
                              .Where(a => OfflineCodePosList.Contains(a.CodeOfflinePOS))
                              .Select(a => new { a.CodeOfflinePOS, a.InvoiceId , a.BranchId,a.Serialize,a.InvoiceTypeId});
            var ItemsHaveNoEffictOnInvoice = new List<int> { (int)ItemTypes.Additives, (int)ItemTypes.Note };

            bool done = false;
            var invoiceDetailsList=new List<InvoiceDetails>();
            var invoiceDetails = new InvoiceDetails();
            var editedItemsList = new List<editedItemsParameter>();
            var editedItems = new editedItemsParameter();
            var compositeItems = new List<CompositeItemsRequest>();
            for (int a =0; a<dt.Rows.Count; a++)
            {
                if (dt.Rows[a]["invoiceCode"].ToString() == "")
                    continue;
                if (invoicesCantBeSaved.Contains(dt.Rows[a]["invoiceCode"].ToString()) || savedOfflineCode.Contains(dt.Rows[a]["invoiceCode"].ToString()))
                    continue;
                
                invoiceDetails = new InvoiceDetails();
                invoiceDetails.CodeOfflinePOS = dt.Rows[a]["invoiceCode"].ToString();
                var invoiceMaster = invoicesIdList.Where(a => a.CodeOfflinePOS == invoiceDetails.CodeOfflinePOS).First();

                invoiceDetails.InvoiceId = invoiceMaster.InvoiceId;
                invoiceDetails.ItemId =int.Parse( dt.Rows[a]["ItemId"].ToString());
                invoiceDetails.ItemTypeId = int.Parse(dt.Rows[a]["ItemTypeId"].ToString());

                if (invoiceDetails.ItemTypeId == (int)ItemTypes.Note)
                {
                    invoiceDetails.UnitId = null;
                    invoiceDetails.Units = null;
                    invoiceDetails.SizeId = null;
                    invoiceDetails.Sizes = null;
                }
                else
                {
                    invoiceDetails.UnitId = int.Parse(dt.Rows[a]["UnitId"].ToString());
                    invoiceDetails.SizeId = int.Parse(dt.Rows[a]["SizeId"].ToString());
                    if(invoiceDetails.SizeId==0)
                    {
                        invoiceDetails.SizeId = null;
                        invoiceDetails.Sizes = null;
                    }
                }
                invoiceDetails.Signal = dt.Rows[a]["invoiceCode"].ToString().Contains("POS_R") ?1:-1;

                invoiceDetails.Quantity = double.Parse(dt.Rows[a]["Quantity"].ToString());
                invoiceDetails.ReturnQuantity = double.Parse(dt.Rows[a]["Quantity_R"].ToString())/double.Parse(dt.Rows[a]["ConversionFactor"].ToString());
                invoiceDetails.Price = double.Parse(dt.Rows[a]["Price"].ToString());
                invoiceDetails.TotalWithOutSplitedDiscount = double.Parse(dt.Rows[a]["Total"].ToString());
                invoiceDetails.TotalWithSplitedDiscount =
                    (double.Parse(dt.Rows[a]["Price"].ToString()) * double.Parse(dt.Rows[a]["Quantity"].ToString())) - double.Parse(dt.Rows[a]["SplitedDiscountValue"].ToString());
             // invoiceDetails.ExpireDate = Convert.ToDateTime( dt.Rows[a]["ExpireDate"].ToString());
                invoiceDetails.DiscountValue = double.Parse(dt.Rows[a]["DiscountValue"].ToString());
                invoiceDetails.DiscountRatio = double.Parse(dt.Rows[a]["DiscountRatio"].ToString());
                invoiceDetails.VatRatio = double.Parse(dt.Rows[a]["VatRatio"].ToString());
                invoiceDetails.VatValue = double.Parse(dt.Rows[a]["VatValue"].ToString());
                invoiceDetails.SplitedDiscountValue = double.Parse(dt.Rows[a]["SplitedDiscountValue"].ToString());
                invoiceDetails.SplitedDiscountRatio = double.Parse(dt.Rows[a]["SplitedDiscountRatio"].ToString());
                invoiceDetails.AutoDiscount = double.Parse(dt.Rows[a]["AutoDiscount"].ToString());
                invoiceDetails.ConversionFactor = double.Parse(dt.Rows[a]["ConversionFactor"].ToString());
                invoiceDetails.indexOfItem = int.Parse(dt.Rows[a]["IndexOfItem"].ToString());
                invoiceDetailsList.Add(invoiceDetails);
                if(invoiceMaster.InvoiceTypeId==(int)DocumentType.POS)
                {
                    if (!ItemsHaveNoEffictOnInvoice.Contains(invoiceDetails.ItemTypeId))
                    {

                        editedItems = new editedItemsParameter()
                        {
                            branchId = invoiceMaster.BranchId,
                            serialize = invoiceMaster.Serialize,
                            invoiceType = invoiceMaster.InvoiceTypeId,
                            itemId = invoiceDetails.ItemId,
                            sizeId = 0,
                            itemTypeId = invoiceDetails.ItemTypeId
                        };
                        editedItemsList.Add(editedItems);
                    }
                }
                if(invoiceDetails.ItemTypeId==(int)ItemTypes.Composite)
                {
                    compositeItems.Add(new CompositeItemsRequest()
                    {invoiceId=invoiceDetails.InvoiceId, itemId=invoiceDetails.ItemId,
                        unitId=invoiceDetails.UnitId.Value , quantity=invoiceDetails.Quantity , indexOfItem=invoiceDetails.indexOfItem});
                }
                // set details of invoice in JournalEntrydetails
                journalEntryList.Where(a => a.invoiceId == invoiceDetails.InvoiceId).ToList()
                 .ForEach(a => a.InvDetails.Add(invoiceDetails));
            }
            // call in offline mode here and call it in online mode at GeneralAPIsService to use it there 
            var itemData_ = GeneralAPIsService.GetComponentsOfCompositItem(compositeItems);
            var ComponentsItem = GeneralAPIsService.setCompositItem(compositeItems,itemData_);
            var compositItems = invoiceDetailsList.Where(a => a.ItemTypeId == (int)ItemTypes.Composite);
      
            for (var item =0;item< ComponentsItem.Count();item++)
            {
                var parentItem = compositItems.Where(a => a.ItemId == ComponentsItem[item].parentItemId && a.indexOfItem== ComponentsItem[item].IndexOfItem
                && a.InvoiceId == ComponentsItem[item].InvoiceId).First();
                invoiceDetails = new InvoiceDetails();
                invoiceDetails.ItemId = ComponentsItem[item].ItemId;
                invoiceDetails.UnitId = ComponentsItem[item].UnitId;
                invoiceDetails.parentItemId = ComponentsItem[item].parentItemId;
                invoiceDetails.Quantity = ComponentsItem[item].Quantity;// *parentItem.Quantity;
                if (parentItem.Signal == -1)// if pos invoice set the return quantity if exist
                {
                    var partQty = itemData_.First(a => a.PartId == ComponentsItem[item].ItemId
                              && a.ItemId == ComponentsItem[item].parentItemId 
                              && a.indexOfItem == ComponentsItem[item].IndexOfItem
                              && a.invoiceId == ComponentsItem[item].InvoiceId).Quantity;
                    invoiceDetails.ReturnQuantity = (parentItem.ReturnQuantity  * partQty);
                }
                 
                invoiceDetails.Price = ComponentsItem[item].Price;
                invoiceDetails.ConversionFactor = ComponentsItem[item].ConversionFactor;

                var total = ComponentsItem[item].Quantity * ComponentsItem[item].Price;
                invoiceDetails.TotalWithOutSplitedDiscount = total;
                invoiceDetails.TotalWithSplitedDiscount = total;
                
                invoiceDetails.CodeOfflinePOS = parentItem.CodeOfflinePOS;
                invoiceDetails.Signal = parentItem.Signal;
                invoiceDetails.InvoiceId= parentItem.InvoiceId;
                  
                invoiceDetailsList.Add(invoiceDetails);
                var invoiceMaster = invoicesIdList.Where(a => a.CodeOfflinePOS == invoiceDetails.CodeOfflinePOS).First();

                if (invoiceMaster.InvoiceTypeId == (int)DocumentType.POS)
                {
                    if (!ItemsHaveNoEffictOnInvoice.Contains(invoiceDetails.ItemTypeId))
                    {

                        editedItems = new editedItemsParameter()
                        {
                            branchId = invoiceMaster.BranchId,
                            serialize = invoiceMaster.Serialize,
                            invoiceType = invoiceMaster.InvoiceTypeId,
                            itemId = invoiceDetails.ItemId,
                            sizeId = 0,
                            itemTypeId = invoiceDetails.ItemTypeId
                        };
                        editedItemsList.Add(editedItems);
                    }
                }

                //// set details of invoice in JournalEntrydetails
                //journalEntryList.Where(a => a.invoiceId == invoiceDetails.InvoiceId).ToList()
                // .ForEach(a => a.InvDetails.Add(invoiceDetails));
            }


            invoicesCantBeSaved = invoicesCantBeSaved.Distinct().ToList();
            invoiceDetailsList.RemoveAll(a => invoicesCantBeSaved.Contains(a.CodeOfflinePOS));
            InvoiceDetailsRepositoryCommand.AddRange(invoiceDetailsList);
            
          done = await InvoiceDetailsRepositoryCommand.SaveAsync();

            // edited items  -> profit
            if (editedItemsList.Count > 0)
              await  addEditedItems(editedItemsList);

          // journal entry
            var result = await mediator.Send(new addJournalEntryForOfflinePosRequest() { data = journalEntryList });

            return done;
        }

        public async Task<List<string>> addInvoicesMaster(DataTable dt)
        {
            List<string> OfflineCodePosList = new List<string>();

            if (dt.Rows.Count > 0)
            {
                var historyList = new List<InvoiceMasterHistory>();
                var journalEntry  = new PurchasesJournalEntryIntegrationDTO();
             
                InvoiceMaster invoice = new InvoiceMaster();
                var codePos = new List<CodeOfPOSOfflineReq>();
                var allMasterUpdated = new List<InvoiceMaster>();
                for (int a = 0; a < dt.Rows.Count; a++)
                {
                    invoice = new InvoiceMaster();
                    if (dt.Rows[a]["invoiceCode"].ToString() == "")
                        continue;
                    if (invoicesCantBeSaved.Contains(dt.Rows[a]["invoiceCode"].ToString()) || savedOfflineCode.Contains(dt.Rows[a]["invoiceCode"].ToString()))
                    {
                        //invoicesCantBeSaved.Add(dt.Rows[a]["invoiceCode"].ToString());
                        continue;
                    }
                    int empId = 1;
                    if (dt.Columns.Contains("emp_id"))
                        empId = int.Parse(dt.Rows[a]["emp_id"].ToString());
                    invoice.EmployeeId = empId;
                    OfflineCodePosList.Add(dt.Rows[a]["invoiceCode"].ToString());
                    invoice.PersonId = int.Parse(dt.Rows[a]["PersonId"].ToString());
                    invoice.CodeOfflinePOS = dt.Rows[a]["invoiceCode"].ToString();
                    invoice.BookIndex = dt.Rows[a]["BookIndex"].ToString();
                    invoice.InvoiceDate = Convert.ToDateTime(dt.Rows[a]["InvoiceDate"]);
                    invoice.StoreId = int.Parse(dt.Rows[a]["StoreId"].ToString());
                    invoice.Notes = dt.Rows[a]["Notes"].ToString();
                    invoice.TotalPrice = double.Parse(dt.Rows[a]["TotalPrice"].ToString());
                    invoice.PriceListId = int.Parse(dt.Rows[a]["PriceListId"].ToString());
                    invoice.TotalDiscountValue = double.Parse(dt.Rows[a]["TotalDiscountValue"].ToString());
                    invoice.TotalDiscountRatio = double.Parse(dt.Rows[a]["TotalDiscountRatio"].ToString());
                    invoice.Net = double.Parse(dt.Rows[a]["Net"].ToString());
                    invoice.Paid = double.Parse(dt.Rows[a]["Paid"].ToString());
                    invoice.Remain = invoice.Net - invoice.Paid;// double.Parse(dt.Rows[a]["Remain"].ToString());
                    invoice.VirualPaid = double.Parse(dt.Rows[a]["VirualPaid"].ToString());
                    invoice.TotalAfterDiscount = double.Parse(dt.Rows[a]["TotalAfterDiscount"].ToString());
                    invoice.TotalVat = double.Parse(dt.Rows[a]["TotalVat"].ToString());
                    invoice.DiscountType = int.Parse(dt.Rows[a]["DiscountType"].ToString());
                    invoice.ActiveDiscount = (dt.Rows[a]["ActiveDiscount"].ToString() == "1" ? true : false);
                    invoice.ApplyVat = (dt.Rows[a]["ApplyVat"].ToString() == "1" ? true : false);
                    invoice.PriceWithVat = (dt.Rows[a]["PriceWithVat"].ToString() == "1" ? true : false);
                    invoice.RoundNumber = int.Parse(dt.Rows[a]["RoundNumber"].ToString());
                    invoice.InvoiceSubTypesId = int.Parse(dt.Rows[a]["InvoiceSubTypesId"].ToString());
                    invoice.InvoiceTypeId = int.Parse(dt.Rows[a]["recType"].ToString());
                   
                    if(invoice.InvoiceTypeId==(int)DocumentType.ReturnPOS)
                         invoice.IsReturn = true;

                    invoice.ActualNet = double.Parse(dt.Rows[a]["ActualNet"].ToString());
                   // invoiceTypeId = invoice.InvoiceTypeId;
                    string[] offlineCode = invoice.CodeOfflinePOS.Split('-');
                    invoice.BranchId = int.Parse(offlineCode[1]);

                    invoice.Code = Autocode(invoice.BranchId, invoice.InvoiceTypeId);
                   
                    if (invoice.Paid == 0)
                    {
                        invoice.PaymentType = (int)PaymentType.Delay;
                    }
                    if (invoice.Paid < invoice.Net && invoice.Paid != 0)
                    {
                        invoice.PaymentType = (int)PaymentType.Partial;
                    }
                    if (invoice.Paid >= invoice.Net)
                    {
                        invoice.PaymentType = (int)PaymentType.Complete;
                    }

                    string invoiceType = "";
                    if(int.Parse(dt.Rows[a]["recType"].ToString())==(int)DocumentType.POS) 
                        invoice.InvoiceType = invoice.BranchId.ToString() + "-" + Aliases.InvoicesCode.POS + "-" + invoice.Code;
                    else if (int.Parse(dt.Rows[a]["recType"].ToString()) == (int)DocumentType.ReturnPOS)
                        invoice.InvoiceType = invoice.BranchId.ToString() + "-" + Aliases.InvoicesCode.ReturnPOS + "-" + invoice.Code;
                    invoice.InvoiceTransferType = invoice.InvoiceType;
                    codePos.Add(new CodeOfPOSOfflineReq() { OnlineCode = invoice.InvoiceType, OfflineCode = dt.Rows[a]["invoiceCode"].ToString() });

                    var test= dt.Rows[a]["ParentInvoiceCode"].ToString();
                    invoice.ParentInvoiceCode = (invoice.InvoiceTypeId == (int)DocumentType.POS ?
                                        invoice.InvoiceType :
                                        codePos.Where(e => e.OfflineCode == dt.Rows[a]["ParentInvoiceCode"].ToString()).FirstOrDefault().OnlineCode);

                    int mainInvoiceId = 0;
                    if(invoice.InvoiceTypeId == (int)DocumentType.ReturnPOS )
                        mainInvoiceId = codePos.Where(e => e.OfflineCode == dt.Rows[a]["ParentInvoiceCode"].ToString()).FirstOrDefault().InvoiceId;
                    invoice.Serialize = Convert.ToDouble(GeneralAPIsService.CreateSerializeOfInvoice(invoice.InvoiceTypeId, mainInvoiceId, invoice.BranchId).ToString());
                    InvoiceMasterRepositoryCommand.AddWithoutSaveChanges(invoice);
                    var saved = await InvoiceMasterRepositoryCommand.SaveAsync();

                 //   if(invoice.InvoiceTypeId == (int)DocumentType.POS)
                    allMaster.Add(invoice);

                    if (!saved)
                    {
                        OfflineCodePosList = new List<string>();
                        return OfflineCodePosList;
                    }
                    if (mainInvoiceId == 0)
                        mainInvoiceId = invoice.InvoiceId;
                    saved = GeneralAPIsService.addSerialize(invoice.Serialize, mainInvoiceId, invoice.InvoiceTypeId, invoice.BranchId);
                    if (!saved)
                    {
                        OfflineCodePosList = new List<string>();
                        return OfflineCodePosList;
                    }
                    // set parent id of invoice
                    codePos.Where(a => a.OnlineCode == invoice.InvoiceType).Select(a => a.InvoiceId = invoice.InvoiceId).ToList();
                      allMasterUpdated = allMaster;
                    if (invoice.InvoiceTypeId==(int)DocumentType.ReturnPOS)
                    {
                        allMasterUpdated.Where(a =>a.InvoiceTypeId==(int)DocumentType.POS && a.InvoiceType == invoice.ParentInvoiceCode).Select(a => a.IsReturn = true).ToList();
                        allMasterUpdated.Where(a => a.InvoiceTypeId == (int)DocumentType.POS && a.InvoiceType == invoice.ParentInvoiceCode).Select(a =>  a.InvoiceSubTypesId = invoice.InvoiceSubTypesId ).ToList();
                    }
                     var history = addInvoiceHistory(empId, invoice);
                    historyList.Add(history);

                    // set journal entry master
                    journalEntryList.Add(setJournalEntryMaster(invoice));
                }
                
              await  InvoiceMasterRepositoryCommand.UpdateAsyn(allMasterUpdated);
               // await InvoiceMasterRepositoryCommand.SaveAsync();

                _InvoiceMasterHistoryRepositoryCommand.AddRange(historyList);
                await _InvoiceMasterHistoryRepositoryCommand.SaveAsync();

             }
            return OfflineCodePosList;
        }
        public int Autocode(int BranchId, int invoiceType)
        {
            var Code = 1;
            Code = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == invoiceType && a.BranchId == BranchId);

            if (Code != null)
                Code++;

            return Code;
        }
        public async Task<string[]> extractFiles(offlineRequest request)
        {
            try
            {
                var userInfo = await _iUserInformation.GetUserInformation();
                string folderOfDay = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                // local
       /*   var OfflinePosFolder =Path.Combine( Environment.CurrentDirectory,"OfflinePos", userInfo.companyLogin, folderOfDay);
                if (!Directory.Exists(OfflinePosFolder))
                    Directory.CreateDirectory(OfflinePosFolder);

                string actulePath = Path.Combine(OfflinePosFolder, request.AttachedFile.FileName);
                if (File.Exists(actulePath))
                    File.Delete(actulePath);
                using (Stream fileStream = new FileStream(actulePath, FileMode.Create))
                {
                    await request.AttachedFile.CopyToAsync(fileStream);

                }

               var extrctedPath = Path.Combine(OfflinePosFolder, request.AttachedFile.FileName.Replace(".zip", ""));
           */
                // on server
               
       string zipFilePath = fileHandler.UploadFile(request.AttachedFile, Path.Combine("OfflinePOS", userInfo.companyLogin, folderOfDay), request.AttachedFile.FileName);

                    var path = configuration["ApplicationSetting:FilesRootPath"];
                    string filePath = Path.Combine("OfflinePOS", userInfo.companyLogin, folderOfDay, request.AttachedFile.FileName);
                    string actulePath = Path.Combine(path, filePath);
                    string extrctedPath = actulePath.Replace(".zip", "");  
            
                ZipFile.ExtractToDirectory(actulePath, extrctedPath);
            
               //string[] files = Directory.GetFiles(Path.Combine(zipFilePath, request.AttachedFile.FileName.Replace(".zip", "")));
                string[] files = Directory.GetFiles(extrctedPath);
                return files;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string readFile(string path)
        {
            string file = File.ReadAllText(path);
            return file;
        }
        public DataTable getInvoiceDT(string query, bool GetList)//, ref List<int> IdsList1, ref List<int> IdsList2)
        {
            DataTable dt = new DataTable();
            dt = convertDataTableToString.convertStringToDataTable(query,GetList ,ref storeList, ref itemList,ref branchList,ref itemUnitsList);
            return dt;
        }
        public DataTable getInvoiceDT(string query)
        {
            List<int> IdsList1=new List<int>();
            List<int> IdsList2=new List<int>();
            return getInvoiceDT(query, false);//,ref IdsList1,ref IdsList2);
        }

        public async Task<bool> addEditedItems (List<editedItemsParameter> itemsForEditedItems)
        {
            List<editedItemsParameter> itemsForEditedItems_ = itemsForEditedItems.OrderBy(a=>a.serialize).DistinctBy(a => a.itemId).ToList();
            //var itemsList = itemsForEditedItems.GroupBy(a => new { a.itemId, a.sizeId }).ToList();
            //itemsList .Select(a=> new { serialize=a.Min(e => e.serialize),
            //      itemId= a.Key.itemId,
            //      sizeId= a.Key.sizeId ,
            //        branchId= a.FirstOrDefault().branchId  , itemTypeId=a.FirstOrDefault().itemTypeId});
          
             await editedItemsService.AddItemInEditedItem(itemsForEditedItems_);


            return true;
        }

        public PurchasesJournalEntryIntegrationDTO setJournalEntryMaster(InvoiceMaster invoice)
        {
            var journalEntryMaster = new PurchasesJournalEntryIntegrationDTO()
            {
                discount=invoice.TotalDiscountValue,
                DocType =(DocumentType)invoice.InvoiceTypeId,
                invDate=invoice.InvoiceDate,
                InvoiceCode = invoice.InvoiceType,
                invoiceId = invoice.InvoiceId,
                isAllowedVAT = invoice.ApplyVat,
                isIncludeVAT = invoice.PriceWithVat,
                net = invoice.Net,
                personId = invoice.PersonId,
                total = invoice.TotalPrice,
                serialize=invoice.Serialize,
                VAT = invoice.TotalVat,
                isDelete=false,
                isUpdate=false,
                branchId=invoice.BranchId,
                employeeId=invoice.EmployeeId
            };

            return journalEntryMaster;
        }



    
    }
}
