using App.Application.Handlers.EntryFund.CustomerAndSuppliers.updateFunds.updateFundsGLRelation;
using App.Application.Handlers.Helper.SafesBanksFundService;
using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Application.Handlers.MultiCollectionReceipts;
using App.Application.Helpers.Service_helper.InvoicesIntegrationServices;
using App.Application.Services.Acquired_And_Premitted_Discount;
using App.Application.Services.Process.GLServices.ReceiptBusiness;
using App.Application.Services.Process.StoreServices.Invoices.Funds.ItemsFund.ItemFundGLIntegrationServices;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Entities.Process;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request;
using DocumentFormat.OpenXml.ExtendedProperties;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ERPTool.ReCreateInvoiceJournalEntry
{
    public class ReCreateInvoiceJournalEntryHandler : IRequestHandler<ReCreateInvoiceJournalEntryRequest, ResponseResult>
    {
        private readonly iInvoicesIntegrationService _iInvoicesIntegrationService;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<InvFundsCustomerSupplier> _InvFundsCustomerSupplierQuery;
        private readonly IRepositoryQuery<InvFundsBanksSafesMaster> _InvFundsBanksSafesMasterQuery;
        private readonly iItemFundGLIntegrationService _iItemFundGLIntegrationService;
        private readonly IMediator _mediator;
        private readonly iUserInformation _iUserInformation;
        private readonly IReceiptsService _IReceiptsService;
        private readonly IDiscountService _IDiscountService;

        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IRepositoryQuery<GLJournalEntry> GLJournalEntryQuery;
        private readonly IRepositoryCommand<GLJournalEntryDetails> GLJournalEntryDetailsCommand;
        private readonly IRepositoryQuery<InvDiscount_A_P> Discount_A_PQuery;

        public ReCreateInvoiceJournalEntryHandler(iInvoicesIntegrationService iInvoicesIntegrationService, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, iItemFundGLIntegrationService iItemFundGLIntegrationService, IMediator mediator, IRepositoryQuery<InvFundsBanksSafesMaster> invFundsBanksSafesMasterQuery, iUserInformation iUserInformation, IRepositoryQuery<GlReciepts> glRecieptsQuery, IReceiptsService iReceiptsService, IRepositoryQuery<InvPersons> invPersonsQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GLBank> gLBankQuery, IHistoryInvoiceService historyInvoiceService, IRepositoryQuery<GLJournalEntry> gLJournalEntryQuery, IRepositoryCommand<GLJournalEntryDetails> gLJournalEntryDetailsCommand, IDiscountService iDiscountService, IRepositoryQuery<InvDiscount_A_P> discount_A_PQuery)
        {
            _iInvoicesIntegrationService = iInvoicesIntegrationService;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _iItemFundGLIntegrationService = iItemFundGLIntegrationService;
            _mediator = mediator;
            _InvFundsBanksSafesMasterQuery = invFundsBanksSafesMasterQuery;
            _iUserInformation = iUserInformation;
            _GlRecieptsQuery = glRecieptsQuery;
            _IReceiptsService = iReceiptsService;
            _InvPersonsQuery = invPersonsQuery;
            _GLSafeQuery = gLSafeQuery;
            _GLBankQuery = gLBankQuery;
            HistoryInvoiceService = historyInvoiceService;
            GLJournalEntryQuery = gLJournalEntryQuery;
            GLJournalEntryDetailsCommand = gLJournalEntryDetailsCommand;
            _IDiscountService = iDiscountService;
            Discount_A_PQuery = discount_A_PQuery;
        }
        public async Task<ResponseResult> Handle(ReCreateInvoiceJournalEntryRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            if (request.type == ERPTool_CreateJournalEntry.puchases)
            {
                var invoices = _InvoiceMasterQuery.TableNoTracking.Where(c => c.InvoiceTypeId == (int)Enums.DocumentType.Purchase || c.InvoiceTypeId == (int)Enums.DocumentType.ReturnPurchase);
                foreach (var invoice in invoices)
                {
                    var res = await _iInvoicesIntegrationService.InvoiceJournalEntryIntegration(new PurchasesJournalEntryIntegrationDTO()
                    {
                        total = invoice.TotalPrice,
                        discount = invoice.TotalDiscountValue,
                        VAT = invoice.TotalVat,
                        net = invoice.ActualNet,
                        personId = invoice.PersonId,
                        invoiceId = invoice.InvoiceId,
                        InvoiceCode = invoice.InvoiceType,
                        isIncludeVAT = invoice.PriceWithVat,
                        DocType = (DocumentType)invoice.InvoiceTypeId,
                        isAllowedVAT = invoice.ApplyVat,
                        isUpdate = true,
                        invDate = invoice.InvoiceDate,
                        branchId = invoice.BranchId,
                        note = invoice.Notes

                    });
                    if (res.Result != Result.Success)
                    {
                        return new ResponseResult
                        {
                            Result = Result.Success
                        };
                    }
                }
            }
            else if (request.type == ERPTool_CreateJournalEntry.sales)
            {
                var invoices = _InvoiceMasterQuery.TableNoTracking.Where(c => c.InvoiceTypeId == (int)Enums.DocumentType.SafeFunds || c.InvoiceTypeId == (int)Enums.DocumentType.ReturnSales);
                foreach (var invoice in invoices)
                {
                    var res = await _iInvoicesIntegrationService.InvoiceJournalEntryIntegration(new PurchasesJournalEntryIntegrationDTO()
                    {
                        total = invoice.TotalPrice,
                        discount = invoice.TotalDiscountValue,
                        VAT = invoice.TotalVat,
                        net = invoice.ActualNet,
                        personId = invoice.PersonId,
                        invoiceId = invoice.InvoiceId,
                        InvoiceCode = invoice.InvoiceType,
                        isIncludeVAT = invoice.PriceWithVat,
                        DocType = (DocumentType)invoice.InvoiceTypeId,
                        isAllowedVAT = invoice.ApplyVat,
                        isUpdate = true,
                        invDate = invoice.InvoiceDate,
                        branchId = invoice.BranchId,
                        note = invoice.Notes

                    });
                    if (res.Result != Result.Success)
                    {
                        return new ResponseResult
                        {
                            Result = Result.Success
                        };
                    }
                }
            }
            else if (request.type == ERPTool_CreateJournalEntry.pos)
            {
                var invoices = _InvoiceMasterQuery.TableNoTracking.Where(c => c.InvoiceTypeId == (int)Enums.DocumentType.POS || c.InvoiceTypeId == (int)Enums.DocumentType.ReturnPOS);
                foreach (var invoice in invoices)
                {
                    var res = await _iInvoicesIntegrationService.InvoiceJournalEntryIntegration(new PurchasesJournalEntryIntegrationDTO()
                    {
                        total = invoice.TotalPrice,
                        discount = invoice.TotalDiscountValue,
                        VAT = invoice.TotalVat,
                        net = invoice.ActualNet,
                        personId = invoice.PersonId,
                        invoiceId = invoice.InvoiceId,
                        InvoiceCode = invoice.InvoiceType,
                        isIncludeVAT = invoice.PriceWithVat,
                        DocType = (DocumentType)invoice.InvoiceTypeId,
                        isAllowedVAT = invoice.ApplyVat,
                        isUpdate = true,
                        invDate = invoice.InvoiceDate,
                        branchId = invoice.BranchId,
                        note = invoice.Notes

                    });
                    if (res.Result != Result.Success)
                    {
                        return new ResponseResult
                        {
                            Result = Result.Success
                        };
                    }
                }
            }
            else if (request.type == ERPTool_CreateJournalEntry.entryFund)
            {
                //ItemFund 
                var invoices = _InvoiceMasterQuery.TableNoTracking.Where(c => c.InvoiceTypeId == (int)Enums.DocumentType.itemsFund).Include(c => c.InvoicesDetails);
                foreach (var invoice in invoices)
                {
                    await _iItemFundGLIntegrationService.AddItemFundJournalEntry(new ItemFundJournalEntryDTO()
                    {
                        documentId = invoice.InvoiceId,
                        isUpdate = true,
                        totalAmount = invoice.InvoicesDetails.Sum(x => x.Quantity * x.Price),
                        date = invoice.InvoiceDate,
                    });
                }

                var CustomerSupplierFund = _InvFundsCustomerSupplierQuery.TableNoTracking.Include(c => c.Person);

                //Customer Fund 
                var Customers = CustomerSupplierFund.Where(c => c.Person.Any(x => x.IsCustomer));
                var customersBranches = Customers.GroupBy(c => c.branchId).Select(c => c.FirstOrDefault());
                foreach (var item in customersBranches)
                {
                    await _mediator.Send(new updateFundsGLRelationRequest
                    {
                        supAndCustUpdateFunds = Customers.Select(c => new supAndCustUpdateFund { Credit = c.Credit, Debit = c.Debit, Id = c.Id }).ToList(),
                        date = DateTime.Now,
                        isCustomer = true,
                        isUpdate = true,
                        branchId = item.branchId
                    });
                }
                //Supplier Fund 
                var Suppliers = CustomerSupplierFund.Where(c => c.Person.Any(x => x.IsCustomer));
                var SuppliersBranches = Suppliers.GroupBy(c => c.branchId).Select(c => c.FirstOrDefault());
                foreach (var item in SuppliersBranches)
                {
                    await _mediator.Send(new updateFundsGLRelationRequest
                    {
                        supAndCustUpdateFunds = Suppliers.Select(c => new supAndCustUpdateFund { Credit = c.Credit, Debit = c.Debit, Id = c.Id }).ToList(),
                        date = DateTime.Now,
                        isCustomer = true,
                        isUpdate = true,
                        branchId = item.branchId
                    });
                }

                var safesAndBanksFund = _InvFundsBanksSafesMasterQuery.TableNoTracking;
                //safes Fund
                var safesFund = safesAndBanksFund.Where(c => c.IsSafe).Include(c => c.FundsDetails_B_S);
                foreach (var item in safesFund)
                {
                    await _mediator.Send(new SafesBanksFundServiceRequest
                    {
                        fundsDetailsList = item.FundsDetails_B_S.ToList(),
                        table = item,
                        userInfo = userInfo,
                        isUpdate = true
                    });
                }
                //bank fund
                var BnnksFund = safesAndBanksFund.Where(c => c.IsBank).Include(c => c.FundsDetails_B_S);
                foreach (var item in BnnksFund)
                {
                    await _mediator.Send(new SafesBanksFundServiceRequest
                    {
                        fundsDetailsList = item.FundsDetails_B_S.ToList(),
                        table = item,
                        userInfo = userInfo,
                        isUpdate = true
                    });
                }
                return new ResponseResult
                {
                    Result = Result.Success
                };
            }
            else if (request.type == ERPTool_CreateJournalEntry.recsAndSettlements)
            {
                var listOfRecs = Lists.ReceiptIds;
                var recs = _GlRecieptsQuery.TableNoTracking.Where(c => listOfRecs.Contains(c.RecieptTypeId));

                //recs
                var normalRecs = recs.Where(c => c.RecieptTypeId != (int)DocumentType.BankMultiCollectionReceipts || c.RecieptTypeId != (int)DocumentType.SafeMultiCollectionReceipts);
                foreach (var reciept in normalRecs)
                {
                    financialData data = await _IReceiptsService.getFinantialAccIdForAuthorty(reciept.Authority, reciept.BenefitId, reciept);

                    int financialIdOfSafeOfBank = 0;
                    _IReceiptsService.updateRecieptsInJournalEntry(new UpdateRecieptsRequest
                    {
                        Id = reciept.Id,
                        SafeID = reciept.SafeID,
                        BankId = reciept.BankId,
                        CollectionMainCode = reciept.CollectionMainCode,
                        PaperNumber = reciept.PaperNumber,
                        RecieptDate = reciept.RecieptDate,
                        Notes = reciept.Notes,
                        InvoiceNotes = reciept.InvoiceNotes,
                        Amount = reciept.Amount,
                        Authority = reciept.Authority,
                        PaymentMethodId = reciept.PaymentMethodId,
                        ChequeNumber = reciept.ChequeNumber,
                        ChequeBankName = reciept.ChequeBankName,
                        ChequeDate = reciept.ChequeDate,
                        RecieptTypeId = reciept.RecieptTypeId,
                        BranchId = reciept.BranchId,
                        subTypeId = reciept.SubTypeId,
                        UserId = reciept.UserId,
                        Creditor = reciept.Creditor,
                        Debtor = reciept.Debtor,
                        SalesManId = reciept.SalesManId,
                        IsIncludeVat = reciept.IsIncludeVat,
                        BenefitId = reciept.BenefitId,
                        fromInvoice = false,
                        FA_Id = reciept.FinancialAccountId ?? 0,
                        Deferre = reciept.Deferre,
                        isPartialPaid = reciept.isPartialPaid,
                        IsAccredit = reciept.IsAccredit,
                        ParentId = reciept.ParentId,
                        ParentTypeId = reciept.ParentTypeId,
                        Code = reciept.Code,
                        Serialize = reciept.Serialize
                    },
                    reciept,
                    financialIdOfSafeOfBank,
                    data);
                }

                //multi collection recs 
                MultiCollectionReceiptsHelper MultiRecsHelper = new MultiCollectionReceiptsHelper(_mediator, HistoryInvoiceService, _iUserInformation, _InvoiceMasterQuery, _GlRecieptsQuery, _InvPersonsQuery, _GLSafeQuery, _GLBankQuery);
                var MultiRecs = recs.Where(c => c.RecieptTypeId == (int)DocumentType.BankMultiCollectionReceipts || c.RecieptTypeId == (int)DocumentType.SafeMultiCollectionReceipts).Where(c => c.MultiCollectionReceiptParentId == null);
                var invoices = _InvoiceMasterQuery.TableNoTracking;
                foreach (var rec in MultiRecs)
                {
                    string note = "";
                    if (rec.SafeID != null)
                    {
                        note = "سند مجمع خزائن" + "_" + rec.RecieptType + (!string.IsNullOrEmpty(rec.Notes) ? "_" + rec.Notes : "");
                    }
                    else if (rec.BankId != null)
                    {
                        note = "سند مجمع بنوك" + "_" + rec.RecieptType + (!string.IsNullOrEmpty(rec.Notes) ? "_" + rec.Notes : "");
                    }
                    int bankOrSafeFAId = 0;
                    if (rec.SafeID != null)
                    {
                        bankOrSafeFAId = _GLSafeQuery.TableNoTracking.FirstOrDefault(c => c.Id == rec.SafeID).FinancialAccountId ?? 0;
                    }
                    else if (rec.BankId != null)
                    {
                        bankOrSafeFAId = _GLBankQuery.TableNoTracking.FirstOrDefault(c => c.Id == rec.BankId).FinancialAccountId ?? 0;

                    }
                    List<UpdateinvoiceForCollectionReceiptRequestList> recInvoices =
                        recs.Where(c => c.MultiCollectionReceiptParentId == rec.Id).Select(c => new UpdateinvoiceForCollectionReceiptRequestList
                        {
                            branchId = c.BranchId,
                            invoiceId = c.ParentId ?? 0,
                            paid = c.Amount,
                            signal = c.Signal
                        }).ToList();
                    MultiRecsHelper.JournalEntryIntegration(new MultiCollectionReceiptsHelper.JournalEntryIntegrationDTO
                    {
                        GLJournalEntryDetailsCommand = GLJournalEntryDetailsCommand,
                        GLJournalEntryQuery = GLJournalEntryQuery,
                        invoices = invoices,
                        isUpdate = true,
                        MastreRec = rec,
                        mediator = _mediator,
                        note = note,
                        recInvoices = recInvoices,
                        safeOrBankFAId = bankOrSafeFAId,
                        userinfo = userInfo
                    });
                }

                //Settlements
                var Discounts = Discount_A_PQuery.TableNoTracking;
                foreach (var item in Discounts)
                {
                    await _IDiscountService.UpdateJEntery(new UpdateDiscountRequest
                    {
                        Id = item.Id,
                        amountMoney = item.Amount,
                        Creditor = item.Creditor,
                        Debtor = item.Debtor,
                        DocDate = item.DocDate,
                        DocNumber = item.DocNumber,
                        DocType = item.DocType,
                        IsCustomer = item.IsCustomer,
                        Notes = item.Notes,
                        PaperNumber = item.PaperNumber,
                        Person = item.PersonId,
                        Refrience = item.Refrience
                    },userInfo,item.Code,item.Id);
                }
                return new ResponseResult
                {
                    Result = Result.Success
                };
            }
            return new ResponseResult
            {
                Result = Result.Failed
            };
        }
    }
}
