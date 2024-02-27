using App.Application.Helpers;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Process.Invoices;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Application.Services.Process.StoreServices.Invoices.AccrediteInvoice;
using App.Application.Services.Process.StoreServices.Invoices.General_Process.Serials;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Entities;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Request.POS;
using App.Domain.Models.Security.Authentication.Request.Invoices;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using App.Infrastructure.Helpers;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.StoreServices.Invoices.POS
{
    public class AddSuspensionInvoice : BaseClass, IAddSuspensionInvoice
    {
        private readonly IRepositoryCommand<POSInvoiceSuspension> InvoiceMasterRepositoryCommand;
        private readonly IRepositoryQuery<POSInvoiceSuspension> InvoiceMasterRepositoryQuery;
        private readonly IRepositoryQuery<InvPersons> PersonRepositorQuery;
        private readonly IRepositoryCommand<POSInvSuspensionDetails> InvoiceDetailsRepositoryCommand;
        private readonly IRepositoryQuery<POSInvSuspensionDetails> InvoiceDetailsRepositoryQuery;
        private readonly IRepositoryQuery<InvStpItemCardMaster> itemMasterQuery;

        private readonly ISecurityIntegrationService _securityIntegrationService;
         private readonly ICalculationSystemService CalcSystem;

        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IHttpContextAccessor httpContext;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ISerialsService serialsService;
        private readonly IAccrediteInvoice accrediteInvoice;
        private readonly iInvoicesIntegrationService _iInvoicesIntegrationService;

        private readonly iUserInformation Userinformation;
        public AddSuspensionInvoice(
                             IRepositoryCommand<POSInvoiceSuspension> _InvoiceMasterRepositoryCommand,
                             IRepositoryCommand<POSInvSuspensionDetails> _InvoiceDetailsRepositoryCommand,
                             IGeneralAPIsService _GeneralAPIsService,
                             IRepositoryQuery<InvPersons> _PersonRepositorQuery,
                             IRepositoryQuery<POSInvSuspensionDetails> InvoiceDetailsRepositoryQuery,
                             IRepositoryQuery<POSInvoiceSuspension> InvoiceMasterRepositoryQuery,
                              IWebHostEnvironment _hostingEnvironment,
                              ICalculationSystemService _CalcSystem,
                              ISecurityIntegrationService securityIntegrationService,
                              IFilesOfInvoices filesOfInvoices, IPaymentMethodsForInvoiceService paymentMethodsService,
                               ISerialsService serialsService, IRepositoryQuery<GlReciepts> recieptQuery,
                               iUserInformation Userinformation, IAccrediteInvoice accrediteInvoice,
                               iInvoicesIntegrationService iInvoicesIntegrationService,
                               IReceiptsFromInvoices receiptsInvoice, ISystemHistoryLogsService systemHistoryLogsService,
                               IRepositoryQuery<InvStpItemCardMaster> _itemMasterQuery,
       IHttpContextAccessor _httpContext) : base(_httpContext)
        {
            InvoiceMasterRepositoryCommand = _InvoiceMasterRepositoryCommand;
            InvoiceDetailsRepositoryCommand = _InvoiceDetailsRepositoryCommand;
            PersonRepositorQuery = _PersonRepositorQuery;
            httpContext = _httpContext;
            GeneralAPIsService = _GeneralAPIsService;
            this.InvoiceDetailsRepositoryQuery = InvoiceDetailsRepositoryQuery;
            this.InvoiceMasterRepositoryQuery = InvoiceMasterRepositoryQuery;
            this._hostingEnvironment = _hostingEnvironment;
            _securityIntegrationService = securityIntegrationService;
            this.serialsService = serialsService;
            this.Userinformation = Userinformation;
            this.accrediteInvoice = accrediteInvoice;
            _iInvoicesIntegrationService = iInvoicesIntegrationService;
            CalcSystem = _CalcSystem;
            itemMasterQuery = _itemMasterQuery;
        }



        public async Task<ResponseResult> SaveSuspensionInvoice(InvoiceSuspensionRequest parameter, InvGeneralSettings setting, SettingsOfInvoice SettingsOfInvoice, int invoiceTypeId, string InvoiceTypeName, int MainInvoiceId)
        {
            try
            {
                var security = await _securityIntegrationService.getCompanyInformation();
                if (!security.isInfinityNumbersOfInvoices)
                {
                    var invoicesCount = InvoiceMasterRepositoryQuery.TableNoTracking.Where(x => x.InvoiceTypeId == invoiceTypeId).Count();
                    if (invoicesCount >= security.AllowedNumberOfInvoices)
                        return new ResponseResult()
                        {
                            Note = Actions.YouHaveTheMaxmumOfInvoices,
                            Result = Result.Failed
                        };
                }
                UserInformationModel userInfo = await Userinformation.GetUserInformation();

                CalculationOfInvoiceParameter calculationOfInvoiceParameter = new CalculationOfInvoiceParameter()
                {
                    DiscountType = parameter.DiscountType,
                    InvoiceTypeId = invoiceTypeId,
                    ParentInvoice = parameter.ParentInvoiceCode,
                    TotalDiscountRatio = parameter.TotalDiscountRatio,
                    TotalDiscountValue = parameter.TotalDiscountValue,
                    PersonId = parameter.PersonId
                };

                foreach (var item in parameter.InvoiceDetails)
                {
                    var itemsInRequest = parameter.InvoiceDetails.Select(e => e.ItemId).Distinct().ToList();
                    var itemsInDb = itemMasterQuery.TableNoTracking.Where(a => itemsInRequest.Contains(a.Id)).ToList();
                    var itemMasterDb = itemsInDb.First(a => a.Id == item.ItemId);
                    item.ItemTypeId = itemMasterDb.TypeId;
                    item.ApplyVat = itemMasterDb.ApplyVAT;
                    item.VatRatio = itemMasterDb.VAT;
                    item.ItemCode = itemMasterDb.ItemCode;
                }

                Mapping.Mapper.Map<List<InvoiceDetailsRequest>, List<InvoiceDetailsAttributes>>(parameter.InvoiceDetails, calculationOfInvoiceParameter.itemDetails);
                var recalculate = await CalcSystem.StartCalculation(calculationOfInvoiceParameter);
                int count = 0;
                foreach (var item in parameter.InvoiceDetails)
                {
                    item.Quantity = recalculate.itemsTotalList[count].Quantity;
                    item.Price = recalculate.itemsTotalList[count].Price;
                    item.SplitedDiscountValue = recalculate.itemsTotalList[count].SplitedDiscountValue;
                    item.SplitedDiscountRatio = recalculate.itemsTotalList[count].SplitedDiscountRatio;
                    item.DiscountValue = recalculate.itemsTotalList[count].DiscountValue;
                    item.DiscountRatio = recalculate.itemsTotalList[count].DiscountRatio;
                    item.VatValue = recalculate.itemsTotalList[count].VatValue;
                   
                    item.Total = recalculate.itemsTotalList[count].ItemTotal;
                    count++;
                }


                int NextCode = Autocode(userInfo.CurrentbranchId, invoiceTypeId);

                InvoiceMasterRepositoryCommand.StartTransaction();

                parameter.SalesManId = parameter.SalesManId == 0 ? parameter.SalesManId = null : parameter.SalesManId;

                DateTime date = DateTime.Now;
                if (setting.Pos_EditingOnDate && parameter.InvoiceDate != null)
                    date = (DateTime)parameter.InvoiceDate;

                var invoice = new POSInvoiceSuspension()
                {
                    EmployeeId = userInfo.employeeId,
                    BranchId = userInfo.CurrentbranchId,
                    Code = NextCode,
                    InvoiceType = userInfo.CurrentbranchId + "-" + InvoiceTypeName + "-" + NextCode,
                    InvoiceTypeId = invoiceTypeId,
                    BookIndex = (parameter.BookIndex != null ? parameter.BookIndex.Trim() : parameter.BookIndex),
                    InvoiceDate = GeneralAPIsService.serverDate(date),
                    StoreId = parameter.StoreId,
                    PersonId = parameter.PersonId,
                    Notes = (parameter.Notes != null ? parameter.Notes.Trim() : parameter.Notes),
                    DiscountType = parameter.DiscountType,
                    Net = recalculate.Net,
                    Paid = parameter.Paid,
                    VirualPaid = parameter.VirualPaid,
                    Remain = parameter.Remain,
                    TotalDiscountRatio = recalculate.TotalDiscountRatio,
                    TotalVat = recalculate.TotalVat,
                    TotalDiscountValue = recalculate.TotalDiscountValue,
                    TotalPrice = recalculate.TotalPrice,
                    TotalAfterDiscount = recalculate.TotalAfterDiscount,
                    ParentInvoiceCode = parameter.ParentInvoiceCode,
                    SalesManId = parameter.SalesManId,
                    PriceListId = parameter.PriceListId,
                    

                };

                InvoiceMasterRepositoryCommand.AddWithoutSaveChanges(invoice);
                var saved = await InvoiceMasterRepositoryCommand.SaveAsync();
                if (!saved)
                    return new ResponseResult { Result = Result.Failed, Note = "Faild to save invoice" };

                if (MainInvoiceId == 0)
                    MainInvoiceId = invoice.InvoiceId;
                var invoiceDetailsList = new List<POSInvSuspensionDetails>();
                if (parameter.InvoiceDetails != null)
                {
                    if (parameter.InvoiceDetails.Count > 0)
                    {
                        int index = 0;
                        foreach (var item in parameter.InvoiceDetails)
                        {
                            index++;
                            var invoiceDetails = new POSInvSuspensionDetails();

                            invoiceDetails = await SuspensionInvItemDetails(invoice, item, setting.Other_Decimals, MainInvoiceId);
                            invoiceDetails.indexOfItem = index;
                            invoiceDetailsList.Add(invoiceDetails);
                        }

                        InvoiceDetailsRepositoryCommand.AddRange(invoiceDetailsList);

                        saved = await InvoiceDetailsRepositoryCommand.SaveAsync();
                        if (!saved)
                            return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };
                    }
                }

                InvoiceMasterRepositoryCommand.CommitTransaction();

                
            }
            catch(Exception ex)
            {
                return new ResponseResult { Result = Result.Failed, Note = "Faild in invoice details" };
                throw;
            }

            return new ResponseResult { Result = Result.Success };
        }

   

        public int Autocode(int BranchId, int invoiceType)
        {
            var Code = 1;
            Code = InvoiceMasterRepositoryQuery.GetMaxCode(e => e.Code, a => a.InvoiceTypeId == invoiceType && a.BranchId == BranchId);

            if (Code != null)
                Code++;

            return Code;
        }

        public async Task<POSInvSuspensionDetails> SuspensionInvItemDetails(POSInvoiceSuspension invoice, InvoiceDetailsRequest item, int setDecimal, int? ParentInvoiceId)
        {
            var invoiceDetails = new POSInvSuspensionDetails();

            Mapping.Mapper.Map<InvoiceDetailsRequest, POSInvSuspensionDetails>(item, invoiceDetails);

            invoiceDetails.InvoiceId = invoice.InvoiceId;
            invoiceDetails.ItemId = item.ItemId;
            invoiceDetails.Price = Math.Round(item.Price, setDecimal);
            invoiceDetails.Quantity = Math.Round(item.Quantity, setDecimal);
            invoiceDetails.Total = item.Total;
            if (item.ItemTypeId == (int)ItemTypes.Note)
            {
                invoiceDetails.UnitId = null;
                invoiceDetails.Units = null;
            }
            else
                invoiceDetails.UnitId = item.UnitId;
            invoiceDetails.Signal = GeneralAPIsService.GetSignal(invoice.InvoiceTypeId);
            invoiceDetails.ConversionFactor = item.ConversionFactor;
            invoiceDetails.AutoDiscount = item.AutoDiscount;
            invoiceDetails.DiscountRatio = item.DiscountRatio;
            invoiceDetails.DiscountValue = item.DiscountValue;
            invoiceDetails.ItemTypeId = item.ItemTypeId;
            invoiceDetails.SplitedDiscountRatio = item.SplitedDiscountRatio;
            invoiceDetails.SplitedDiscountValue = item.SplitedDiscountValue;
            invoiceDetails.VatRatio = item.VatRatio;
            invoiceDetails.VatValue = item.VatValue;
            invoiceDetails.Price = item.Price;
            invoiceDetails.SizeId = item.SizeId == 0 ? null : item.SizeId;
            invoiceDetails.SerialTexts = String.Join(',',item.ListSerials);

            if (item.ItemTypeId == (int)ItemTypes.Expiary)
                invoiceDetails.ExpireDate = item.ExpireDate;
            else
                invoiceDetails.ExpireDate = null;


            return invoiceDetails;
        }

    }
}
