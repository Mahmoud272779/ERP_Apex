using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.Invoices.Purchase;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.OfferPrice.OfferPriceService
{
    public class GetTempInvoiceByIdService : BaseClass, IGetTempInvoiceByIdService
    {

        private readonly IRepositoryQuery<OfferPriceMaster> OfferPriceMasterRepositoryQuery;
        private readonly IRepositoryQuery<OfferPriceDetails> OfferPriceDetailsRepositoryQuery;
        private readonly IHttpContextAccessor httpContext;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery;

        public GetTempInvoiceByIdService(IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterRepositoryQuery,
                              IRepositoryQuery<OfferPriceDetails> _OfferPriceDetailsRepositoryQuery,
                              iUserInformation Userinformation, IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                              IHttpContextAccessor _httpContext, IRoundNumbers roundNumbers) : base(_httpContext)
        {
            OfferPriceMasterRepositoryQuery = _OfferPriceMasterRepositoryQuery;
            OfferPriceDetailsRepositoryQuery = _OfferPriceDetailsRepositoryQuery;
            httpContext = _httpContext;
            this.Userinformation = Userinformation;
            this.InvGeneralSettingsQuery = InvGeneralSettingsQuery;
        }
      
        public async Task<ResponseResult> GetInvoiceById(int InvoiceId, bool? isCopy)
        {
            var listInvoice = new List<InvoiceDto>();
            var res = await GetInvoiceDto(InvoiceId, isCopy);
            listInvoice.Add(res);
            return new ResponseResult() { Data = listInvoice, Id = null, Result = Result.Success };
        }

       
        public async Task<InvoiceDto> GetInvoiceDto(int InvoiceId, bool? isCopy)
        {
            try
            {
                UserInformationModel userInfo = await Userinformation.GetUserInformation();
                var mainInvoiceId = new List<string>();
                if (isCopy.Value)
                {
                    mainInvoiceId = OfferPriceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == InvoiceId && isCopy.Value )
                     //(Lists.deleteInvoiceAddingToStore.Contains(a.InvoiceTypeId) || Lists.deleteInvoiceExtractFromStore.Contains(a.InvoiceTypeId)))
                      .Select(a => a.ParentInvoiceCode).ToList();
                }

                var Invoice_ = OfferPriceMasterRepositoryQuery.TableNoTracking
                                                                .Include(a => a.store)
                                                                .Include(a => a.Branch)
                                                                .Include(a => a.Person)
                                                                .Include(a => a.salesMan)
                                                                .Include(a => a.Employee)
                                                                .Where(q => (mainInvoiceId.Count() == 0 ? q.InvoiceId == InvoiceId : q.InvoiceType == mainInvoiceId.FirstOrDefault())
                                                                  && userInfo.employeeBranches.Contains(q.BranchId) /*&&  q.BranchId == userInfo.CurrentbranchId*/).ToList();

                if (!Invoice_.Any())
                    return null;
                var setting = await InvGeneralSettingsQuery.GetByAsync(q => 1 == 1);

                var Invoice = Invoice_.First();

                var InvoiceDto = new InvoiceDto();
                //    var listInvoice = new List<InvoiceDto>();
                InvoiceDto.InvoiceCode = Invoice.InvoiceType;

                InvoiceDto.TotalPrice = Invoice.TotalPrice;//Math.Round( Invoice.TotalPrice, setting.Other_Decimals);
                InvoiceDto.InvoiceId = Invoice.InvoiceId;
                InvoiceDto.StoreId = Invoice.StoreId;
                InvoiceDto.StoreNameAr =  Invoice.store.ArabicName;
                InvoiceDto.StoreNameEn = Invoice.store.LatinName;
                InvoiceDto.StoreStatus = Invoice.store.Status;
                InvoiceDto.BranchId = Invoice.BranchId;
            


                InvoiceDto.BranchNameAr = Invoice.Branch.ArabicName;
                InvoiceDto.BranchNameEn = Invoice.Branch.LatinName;
                InvoiceDto.BookIndex = Invoice.BookIndex;
                InvoiceDto.InvoiceTypeId = Invoice.InvoiceTypeId;
                InvoiceDto.Notes = Invoice.Notes;
                InvoiceDto.InvoiceDate = (isCopy.Value ? DateTime.Now : Invoice.InvoiceDate).ToString(defultData.datetimeFormat);
                InvoiceDto.employeeId = Invoice.EmployeeId;
                InvoiceDto.EmployeeNameAr = Invoice.Employee.ArabicName;
                InvoiceDto.EmployeeNameEn = Invoice.Employee.LatinName;
                InvoiceDto.IsDeleted = Invoice.IsDeleted;
                InvoiceDto.IsAccredited = Invoice.IsAccredite;
                InvoiceDto.BrowserName = Invoice.BrowserName;
                InvoiceDto.Code = Invoice.Code;
                InvoiceDto.Serialize = Invoice.Serialize;
                InvoiceDto.InvoiceType = Invoice.InvoiceType;
                InvoiceDto.InvoiceSubTypesId = Invoice.InvoiceSubTypesId;
                InvoiceDto.PersonId = Invoice.PersonId;
                InvoiceDto.PersonNameAr = Invoice.Person.ArabicName;
                InvoiceDto.PersonNameEn = Invoice.Person.LatinName;
                InvoiceDto.PersonStatus = Invoice.Person.Status;
                InvoiceDto.PersonAddressAr = Invoice.Person.AddressAr;
                InvoiceDto.PersonTaxNumber = Invoice.Person.TaxNumber;
                InvoiceDto.PersonEmail = Invoice.Person.Email;
                InvoiceDto.PersonPhone = Invoice.Person.Phone;
                InvoiceDto.PersonResponsibleAr = Invoice.Person.ResponsibleAr;
                InvoiceDto.PersonResponsibleEn = Invoice.Person.ResponsibleEn;
                InvoiceDto.PersonCode = Invoice.Person.Code;
                InvoiceDto.PriceListId = Invoice.PriceListId;

                if (Invoice.InvoiceTypeId==(int)DocumentType.OfferPrice || Invoice.InvoiceTypeId == (int)DocumentType.DeleteOfferPrice )
                {
                    InvoiceDto.SalesManId = Invoice.SalesManId.Value;
                    InvoiceDto.SalesManNameAr = Invoice.salesMan.ArabicName;
                    InvoiceDto.SalesManNameEn = Invoice.salesMan.LatinName;
                    InvoiceDto.SalesManEmail = Invoice.salesMan.Email;
                    InvoiceDto.SalesManPhone= Invoice.salesMan.Phone;

                }
                   
               

                InvoiceDto.TotalDiscountValue = Invoice.TotalDiscountValue;// Math.Round( Invoice.TotalDiscountValue, setting.Other_Decimals);//قيمه الخصم
                InvoiceDto.TotalDiscountRatio = Invoice.TotalDiscountRatio;// Math.Round( Invoice.TotalDiscountRatio, setting.Other_Decimals);//نسبة الخصم
                InvoiceDto.Net = Invoice.Net;// Math.Round(Invoice.Net, setting.Other_Decimals);//الصافي
                InvoiceDto.TotalAfterDiscount = Invoice.TotalAfterDiscount;// Math.Round(Invoice.TotalAfterDiscount, setting.Other_Decimals); //اجمالي بعد الخصم
                InvoiceDto.TotalVat = Invoice.TotalVat;// Math.Round(Invoice.TotalVat, setting.Other_Decimals);//اجمالي قيمه الضريبه 
                InvoiceDto.ApplyVat = Invoice.ApplyVat;//يخضع للضريبه ام لا
                InvoiceDto.PriceWithVat = Invoice.PriceWithVat;//السعر شامل الضريبه ام لا
                InvoiceDto.DiscountType = Invoice.DiscountType;//نوع الخصم (اجمالي او على الصنف)
                InvoiceDto.PaymentType = Invoice.PaymentType;
                InvoiceDto.ActiveDiscount = Invoice.ActiveDiscount;
                InvoiceDto.ParentInvoiceCode = Invoice.ParentInvoiceCode;
                InvoiceDto.RoundNumber = Invoice.RoundNumber;
                InvoiceDto.isDiscountRatio = Invoice.isDiscountRatio;

                /*  if(isCopy.Value && setting.Vat_Active)
                  {
                      if ((InvoiceDto.InvoiceTypeId == (int)DocumentType.Purchase && !setting.Purchases_PriceIncludeVat) ||
                          (InvoiceDto.InvoiceTypeId == (int)DocumentType.POS && !setting.Pos_PriceIncludeVat) ||
                          (InvoiceDto.InvoiceTypeId == (int)DocumentType.Sales && !setting.Sales_PriceIncludeVat))
                          InvoiceDto.Net = roundNumbers.GetRoundNumber(InvoiceDto.TotalAfterDiscount + InvoiceDto.TotalVat, setting.Other_Decimals);
                      else
                          InvoiceDto.Net = roundNumbers.GetRoundNumber(InvoiceDto.TotalAfterDiscount , setting.Other_Decimals);

                  }*/

                var OfferPriceDetails = OfferPriceDetailsRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == InvoiceId && (a.parentItemId == null || a.parentItemId == 0)) // لمنع عرض مكونات الصنف المركب
                    .Include(a => a.Units).Include(a => a.Items).OrderBy(a => a.indexOfItem).ToList();
                foreach (var item in OfferPriceDetails)
                {
                    var OfferPriceDetailsDto = new InvoiceDetailsDto()
                    {
                        Id=item.Id,
                        InvoiceId = item.InvoiceId,
                        ItemId = item.ItemId,
                        ItemNameAr = item.Items.ArabicName,
                        ItemNameEn = item.Items.LatinName,
                        ItemCode = item.Items.ItemCode,
                        Price = item.Price,// Math.Round(item.Price, setting.Other_Decimals),

                        Quantity = item.Quantity,// Math.Round(item.Quantity, setting.Other_Decimals),
                        //UnitId = item.UnitId,
                        //UnitName = item.Units.ArabicName,
                        DiscountValue = item.DiscountValue,//Math.Round(item.DiscountValue, setting.Other_Decimals),//قيمه الخصم على مستوى الصنف
                        DiscountRatio = item.DiscountRatio,//Math.Round(item.DiscountRatio, setting.Other_Decimals),// نسبه الخصم على مستوى الصنف
                        VatRatio = item.VatRatio,//Math.Round(item.VatRatio, setting.Other_Decimals),// الضريبه على مستوى الصنف 
                        VatValue = item.VatValue,// Math.Round(item.VatValue, setting.Other_Decimals),// الضريبه على مستوى الصنف 
                        Signal = item.Signal,
                        AutoDiscount = item.AutoDiscount,
                        AvgPrice = item.AvgPrice,//Math.Round(item.AvgPrice, setting.Other_Decimals),
                        ConversionFactor = item.ConversionFactor,
                        Cost = item.Cost, //Math.Round(item.Cost, setting.Other_Decimals),
                        ItemTypeId = item.ItemTypeId,
                        MinimumPrice = item.MinimumPrice,
                        PriceList = item.PriceList,
                        ReturnQuantity = item.ReturnQuantity,//Math.Round(item.ReturnQuantity, setting.Other_Decimals),
                        SplitedDiscountRatio = item.SplitedDiscountRatio,// Math.Round(item.SplitedDiscountRatio, setting.Other_Decimals),
                        SplitedDiscountValue = item.SplitedDiscountValue,// Math.Round(item.SplitedDiscountValue, setting.Other_Decimals),
                        StatusOfTrans = item.StatusOfTrans,
                        Total = item.TotalWithOutSplitedDiscount, //Math.Round(item.Total, setting.Other_Decimals),
                        TransQuantity = item.TransQuantity,// Math.Round(item.TransQuantity, setting.Other_Decimals),
                        IndexOfItem = item.indexOfItem,
                        ApplyVat = item.Items.ApplyVAT,
                        isBalanceBarcode = item.balanceBarcode == null ? false : true,
                        balanceBarcode = item.balanceBarcode,
                        isDiscountRatioItem= item.isDiscountRatioItem,
                        VatValueForPrint = item.VatValueForPrint

                    };

                    if (item.Items != null)
                    {
                        if (item.Items.TypeId != (int)ItemTypes.Note)
                        {
                            OfferPriceDetailsDto.UnitId = item.UnitId;
                            OfferPriceDetailsDto.UnitNameAr = item.Units.ArabicName;
                            OfferPriceDetailsDto.UnitNameEn = item.Units.LatinName;

                        }
                        
                    }
                    InvoiceDto.InvoiceDetails.Add(OfferPriceDetailsDto);
                }

                return InvoiceDto;

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
