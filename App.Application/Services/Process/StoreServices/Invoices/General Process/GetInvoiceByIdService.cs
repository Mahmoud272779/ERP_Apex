using App.Application.Handlers.Persons.GetPersonBalance;
using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.POS;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.Store.Reports.Purchases;
using App.Domain.Models.Response.General;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.settings;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
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

namespace App.Application.Services.Process.Invoices.Purchase
{
    public class GetInvoiceByIdService : BaseClass, IGetInvoiceByIdService
    {

        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceFiles> InvoiceFilesRepositoryQuery;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentsMethodsRepositoryQuery;
        private readonly IRepositoryQuery<InvPurchaseAdditionalCostsRelation> InvoiceAdditionalCostsRelationRepositoryQuery;
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly iUserInformation Userinformation;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery;
        private readonly IRoundNumbers roundNumbers;
        private readonly IMediator mediator;
        private readonly IPosService posService;
        private readonly IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery;

        public GetInvoiceByIdService(IRepositoryQuery<InvoiceMaster> _InvoiceMasterRepositoryQuery,
                              IRepositoryQuery<InvoiceDetails> _InvoiceDetailsRepositoryQuery,
                              IRepositoryQuery<InvoiceFiles> _InvoiceFilesRepositoryQuery,
                              IRepositoryQuery<InvoicePaymentsMethods> _InvoicePaymentsMethodsRepositoryQuery,
                              IRepositoryQuery<InvPurchaseAdditionalCostsRelation> _InvoiceAdditionalCostsRelationRepositoryQuery,
                              IRepositoryQuery<InvSerialTransaction> _InvSerialTransactionRepositoryQuery,
                              IGeneralAPIsService _GeneralAPIsService, iUserInformation Userinformation, IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery,
                              IHttpContextAccessor _httpContext, IRoundNumbers roundNumbers, IMediator mediator, IPosService posService, IRepositoryQuery<InvStpItemCardUnit> itemUnitsQuery) : base(_httpContext)
        {
            InvoiceMasterRepositoryQuery = _InvoiceMasterRepositoryQuery;
            InvoiceDetailsRepositoryQuery = _InvoiceDetailsRepositoryQuery;
            InvSerialTransactionRepositoryQuery = _InvSerialTransactionRepositoryQuery;
            GeneralAPIsService = _GeneralAPIsService;
            InvoicePaymentsMethodsRepositoryQuery = _InvoicePaymentsMethodsRepositoryQuery;
            InvoiceAdditionalCostsRelationRepositoryQuery = _InvoiceAdditionalCostsRelationRepositoryQuery;
            httpContext = _httpContext;
            InvoiceFilesRepositoryQuery = _InvoiceFilesRepositoryQuery;
            this.Userinformation = Userinformation;
            this.InvGeneralSettingsQuery = InvGeneralSettingsQuery;
            this.roundNumbers = roundNumbers;
            this.mediator = mediator;
            this.posService = posService;
            this.itemUnitsQuery = itemUnitsQuery;
        }
        public async Task<ResponseResult> GetInvoiceById(int InvoiceId, bool? isCopy)
        {
            return await  GetInvoiceById(InvoiceId, isCopy , false);
        }  
        public async Task<ResponseResult> GetInvoiceById(int InvoiceId, bool? isCopy,bool? ForIOS)
        {
            return await  GetInvoiceById(InvoiceId, isCopy , false,ForIOS);
        }
        public async Task<ResponseResult> GetInvoiceById(int InvoiceId,bool? isCopy, bool? forIncommingTransfer,bool? ForIOS)
        {   
             var listInvoice = new List<InvoiceDto>();
            var res =await  GetInvoiceDto(InvoiceId , isCopy,forIncommingTransfer, ForIOS);
            listInvoice.Add(res);
            if(res ==null )
                return new ResponseResult() {  Id = null, Result = Result.NoDataFound , ErrorMessageAr=ErrorMessagesAr.InvoiceNotExist , ErrorMessageEn=ErrorMessagesEn.InvoiceNotExist };
            else
                return new ResponseResult() { Data = listInvoice, Id = null, Result = Result.Success };
        }

        // separate it for return InvoiceDto For print
        public async Task<InvoiceDto> GetInvoiceDto(int InvoiceId, bool? isCopy)
        {
            return await GetInvoiceDto( InvoiceId,   isCopy ,  false,false);
        }
        public async Task<InvoiceDto> GetInvoiceDto(int InvoiceId,bool? isCopy , bool? forIncommingTransfer ,bool? ForIOS)
        {
            try
            {
                UserInformationModel userInfo = await Userinformation.GetUserInformation();

                var mainInvoiceId = new List<string>();
                if(isCopy.Value)
                {
                    mainInvoiceId = InvoiceMasterRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == InvoiceId && isCopy.Value &&
                     (Lists.deleteInvoiceAddingToStore.Contains(a.InvoiceTypeId) || Lists.deleteInvoiceExtractFromStore.Contains(a.InvoiceTypeId)))
                      .Select(a => a.ParentInvoiceCode).ToList();
                }

                var Invoice_ =   InvoiceMasterRepositoryQuery.TableNoTracking
                                                                .Include(a => a.store)
                                                                .Include(a => a.Branch)
                                                                .Include(a => a.Person)
                                                                .Include(a => a.salesMan)
                                                                .Include(a => a.Employee)
                                                                .Include(a=>a.storeTo)
                                                                .Where(q => (mainInvoiceId.Count() == 0 ? q.InvoiceId == InvoiceId : q.InvoiceType == mainInvoiceId.FirstOrDefault())
                                                                  /* && (forIncommingTransfer.Value?true: q.BranchId == userInfo.CurrentbranchId)*/).ToList();

                if (!Invoice_.Any())
                    return null;
                var setting = await InvGeneralSettingsQuery.GetByAsync(q => 1 == 1);

                var Invoice = Invoice_.First();

                if ((((Lists.purchasesInvoicesList.Contains(Invoice.InvoiceTypeId) || Lists.purchasesWithoutVatInvoicesList.Contains(Invoice.InvoiceTypeId)) && !userInfo.otherSettings.purchasesShowOtherPersonsInv ) 
                     || (Lists.salesInvoicesList.Contains(Invoice.InvoiceTypeId) && !userInfo.otherSettings.salesShowOtherPersonsInv)
                     || (Lists.POSInvoicesList.Contains(Invoice.InvoiceTypeId) && !userInfo.otherSettings.posShowOtherPersonsInv)) 
                       && Invoice.EmployeeId!= userInfo.employeeId)
                {
                    return null;
                }
                var InvoiceDto = new InvoiceDto();
                //    var listInvoice = new List<InvoiceDto>();
                
                InvoiceDto.InvoiceCode = Invoice.InvoiceType;
                InvoiceDto.isReturn=Invoice.IsReturn;

                InvoiceDto.TotalPrice = Invoice.TotalPrice;//Math.Round( Invoice.TotalPrice, setting.Other_Decimals);
                InvoiceDto.InvoiceId = Invoice.InvoiceId;
                InvoiceDto.StoreId = Invoice.StoreId;
                InvoiceDto.StoreIdTo = Invoice.StoreIdTo;
                InvoiceDto.StoreNameAr = (Invoice.InvoiceTypeId==(int)DocumentType.IncomingTransfer ? Invoice.storeTo.ArabicName : Invoice.store.ArabicName);
                InvoiceDto.StoreNameEn = (Invoice.InvoiceTypeId == (int)DocumentType.IncomingTransfer ? Invoice.storeTo.LatinName  : Invoice.store.LatinName );
                InvoiceDto.StoreStatus = Invoice.store.Status;
                InvoiceDto.BranchId = Invoice.BranchId;
                if(Invoice.storeTo!=null)
                {
                    InvoiceDto.StoreToNameAr = (Invoice.InvoiceTypeId == (int)DocumentType.IncomingTransfer ? Invoice.store.ArabicName : Invoice.storeTo.ArabicName);
                    InvoiceDto.StoreToNameEn = (Invoice.InvoiceTypeId==(int)DocumentType.IncomingTransfer ? Invoice.store.LatinName : Invoice.storeTo.LatinName);
                }
              

                InvoiceDto.BranchNameAr = Invoice.Branch.ArabicName;
                InvoiceDto.BranchNameEn = Invoice.Branch.LatinName;
                InvoiceDto.BranchAddressAr = Invoice.Branch.AddressAr;
                InvoiceDto.BranchAddressEn= Invoice.Branch.AddressEn;
                InvoiceDto.BranchPhoneNumber = Invoice.Branch.Phone;
                InvoiceDto.CommercialRegisterNumber = Invoice.Branch.Fax;

                InvoiceDto.BookIndex = Invoice.BookIndex;
                InvoiceDto.InvoiceTypeId = Invoice.InvoiceTypeId;
                InvoiceDto.Notes = Invoice.Notes;
                InvoiceDto.InvoiceDate = (isCopy.Value?DateTime.Now: Invoice.InvoiceDate ).ToString(defultData.datetimeFormat);
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
                InvoiceDto.PriceListId = Invoice.PriceListId;

                if (!Lists.storesInvoicesList.Contains(Invoice.InvoiceTypeId) && !Lists.transferStore.Contains(Invoice.InvoiceTypeId))
                {
                    InvoiceDto.PersonId = Invoice.PersonId;
                    InvoiceDto.PersonNameAr = Invoice.Person.ArabicName;
                    InvoiceDto.PersonNameEn = Invoice.Person.LatinName;
                    InvoiceDto.PersonStatus = Invoice.Person.Status;
                    InvoiceDto.PersonAddressAr = Invoice.Person.AddressAr;
                    InvoiceDto.PersonAddressEn = Invoice.Person.AddressEn;
                    InvoiceDto.PersonTaxNumber = Invoice.Person.TaxNumber;
                    InvoiceDto.PersonFax = Invoice.Person.Fax;
                    InvoiceDto.PersonEmail= Invoice.Person.Email;
                    InvoiceDto.PersonPhone = Invoice.Person.Phone;
                    InvoiceDto.PersonCreditLimit = Invoice.Person.CreditLimit;
                    InvoiceDto.PersonResponsibleAr= Invoice.Person.ResponsibleAr;
                    InvoiceDto.PersonResponsibleEn= Invoice.Person.ResponsibleEn;
                    InvoiceDto.PersonCode = Invoice.Person.Code;
                    InvoiceDto.PersonCreditPeriod = Invoice.Person.CreditPeriod;
                    InvoiceDto.PersonDiscountRatio = Invoice.Person.DiscountRatio;

                    InvoiceDto.PersonBuildingNumber = Invoice.Person.BuildingNumber;
                    InvoiceDto.PersonStreetName = Invoice.Person.StreetName;

                    InvoiceDto.PersonNeighborhood = Invoice.Person.Neighborhood;

                    InvoiceDto.PersonCity = Invoice.Person.City;

                    InvoiceDto.PersonCountry = Invoice.Person.Country;

                    InvoiceDto.PersonPostalNumber = Invoice.Person.PostalNumber;

                    if (setting.Other_ShowBalanceOfPerson && (((Invoice.InvoiceTypeId == (int)DocumentType.Purchase || Invoice.InvoiceTypeId == (int)DocumentType.wov_purchase)
                   && userInfo.otherSettings.purchaseShowBalanceOfPerson)
                   || ((Invoice.InvoiceTypeId == (int)DocumentType.Sales)
                   && userInfo.otherSettings.salesShowBalanceOfPerson)))
                    {
                        int authory = (int)AuthorityTypes.suppliers;

                        if (Invoice.InvoiceTypeId == (int)DocumentType.Sales)
                            authory = (int)AuthorityTypes.customers;
                        List<personsForBalanceDto> personsForBalance = new List<personsForBalanceDto>();
                        personsForBalance.Add(new personsForBalanceDto() { Id = InvoiceDto.PersonId.Value });
                      var res = await mediator.Send(new GetReceiptBalanceForBenifitForInvoicesRequest()
                             { AuthorityId = authory, persons = personsForBalance,fromGetInvoice=true,BranchId= userInfo .CurrentbranchId});
                        var data = (personsForBalanceDto)res.Data;
                        InvoiceDto.balance = data.balance;
                        InvoiceDto.isCreditor = data.isCreditor;
                    }

                }

                if (Lists.salesInvoicesList.Contains(Invoice.InvoiceTypeId))
                {
                    InvoiceDto.SalesManId = Invoice.SalesManId.Value;
                    InvoiceDto.SalesManNameAr = Invoice.salesMan.ArabicName;
                    InvoiceDto.SalesManNameEn = Invoice.salesMan.LatinName;
                }


                InvoiceDto.TotalDiscountValue = Invoice.TotalDiscountValue;// Math.Round( Invoice.TotalDiscountValue, setting.Other_Decimals);//قيمه الخصم
                InvoiceDto.TotalDiscountRatio = Invoice.TotalDiscountRatio;// Math.Round( Invoice.TotalDiscountRatio, setting.Other_Decimals);//نسبة الخصم
                InvoiceDto.Net = Invoice.Net;// Math.Round(Invoice.Net, setting.Other_Decimals);//الصافي
                InvoiceDto.Paid = Invoice.Paid;// Math.Round(Invoice.Paid, setting.Other_Decimals);//المدفوع 
                InvoiceDto.Remain = Invoice.Remain;// Math.Round(Invoice.Remain, setting.Other_Decimals);//المتبقي
                InvoiceDto.VirualPaid = Invoice.VirualPaid;// Math.Round(Invoice.VirualPaid, setting.Other_Decimals);//المدوفوع من العميل 
                InvoiceDto.TotalAfterDiscount = Invoice.TotalAfterDiscount;// Math.Round(Invoice.TotalAfterDiscount, setting.Other_Decimals); //اجمالي بعد الخصم
                InvoiceDto.TotalVat = Invoice.TotalVat;// Math.Round(Invoice.TotalVat, setting.Other_Decimals);//اجمالي قيمه الضريبه 
                InvoiceDto.ApplyVat = Invoice.ApplyVat;//يخضع للضريبه ام لا
                InvoiceDto.PriceWithVat = Invoice.PriceWithVat;//السعر شامل الضريبه ام لا
                InvoiceDto.DiscountType = Invoice.DiscountType;//نوع الخصم (اجمالي او على الصنف)
                InvoiceDto.PaymentType = Invoice.PaymentType;
                InvoiceDto.ActiveDiscount = Invoice.ActiveDiscount;
                InvoiceDto.ParentInvoiceCode = Invoice.ParentInvoiceCode;
                InvoiceDto.RoundNumber = Invoice.RoundNumber;
               InvoiceDto.IsCollectionReceipt = Invoice.IsCollectionReceipt;
                InvoiceDto.isDiscountRatio = Invoice.isDiscountRatio;
                InvoiceDto.isExpenses = Invoice.isExpenses;
                /*  if(isCopy.Value && setting.Vat_Active)
                  {
                      if ((InvoiceDto.InvoiceTypeId == (int)DocumentType.Purchase && !setting.Purchases_PriceIncludeVat) ||
                          (InvoiceDto.InvoiceTypeId == (int)DocumentType.POS && !setting.Pos_PriceIncludeVat) ||
                          (InvoiceDto.InvoiceTypeId == (int)DocumentType.Sales && !setting.Sales_PriceIncludeVat))
                          InvoiceDto.Net = roundNumbers.GetRoundNumber(InvoiceDto.TotalAfterDiscount + InvoiceDto.TotalVat, setting.Other_Decimals);
                      else
                          InvoiceDto.Net = roundNumbers.GetRoundNumber(InvoiceDto.TotalAfterDiscount , setting.Other_Decimals);

                  }*/

                var InvoiceDetails = InvoiceDetailsRepositoryQuery.TableNoTracking.Where(a => a.InvoiceId == InvoiceId && (a.parentItemId==null|| a.parentItemId ==0)) // لمنع عرض مكونات الصنف المركب
                    .Include(a => a.Units).Include(a => a.Items).OrderBy(a => a.indexOfItem).ToList();
                var ItemUnitsDto = new List<ItemUnitsDto>();

                if(ForIOS!=null && ForIOS.Value)
                {
                    var itemIds = InvoiceDetails.Select(a=>a.ItemId).ToList();
                    var itemUnits = posService.getItemUnitsForPOS(itemIds);
                   var itemUnits_ = (IQueryable<ItemUnitsDto>) itemUnits.Result.Data;
                    ItemUnitsDto = itemUnits_.ToList();
                }
      
                foreach (var item in InvoiceDetails)
                {
                    var InvoiceDetailsDto = new InvoiceDetailsDto()
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
                        Cost =item.Cost, //Math.Round(item.Cost, setting.Other_Decimals),
                        ItemTypeId = item.ItemTypeId,
                        MinimumPrice = item.MinimumPrice,
                        PriceList = item.PriceList,
                        ReturnQuantity = item.ReturnQuantity,//Math.Round(item.ReturnQuantity, setting.Other_Decimals),
                        SplitedDiscountRatio = item.SplitedDiscountRatio,// Math.Round(item.SplitedDiscountRatio, setting.Other_Decimals),
                        SplitedDiscountValue = item.SplitedDiscountValue,// Math.Round(item.SplitedDiscountValue, setting.Other_Decimals),
                        StatusOfTrans = item.StatusOfTrans,
                        Total =item.TotalWithOutSplitedDiscount, //Math.Round(item.Total, setting.Other_Decimals),
                        TransQuantity = item.TransQuantity,// Math.Round(item.TransQuantity, setting.Other_Decimals),
                        IndexOfItem = item.indexOfItem,
                        ApplyVat = item.Items.ApplyVAT,
                        isBalanceBarcode =string.IsNullOrEmpty( item.balanceBarcode) ? false : true,
                        balanceBarcode = item.balanceBarcode,
                        VatValueForPrint=item.VatValueForPrint,
                        isDiscountRatioItem=item.isDiscountRatioItem

                    };

                    if (item.Items != null)
                    {
                        if (item.Items.TypeId != (int)ItemTypes.Note)
                        {
                            InvoiceDetailsDto.UnitId = item.UnitId;
                            InvoiceDetailsDto.UnitNameAr = item.Units.ArabicName;
                            InvoiceDetailsDto.UnitNameEn = item.Units.LatinName;

                        }
                        if (item.Items.TypeId == (int)ItemTypes.Expiary)
                        {
                            if(isCopy.Value && !Lists.InvoicesTypeOfAddingToStore.Contains(Invoice.InvoiceTypeId))
                              InvoiceDetailsDto.Quantity = 0;
                            InvoiceDetailsDto.ExpireDate = Convert.ToDateTime(item.ExpireDate).ToString("yyyy-MM-dd");
                        }

                        //this is the problem i think
                        if (item.Items.TypeId == (int)ItemTypes.Serial)
                        {
                            if (isCopy.Value && Lists.QuantityNotCheckedInvoicesList.Contains(Invoice.InvoiceTypeId))
                            {
                                InvoiceDetailsDto.Quantity = 0;
                               InvoiceDto.InvoiceDetails.Add(InvoiceDetailsDto);
                                continue;
                            }
                

                            var serialsOfInvoice = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(q => (q.AddedInvoice == InvoiceDto.InvoiceCode || q.ExtractInvoice == InvoiceDto.InvoiceCode) && q.ItemId == item.ItemId &&
                                              (Lists.InvoicesTypeOfAddingToStore.Contains(Invoice.InvoiceTypeId) ? q.indexOfSerialForAdd == item.indexOfItem : q.indexOfSerialForExtract == item.indexOfItem)
                                        && (Lists.QuantityNotCheckedInvoicesList.Contains(Invoice.InvoiceTypeId) ? ! q.IsDeleted && q.ExtractInvoice!=Actions.deleteSerial : true));

                            if (Lists.InvoicesTypesOfExtractFromStore.Contains(Invoice.InvoiceTypeId)) // if added again??
                            {
                                var serials = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(q => q.ExtractInvoice == null &&//q.ItemId == item.ItemId &&
                                serialsOfInvoice.Select(a => a.SerialNumber).Contains(q.SerialNumber));

                                var serialsCanNotDelete = (serials.Count() > 0 ? serials.Where(a => a.AddedInvoice != null && a.ExtractInvoice == null) : null);

                               InvoiceDetailsDto.CanDelete = true;
                                foreach (var serial in serialsOfInvoice)
                                {
                                    var invSerial = new InvoiceSerialDto();
                                    invSerial.CanDelete = true;
                                   
                                    invSerial.SerialNumber = serial.SerialNumber;
                                    invSerial.ItemId = serial.ItemId;
                                    //      invSerial.InvoiceId = Invoice.InvoiceId;
                                    if (serialsCanNotDelete != null)
                                        if (serialsCanNotDelete.Select(a => a.SerialNumber).ToList().Contains(serial.SerialNumber))
                                        {
                                            invSerial.CanDelete = false;
                                            InvoiceDetailsDto.CanDelete = false;
                                        }

                                    InvoiceDetailsDto.InvoiceSerialDtos.Add(invSerial);
                                }

                            }
                            else if (Lists.InvoicesTypeOfAddingToStore.Contains(Invoice.InvoiceTypeId))
                            {
                                // check if  serials from invoice added again to store
                                var serialExistInStore = InvSerialTransactionRepositoryQuery.TableNoTracking.Where(a =>a.StoreId==Invoice.StoreId &&
                                    serialsOfInvoice.Select(a => a.SerialNumber).Contains(a.SerialNumber) && a.ExtractInvoice == null).Select(a => a.SerialNumber);

                                InvoiceDetailsDto.CanDelete = true;
                                foreach (var serial in serialsOfInvoice)
                                {
                                    var invSerial = new InvoiceSerialDto();
                                    invSerial.CanDelete = true;

                                    invSerial.SerialNumber = serial.SerialNumber;
                                    invSerial.ItemId = serial.ItemId;
                                    //      invSerial.InvoiceId = Invoice.InvoiceId;
                                    if (serial.ExtractInvoice != null && !serialExistInStore.Contains(serial.SerialNumber))
                                    {
                                        invSerial.CanDelete = false;
                                        InvoiceDetailsDto.CanDelete = false;
                                    }

                                    InvoiceDetailsDto.InvoiceSerialDtos.Add(invSerial);
                                }
                                if(InvoiceDto.InvoiceTypeId==(int)DocumentType.IncomingTransfer)
                                {
                                    var transferSerials = serialsOfInvoice.Where(a => a.StoreId == InvoiceDto.StoreId).Select(a=>a.SerialNumber).ToList();
                                    InvoiceDetailsDto.transferSerialDtos = String.Join(",", transferSerials);
                                }
                            }

                        }

                        if(ItemUnitsDto.Count()>0)
                        {
                            InvoiceDetailsDto.itemUnits.AddRange(ItemUnitsDto.Where(a => a.ItemId == item.ItemId).ToList());
                        }
                    }
                    InvoiceDto.InvoiceDetails.Add(InvoiceDetailsDto);
                }

                var PaymentDetails = InvoicePaymentsMethodsRepositoryQuery.TableNoTracking.Include(a => a.PaymentMethod).Where(a => a.InvoiceId == InvoiceId);
                foreach (var item in PaymentDetails)
                {
                    var PaymentDetail = new PaymentListDto()
                    {
                        PaymentMethodId = item.PaymentMethodId,
                        PaymentNameAr = item.PaymentMethod.ArabicName,
                        PaymentNameEn = item.PaymentMethod.LatinName,
                        Value = item.Value,//Math.Round( item.Value,setting.Other_Decimals),
                        Cheque = item.Cheque
                    };
                    InvoiceDto.PaymentsMethods.Add(PaymentDetail);
                }
                
                var OtherAdditionalDetails = InvoiceAdditionalCostsRelationRepositoryQuery.TableNoTracking.Include(a => a.InvoiceAdditionalCosts).Where(a => a.InvoiceId == InvoiceId);
                foreach (var item in OtherAdditionalDetails)
                {
                    var OtherAdditionDetail = new OtherAdditionListDto()
                    {
                        AddtionalCostId = item.AddtionalCostId,
                        AddtionalCostNameAr = item.InvoiceAdditionalCosts.ArabicName,
                        AddtionalCostNameEn = item.InvoiceAdditionalCosts.LatinName,
                        Code = item.InvoiceAdditionalCosts.Code,
                        Amount = item.Amount,
                        AdditionalType = item.InvoiceAdditionalCosts.AdditionalType,
                         Total=item.Total
                    };
                    InvoiceDto.OtherAdditionList.Add(OtherAdditionDetail);
                }

                var FilesDetails = InvoiceFilesRepositoryQuery.FindAll(q => q.InvoiceId == InvoiceId);
                if (FilesDetails.Count > 0)
                {
                    foreach (var item in FilesDetails)
                    {

                        var FileDetail = new FilesListDto()
                        {
                            FileId = item.InvoiceFileId,
                            InvoiceId = item.InvoiceId,
                            FileLink = item.FileLink,
                            FileExtensions = item.FileExtensions,
                            FileName = item.FileName
                        };
                        InvoiceDto.FilesDetails.Add(FileDetail);
                    }
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
