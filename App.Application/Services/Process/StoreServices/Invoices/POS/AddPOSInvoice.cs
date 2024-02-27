using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Application.Services.Process.Invoices.Purchase;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public class AddPOSInvoice : BaseClass, IAddPOSInvoice
    {
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private IAddInvoice AddInvoiceService;
        private readonly IGetInvoiceByIdService _getInvoiceByIdService;
        private SettingsOfInvoice SettingsOfInvoice;
        public AddPOSInvoice(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
            IRepositoryQuery<InvGeneralSettings> _InvGeneralSettingsRepositoryQuery,
            IAddInvoice _AddInvoiceService,
            IGetInvoiceByIdService getInvoiceByIdService,
            IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            InvGeneralSettingsRepositoryQuery = _InvGeneralSettingsRepositoryQuery;
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            AddInvoiceService = _AddInvoiceService;
            _getInvoiceByIdService = getInvoiceByIdService;
        }

        public async Task<ResponseResult> AddPOSReturnInvoice(InvoiceMasterRequest request)
        {
            try
            {

                if (request.InvoiceDetails == null)
                {

                    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = "you should send at least one item" };

                }
                var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

                //setting validation

                var MainInvoiceId = 0;
                if (!string.IsNullOrEmpty(request.ParentInvoiceCode))  // return with invoice
                {

                    // use old settings of the invoice
                    var MainInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceType == request.ParentInvoiceCode || a.Code.ToString() == request.ParentInvoiceCode).ToList();

                    SettingsOfInvoice = new SettingsOfInvoice
                    {
                        ActiveDiscount = setting.Pos_ActiveDiscount,
                        ActiveVat = MainInvoice.First().ApplyVat,
                        PriceIncludeVat = MainInvoice.First().PriceWithVat,
                        setDecimal = MainInvoice.First().RoundNumber,
                    };

                    MainInvoiceId = MainInvoice.First().InvoiceId;
                 

                }
                else  // return without invoice
                {
                    SettingsOfInvoice = new SettingsOfInvoice
                    {
                        ActiveDiscount = setting.Pos_ActiveDiscount,
                        ActiveVat = setting.Vat_Active,
                        PriceIncludeVat = setting.Pos_PriceIncludeVat,
                        setDecimal = setting.Other_Decimals,
                    };
                }

                var res = await AddInvoiceService.SaveInvoice(request, setting, SettingsOfInvoice, (int)DocumentType.ReturnPOS, Aliases.InvoicesCode.ReturnPOS, MainInvoiceId, null);
                
                return res;
            }

            catch (Exception ex)
            {

                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };
            }
        }

        public async Task<ResponseResult> AddPOSTotalReturnInvoice(int Id)
        {
            if (Id == 0)
                return new ResponseResult()
                {
                    Result = Result.RequiredData,
                    Data = null,
                    Id = null,
                    ErrorMessageEn = "Id must be entered",
                    ErrorMessageAr = "Id must be entered"
                };


            InvoiceDto inv = await _getInvoiceByIdService.GetInvoiceDto(Id, false);
            if (inv == null)
                return new ResponseResult()
                {
                    Result = Result.NoDataFound,
                    Data = null,
                    Id = null,
                    ErrorMessageEn = "No Data Found",
                    ErrorMessageAr = "No Data Found"
                };

            InvoiceMasterRequest request = new InvoiceMasterRequest()
            {
                BookIndex = inv.BookIndex,
                InvoiceDate = DateTime.Now,
                StoreId = inv.StoreId,
                Notes = inv.Notes,
                TotalPrice = inv.TotalPrice,
                SalesManId = inv.SalesManId,
                PriceListId = 0,
                PersonId = inv.PersonId,
                TotalDiscountValue = inv.TotalDiscountValue,
                TotalDiscountRatio = inv.TotalDiscountRatio,
                Net = inv.Net,
                Paid = inv.Paid,
                Remain = inv.Remain,
                VirualPaid = inv.VirualPaid,
                TotalAfterDiscount = inv.TotalAfterDiscount,
                TotalVat = inv.TotalVat,
                DiscountType = inv.DiscountType,
                ActiveDiscount = (bool)inv.ActiveDiscount,
                ApplyVat = (bool)inv.ApplyVat,

                PriceWithVat = (bool)inv.PriceWithVat,
                ParentInvoiceCode = inv.InvoiceType,
                InvoiceDetails = FillInvDetails(inv.InvoiceDetails),
                PaymentsMethods = inv.PaymentsMethods.Select(a => new PaymentList() { PaymentMethodId = a.PaymentMethodId,Cheque = a.Cheque,Value = a.Value}).ToList(),
            };

            var setting = await InvGeneralSettingsRepositoryQuery.GetByAsync(q => 1 == 1);

            var MainInvoiceId = 0;
            if (request.ParentInvoiceCode != "")  // return with invoice
            {

                // use old settings of the invoice
                var MainInvoice = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceType == request.ParentInvoiceCode || a.Code.ToString() == request.ParentInvoiceCode).ToList();

                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Pos_ActiveDiscount,
                    ActiveVat = MainInvoice.First().ApplyVat,
                    PriceIncludeVat = MainInvoice.First().PriceWithVat,
                    setDecimal = MainInvoice.First().RoundNumber,
                };

                MainInvoiceId = MainInvoice.First().InvoiceId;
              


            }
            else  // return without invoice
            {
                SettingsOfInvoice = new SettingsOfInvoice
                {
                    ActiveDiscount = setting.Pos_ActiveDiscount,
                    ActiveVat = setting.Vat_Active,
                    PriceIncludeVat = setting.Pos_PriceIncludeVat,
                    setDecimal = setting.Other_Decimals,
                };
            }
            
            var res = await AddInvoiceService.SaveInvoice(request, setting, SettingsOfInvoice, (int)DocumentType.ReturnPOS, Aliases.InvoicesCode.ReturnPOS, MainInvoiceId, null);
            return res;
        }

        private List<InvoiceDetailsRequest> FillInvDetails(List<InvoiceDetailsDto> details)
        {
            List < InvoiceDetailsRequest > invDetails = new List < InvoiceDetailsRequest >();
            foreach (var item in details)
            {
                InvoiceDetailsRequest req = new InvoiceDetailsRequest()
                {
                    ItemId = item.ItemId,
                    ItemCode = item.ItemCode,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = item.Total,
                    ItemTypeId = item.ItemTypeId,
                    UnitId = item.UnitId,
                    DiscountValue = item.DiscountValue,
                    DiscountRatio = item.DiscountRatio,
                    VatRatio = item.VatRatio,
                    VatValue = item.VatValue,
                    SplitedDiscountValue = item.SplitedDiscountValue,
                    SplitedDiscountRatio = item.SplitedDiscountRatio,
                    AutoDiscount = item.AutoDiscount,
                    ConversionFactor = item.ConversionFactor,
                    IndexOfItem = item.IndexOfItem,
                    ApplyVat = item.ApplyVat,
                    ListSerials = item.InvoiceSerialDtos.Select(a => a.SerialNumber).ToList(),
                    InvoiceId = item.InvoiceId,
                    balanceBarcode = item.balanceBarcode,
                    ExpireDate = Convert.ToDateTime(item.ExpireDate),
                    isBalanceBarcode = item.isBalanceBarcode,
                    TransQuantity = item.TransQuantity,
                    TransStatus = item.StatusOfTrans,
                    isDiscountRatioItem = item.isDiscountRatioItem,
                    VatValueForPrint=item.VatValueForPrint
                };
                invDetails.Add(req);
            }
            return invDetails;
        }
    }
}
