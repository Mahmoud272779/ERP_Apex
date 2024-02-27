using App.Application.Handlers.Invoices.PriceLists;
using App.Application.Helpers;
using App.Application.Services.HelperService;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Migrations;
using App.Infrastructure.settings;
using Azure.Core;
using DocumentFormat.OpenXml.Drawing.Diagrams;
 using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.Invoices
{
    public class CalculationSystemService : ICalculationSystemService
    {

        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsQuery;
        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterQuery;

        private readonly IRepositoryQuery<OfferPriceDetails> OfferPriceDetailsQuery;

        private InvoiceResultCalculateDto resultTotalDiscountItems;
        private ReturnNotes ReturnNotes; // بيحدد اذا كان هيتم اعادة حساب الخصم او لا مع توضيح السبب فى الملاحظة
        private SettingsOfInvoice SettingsOfInvoice; // اعدادات الفاتورة سواء جديده او قديمة
        private ResponseResult ResponseResult;
        private IRepositoryQuery<InvPersons> personQuery;
        private IRepositoryQuery<InvStpItemCardUnit> ItemsQuery;
        private readonly iUserInformation Userinformation;
        private readonly IRoundNumbers roundNumbers;
        private readonly IMediator mediator;
        public CalculationSystemService(IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery,
             IRepositoryQuery<InvGeneralSettings> generalSettings, IRepositoryQuery<InvPersons> personQuery, IRepositoryQuery<InvStpItemCardUnit> itemsQuery,
             IRoundNumbers roundNumbers, iUserInformation Userinformation, IRepositoryQuery<OfferPriceMaster> OfferPriceMasterQuery, IRepositoryQuery<OfferPriceDetails> offerPriceDetailsQuery, IMediator mediator)
        {
            InvoiceMasterQuery = invoiceMasterQuery;
            GeneralSettings = generalSettings;
            this.personQuery = personQuery;
            this.Userinformation = Userinformation;
            ItemsQuery = itemsQuery;
            InvoiceDetailsQuery = invoiceDetailsQuery;
            this.roundNumbers = roundNumbers;
            this.OfferPriceMasterQuery = OfferPriceMasterQuery;
            OfferPriceDetailsQuery = offerPriceDetailsQuery;
            this.mediator = mediator;
        }

        public async Task<ResponseResult> CalculationOfInvoice(CalculationOfInvoiceParameter parameter)
        {
           
            await StartCalculation(parameter ,false);
            return ResponseResult;
        }

        #region Functions

        public async Task<InvoiceResultCalculateDto> StartCalculation(CalculationOfInvoiceParameter parameter)
        {
            InvoiceResultCalculateDto res= await StartCalculation(parameter, false);
            return res;
        }
        public async Task<InvoiceResultCalculateDto> StartCalculation(CalculationOfInvoiceParameter parameter , bool forReturn)
        {
            // get saved settings in edit mood
            if (parameter.InvoiceTypeId == (int)DocumentType.OfferPrice || parameter.InvoiceTypeId ==(int)DocumentType.PurchaseOrder)
            {
                var invoiceExist = await OfferPriceMasterQuery.GetByAsync(a => a.InvoiceId == parameter.InvoiceId || a.InvoiceType == parameter.ParentInvoice);
                if (invoiceExist != null) // invoice is old
                {
                    parameter.invoiceExistSetting = new invoiceExistSettings()
                    {
                        //ActiveDiscount = invoiceExist.ActiveDiscount,
                        ApplyVat = invoiceExist.ApplyVat,
                        PriceWithVat = invoiceExist.PriceWithVat,
                        RoundNumber = invoiceExist.RoundNumber
                    };
                }
            }
            else
            {
                var invoiceExist = await InvoiceMasterQuery.GetByAsync(a => a.InvoiceId == parameter.InvoiceId ||(!string.IsNullOrEmpty( parameter.ParentInvoice)? a.InvoiceType == parameter.ParentInvoice:false));
                if (invoiceExist != null) // invoice is old
                {
                    parameter.invoiceExistSetting = new invoiceExistSettings();

                    parameter.invoiceExistSetting.ActiveDiscount = invoiceExist.ActiveDiscount; // هحتاجها لو مرتجع غير كده هاخد اللى فى الاعدادات 
                    parameter.invoiceExistSetting.ApplyVat = invoiceExist.ApplyVat;
                    parameter.invoiceExistSetting.PriceWithVat = invoiceExist.PriceWithVat;
                    parameter.invoiceExistSetting.RoundNumber = invoiceExist.RoundNumber;
                }
            }

            // Get new settings
            var Settings_ = await GeneralSettings.SingleOrDefault(a => a.Id == 1);

            try
            {

                // لو الاصناف فاضية رجع داتا فاضيه 
                if (parameter.itemDetails == null || parameter.itemDetails.Count == 0)
                {
                    resultTotalDiscountItems = new InvoiceResultCalculateDto();
                    ResponseResult = new ResponseResult();
                    ResponseResult.Data = resultTotalDiscountItems;
                    return resultTotalDiscountItems;
                }

                ResponseResult = new ResponseResult();
                ReturnNotes = new ReturnNotes();
                SettingsOfInvoice = new SettingsOfInvoice();


                // chech if invoice isNew get new settings else get saved settings in invoice
                SettingsOfInvoice.setDecimal = Settings_.Other_Decimals;
              
                if (parameter.invoiceExistSetting != null) // invoice is old
                {
                    SettingsOfInvoice.ActiveVat = parameter.invoiceExistSetting.ApplyVat; // تفعيل الضريبة
                    SettingsOfInvoice.PriceIncludeVat = parameter.invoiceExistSetting.PriceWithVat; // السعر يشمل الضريبة
                    SettingsOfInvoice.setDecimal = parameter.invoiceExistSetting.RoundNumber;

                    SettingsOfInvoice.ActiveDiscount = Settings_.Purchases_ActiveDiscount;// parameter.invoiceExistSetting.ActiveDiscount;  // تفعيل الخصم
                    // in returns get new settings
                   if(forReturn || Lists.returnInvoiceList.Contains(parameter.InvoiceTypeId))
                        SettingsOfInvoice.ActiveDiscount = parameter.invoiceExistSetting.ActiveDiscount; // Settings_.Purchases_ActiveDiscount;
                    else
                    {
                        if (parameter.InvoiceTypeId == (int)DocumentType.Purchase || parameter.InvoiceTypeId == (int)DocumentType.wov_purchase)
                            SettingsOfInvoice.ActiveDiscount =  Settings_.Purchases_ActiveDiscount;

                        if (parameter.InvoiceTypeId == (int)DocumentType.Sales)
                            SettingsOfInvoice.ActiveDiscount = Settings_.Sales_ActiveDiscount;
                        
                        if (parameter.InvoiceTypeId == (int)DocumentType.POS)
                            SettingsOfInvoice.ActiveDiscount = Settings_.Pos_ActiveDiscount;

                    }

                }
                else if (parameter.InvoiceTypeId == (int) DocumentType.Purchase || parameter.InvoiceTypeId == (int)DocumentType.PurchaseOrder
                          || parameter.InvoiceTypeId == (int)DocumentType.wov_purchase
                     ||( (parameter.InvoiceTypeId == (int)DocumentType.ReturnPurchase || parameter.InvoiceTypeId ==(int)DocumentType.ReturnWov_purchase )
                     && string.IsNullOrEmpty(parameter.ParentInvoice))) // invoice is new
                {
                    SettingsOfInvoice.ActiveVat = Settings_.Vat_Active;
                    SettingsOfInvoice.PriceIncludeVat = Settings_.Purchases_PriceIncludeVat;
                    SettingsOfInvoice.ActiveDiscount = Settings_.Purchases_ActiveDiscount;
                }
                else if (parameter.InvoiceTypeId == (int)DocumentType.Sales || parameter.InvoiceTypeId == (int)DocumentType.OfferPrice
                      || (parameter.InvoiceTypeId == (int)DocumentType.ReturnSales && string.IsNullOrEmpty(parameter.ParentInvoice)))
                {
                    SettingsOfInvoice.ActiveVat = Settings_.Vat_Active;
                    SettingsOfInvoice.PriceIncludeVat = Settings_.Sales_PriceIncludeVat;
                    SettingsOfInvoice.ActiveDiscount = Settings_.Sales_ActiveDiscount;
                }
                else if (parameter.InvoiceTypeId == (int)DocumentType.POS || (parameter.InvoiceTypeId == (int)DocumentType.ReturnPOS && string.IsNullOrEmpty(parameter.ParentInvoice)))
                {
                    SettingsOfInvoice.ActiveVat = Settings_.Vat_Active;
                    SettingsOfInvoice.PriceIncludeVat = Settings_.Pos_PriceIncludeVat;
                    SettingsOfInvoice.ActiveDiscount = Settings_.Pos_ActiveDiscount;
                }

                var result = StartCalculation(parameter, true, Settings_);
                return result;
            }
            catch (Exception ex)
            {
                return StartCalculation(parameter, false, Settings_);

                // return false;
            }

        }

        public InvoiceResultCalculateDto StartCalculation(CalculationOfInvoiceParameter parameter, bool AllowRecursive, InvGeneralSettings settings)
        {
            UserInformationModel userInfo = Userinformation.GetUserInformation().Result;

            resultTotalDiscountItems = new InvoiceResultCalculateDto();
            // Calculate Total price
            var resultOfTotalPrice = CalculateTotalPrice(ref parameter);
            double totalPrice = resultOfTotalPrice.Item1;
            if(!string.IsNullOrEmpty(resultOfTotalPrice.Item2))
            {
                ResponseResult.ErrorMessageAr = resultOfTotalPrice.Item2;
                ResponseResult.ErrorMessageEn = resultOfTotalPrice.Item3;
                ResponseResult.Result = Result.RequiredData;
                return resultTotalDiscountItems;
            }
           // if (!Lists.storesInvoicesList.Contains(parameter.InvoiceTypeId))
           // {
                // Calculate Discount
                 if (((Lists.purchasesInvoicesList.Contains(parameter.InvoiceTypeId)|| Lists.purchasesWithoutVatInvoicesList.Contains(parameter.InvoiceTypeId)
                        || parameter.InvoiceTypeId == (int)DocumentType.PurchaseOrder) 
                      && !userInfo.otherSettings.purchasesAddDiscount)
                      || ((Lists.salesInvoicesList.Contains(parameter.InvoiceTypeId)|| parameter.InvoiceTypeId==(int)DocumentType.OfferPrice)
                      && !userInfo.otherSettings.salesAddDiscount)
                      || (Lists.POSInvoicesList.Contains(parameter.InvoiceTypeId) && !userInfo.otherSettings.posAddDiscount)
                      || !Lists.InvoicesHaveDiscounts.Contains(parameter.InvoiceTypeId))
                {

                    resultTotalDiscountItems.TotalAfterDiscount = totalPrice;
                    resultTotalDiscountItems.TotalPrice = totalPrice;
                }
                else
                {
                    var DiscountDone = CalculateDiscount(ref parameter, totalPrice, SettingsOfInvoice, settings);

                    // make discount = 0 and  reCalculate the discount if  exceeds the total / total of Item
                    if (!DiscountDone.AllowRecersive)
                    {
                        if (AllowRecursive)
                        {
                            //  ResponseResult.Result = DiscountDone.result;
                            ResponseResult.ErrorMessageAr = DiscountDone.MessaageAr;
                            ResponseResult.ErrorMessageEn = DiscountDone.MessaageEn;
                            return StartCalculation(parameter, false, settings);
                        }
                    }

                    ResponseResult.ErrorMessageAr = DiscountDone.MessaageAr;
                    ResponseResult.ErrorMessageEn = DiscountDone.MessaageEn;
                    DiscountDone.AllowRecersive = true;
                }


                // Calculate Vat

                CalculateVat(parameter, SettingsOfInvoice);
                //   ResponseResult.Result = DiscountDone.result;

          //  }

            //Calculate Net
            CalculateNet();

            ResponseResult.Data = resultTotalDiscountItems;
            ResponseResult.Id = null;
            ResponseResult.Result = (string.IsNullOrEmpty(ResponseResult.ErrorMessageAr) ? Result.Success : Result.Failed);

            return resultTotalDiscountItems;
        }

        /// <summary>
        /// Calculate total price foreach item and for all invoice
        /// </summary>
        /// <param name="parameter"> pass parameter by refrance</param>
        /// <returns> total price , msg  if thier are error </returns>
        public Tuple<double, string, string> CalculateTotalPrice(ref CalculationOfInvoiceParameter parameter)
        
        {
            double totalPrice = 0.0;
            // اصناف تقبل كميات صفرية في حالة النسخ
            var itemTypeWithZeroQty = new List<int> { (int)ItemTypes.Note, (int)ItemTypes.Serial, (int)ItemTypes.Expiary };
            foreach (var item in parameter.itemDetails)
            {
                var itemQty = roundNumbers.GetRoundNumber(item.Quantity,
                                     (item.isBalanceBarcode? (int)generalEnum.BalanceBarcodeDecimal : SettingsOfInvoice.setDecimal));
                item.Quantity = itemQty;
                var itemPrice = roundNumbers.GetRoundNumber(item.Price, SettingsOfInvoice.setDecimal);
                    //Calculate total for each item  
                var TotalItem = new ItemsTotalList();
                TotalItem.Quantity = itemQty;
                TotalItem.Price = item.Price;
                if( parameter.InvoiceTypeId != (int)DocumentType.IncomingTransfer)
                {
                    // if itemCode is empty then is component of composite item in add and update api
                    // i will check quantity and price of composite not component
                    if(!string.IsNullOrEmpty(item.itemCode) && (item.ItemTypeId!=(int)ItemTypes.Note  && (itemQty <= 0 || itemPrice <= 0 ) &&!parameter.isCopy.Value  )
                         || (parameter.isCopy.Value &&(itemQty <= 0 || itemPrice <= 0)  && !itemTypeWithZeroQty.Contains(item.ItemTypeId))) //لو بنسخ الفاتورة بتخطي الكميات الصفريه للسيريال والصلاحية
                                 return new Tuple<double, string, string>(0, ErrorMessagesAr.CanNotEnterQuantityORPriceZero + item.itemCode, ErrorMessagesEn.CanNotEnterQuantityORPriceZero + item.itemCode);
                }
               // اجمالى سعر الصنف = الكمية  * السعر 
               // skip component  of composite item to avoid duplication 
               if(item.ItemTypeId!=0 || !string.IsNullOrEmpty( item.itemCode))
                {
                    TotalItem.ItemTotal = roundNumbers.GetRoundNumber(itemQty * itemPrice, SettingsOfInvoice.setDecimal);
                    TotalItem.TotalWithSplitedDiscount = TotalItem.ItemTotal;
                    totalPrice += TotalItem.ItemTotal;// roundNumbers.GetRoundNumber(itemQty * itemPrice, SettingsOfInvoice.setDecimal);

                }
                resultTotalDiscountItems.itemsTotalList.Add(TotalItem);

            }
            return new Tuple<double, string, string>(roundNumbers.GetRoundNumber(totalPrice, SettingsOfInvoice.setDecimal), "", "");
        }

        /// <summary>
        /// calculate discounts foreach item or total discount according to discount type
        /// </summary>
        /// <param name="parameter"> </param>
        /// <param name="totalPrice"></param>
        /// <param name="SettingsOfInvoice"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ReturnNotes CalculateDiscount(ref CalculationOfInvoiceParameter parameter, double totalPrice, SettingsOfInvoice SettingsOfInvoice, InvGeneralSettings settings)
        {

            // check discount value 
            if (parameter.TotalDiscountValue < 0 || parameter.TotalDiscountRatio < 0)
            {
                parameter.TotalDiscountValue = 0;
                parameter.TotalDiscountRatio = 0;
                ReturnNotes.MessaageEn = ErrorMessagesEn.canNotEnterNegativeDiscount;
                ReturnNotes.MessaageAr = ErrorMessagesAr.canNotEnterNegativeDiscount;
                ReturnNotes.AllowRecersive = false;
                return ReturnNotes;
            }

            int counter = 0;
            resultTotalDiscountItems.itemsTotalList = new List<ItemsTotalList>();
            foreach (var item in parameter.itemDetails)
            {
                if(item.DiscountValue<0||item.DiscountRatio<0)
                {
                    parameter.itemDetails[counter].DiscountValue = 0;
                    parameter.itemDetails[counter].DiscountRatio = 0;
                    ReturnNotes.MessaageEn = ErrorMessagesEn.canNotEnterNegativeDiscount;
                    ReturnNotes.MessaageAr = ErrorMessagesAr.canNotEnterNegativeDiscount;
                    ReturnNotes.AllowRecersive = false;
                    return ReturnNotes;
                }
            
                //Calculate for each item total 
                var ItemData = new ItemsTotalList();

                if (item.ItemTypeId == (int)ItemTypes.Note)
                {
                    resultTotalDiscountItems.itemsTotalList.Add(ItemData);
                    counter++;
                    continue;
                }
                
                ItemData.Quantity = item.Quantity;
                ItemData.Price = item.Price;
                ItemData.ItemTotal = roundNumbers.GetRoundNumber(item.Quantity * item.Price, SettingsOfInvoice.setDecimal);
                ItemData.TotalWithSplitedDiscount = ItemData.ItemTotal;

                // check invoice type to calculate AutoDiscount in sales and pos

                // calculate Total Discount value and total discount ratio
                if (SettingsOfInvoice.ActiveDiscount && parameter.DiscountType == (int)DiscountType.DiscountOnItem) //if discount done for each item
                {
                    //Calculate for each item total 

                    if (!item.isDiscountRatioItem)
                    {
                        ItemData.DiscountRatio = roundNumbers.GetRoundNumber(((item.DiscountValue / ItemData.ItemTotal) * 100), SettingsOfInvoice.setDecimal);
                        ItemData.DiscountValue = roundNumbers.GetRoundNumber(item.DiscountValue, SettingsOfInvoice.setDecimal);
                    }
                    else
                    {
                        ItemData.DiscountValue = roundNumbers.GetRoundNumber(((item.DiscountRatio * ItemData.ItemTotal) / 100), SettingsOfInvoice.setDecimal);
                        ItemData.DiscountRatio = roundNumbers.GetRoundNumber(item.DiscountRatio, SettingsOfInvoice.setDecimal);
                    }

                    // check if discount exceeds the total item and set discount =0 and recalculate Discount
                    if (ItemData.DiscountValue > ItemData.ItemTotal && ItemData.DiscountValue > 0)
                    {
                        parameter.itemDetails[counter].DiscountValue = 0;
                        parameter.itemDetails[counter].DiscountRatio = 0;
                        ReturnNotes.AllowRecersive = false;
                        ReturnNotes.MessaageEn = ErrorMessagesEn.discountExccedTotal;
                        ReturnNotes.MessaageAr = ErrorMessagesAr.discountExccedTotal;
                        //   ReturnNotes.result = Result.Failed;
                        return ReturnNotes;
                    }
                    ItemData.ItemTotal = roundNumbers.GetRoundNumber(ItemData.ItemTotal - ItemData.DiscountValue, SettingsOfInvoice.setDecimal);
                    ItemData.TotalWithSplitedDiscount = ItemData.ItemTotal;
                    resultTotalDiscountItems.TotalDiscountValue += ItemData.DiscountValue;
                 //     resultTotalDiscountItems.TotalDiscountRatio += ItemData.DiscountRatio;

                }
                else if (SettingsOfInvoice.ActiveDiscount && (parameter.DiscountType == (int)DiscountType.DiscountOnInvoice)) // calculate split discount for each item 
                {
                    // calc total discount 

                    if (parameter.isDiscountRatio )// && parameter.TotalDiscountValue == 0)
                    {
                        resultTotalDiscountItems.TotalDiscountValue = roundNumbers.GetRoundNumber((parameter.TotalDiscountRatio * totalPrice) / 100, SettingsOfInvoice.setDecimal);
                        resultTotalDiscountItems.TotalDiscountRatio = roundNumbers.GetRoundNumber(parameter.TotalDiscountRatio, (int)generalEnum.vatDecimal); //SettingsOfInvoice.setDecimal);
                    }
                    else
                    {
                        resultTotalDiscountItems.TotalDiscountValue = roundNumbers.GetRoundNumber(parameter.TotalDiscountValue, SettingsOfInvoice.setDecimal);
                        resultTotalDiscountItems.TotalDiscountRatio = roundNumbers.GetRoundNumber((parameter.TotalDiscountValue / totalPrice) * 100, (int)generalEnum.vatDecimal);// SettingsOfInvoice.setDecimal);
                    }


                    if (resultTotalDiscountItems.TotalDiscountValue > totalPrice)
                    {
                        parameter.TotalDiscountValue = 0;
                        parameter.TotalDiscountRatio = 0;
                        ReturnNotes.AllowRecersive = false;
                        ReturnNotes.MessaageEn =ErrorMessagesEn.discountExccedTotal;
                        ReturnNotes.MessaageAr = ErrorMessagesAr.discountExccedTotal;
                        //   ReturnNotes.result = Result.Failed;
                        return ReturnNotes;
                    }
             
                    ItemData.SplitedDiscountRatio = resultTotalDiscountItems.TotalDiscountRatio;
                    ItemData.SplitedDiscountValue = roundNumbers.GetRoundNumber((ItemData.SplitedDiscountRatio * ItemData.ItemTotal) / (100), SettingsOfInvoice.setDecimal);

                    //ItemData.SplitedDiscountValue = roundNumbers.GetRoundNumber((ItemData.ItemTotal / totalPrice) * (totalSPDiscount), SettingsOfInvoice.setDecimal);
                    //ItemData.SplitedDiscountRatio = roundNumbers.GetRoundNumber((ItemData.SplitedDiscountValue / ItemData.ItemTotal) * 100, SettingsOfInvoice.setDecimal);

                    ItemData.ItemTotal = roundNumbers.GetRoundNumber(ItemData.ItemTotal , SettingsOfInvoice.setDecimal);
                    ItemData.TotalWithSplitedDiscount = roundNumbers.GetRoundNumber(ItemData.ItemTotal - ItemData.SplitedDiscountValue, SettingsOfInvoice.setDecimal);
                   
                }


                if (parameter.InvoiceTypeId == (int)DocumentType.Sales || parameter.InvoiceTypeId == (int)DocumentType.OfferPrice
                    || parameter.InvoiceTypeId == (int)DocumentType.POS || parameter.InvoiceTypeId == (int)DocumentType.ReturnPOS  )
                {
                    if ((Lists.POSInvoicesList.Contains(parameter.InvoiceTypeId) && !settings.Pos_ExceedDiscountRatio)
                        || (Lists.salesInvoicesList.Contains(parameter.InvoiceTypeId) && !settings.Sales_ExceedDiscountRatio))
                    {
                        
                        var customer = parameter.PersonId.Value;
                        var discountLimitForCustomer = personQuery.TableNoTracking.Where(a => a.Id == customer)
                              .Select(a => a.DiscountRatio).First();
                        if (ItemData.DiscountRatio > discountLimitForCustomer || resultTotalDiscountItems.TotalDiscountRatio > discountLimitForCustomer)
                        {
                            parameter.itemDetails[counter].DiscountValue = 0;
                            parameter.itemDetails[counter].DiscountRatio = 0;
                            if (parameter.DiscountType == (int)DiscountType.DiscountOnInvoice)
                            {
                                parameter.TotalDiscountRatio = 0;
                                parameter.TotalDiscountValue = 0;
                            }
                            ReturnNotes.AllowRecersive = false;
                            ReturnNotes.MessaageEn =string.Concat( "You have exceeded the discount ratio for the customer"," ",discountLimitForCustomer);
                            ReturnNotes.MessaageAr =string.Concat( "لقد تجاوزت نسبة الخصم المحددة للعميل" ," ", discountLimitForCustomer);
                            //  ReturnNotes.result = Result.Failed;
                            return ReturnNotes;
                        }
                    }
                }

                counter++;

                resultTotalDiscountItems.itemsTotalList.Add(ItemData);
            }

            resultTotalDiscountItems.TotalDiscountValue = Math.Round(resultTotalDiscountItems.TotalDiscountValue, SettingsOfInvoice.setDecimal);
          // to avoid mistake in round after each item
            if (totalPrice != 0)
                resultTotalDiscountItems.TotalDiscountRatio = roundNumbers.GetRoundNumber(((resultTotalDiscountItems.TotalDiscountValue / totalPrice) * 100), SettingsOfInvoice.setDecimal);

            resultTotalDiscountItems.TotalPrice = roundNumbers.GetRoundNumber(totalPrice, SettingsOfInvoice.setDecimal);
            if (SettingsOfInvoice.ActiveDiscount)
                resultTotalDiscountItems.TotalAfterDiscount = roundNumbers.GetRoundNumber(totalPrice - resultTotalDiscountItems.TotalDiscountValue, SettingsOfInvoice.setDecimal);
            else
                resultTotalDiscountItems.TotalAfterDiscount = roundNumbers.GetRoundNumber(totalPrice, SettingsOfInvoice.setDecimal);




            ReturnNotes.AllowRecersive = true;
            return ReturnNotes;
        }
        //vat calculation
        public void CalculateVat(CalculationOfInvoiceParameter parameter, SettingsOfInvoice SettingsOfInvoice)
        {
            if (!InvoiceWithoutVAT(parameter.InvoiceTypeId) || !SettingsOfInvoice.ActiveVat)
                return;

            double totalVat = 0;
            if (resultTotalDiscountItems.itemsTotalList.Count() > 0)
                for (var a = 0; a < parameter.itemDetails.Count(); a++)
                {
                    if (!parameter.itemDetails[a].ApplyVat)
                        continue;
                    double vatValue = 0;
                    double vatRatio = parameter.itemDetails[a].VatRatio;
                    double ItemTotal = resultTotalDiscountItems.itemsTotalList[a].TotalWithSplitedDiscount;//.ItemTotal;
                    double ItemTotalForPrint = resultTotalDiscountItems.itemsTotalList[a].ItemTotal;
                    double vatValueForPrint = 0;

                    // if (parameter.itemDetails[a].ApplyVat)
                    if (vatRatio > 0)
                    {
                        if (SettingsOfInvoice.PriceIncludeVat) // السعر شامل الضريبة
                        {
                           
                            vatValue = (ItemTotal / (100 + vatRatio) * vatRatio);
                            vatValueForPrint = (ItemTotalForPrint / (100 + vatRatio) * vatRatio);
                        }
                        else // السعر غير شامل الضريبة
                        {
                            vatValue = (ItemTotal / 100 * vatRatio);
                            vatValueForPrint = (ItemTotalForPrint / 100 * vatRatio);
                        }
                    }

                    resultTotalDiscountItems.itemsTotalList[a].VatValue = roundNumbers.GetRoundNumber(vatValue, (int)generalEnum.vatDecimal); // SettingsOfInvoice.setDecimal);
                    resultTotalDiscountItems.itemsTotalList[a].VatValueForPrint = roundNumbers.GetRoundNumber(vatValueForPrint, (int)generalEnum.vatDecimal); // SettingsOfInvoice.setDecimal);
                    totalVat += vatValue;
                }
            resultTotalDiscountItems.TotalVat = roundNumbers.GetRoundNumber(totalVat, (int)generalEnum.vatDecimal);

        }

        // تحديد اذا كانت الفاتورة لها ضريبة ام لا 
        public bool InvoiceWithoutVAT(int recType)
        {

            if (recType == (int)DocumentType.Purchase) return true;
            if (recType == (int)DocumentType.DeletePurchase) return true;
            if (recType == (int)DocumentType.ReturnPurchase) return true;
            if (recType == (int)DocumentType.Sales) return true;
            if (recType == (int)DocumentType.DeleteSales) return true;
            if (recType == (int)DocumentType.ReturnSales) return true;
            if (recType == (int)DocumentType.POS) return true;
            if (recType == (int)DocumentType.DeletePOS) return true;
            if (recType == (int)DocumentType.ReturnPOS) return true;
            if (recType == (int)DocumentType.PurchaseOrder) return true;
            if (recType == (int)DocumentType.OfferPrice) return true;
            
            // if (recType == (int)DocumentType.offer) return true;

            if (recType == (int)DocumentType.AddPermission) return false;
            if (recType == (int)DocumentType.DeleteAddPermission) return false;
            if (recType == (int)DocumentType.ExtractPermission) return false;
            if (recType == (int)DocumentType.DeleteExtractPermission) return false;
            if (recType == (int)DocumentType.wov_purchase) return false;
            if (recType == (int)DocumentType.ReturnWov_purchase) return false;
            if (recType == (int)DocumentType.DeleteWov_purchase) return false;
            //if (recType == (int)DocumentType.balance) return false;
            //if (recType == (int)DocumentType.balance_D) return false;
            //if (recType == (int)DocumentType.outgoingTransfer) return false;
            //if (recType == (int)DocumentType.transfer_A) return false;
            //if (recType == (int)DocumentType.transfer_R) return false;
            //if (recType == (int)DocumentType.stocking) return false;
            //if (recType == (int)DocumentType.outgoingTransfer_D) return false;
            //if (recType == (int)DocumentType.incomingTransfer) return false;
            //if (recType == (int)DocumentType.itemIngredients) return false;
            //if (recType == (int)DocumentType.itemsFund) return false;
            //if (recType == (int)DocumentType.itemsFund_D) return false;
            return true;
        }


        public void CalculateNet()
        {
            resultTotalDiscountItems.Net = roundNumbers.GetRoundNumber(resultTotalDiscountItems.TotalAfterDiscount, SettingsOfInvoice.setDecimal);
            resultTotalDiscountItems.ActualNet = roundNumbers.GetDefultRoundNumber(resultTotalDiscountItems.TotalAfterDiscount );


            if (SettingsOfInvoice.ActiveVat && !SettingsOfInvoice.PriceIncludeVat)
            {
                resultTotalDiscountItems.Net = roundNumbers.GetRoundNumber(resultTotalDiscountItems.TotalAfterDiscount + resultTotalDiscountItems.TotalVat, SettingsOfInvoice.setDecimal);
                resultTotalDiscountItems.ActualNet = roundNumbers.GetDefultRoundNumber(resultTotalDiscountItems.TotalAfterDiscount + resultTotalDiscountItems.TotalVat);

            }

        }


        #endregion


        public async Task<ResponseResult> checkItemPrice(checkItemPrice paramter)
        
        {
            UserInformationModel userInfo =await  Userinformation.GetUserInformation();
            var settings = await GeneralSettings.SingleOrDefault(a => a.Id == 1);
            InvStpItemCardUnit item = null;

            double lastSalePrice = 0;
            bool ExceedPrices = false;
            //pos or sales
            if (Lists.salesInvoicesList.Contains( paramter.invoiceTypeId) || paramter.invoiceTypeId == (int)DocumentType.OfferPrice)
            {
                if (!settings.Sales_ModifyPrices)
                {
                    double itemPrice = await GetItemPriceByInvoiceTypeId(paramter, settings, userInfo);
                    lastSalePrice = itemPrice;
                    if (itemPrice != paramter.price)
                    {
                        return new ResponseResult()
                        {
                            Result = Result.Failed,
                            ErrorMessageAr = ErrorMessagesAr.cantEditPrice,
                            ErrorMessageEn = ErrorMessagesEn.cantEditPrice
                        };
                    }
                }
                else
                {
                    // increase price of sales
                    if (settings.Sales_ModifyPricesType==(int)ModifySalePricesType.allowModifyByIncreaseOnly)
                    {
                        double itemPrice = await GetItemPriceByInvoiceTypeId(paramter, settings, userInfo);
                        if (itemPrice > paramter.price)
                            ExceedPrices = true;
                    }
                    else if (settings.Sales_ModifyPricesType==(int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4)
                    { 
                        var item_ = ItemsQuery.TableNoTracking.Where(h => h.ItemId == paramter.itemId && h.UnitId == paramter.unitId);
                        if (item_.First().SalePrice4 > paramter.price)
                          ExceedPrices= true;
                       
                    }

                }
            }
      
            //pos
            if(Lists.POSInvoicesList.Contains( paramter.invoiceTypeId ) )
            {
                if (!settings.Pos_ModifyPrices)
                {
                    double itemPrice = await GetItemPriceByInvoiceTypeId(paramter, settings, userInfo);

                    if (itemPrice != paramter.price)
                    {
                        return new ResponseResult()
                        {
                            Result = Result.Failed,
                            ErrorMessageAr = ErrorMessagesAr.cantEditPrice,
                            ErrorMessageEn = ErrorMessagesEn.cantEditPrice
                        };
                    }
                }
                else
                {
                    // increase price of sales
                    if (settings.Pos_ModifyPricesType == (int)ModifySalePricesType.allowModifyByIncreaseOnly)
                    {
                        double itemPrice = await GetItemPriceByInvoiceTypeId(paramter, settings, userInfo);
                        if (itemPrice > paramter.price)
                            ExceedPrices = true;
                    }
                    else if (settings.Pos_ModifyPricesType == (int)ModifySalePricesType.allowModifyByIncreaseUntilPrice4)
                    {
                        var item_ = ItemsQuery.TableNoTracking.Where(h => h.ItemId == paramter.itemId && h.UnitId == paramter.unitId);
                        if (item_.First().SalePrice4 > paramter.price)
                            ExceedPrices = true;

                    }
                }

            }

            if(ExceedPrices)
                return new ResponseResult()
                {
                    Result = Result.Failed,
                    ErrorMessageAr = ErrorMessagesAr.itemExceedSalePrice,
                    ErrorMessageEn = ErrorMessagesEn.itemExceedSalePrice
                };

            //purchase
            if (!settings.Purchases_ModifyPrices && ( paramter.invoiceTypeId == (int)DocumentType.Purchase || paramter.invoiceTypeId == (int)DocumentType.wov_purchase || paramter.invoiceTypeId ==(int)DocumentType.PurchaseOrder))
            {
                double itemPrice =await  GetItemPriceByInvoiceTypeId(paramter, settings , userInfo);

                if (itemPrice != paramter.price)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        ErrorMessageAr = ErrorMessagesAr.cantEditPrice,
                        ErrorMessageEn = ErrorMessagesEn.cantEditPrice
                    };
                }
            }

        
       
            return new ResponseResult() { Result = Result.Success };
        }

        private async Task< double> GetItemPriceByInvoiceTypeId(checkItemPrice paramter, InvGeneralSettings settings, UserInformationModel userInfo)
        {
            
                double itemPrice = 0;
     
                //if option of the last of sales price  
                if ((settings.other_ActivatePriceLists && settings.PriceListType == (int)PriceListsType.LastPriceForCustomer)
                  || ((paramter.invoiceTypeId == (int)DocumentType.Purchase || paramter.invoiceTypeId == (int)DocumentType.wov_purchase 
                  || paramter.invoiceTypeId == (int)DocumentType.PurchaseOrder) && settings.Purchases_UseLastPrice))
                {
                    itemPrice = getLastPrice(paramter);
                }
                else
                {
                   var item = ItemsQuery.TableNoTracking.Where(h => h.ItemId == paramter.itemId && h.UnitId == paramter.unitId).FirstOrDefault();
                    if (item != null)
                    {
                       if (paramter.invoiceTypeId == (int)DocumentType.Purchase || paramter.invoiceTypeId == (int)DocumentType.wov_purchase || paramter.invoiceTypeId == (int)DocumentType.PurchaseOrder)
                           itemPrice = item.PurchasePrice;
                       else if(paramter.invoiceTypeId == (int)DocumentType.Sales || paramter.invoiceTypeId == (int)DocumentType.POS 
                                     || paramter.invoiceTypeId == (int)DocumentType.OfferPrice)
                       {
                          itemPrice = item.SalePrice1;
                          if (settings.other_ActivatePriceLists)
                           {
                              
                              int  salePriceId =  await  mediator.Send(new salePriceListsRequest()
                                                 {
                                                     branchId = userInfo.CurrentbranchId,
                                                     personId = paramter.personId.Value,
                                                     salesManId = paramter.salesManId.Value,
                                                     employeeId = userInfo.employeeId,
                                                     PriceListId = paramter.PriceListId.Value,
                                                     invoiceTypeId = paramter.invoiceTypeId,
                                                     setting = settings
                                                 });
                          
                               if (salePriceId == (int)SalePricesList.SalePrice1)
                                   itemPrice = item.SalePrice1;
                               else if (salePriceId == (int)SalePricesList.SalePrice2)
                                   itemPrice = item.SalePrice2;
                               else if (salePriceId == (int)SalePricesList.SalePrice3)
                                   itemPrice = item.SalePrice3;
                               else if (salePriceId == (int)SalePricesList.SalePrice4)
                                   itemPrice = item.SalePrice4;
                              
                           }
                    }
                }
            }

               return itemPrice;    
        }
        

        private double getLastPrice(checkItemPrice paramter)
        {
            
            if(paramter.invoiceTypeId==(int)DocumentType.OfferPrice)
            {
                var LastPrice = OfferPriceDetailsQuery.TableNoTracking.Where(a => a.OfferPriceMaster.InvoiceTypeId == paramter.invoiceTypeId
                 && a.OfferPriceMaster.PersonId == paramter.personId && a.ItemId == paramter.itemId
                 && a.UnitId == paramter.unitId).OrderBy(a => a.Id).LastOrDefault();
                if (LastPrice == null)
                    return 0;
                return LastPrice.Price;
            }
            else
            {
                var LastPrice = InvoiceDetailsQuery.TableNoTracking.Where(a => a.InvoicesMaster.InvoiceTypeId == paramter.invoiceTypeId
                                 && a.InvoicesMaster.PersonId == paramter.personId && a.ItemId == paramter.itemId
                                 && a.UnitId == paramter.unitId).OrderBy(a => a.Id).LastOrDefault();
                if (LastPrice == null)
                    return 0;
                return LastPrice.Price;
            }
            
            
               
        }
    }

    public class ReturnNotes
    {
        // public Result result { get; set; }
        public bool AllowRecersive { get; set; }
        public string MessaageAr { get; set; }
        public string MessaageEn { get; set; }
    }



}
