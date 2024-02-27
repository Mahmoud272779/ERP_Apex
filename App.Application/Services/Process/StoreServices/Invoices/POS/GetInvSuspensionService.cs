using App.Application.Helpers;
using App.Application.Services.Process.Invoices.General_Process;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Response.POS;
using App.Domain.Models.Response.Store.Invoices;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public class GetInvSuspensionService : BaseClass, IGetInvSuspensionService
    {
        private readonly IRepositoryQuery<POSInvoiceSuspension> POSInvoiceSuspensionQuery;
        private readonly IRepositoryCommand<POSInvoiceSuspension> _POSInvoiceSuspensionCommand;
        private readonly IHttpContextAccessor httpContext;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvPersons>   _invPersonsQuery;
        private readonly IRepositoryQuery<POSInvSuspensionDetails> _POSInvSuspensionDetailsQuery;
        private readonly IRepositoryCommand<POSInvSuspensionDetails> _POSInvSuspensionDetailsCommand;
        private readonly IRepositoryQuery<InvStpStores> _invStpStoresQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> _invStpItemCardMasterQuery;
        private readonly IRepositoryQuery<InvStpUnits> _InvStpUnitsQuery;
        private readonly IRepositoryQuery<GLBranch> _gLBranchQuery;
        private readonly IRepositoryQuery<InvSalesMan> _invSalesManQuery;

        public GetInvSuspensionService(IRepositoryQuery<POSInvoiceSuspension> _POSInvoiceSuspensionQuery,
            iUserInformation Userinformation,
            IRepositoryQuery<InvPersons> InvPersonsQuery,
            IRepositoryQuery<POSInvSuspensionDetails> POSInvSuspensionDetailsQuery,
            IRepositoryCommand<POSInvSuspensionDetails> POSInvSuspensionDetailsCommand,
            IRepositoryQuery<InvStpStores> InvStpStoresQuery,
            IRepositoryQuery<InvStpItemCardMaster> invStpItemCardMasterQuery,
            IRepositoryQuery<InvStpUnits> invStpUnitsQuery,
            IRepositoryCommand<POSInvoiceSuspension> POSInvoiceSuspensionCommand,
            IRepositoryQuery<GLBranch> gLBranchQuery,
            IRepositoryQuery<InvSalesMan> invSalesManQuery,
            IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            POSInvoiceSuspensionQuery = _POSInvoiceSuspensionQuery;
            httpContext = _httpContext;
            this.Userinformation = Userinformation;
            _invPersonsQuery = InvPersonsQuery;
            _invStpStoresQuery = InvStpStoresQuery;
            _POSInvSuspensionDetailsQuery = POSInvSuspensionDetailsQuery;
            _invStpItemCardMasterQuery = invStpItemCardMasterQuery;
            _InvStpUnitsQuery = invStpUnitsQuery;
            _gLBranchQuery = gLBranchQuery;
            _invSalesManQuery = invSalesManQuery;
            _POSInvoiceSuspensionCommand = POSInvoiceSuspensionCommand;
            _POSInvSuspensionDetailsCommand = POSInvSuspensionDetailsCommand;
        }

        public async Task<ResponseResult> GetSuspensionInvoices(int? PageNumber, int? PageSize)
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();

            var treeData = POSInvoiceSuspensionQuery.TableNoTracking.Include(a => a.POSInvSuspensionDetails)
                .Where(q => q.BranchId == userInfo.CurrentbranchId).OrderByDescending(a => a.Code);
                
            var count = treeData.Count();

            if (count == 0)
                return new ResponseResult()
                {
                    Result = Result.NoDataFound,
                    ErrorMessageAr = "No data found",
                    ErrorMessageEn = "No data found"
                };

            List<POSInvoiceSuspension> response = new List<POSInvoiceSuspension>();
            if (PageSize > 0 && PageNumber > 0)
            {
                response = treeData.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize).ToList();
            }
            else
               response = treeData.ToList();

            var list2 = new List<POSInvoiceDTO>();


            if (treeData == null || count == 0)
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            }
       
            await GetAllInvoices(response, list2, userInfo);
            List<SuspensionInvoiceInfoDTO> res = new List<SuspensionInvoiceInfoDTO>();
            foreach (var inv in list2)
            {
                res.Add(new SuspensionInvoiceInfoDTO()
                {
                    InvoiceId = inv.InvoiceId,
                    InvoiceDate = inv.InvoiceDate,
                    InvoiceType = inv.InvoiceType,
                    TotalPrice = inv.TotalPrice,
                    Discount = inv.Discount,
                    InvoiceSubTypesId = inv.InvoiceSubTypesId,
                    arabicName = inv.PersonNameAr,
                    latinName = inv.PersonNameEn,
                    itemsCount = inv.ItemsQuantity
                });
            }
            return new ResponseResult() { Id = null, Data = res, DataCount = res.Count, Result = res.Count > 0 ? Result.Success : Result.NoDataFound, Note = "", TotalCount = count };


        }

        public async Task GetAllInvoices(List<POSInvoiceSuspension> list, List<POSInvoiceDTO> list2, UserInformationModel userInfo)
        {

            var invoicesOfCurrentBranch = list.Where(a => a.BranchId == userInfo.CurrentbranchId).ToList();
            foreach (var item in invoicesOfCurrentBranch)
            {
                var store = await _invStpStoresQuery.GetByIdAsync(item.StoreId);
                var person = await _invPersonsQuery.GetByIdAsync(item.PersonId);

                int itemsQuantity = _POSInvSuspensionDetailsQuery.TableNoTracking.Where(a => a.InvoiceId == item.InvoiceId).Count();
                var InvoiceDto = new POSInvoiceDTO()
                {
                    InvoiceId = item.InvoiceId,
                    InvoiceDate = item.InvoiceDate,
                    InvoiceType = item.InvoiceType,
                    InvoiceTypeId = item.InvoiceTypeId,
                    InvoiceSubTypesId = item.InvoiceSubTypesId,
                    BookIndex = item.BookIndex,
                    IsDeleted = item.IsDeleted,
                    TotalPrice = item.TotalPrice,
                    StoreId = item.StoreId,
                    StoreNameAr = store.ArabicName,
                    StoreNameEn = store.LatinName,
                    ParentInvoiceCode = item.ParentInvoiceCode,
                    IsAccredited = item.IsAccredite,
                    ItemsQuantity = itemsQuantity,
                    Discount = item.TotalDiscountValue,
                    PersonId = item.PersonId,
                    PersonNameAr = person.ArabicName,
                    PersonNameEn = person.LatinName,
                    PersonStatus = person.Status,
                };

                //if (!Lists.storesInvoicesList.Contains(item.InvoiceTypeId))
                //{
                //    InvoiceDto.PersonId     = item.PersonId;
                //    InvoiceDto.PersonNameAr = person.ArabicName;
                //    InvoiceDto.PersonNameEn = person.LatinName;
                //    InvoiceDto.PersonStatus = person.Status;
                //}

                list2.Add(InvoiceDto);
            }
        }

        public async Task<ResponseResult> GetSuspensionInvoicesById(int Id)
        {
            InvoiceDto invoice = await GetInvoiceDto(Id);

            if(invoice == null)
                return new ResponseResult { Result = Result.NoDataFound, Data = null, Id = 0, ErrorMessageAr = "No Data Found", ErrorMessageEn = "No Data Found" };
            return new ResponseResult() { Result = Result.Success,Data = invoice};
        }

        public async Task<InvoiceDto> GetInvoiceDto(int InvoiceId)
        {
            try
            {
                UserInformationModel userInfo = await Userinformation.GetUserInformation();
                var Invoice = await POSInvoiceSuspensionQuery.SingleOrDefault(q => q.InvoiceId == InvoiceId && q.BranchId == userInfo.CurrentbranchId);

                if (Invoice == null)
                    return null;

                var store = await _invStpStoresQuery.GetByIdAsync(Invoice.StoreId);
                var person = await _invPersonsQuery.GetByIdAsync(Invoice.PersonId);
                var salesman = await _invSalesManQuery.GetByIdAsync(Invoice.SalesManId);
                var branch = await _gLBranchQuery.GetByIdAsync(Invoice.BranchId);

                var InvoiceDto = new InvoiceDto();

                InvoiceDto.InvoiceCode = Invoice.InvoiceType;
                InvoiceDto.InvoiceId = Invoice.InvoiceId;
                InvoiceDto.StoreId = Invoice.StoreId;
                InvoiceDto.StoreNameAr = store.ArabicName;
                InvoiceDto.StoreNameEn = store.LatinName;
                InvoiceDto.StoreStatus = store.Status;
                InvoiceDto.BranchId = Invoice.BranchId;
                InvoiceDto.BranchNameAr = branch.ArabicName;
                InvoiceDto.BranchNameEn = branch.LatinName;
                InvoiceDto.BookIndex = Invoice.BookIndex;
                InvoiceDto.InvoiceTypeId = Invoice.InvoiceTypeId;
                InvoiceDto.Notes = Invoice.Notes;
                if (!Lists.storesInvoicesList.Contains(Invoice.InvoiceTypeId))
                {
                    InvoiceDto.PersonId = Invoice.PersonId;
                    InvoiceDto.PersonNameAr = person.ArabicName;
                    InvoiceDto.PersonNameEn = person.LatinName;
                    InvoiceDto.PersonStatus = person.Status;
                    InvoiceDto.PersonAddressAr = person.AddressAr;
                    InvoiceDto.PersonTaxNumber = person.TaxNumber;
                }

                if (Lists.salesInvoicesList.Contains(Invoice.InvoiceTypeId))
                {
                    InvoiceDto.SalesManId = Invoice.SalesManId.Value;
                    InvoiceDto.SalesManNameAr = salesman.ArabicName;
                    InvoiceDto.SalesManNameEn = salesman.LatinName;
                }

                InvoiceDto.TotalDiscountValue = Invoice.TotalDiscountValue;//قيمه الخصم
                InvoiceDto.TotalDiscountRatio = Invoice.TotalDiscountRatio;//نسبة الخصم
                InvoiceDto.Net = Invoice.Net;//الصافي
                InvoiceDto.Paid = Invoice.Paid;//المدفوع 
                InvoiceDto.Remain = Invoice.Remain;//المتبقي
                InvoiceDto.VirualPaid = Invoice.VirualPaid;//المدوفوع من العميل 
                InvoiceDto.TotalAfterDiscount = Invoice.TotalAfterDiscount; //اجمالي بعد الخصم
                InvoiceDto.TotalVat = Invoice.TotalVat;//اجمالي قيمه الضريبه 
                InvoiceDto.ApplyVat = Invoice.ApplyVat;//يخضع للضريبه ام لا
                InvoiceDto.PriceWithVat = Invoice.PriceWithVat;//السعر شامل الضريبه ام لا
                InvoiceDto.DiscountType = Invoice.DiscountType;//نوع الخصم (اجمالي او على الصنف)
                InvoiceDto.ActiveDiscount = Invoice.ActiveDiscount;
                InvoiceDto.ParentInvoiceCode = Invoice.ParentInvoiceCode;
                InvoiceDto.TotalPrice = Invoice.TotalPrice;



                var InvoiceDetails = _POSInvSuspensionDetailsQuery.TableNoTracking.Where(a => a.InvoiceId == InvoiceId).OrderBy(a => a.indexOfItem).ToList();


                foreach (var item in InvoiceDetails)
                {
                    var unit = await _InvStpUnitsQuery.GetByIdAsync(item.UnitId);
                    var itemCard = await _invStpItemCardMasterQuery.GetByIdAsync(item.ItemId);

                    var InvoiceDetailsDto = new InvoiceDetailsDto()
                    {
                        InvoiceId = item.InvoiceId,
                        ItemId = item.ItemId,
                        ItemNameAr = itemCard.ArabicName,
                        ItemNameEn = itemCard.LatinName,
                        ItemCode = itemCard.ItemCode,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        DiscountValue = item.DiscountValue,//قيمه الخصم على مستوى الصنف
                        DiscountRatio = item.DiscountRatio,// نسبه الخصم على مستوى الصنف
                        VatRatio = item.VatRatio,// الضريبه على مستوى الصنف 
                        VatValue = item.VatValue,// الضريبه على مستوى الصنف 
                        Signal = item.Signal,
                        AutoDiscount = item.AutoDiscount,
                        AvgPrice = item.AvgPrice,
                        ConversionFactor = item.ConversionFactor,
                        Cost = item.Cost,
                        ItemTypeId = item.ItemTypeId,
                        MinimumPrice = item.MinimumPrice,
                        PriceList = item.PriceList,
                        ReturnQuantity = item.ReturnQuantity,
                        SplitedDiscountRatio = item.SplitedDiscountRatio,
                        SplitedDiscountValue = item.SplitedDiscountValue,
                        StatusOfTrans = item.StatusOfTrans,
                        Total = item.Total,
                        TransQuantity = item.TransQuantity,
                        IndexOfItem = item.indexOfItem,
                        ApplyVat = itemCard.ApplyVAT,
                        ExpireDate = item.ExpireDate == null ? null : Convert.ToDateTime(item.ExpireDate).ToString("yyyy-MM-dd"),
                        InvoiceSerialDtos = item.SerialTexts.Split(new char[] {','}).Select(a => new InvoiceSerialDto() { ItemId=0,SerialNumber=a}).ToList()


                    };


                    InvoiceDetailsDto.UnitId = item.UnitId;
                    InvoiceDetailsDto.UnitNameAr = unit.ArabicName;
                    InvoiceDetailsDto.UnitNameEn = unit.LatinName;

                    InvoiceDto.InvoiceDetails.Add(InvoiceDetailsDto);
                }

                _POSInvSuspensionDetailsCommand.RemoveRange(InvoiceDetails);
                var isDeleted = await _POSInvoiceSuspensionCommand.SaveAsync();

                if(isDeleted)
                {
                    _POSInvoiceSuspensionCommand.Remove(Invoice);
                    isDeleted = await _POSInvoiceSuspensionCommand.SaveAsync();
                }

                if (!isDeleted)
                    return null;

                return InvoiceDto;

            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
