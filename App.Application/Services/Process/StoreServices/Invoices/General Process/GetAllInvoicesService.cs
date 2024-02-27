using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.Invoices.General_Process
{
    public class GetAllInvoicesService : BaseClass, IGetAllInvoicesService
    {
        private readonly IRepositoryQuery<InvoiceDetails> InvoiceDetailsRepositoryQuery;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IRepositoryQuery<InvGeneralSettings> GeneralSettings;
        private readonly iUserInformation Userinformation;
        private readonly ISerialsService serialsService;
        private readonly IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery;
        private readonly IRepositoryQuery<InvSerialTransaction> serialsTransactionQuery;
        private readonly IRoundNumbers roundNumber;
        public GetAllInvoicesService(
                              IRepositoryQuery<InvoiceDetails> _InvoiceDetailsRepositoryQuery,
                              IGeneralAPIsService _GeneralAPIsService,
                              IRepositoryQuery<InvGeneralSettings> GeneralSettings, iUserInformation Userinformation,
                              ISerialsService serialsService, IRepositoryQuery<InvSerialTransaction> InvSerialTransactionRepositoryQuery,
                              IRepositoryQuery<InvSerialTransaction> serialsTransactionQuery, IRoundNumbers roundNumber,
                              IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceDetailsRepositoryQuery = _InvoiceDetailsRepositoryQuery;
            GeneralAPIsService = _GeneralAPIsService;
            httpContext = _httpContext;
            this.GeneralSettings = GeneralSettings;
            this.Userinformation = Userinformation;
            this.serialsService = serialsService;
            this.InvSerialTransactionRepositoryQuery = InvSerialTransactionRepositoryQuery;
            this.serialsTransactionQuery = serialsTransactionQuery;
            this.roundNumber = roundNumber;
        }
      
        public async void GetAllInvoices(List<InvoiceMaster> list, List<AllInvoiceDto> list2  )
        {
            UserInformationModel userInfo = await Userinformation.GetUserInformation();
        //    var setting =   GeneralSettings.TableNoTracking.First().Other_Decimals;

            //bool ShowOtherPersonsInvoice = true;
            //if (Lists.purchasesInvoicesList.Contains(list.FirstOrDefault().InvoiceTypeId))
            //    ShowOtherPersonsInvoice =

            var invoicesOfCurrentBranch = list.Where(a => a.BranchId == userInfo.CurrentbranchId );
            foreach (var item in invoicesOfCurrentBranch)
            {
                var InvoiceDto = new AllInvoiceDto()
                {
                    InvoiceId = item.InvoiceId,
                    InvoiceDate = item.InvoiceDate,
                    InvoiceType = item.InvoiceType,
                    InvoiceTypeId = item.InvoiceTypeId,
                    InvoiceSubTypesId = item.InvoiceSubTypesId,
                    BookIndex = item.BookIndex,
                    PaymentType = item.PaymentType,
                    IsDeleted = item.IsDeleted,
                    TotalPrice = item.Net,// Math.Round( item.Net, item.RoundNumber),
                    StoreId = item.StoreId, 
                    StoreNameAr = item.store.ArabicName,
                    StoreNameEn = item.store.LatinName,
                    ParentInvoiceCode = item.ParentInvoiceCode,
                    IsAccredited=item.IsAccredite,
                    IsReturn=item.IsReturn,
                    Discount = item.TotalDiscountValue,
                    paid=item.Paid,
                    isExpenses = item.isExpenses
                };

                if(!Lists.storesInvoicesList.Contains(item.InvoiceTypeId))
                {
                    InvoiceDto.PersonId = item.PersonId;
                    InvoiceDto.PersonNameAr = item.Person.ArabicName;
                    InvoiceDto.PersonNameEn = item.Person.LatinName ;
                    InvoiceDto.PersonStatus = item.Person.Status;
                    InvoiceDto.PersonEmail = item.Person.Email;
                }
              
                list2.Add(InvoiceDto);

               
            }

        }

   
    }
}
