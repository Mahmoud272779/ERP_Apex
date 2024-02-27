using App.Application.Handlers.Profit.CalculatProfit;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.SignalRHub;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Setup;
using App.Infrastructure.settings;
using Castle.Core.Internal;
using Dapper;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace App.Application.Handlers.Profit
{
    public class PrepareDataForProfit : IPrepareDataForProfit
    {
        private readonly IRepositoryQuery<EditedItems> EditedItemQuery;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;//1
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemCardUnitQuery;
        private readonly IRepositoryQuery<GLJournalEntry> jounralEnteryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand;
        private readonly IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardParts> InvStpItemCardPartsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> InvStpItemCardUnitsQuery;
        private readonly IRepositoryCommand<InvoiceDetails> InvoiceDetailsCommand;
        private readonly IRepositoryCommand<EditedItems> editedItemsCommand;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettingsQuary;
        private readonly iUserInformation Userinformation;
        private readonly IHubContext<NotificationHub> _hub;
        private UserInformationModel userData;
        private readonly IMemoryCache _memoryCache;
        private readonly IRoundNumbers roundNumbers;



        public PrepareDataForProfit(IRepositoryQuery<EditedItems> editedItemQuery,
            IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery,
            IRepositoryQuery<InvStpItemCardUnit> ItemCardUnitQuery,
            IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery,
            IRepositoryCommand<InvoiceDetails> invoiceDetailsCommand,
            IRepositoryCommand<EditedItems> EditedItemsCommand,
                                  iUserInformation Userinformation,
                                  IHubContext<NotificationHub> hub,
                                  IRepositoryQuery<InvStpItemCardParts> invStpItemCardPartsQuery,
                                  IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitsQuery,
                                  IRepositoryQuery<GLJournalEntry> jounralEnteryQuery,
                                  IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery,
                                  IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand,
                                  IRepositoryQuery<GLPurchasesAndSalesSettings> gLPurchasesAndSalesSettingsQuary,
                                  IRoundNumbers roundNumbers,
                                  IMemoryCache memoryCache)
        {
            EditedItemQuery = editedItemQuery;
            invoiceMasterQuery = InvoiceMasterQuery;
            itemCardUnitQuery = ItemCardUnitQuery;
            invoiceDetailsQuery = InvoiceDetailsQuery;
            this.Userinformation = Userinformation;
            InvoiceDetailsCommand = invoiceDetailsCommand;
            editedItemsCommand = EditedItemsCommand;
            _hub = hub;
            InvStpItemCardPartsQuery = invStpItemCardPartsQuery;
            InvStpItemCardUnitsQuery = invStpItemCardUnitsQuery;
            this.jounralEnteryQuery = jounralEnteryQuery;
            this.journalEntryDetailsQuery = journalEntryDetailsQuery;
            this.journalEntryDetailsCommand = journalEntryDetailsCommand;
            GLPurchasesAndSalesSettingsQuary = gLPurchasesAndSalesSettingsQuary;
            userData = Userinformation.GetUserInformation().Result;
            this.roundNumbers = roundNumbers;
            _memoryCache = memoryCache;
        }

        public async Task<ResponseResult> PreparingDataForProfit(int BranchIdd, bool calcJournals, int? itemId = null)
        {   
            IPCS pcs = new PCS();
            SqlConnection con = new SqlConnection(EditedItemQuery.connectionString());



            int counter = 0;
            var currentCompanyLogin = userData.companyLogin;

            var totalItemData = EditedItemQuery.TableNoTracking
                .Where(h => !Lists.itemNotRegular.Contains(h.type) && itemId == null ? 1 == 1 : h.itemId == itemId);
            var ItemData = totalItemData.Take(1).ToList();
            
            if (!ItemData.Any())
                return new ResponseResult {Result = Result.Success,Id = (int)Result.profitDone };
            try
            {
                List<CompositeData> compositeItem = new List<CompositeData>();
                List<EditedItems> ReqularItems = ItemData.Where(h => h.type != (int)ItemTypes.Composite).ToList();
                ProfiteRequest profitParameter = new ProfiteRequest();

                var editedInvoices = new List<InvoiceDetails>();

                //CompositeItems
               if(ItemData.FirstOrDefault().type == (int)ItemTypes.Composite)
               {
                   counter++;
                   compositeItem = GetCompositeFromItsComponent(ItemData.FirstOrDefault().itemId, ItemData.FirstOrDefault().serialize, ItemData.FirstOrDefault().BranchID, compositeItem);
               }
                else
                {
                    foreach (var item in ReqularItems)
                    {
                        counter++;

                        //begin collecting data
                        lastInvoiceEditedDTO lastinvoiceEdit = Getlastinvoice(item);
                        List<InvoicesData> invoices = GetInvoiceData(new List<int> { item.itemId }, item.sizeId, item.serialize, item.BranchID);
                        List<InvStpItemCardUnit> itemCard = itemCardUnitQuery.TableNoTracking.Where(h => h.ItemId == item.itemId).OrderBy(h => h.ConversionFactor).ToList();
                        profitParameter.Invoices = invoices;
                        profitParameter.ItemDataDTO = itemCard;
                        profitParameter.lastDataDTO = lastinvoiceEdit;
                        //calculte cost regular items
                        List<InvoicesData> Updatedinvoices = pcs.calculateItemsProfite(profitParameter);
                        //the last step update invoices after calculate cost for them
                        if (Updatedinvoices.Count > 0)
                        {
                            editedInvoices.AddRange(await UpdateInvoiceData(Updatedinvoices));

                        }

                    }
                }

                
                if (!editedInvoices.Any() && !ItemData.Where(h => h.type == (int)ItemTypes.Composite).Any())
                {
                    await editedItemsCommand.DeleteAsync(c => c.itemId == ItemData.FirstOrDefault().itemId && c.BranchID == ItemData.FirstOrDefault().BranchID);
                    return new ResponseResult() { Result = Result.Success, ErrorMessageAr = " Done ", TotalCount = totalItemData.Count() };
                }
                InvoiceDetailsCommand.StartTransaction();
                bool isupdated = await InvoiceDetailsCommand.UpdateAsyn(editedInvoices);
                con.Open();
                if (editedInvoices.Any())
                {
                    var query = $"update InvoiceMaster set lastTimeProfitCalced = GETDATE() where InvoiceId in ({string.Join(",", editedInvoices.Select(c => c.InvoiceId))})";
                    con.Execute(query);
                }
                //editedItemsCommand.RemoveRange(ItemData);
                
                //var editedItedmSaved = await editedItemsCommand.SaveAsync();
                var editedItedmSaved = await editedItemsCommand.DeleteAsync(c => c.itemId == ItemData.FirstOrDefault().itemId && c.BranchID == ItemData.FirstOrDefault().BranchID);
                InvoiceDetailsCommand.CommitTransaction();
                if(editedInvoices.Any())
                    if (!isupdated || !editedItedmSaved)
                        InvoiceDetailsCommand.Rollback();


                //calculat composte item cost and update its invoices
                if (compositeItem != null && compositeItem.Count > 0)
                {
                    bool isCalculated = await CalcualateCompositeItemProfit(compositeItem); // Calculate composite items
                    await editedItemsCommand.DeleteAsync(c => c.itemId == ItemData.FirstOrDefault().itemId && c.BranchID == ItemData.FirstOrDefault().BranchID);
                }

                return new ResponseResult() { Result = Result.Success, ErrorMessageAr = " Done " ,TotalCount = totalItemData.Count()};

            }
            catch (Exception e)
            {
                InvoiceDetailsCommand.Rollback();
                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = e.Message, ErrorMessageEn = e.Message};

            }
            finally
            {
                con.Close();
                con.Dispose();
            }

        }
        private double Percentage(int count, double totalCount)
        {
            double percentage = 1;
            if (totalCount != 0)
            {
                percentage = ((count / totalCount) * 100);
            }
            return percentage;
        }
        private async Task UpdateJournalEntery(List<EditedItems> itemData, string currentCompanyLogin)
        {
            List<JEnteryInvoicedata> invoices = GetInvoiceDataForJEntery(itemData.Select(a => a.itemId).ToList(), itemData.Min(h => h.serialize));

            int counter = 0;
            string[] usersSignalRConnectionId = null;
            var listOf_JournalEntries = new List<GLJournalEntryDetails>();
            var listOfJounrals = jounralEnteryQuery.TableNoTracking.Where(c => invoices.Select(x => x.invoiceId).ToArray().Contains(c.InvoiceId.Value));
            var purchasesAndSalesSettings = GLPurchasesAndSalesSettingsQuary.TableNoTracking.ToList();

            foreach (var invoice in invoices)
            {
                counter++;
                usersSignalRConnectionId = _memoryCache.Get<List<SignalRCash>>(defultData.SignalRKey).Where(x => x.CompanyLogin == currentCompanyLogin).Select(x => x.connectionId).ToArray();
                await _hub.Clients.Clients(usersSignalRConnectionId).SendAsync(defultData.ProfitProgress, new porgressData { Notes = "Update Invoices by Profit Data", percentage = Percentage(counter, invoices.Count), Count = counter, totalCount = invoices.Count, status = Aliases.ProgressStatus.InProgress });
                var journalentryID = await listOfJounrals.Where(q => q.InvoiceId == invoice.invoiceId).Select(a => a.Id).FirstOrDefaultAsync();
                listOf_JournalEntries =  await SetJournalEntery(invoice, journalentryID, purchasesAndSalesSettings);
            }
            journalEntryDetailsCommand.AddRange(listOf_JournalEntries);
            await journalEntryDetailsCommand.SaveAsync();
        }

        private async Task<bool> CalcualateCompositeItemProfit(List<CompositeData> compositeItem)
        {
            foreach (CompositeData item in compositeItem)
            {
                List<CompositeForDataProfit> componentItems = GetAllCompositeComponent(item.itemId);
                var InvoiceData = GetInvoiceData(new List<int> { item.itemId }, item.size, item.serialize, item.branchId);//get all invoices for each composite item 

                await CalculatecompostCost(componentItems, InvoiceData, item.branchId); // calculate cost after each invoice 
            }
            return true;
        }

        private async Task CalculatecompostCost(List<CompositeForDataProfit> componentItems, List<InvoicesData> invoicesData, int BranchId)
        {
            List<InvoicesData> NewInvoicesData = new List<InvoicesData>();
            List<InvoiceDetails> invoicesForUpdate = new List<InvoiceDetails>();
            foreach (var invoice in invoicesData)
            {
                var ComponentsCost = GetLastCostOfAllComponent(componentItems, invoice.Serialize, BranchId);//get all last cost of each component of this composite item
                invoice.Cost = ComponentsCost.Sum(h => (h.Cost * h.Quantity * h.Factor));
                invoicesForUpdate = await UpdateInvoiceData(new List<InvoicesData> { invoice });
            }
            await InvoiceDetailsCommand.UpdateAsyn(invoicesForUpdate);

        }
        //last cost for composit items
        private List<CompositeForDataProfit> GetLastCostOfAllComponent(List<CompositeForDataProfit> componentItems, double serialize, int BranchId)
        {
            List<CompositeForDataProfit> ComponentItemsCost = componentItems.ToList();
            foreach (var item in ComponentItemsCost)
            {
                var Itemcost = GetInvoicesCost(item.PartId, 0, serialize, BranchId);
                item.Cost = Itemcost.Cost;
                item.AvgPrice = item.AvgPrice;
                #region MyRegion
                //CompositeForDataProfit componentItemCost = new CompositeForDataProfit
                //{
                //    PartId = item.PartId,
                //    ItemId = item.ItemId,
                //    AvgPrice = Itemcost.AvgPrice,
                //    Cost = Itemcost.Cost,
                //    Quantity = item.Quantity,
                //    UnitId = item.UnitId,

                //};
                //ComponentItemsCost.Add(componentItemCost);
                #endregion

            }
            return ComponentItemsCost;
        }

        private List<CompositeForDataProfit> GetAllCompositeComponent(int itemId)
        {
            List<CompositeForDataProfit> itemsCompositeData = new List<CompositeForDataProfit>();
            var items = InvStpItemCardPartsQuery.TableNoTracking.Where(h => h.ItemId == itemId).ToList();
            var itemunits = InvStpItemCardUnitsQuery.TableNoTracking.Where(h => items.Select(a => a.PartId).Contains(h.ItemId));
            foreach (var item in items)
            {
                itemsCompositeData.Add(new CompositeForDataProfit
                {
                    PartId = item.PartId,
                    Quantity = item.Quantity,
                    UnitId = item.UnitId,
                    ItemId = item.ItemId,
                    Factor = itemunits.Where(a => a.UnitId == item.UnitId).Select(a => a.ConversionFactor).FirstOrDefault()

                });


            }
            return itemsCompositeData;
        }

        private List<CompositeData> GetCompositeFromItsComponent(int item, double serialize, int BranchId, List<CompositeData> listOfCompositItem)
        {
            // need check again 
            var items = InvStpItemCardPartsQuery.TableNoTracking.Where(h => h.ItemId == item)
                .Select(h => new CompositeData { itemId = h.ItemId, serialize = serialize, size = 0, branchId = BranchId })
                .ToList();
            foreach (var obj in items)
            {
                if (!listOfCompositItem.Where(h => h.itemId == obj.itemId && h.size == obj.size && h.branchId == BranchId).Any())
                {
                    listOfCompositItem.Add(obj);
                }
            }
            return listOfCompositItem;
        }
        //update invoice with new cost
        private async Task<List<InvoiceDetails>> UpdateInvoiceData(List<InvoicesData> updatedinvoices)
        {
            var invoices = await invoiceDetailsQuery.FindByAsyn((a => updatedinvoices.Select(h => h.InvoiceId).Contains(a.InvoiceId)
                                                                  && a.ItemId == updatedinvoices.FirstOrDefault().ItemId
                                                                  && a.SizeId == (updatedinvoices.FirstOrDefault().SizeId == 0 ? null : updatedinvoices.FirstOrDefault().SizeId)));
            var listOfInvoices = new List<InvoiceDetails>();
            foreach (var item in invoices)
            {
                item.Cost = updatedinvoices.Where(a => a.InvoiceId == item.InvoiceId).OrderByDescending(a => a.ItemIndex).Select(a => a.Cost).FirstOrDefault();
                item.AvgPrice = updatedinvoices.Where(a => a.InvoiceId == item.InvoiceId).OrderByDescending(a => a.ItemIndex).Select(a => a.AvgPrice).FirstOrDefault();
                item.ConversionFactor = updatedinvoices.Where(a => a.InvoiceId == item.InvoiceId && a.ItemIndex == item.indexOfItem).Select(a => a.factor).FirstOrDefault();
                listOfInvoices.Add(item);
            }

            return listOfInvoices;
            //return await InvoiceDetailsCommand.UpdateAsyn(invoices);
            //return await InvoiceDetailsCommand.SaveChanges() == 1;
        }
        //get last invoice
        private lastInvoiceEditedDTO Getlastinvoice(EditedItems item)
        {
            #region
            //var Lastinvoices1 = invoiceDetailsQuery.TableNoTracking.Include(a=>a.InvoicesMaster)
            //    .Where(h => h.ItemId == item.itemId && h.SizeId == (item.sizeId == 0 ? null : item.sizeId) && h. InvoicesMaster.Serialize < item.serialize)                           
            //    //.Select(a=>new { Signal = a.Signal, Cost = a.Cost, AvgPrice = a.AvgPrice, invoiceId = a.InvoiceId, Quantity = a.Quantity })
            //  .OrderByDescending(a =>a.InvoiceId) 
            //  .GroupBy(a=>1)
            //    .Select(h => new lastInvoiceEditedDTO
            //    {
            //        QTyOfPurchase = h.FirstOrDefault().Signal > 0 ? h.Sum(a => (a.Signal * a.Quantity)) : 0,
            //        Cost = h.FirstOrDefault().Cost,
            //        AvgPrice = h.FirstOrDefault().AvgPrice,
            //        invoiceId= h.FirstOrDefault().InvoiceId,    
            //        LastQTY = h.Sum(a => (a.Signal * a.Quantity))
            //    });
            #endregion
            //var AllLastinvoicestest = invoiceDetailsQuery.TableNoTracking

            //      .Where(h => h.ItemId == item.itemId
            //      && h.InvoicesMaster.BranchId == item.BranchID
            //      && h.SizeId == (item.sizeId == 0 ? null : item.sizeId)
            //      && h.InvoicesMaster.Serialize < item.serialize)
            //      ;
            //factor how know if purchase or not 
            lastInvoiceEditedDTO AllLastinvoices = invoiceDetailsQuery.TableNoTracking

                .Where(h =>
                   h.ItemId == item.itemId
                && h.Quantity > 0
                && h.InvoicesMaster.BranchId == item.BranchID
                && h.SizeId == (item.sizeId == 0 ? null : item.sizeId)
                && h.InvoicesMaster.Serialize < item.serialize)
                .GroupBy(a => 1)
                .Select(h => new lastInvoiceEditedDTO
                {
                    QTyOfPurchase = h.Sum(a => (a.Signal > 0 && a.InvoicesMaster.IsDeleted == false) ? (a.Signal * a.Quantity) : 0),
                    LastQTY = h.Sum(a => (a.Signal * a.Quantity * a.ConversionFactor))// ( a.Units.CardUnits==null ? 1: a.Units.CardUnits.Select(h=>h.ConversionFactor).FirstOrDefault())))
                })
                .FirstOrDefault();
            SelectionData Lastinvoices = GetInvoicesCost(item.itemId, item.sizeId, item.serialize, item.BranchID);


            if (AllLastinvoices != null && Lastinvoices != null)
            {
                AllLastinvoices.Cost = Lastinvoices.Cost;
                AllLastinvoices.AvgPrice = Lastinvoices.AvgPrice;
                AllLastinvoices.invoiceId = Lastinvoices.invoiceId;
            }
            //var objectQuery1 = (AllLastinvoices).ToQueryString();
            //var objectQuery = (Lastinvoices).ToQueryString();

            return AllLastinvoices ?? new lastInvoiceEditedDTO() { Cost = 0, AvgPrice = 0 };
        }

        private SelectionData GetInvoicesCost(int itemId, int sizeId, double serialize, int branchId)
        {

            SelectionData Lastinvoices = invoiceDetailsQuery.TableNoTracking
                .Where(h => h.ItemId == itemId
                && h.SizeId == (sizeId == 0 ? null : sizeId)
                && h.InvoicesMaster.Serialize < serialize
                && h.InvoicesMaster.IsDeleted == false
                && h.Quantity > 0
                && h.InvoicesMaster.BranchId == branchId)
                .OrderByDescending(a => a.InvoicesMaster.Serialize)
                .Select(h => new SelectionData
                {
                    Cost = h.Cost,
                    AvgPrice = h.AvgPrice,
                    invoiceId = h.InvoiceId,
                })
                .FirstOrDefault();

            return Lastinvoices ?? new SelectionData() { Cost = 0, AvgPrice = 0 };
        }

        public async Task<List<GLJournalEntryDetails>> SetJournalEntery(JEnteryInvoicedata invoice,int journalentryID,List<GLPurchasesAndSalesSettings> purchasesAndSalesSettings)
        {
            try
            {



                if (journalentryID == 0)
                    return null;
                await journalEntryDetailsCommand.DeleteAsync(a => a.JournalEntryId == journalentryID && a.isCostSales == true);

                List<GLJournalEntryDetails> journalEntryDetails = new List<GLJournalEntryDetails>();

                journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
                {
                    JournalEntryId = journalentryID,


                    FinancialAccountId = purchasesAndSalesSettings
                                          .Where(h => h.RecieptsType == (int)DocumentType.Purchase)
                                          .Where(h => h.branchId == userData.CurrentbranchId)
                                          .Select(a => a.FinancialAccountId)
                                          .FirstOrDefault(),// المشتريات 



                    Credit = Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost, // total cost
                    Debit = !Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost,
                    DescriptionAr = purAndSalesSettingNames.SalesCostAr,
                    DescriptionEn = purAndSalesSettingNames.SalesCostEn,
                    isCostSales = true

                });
                journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
                {
                    JournalEntryId = journalentryID,

                    FinancialAccountId =  purchasesAndSalesSettings
                                            .Where(h => h.RecieptsType == (int)DocumentType.Inventory && h.branchId == userData.CurrentbranchId)
                                            .Select(a => a.FinancialAccountId)
                                            .FirstOrDefault(),// تكلفه المبيعات  


                    Debit = Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost, // total cost
                    Credit = !Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost,
                    DescriptionAr = purAndSalesSettingNames.SalesCostAr,
                    DescriptionEn = purAndSalesSettingNames.SalesCostEn,
                    isCostSales = true

                });

                //journalEntryDetailsCommand.AddRange(journalEntryDetails);

                //var saved = await journalEntryDetailsCommand.SaveAsync();

                return journalEntryDetails;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public List<InvoicesData> GetInvoiceData(List<int> itemId, int? sizeId, double serialize, int BranchId)
        {   
            //need refactor

            var _invoices = invoiceDetailsQuery.TableNoTracking
                   .Include(h => h.Units)
                   .ThenInclude(a => a.CardUnits)
                   .Include(c=> c.InvoicesMaster)
                   .Where(a => itemId.Contains(a.ItemId)
                   && a.InvoicesMaster.BranchId == BranchId
                   && a.ItemTypeId != (int)ItemTypes.Note
                   && a.Quantity > 0
                   && (sizeId == null ? 1 == 1 : (a.SizeId == (sizeId == 0 ? null : sizeId)))
                   && a.InvoicesMaster.Serialize >= serialize
                   && a.InvoicesMaster.IsDeleted == false)
                   .OrderBy(a => a.InvoicesMaster.Serialize)
                   .ToList();
            var invoices = new List<InvoicesData>();
            foreach (var item in _invoices)
            {
                var one_invoices = new InvoicesData();
                one_invoices.InvoiceId = item.InvoiceId;
                one_invoices.Serialize = item.InvoicesMaster.Serialize;
                one_invoices.ItemId = item.ItemId;

                var factor = item.Units.CardUnits.Where(c => c.UnitId == item.UnitId && c.ItemId == item.ItemId);

                one_invoices.factor = factor.Any() ? factor.Max(a => a.ConversionFactor) : item.ConversionFactor;
                one_invoices.Price = item.InvoicesMaster.InvoiceTypeId != (int)DocumentType.ReturnPurchase ? ((item.TotalWithSplitedDiscount + item.OtherAdditions) / item.Quantity) : ((item.TotalWithSplitedDiscount - item.OtherAdditions) / item.Quantity);
                one_invoices.AvgPrice = item.AvgPrice;
                one_invoices.Cost = item.Cost;
                one_invoices.QTY = (item.Quantity * item.Signal);
                one_invoices.ItemIndex = item.indexOfItem;
                one_invoices.InvoiceType = item.InvoicesMaster.InvoiceTypeId;
                one_invoices.SizeId = item.SizeId;
                one_invoices.PriceWithVate = item.InvoicesMaster.PriceWithVat;
                one_invoices.VatRatio = item.VatRatio;
                invoices.Add(one_invoices);
            }


            //var invoices = _invoices.Select(h => new InvoicesData
            //       {
            //           InvoiceId = h.InvoiceId,
            //           Serialize = h.InvoicesMaster.Serialize,
            //           ItemId = h.ItemId,
            //           factor = (h.Units.CardUnits.Where(c => c.UnitId == h.UnitId && c.ItemId == h.ItemId).Max(a => a.ConversionFactor)),
            //           //Price = (h.TotalWithSplitedDiscount / h.Quantity) + h.OtherAdditions,//add addition price 
            //           Price = h.InvoicesMaster.InvoiceTypeId != (int)DocumentType.ReturnPurchase ? ((h.TotalWithSplitedDiscount + h.OtherAdditions) / h.Quantity) : ((h.TotalWithSplitedDiscount - h.OtherAdditions) / h.Quantity),//add addition price 
            //           AvgPrice = h.AvgPrice,
            //           Cost = h.Cost,
            //           QTY = (h.Quantity * h.Signal),
            //           ItemIndex = h.indexOfItem,
            //           InvoiceType = h.InvoicesMaster.InvoiceTypeId,
            //           SizeId = h.SizeId,
            //           PriceWithVate = (h.InvoicesMaster.PriceWithVat),
            //           VatRatio = h.VatRatio,
            //       }).ToList();

            return invoices;

        }


        public List<JEnteryInvoicedata> GetInvoiceDataForJEntery(List<int> itemIds, double serialize)
        {
            //need branches
            var invoices = invoiceMasterQuery.TableNoTracking
                .Include(h => h.InvoicesDetails)
                .Where(h => h.InvoicesDetails.Where(w => itemIds.Contains(w.ItemId)).Select(a => a.InvoiceId).Contains(h.InvoiceId) && h.Serialize >= serialize && h.IsDeleted == false)
                .Where(h => Lists.SalesTransaction.Contains(h.InvoiceTypeId))
                .OrderBy(a => a.Serialize)
                .Select(h => new JEnteryInvoicedata
                {
                    invoiceId = h.InvoiceId,
                    cost = roundNumbers.GetRoundNumber(h.InvoicesDetails.Sum(a => a.Cost * a.Quantity * a.ConversionFactor)),
                    serial = h.Serialize,
                    invoiceType = h.InvoiceTypeId
                }).ToList();



            return invoices;

        }
    }
}
