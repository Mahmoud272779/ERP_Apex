using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.POS;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.General_Process
{
    class GetInvoiceForReturn: BaseClass , IGetInvoiceForReturn
    {
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly ICalculationSystemService CalculationSystemService;
        private readonly iUserInformation Userinformation;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly IRoundNumbers roundNumbers;
        private readonly IRepositoryQuery<InvPurchaseAdditionalCostsRelation> InvoiceAdditionalCostsRelationRepositoryQuery;
        private readonly IPosService posService;

        public GetInvoiceForReturn(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                               IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery,
                               IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery,
                               ICalculationSystemService CalculationSystemService,
                               iUserInformation Userinformation, IRepositoryQuery<InvGeneralSettings> GeneralSettings,
                              IGeneralAPIsService generalAPIsService, IRoundNumbers roundNumbers, IHttpContextAccessor httpContext, IRepositoryQuery<InvPurchaseAdditionalCostsRelation> invoiceAdditionalCostsRelationRepositoryQuery, IPosService posService)
            : base(httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            this.InvoiceDetailsRepositoryQuery = InvoiceDetailsRepositoryQuery;
            this.InvSerialTransactionRepositoryQuery = InvSerialTransactionRepositoryQuery;
            this.CalculationSystemService = CalculationSystemService;
            this.generalAPIsService = generalAPIsService;
            this.Userinformation = Userinformation;
            this.httpContext = httpContext;
            this.GeneralSettings = GeneralSettings;
            this.roundNumbers = roundNumbers;
            InvoiceAdditionalCostsRelationRepositoryQuery = invoiceAdditionalCostsRelationRepositoryQuery;
            this.posService = posService;
        }

        public async Task<ResponseResult> GetMainInvoiceForReturn(string InvoiceCode, int InvoiceTypeId)
        {
            return await GetMainInvoiceForReturn(InvoiceCode, InvoiceTypeId, false);
        }
        public async Task<ResponseResult> GetMainInvoiceForReturn(string InvoiceCode, int InvoiceTypeId, bool? ForIOS)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var listOfReturnPurchase = new List<InvoiceDto>();
            var MainInvoice = await InvoiceMasterRepositoryQuery.TableNoTracking.Where(q =>
                    (q.InvoiceType == InvoiceCode || q.Code.ToString() == InvoiceCode ||
                         (InvoiceTypeId == (int)DocumentType.POS ? q.CodeOfflinePOS == InvoiceCode : false))
                                && q.InvoiceTypeId==InvoiceTypeId && q.BranchId == userInfo.CurrentbranchId)
                .Include(a => a.store)
                .Include(a => a.Branch)
                .Include(a => a.Person)
                .Include(a => a.salesMan).FirstOrDefaultAsync();

                                 

            if (MainInvoice == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound , Note=Actions.NotFound };
             
            if (MainInvoice.IsDeleted)
                return new ResponseResult() { Data = null, Id = null, Result =  Result.Deleted ,Note=Actions.DeletedInvoice};

            if (MainInvoice.InvoiceSubTypesId == (int)SubType.TotalReturn)
                return new ResponseResult() { Data = null, Id = null, Result = Result.InvoiceTotalReturned , Note=Actions.ReturnedInvoice };

            var InvoiceDto = new InvoiceDto();
            var listPurchase = new List<InvoiceDto>();

            
          
            InvoiceDto.InvoiceId = MainInvoice.InvoiceId;
            InvoiceDto.StoreId = MainInvoice.StoreId;
            InvoiceDto.StoreNameAr = MainInvoice.store.ArabicName;
            InvoiceDto.StoreNameEn = MainInvoice.store.LatinName;
            InvoiceDto.StoreStatus = MainInvoice.store.Status;
            InvoiceDto.BranchId = MainInvoice.BranchId;
            InvoiceDto.BranchNameAr = MainInvoice.Branch.ArabicName;
            //InvoiceDto.BookIndex = MainInvoice.BookIndex;
            InvoiceDto.Notes = MainInvoice.Notes;
            InvoiceDto.InvoiceDate = DateTime.Now.ToString(defultData.datetimeFormat);
            InvoiceDto.InvoiceCode = "";
            InvoiceDto.ParentInvoiceCode = MainInvoice.InvoiceType;
            //////
            InvoiceDto.PersonId  = MainInvoice.PersonId;
            InvoiceDto.PersonNameAr = MainInvoice.Person.ArabicName;
            InvoiceDto.PersonNameEn = MainInvoice.Person.LatinName;
            InvoiceDto.PersonStatus = MainInvoice.Person.Status ;
            InvoiceDto.Paid =0;//المدفوع 
            InvoiceDto.VirualPaid = 0;//المدوفوع من العميل 
            InvoiceDto.ApplyVat = MainInvoice.ApplyVat;//يخضع للضريبه ام لا
            InvoiceDto.PriceWithVat = MainInvoice.PriceWithVat;//السعر شامل الضريبه ام لا
            InvoiceDto.DiscountType = MainInvoice.DiscountType;//نوع الخصم (اجمالي او على الصنف)
            InvoiceDto.PaymentType = 0;
            InvoiceDto.ActiveDiscount = MainInvoice.ActiveDiscount;
            InvoiceDto.RoundNumber = MainInvoice.RoundNumber;
            InvoiceDto.isDiscountRatio = MainInvoice.isDiscountRatio;
            if (MainInvoice.SalesManId!=null)
            {
                InvoiceDto.SalesManId = MainInvoice.SalesManId.Value;
                InvoiceDto.SalesManNameAr = MainInvoice.salesMan.ArabicName;
                InvoiceDto.SalesManNameEn = MainInvoice.salesMan.LatinName;

            }
           
            // prepare paramaters for calculation
            var ParameterOfCalculation = new CalculationOfInvoiceParameter();
            ParameterOfCalculation.DiscountType = MainInvoice.DiscountType;
            ParameterOfCalculation.InvoiceId = MainInvoice.InvoiceId;
            ParameterOfCalculation.InvoiceTypeId = MainInvoice.InvoiceTypeId;
            ParameterOfCalculation.TotalDiscountRatio = MainInvoice.TotalDiscountRatio;
            ParameterOfCalculation.TotalDiscountValue = MainInvoice.TotalDiscountValue;
            ParameterOfCalculation.PersonId = MainInvoice.PersonId.Value;
            ParameterOfCalculation.isDiscountRatio = MainInvoice.isDiscountRatio;
            var MainInvoiceDetails =  InvoiceDetailsRepositoryQuery.TableNoTracking.Include( a => a.Units)
                                      .Include(a=>a.Items).Where(q => q.InvoiceId == MainInvoice.InvoiceId );
            var setting = await GeneralSettings.SingleOrDefault(a => a.Id == 1);

           var  ItemUnitsDto_ = new List<ItemUnitsDto>();

            if (ForIOS != null && ForIOS.Value)
            {
                var itemIds = MainInvoiceDetails.Select(a => a.ItemId).ToList();
                var itemUnits = posService.getItemUnitsForPOS(itemIds);
                var itemUnits_ = (IQueryable<ItemUnitsDto>)itemUnits.Result.Data;
                ItemUnitsDto_ = itemUnits_.ToList();
            }

            foreach (var item in MainInvoiceDetails)
            {

                if (item.Quantity == item.ReturnQuantity||( item.parentItemId != null && item.parentItemId!=0))
                    continue;
               
                var DetailsParameterForCalc = new InvoiceDetailsAttributes();
                var invoiceDetailsDto = new InvoiceDetailsDto()
                {

                    ItemId = item.ItemId,
                    ItemCode = item.Items.ItemCode,
                    ItemNameAr = item.Items.ArabicName,
                    ItemNameEn = item.Items.LatinName, 
                    Price = item.Price,
                    Quantity = roundNumbers.GetRoundNumber(item.Quantity - item.ReturnQuantity, InvoiceDto.RoundNumber),
                    Total = item.TotalWithSplitedDiscount,
                    Signal = generalAPIsService.GetSignal(InvoiceTypeId),
                    ItemTypeId = item.ItemTypeId,
                    UnitId = item.UnitId,
                    UnitNameAr = item.Units.ArabicName,
                    UnitNameEn = item.Units.LatinName,
                    DiscountValue = item.DiscountValue,//قيمه الخصم على مستوى الصنف
                    DiscountRatio = item.DiscountRatio,// نسبه الخصم على مستوى الصنف
                    VatRatio = item.VatRatio,// الضريبه على مستوى الصنف 
                    VatValue = item.VatValue,// الضريبه على مستوى الصنف 
                    TransQuantity = item.Quantity,
                    ReturnQuantity = 0,
                    StatusOfTrans = item.StatusOfTrans,
                    SplitedDiscountValue = item.SplitedDiscountValue,
                    SplitedDiscountRatio = item.SplitedDiscountRatio,
                    AvgPrice = item.AvgPrice,
                    Cost = item.Cost,
                    AutoDiscount = item.AutoDiscount,
                    PriceList = item.PriceList,
                    MinimumPrice = item.MinimumPrice,
                    ConversionFactor = item.ConversionFactor,
                    IndexOfItem = item.indexOfItem,
                    ApplyVat=item.Items.ApplyVAT ,
                    isDiscountRatioItem = item.isDiscountRatioItem,
                    VatValueForPrint = item.VatValueForPrint
                };

                invoiceDetailsDto.OldQuantity = invoiceDetailsDto.Quantity;
                DetailsParameterForCalc.DiscountValue = invoiceDetailsDto.DiscountValue;
                DetailsParameterForCalc.ItemTypeId = invoiceDetailsDto.ItemTypeId;
                DetailsParameterForCalc.Price = invoiceDetailsDto.Price;
                DetailsParameterForCalc.Quantity = invoiceDetailsDto.Quantity;
                DetailsParameterForCalc.VatRatio = invoiceDetailsDto.VatRatio;
                DetailsParameterForCalc.ApplyVat = invoiceDetailsDto.ApplyVat;
                DetailsParameterForCalc.isDiscountRatioItem = invoiceDetailsDto.isDiscountRatioItem;
 
                ParameterOfCalculation.itemDetails.Add(DetailsParameterForCalc);
                if (item.Items != null)
                {
                    if (item.Items.TypeId == (int)ItemTypes.Expiary)
                    {
                        invoiceDetailsDto.ExpireDate =Convert .ToDateTime( item.ExpireDate).ToString("yyyy-MM-dd");
                    }
                    if (item.Items.TypeId == (int)ItemTypes.Serial)
                    {
                        // serials added to store after sale can not return to store again from this sales invoice

                       
                        var serialsOfInvoice = InvSerialTransactionRepositoryQuery.FindAll(q => q.ItemId == item.ItemId && 
                        (MainInvoice.InvoiceTypeId==(int)DocumentType.Purchase)? (q.AddedInvoice == MainInvoice.InvoiceType && q.indexOfSerialForAdd == item.indexOfItem )
                          : (q.ExtractInvoice == MainInvoice.InvoiceType   && q.indexOfSerialForExtract == item.indexOfItem ));

                       
                        //if (MainInvoice.InvoiceTypeId == (int)DocumentType.Purchase)
                        //    serialsOfInvoice = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(a => a.ExtractInvoice == null &&
                        //                         serialsOfInvoice.Select(e => e.SerialNumber).ToList().Contains(a.SerialNumber)).ToList();

                        //var SerialsCantReturn = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(a =>
                        //  ((MainInvoice.InvoiceTypeId == (int)DocumentType.Purchase) ? ((a.ExtractInvoice != null || a.IsDeleted
                        //                                 || a.IsAccridited) && a.AddedInvoice == MainInvoice.InvoiceType)// مش شرط ارجعه من نفس الفاتوره ممكن يكون اتباع واتضاف تانى
                        //                 : (a.ExtractInvoice == null && !a.IsDeleted))
                        //                    && serialsOfInvoice.Select(e => e.SerialNumber).Contains(a.SerialNumber))
                        //    .Select(a => a.SerialNumber);

                        var SerialsForReturn = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(a => a.StoreId== InvoiceDto.StoreId&&
                        ((MainInvoice.InvoiceTypeId == (int)DocumentType.Purchase) ? ((a.ExtractInvoice == null && !a.IsDeleted
                                                       && !a.IsAccridited))// && a.AddedInvoice == MainInvoice.InvoiceType)// مش شرط ارجعه من نفس الفاتوره ممكن يكون اتباع واتضاف تانى
                                       : (a.ExtractInvoice == null && !a.IsDeleted))
                                          && serialsOfInvoice.Select(e => e.SerialNumber).Contains(a.SerialNumber))
                          .Select(a => a.SerialNumber);

                        var serialsWillReturn = serialsOfInvoice.Where(a => (MainInvoice.InvoiceTypeId == (int)DocumentType.Purchase)?
                                                                  SerialsForReturn.Contains(a.SerialNumber) : !SerialsForReturn.Contains(a.SerialNumber));
                        foreach (var ser in serialsWillReturn)
                        {
                            var serial = new InvoiceSerialDto();
                            serial.SerialNumber = ser.SerialNumber;
                            serial.ItemId = ser.ItemId;
                         //   serial.InvoiceId = MainInvoice.InvoiceId;
                            invoiceDetailsDto.InvoiceSerialDtos.Add(serial);

                        }
                        invoiceDetailsDto.Quantity = invoiceDetailsDto.InvoiceSerialDtos.Count();
                    }

                    if (ItemUnitsDto_.Count() > 0)
                    {
                        invoiceDetailsDto.itemUnits.AddRange(ItemUnitsDto_.Where(a => a.ItemId == item.ItemId).ToList());
                    }
                }
                InvoiceDto.InvoiceDetails.Add(invoiceDetailsDto);

            }
            // recalculate system
            InvoiceResultCalculateDto calculatedData = await CalculationSystemService.StartCalculation(ParameterOfCalculation,true);
            // set result of calculation in  InvoiceDto
            InvoiceDto.TotalPrice = calculatedData.TotalPrice;
            InvoiceDto.TotalDiscountValue = calculatedData.TotalDiscountValue;
            InvoiceDto.TotalDiscountRatio = calculatedData.TotalDiscountRatio;
            InvoiceDto.TotalAfterDiscount = calculatedData.TotalAfterDiscount;
            InvoiceDto.Net = calculatedData.Net;
            InvoiceDto.Remain = calculatedData.Net;//المتبقي
            InvoiceDto.TotalVat = calculatedData.TotalVat;
            // set result of calculation in  InvoiceDto.PurchaseDetails
            for (int i = 0; i < InvoiceDto.InvoiceDetails.Count(); i++)
            {
                InvoiceDto.InvoiceDetails[i].SplitedDiscountRatio = calculatedData.itemsTotalList[i].SplitedDiscountRatio;
                InvoiceDto.InvoiceDetails[i].SplitedDiscountValue = calculatedData.itemsTotalList[i].SplitedDiscountValue;
                InvoiceDto.InvoiceDetails[i].DiscountRatio = calculatedData.itemsTotalList[i].DiscountRatio;
                InvoiceDto.InvoiceDetails[i].DiscountValue = calculatedData.itemsTotalList[i].DiscountValue;
                InvoiceDto.InvoiceDetails[i].VatValue = calculatedData.itemsTotalList[i].VatValue;
                InvoiceDto.InvoiceDetails[i].Total = calculatedData.itemsTotalList[i].ItemTotal;
                InvoiceDto.InvoiceDetails[i].VatValueForPrint = calculatedData.itemsTotalList[i].VatValueForPrint;
                

            }
            /*var OtherAdditionalDetails = InvoiceAdditionalCostsRelationRepositoryQuery.TableNoTracking.Include(a => a.InvoiceAdditionalCosts).Where(a => a.InvoiceId == MainInvoice.InvoiceId);
            foreach (var item in OtherAdditionalDetails)
            {
                var OtherAdditionDetail = new OtherAdditionListDto()
                {
                    AddtionalCostId = item.AddtionalCostId,
                    AddtionalCostNameAr = item.InvoiceAdditionalCosts.ArabicName,
                    AddtionalCostNameEn = item.InvoiceAdditionalCosts.LatinName,
                    Code = item.InvoiceAdditionalCosts.Code,
                    Amount = item.Amount,
                    AdditionalType = item.InvoiceAdditionalCosts.AdditionalType
                };
                InvoiceDto.OtherAdditionList.Add(OtherAdditionDetail);
            }*/

            listPurchase.Add(InvoiceDto);

            return new ResponseResult() { Data = listPurchase, Id = null, Result = Result.Success };


        }
    }

  
}
