using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Response.Store;
//using App.Application.Services.Process.JournalEntries;
using Attendleave.Erp.Core.APIUtilities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace App.Application.Helpers.Service_helper.InvoicesIntegrationServices
{
    public class InvoicesIntegrationService : iInvoicesIntegrationService
    {
        //private readonly IJournalEntryBusiness _journalEntryBusiness;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<GLPurchasesAndSalesSettings> _invoiceQuery;
        private readonly IRepositoryQuery<InvoiceDetails> _invoiceDetailsQuery;
        private readonly IRepositoryQuery<GLBank> _gLBankQuery;
        private readonly IRepositoryQuery<GLJournalEntry> _gLJournalEntryQuery;
        private readonly IRepositoryQuery<GLJournalEntryDetails> _gLJournalEntrydetailsQuery;
        private readonly IRepositoryQuery<InvStpItemCardUnit> InvStpItemCardUnitQuery;
        private readonly IRepositoryCommand<GLJournalEntry> _gLJournalEntryCommand;
        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> _invSettingsQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _GLGeneralSettingQuery;
        private readonly IRepositoryQuery<InvPaymentMethods> _invPaymentMethodsQuery;
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;
        private readonly IMediator _mediator;
        private readonly IRoundNumbers _roundNumbers;
        public int stockFA_Id = 28;
        public int SalesCostFA_Id = 58;
        public InvoicesIntegrationService(
                                            //IJournalEntryBusiness journalEntryBusiness,
                                            IHttpContextAccessor httpContext,
                                            ISecurityIntegrationService securityIntegrationService,
                                            iUserInformation iUserInformation,
                                            IRepositoryQuery<GLPurchasesAndSalesSettings> invoiceQuery,
                                            IRepositoryQuery<GLBank> GLBankQuery,
                                            IRepositoryQuery<GLJournalEntry> GLJournalEntryQuery,
                                            IRepositoryQuery<GLSafe> GLSafeQuery,
                                            IRepositoryQuery<InvGeneralSettings> invSettingsQuery,
                                            IRepositoryQuery<InvPaymentMethods> InvPaymentMethodsQuery,
                                            IRepositoryQuery<InvPersons> InvPersonsQuery,
                                            IRepositoryCommand<GLJournalEntry> gLJournalEntryCommand,
                                            IMediator mediator,
                                            IRepositoryQuery<InvStpItemCardUnit> invStpItemCardUnitQuery,
                                            IRepositoryQuery<GLJournalEntryDetails> gLJournalEntrydetailsQuery,
                                            IRepositoryQuery<InvoiceDetails> invoiceDetailsQuery,
                                            IRoundNumbers roundNumbers,
                                            IRepositoryQuery<GLGeneralSetting> gLGeneralSettingQuery)
        {
            //_journalEntryBusiness = journalEntryBusiness;
            _httpContext = httpContext;
            _securityIntegrationService = securityIntegrationService;
            _iUserInformation = iUserInformation;
            _invoiceQuery = invoiceQuery;
            _gLBankQuery = GLBankQuery;
            _gLJournalEntryQuery = GLJournalEntryQuery;
            _gLSafeQuery = GLSafeQuery;
            _invSettingsQuery = invSettingsQuery;
            _invPaymentMethodsQuery = InvPaymentMethodsQuery;
            _invPersonsQuery = InvPersonsQuery;
            _gLJournalEntryCommand = gLJournalEntryCommand;
            _mediator = mediator;
            InvStpItemCardUnitQuery = invStpItemCardUnitQuery;
            _gLJournalEntrydetailsQuery = gLJournalEntrydetailsQuery;
            _invoiceDetailsQuery = invoiceDetailsQuery;
            _roundNumbers = roundNumbers;
            _GLGeneralSettingQuery = gLGeneralSettingQuery;
        }

        public List<JournalEntryDetail> SetJournalEntryDetails(PurchasesJournalEntryIntegrationDTO parm, GenralData generalData, int JEnteryId)
        {


            int DisountFA_Id = 0;
            int InvocieFA_Id = 0;
            int VATFA_Id = 0;

            if (parm.isIncludeVAT)
                parm.total = parm.total - parm.VAT;

            if (Lists.purchasesWithOutDeleteInvoicesList.Contains((int)parm.DocType))
            {
                DisountFA_Id = generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.AcquiredDiscount).FirstOrDefault().FinancialAccountId ?? 0;
                VATFA_Id = generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.VATPurchase).FirstOrDefault().FinancialAccountId ?? 0;
                InvocieFA_Id = generalData.glSettings
                    .Where(x => parm.DocType == DocumentType.Purchase ? x.RecieptsType == (int)DocumentType.Purchase
                    : parm.DocType == DocumentType.ReturnPurchase ? x.RecieptsType == (int)DocumentType.ReturnPurchase
                   : parm.DocType == DocumentType.wov_purchase ? x.RecieptsType == (int)DocumentType.wov_purchase
                   : x.RecieptsType == (int)DocumentType.ReturnWov_purchase)
                    .FirstOrDefault().FinancialAccountId ?? 0;
            }
            else if (parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.ReturnSales)
            {
                DisountFA_Id = generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.PermittedDiscount).FirstOrDefault().FinancialAccountId ?? 0;


                VATFA_Id = generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.VATSale).FirstOrDefault().FinancialAccountId ?? 0;


                InvocieFA_Id = generalData.glSettings
                    .Where(x =>
                    parm.DocType == DocumentType.Sales ?
                    x.RecieptsType == (int)DocumentType.Sales
                    :
                    x.RecieptsType == (int)DocumentType.ReturnSales)
                    .FirstOrDefault().FinancialAccountId ?? 0;
            }
            else if (parm.DocType == DocumentType.POS || parm.DocType == DocumentType.ReturnPOS)
            {
                DisountFA_Id = generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.PermittedDiscount).FirstOrDefault().FinancialAccountId ?? 0;


                VATFA_Id = generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.VATSale).FirstOrDefault().FinancialAccountId ?? 0;


                InvocieFA_Id = generalData.glSettings
                    .Where(x =>
                    parm.DocType == DocumentType.POS ?
                    x.RecieptsType == (int)DocumentType.POS
                    :
                    x.RecieptsType == (int)DocumentType.ReturnPOS)
                    .FirstOrDefault().FinancialAccountId ?? 0;
            }
            else if (parm.DocType == DocumentType.AddPermission || parm.DocType == DocumentType.ExtractPermission)
            {

                InvocieFA_Id = generalData.glSettings.Where(x => parm.DocType == DocumentType.AddPermission ?
                    (x.RecieptsType == (int)DocumentType.AddPermission && x.ReceiptElemntID == (int)FA_Nature.Credit)
                    :
                    (x.RecieptsType == (int)DocumentType.ExtractPermission && x.ReceiptElemntID == (int)FA_Nature.Debit))
                    .FirstOrDefault().FinancialAccountId ?? 0;
            }

            var InvoiceJournalEntryDetails = new List<JournalEntryDetail>();
            //Disount
            if (parm.discount != 0 && generalData.gLGeneralSettingQuery.AddDiscountInJournalEntryForInvoics)
            {
                InvoiceJournalEntryDetails.Add(new JournalEntryDetail()
                {
                    Credit = parm.DocType == DocumentType.Purchase || parm.DocType == DocumentType.wov_purchase || parm.DocType == DocumentType.ReturnSales || parm.DocType == DocumentType.ReturnPOS ? parm.discount : 0,
                    Debit = parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase || parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.POS ? parm.discount : 0,
                    DescriptionAr = generalData.invoiceNameAr + " - " + parm.InvoiceCode,
                    DescriptionEn = generalData.invoiceNameAr + " - " + parm.InvoiceCode,
                    FinancialAccountId = DisountFA_Id,
                    JournalEntryId = parm.journalEntryId != null ? parm.journalEntryId.Value : 0
                });

            }

            //person
            if (generalData.person != null)
            {
                InvoiceJournalEntryDetails.Add(new JournalEntryDetail()
                {
                    Credit = parm.DocType == DocumentType.Purchase || parm.DocType == DocumentType.wov_purchase || parm.DocType == DocumentType.ReturnSales || parm.DocType == DocumentType.ReturnPOS ? parm.net : 0,
                    Debit = parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase || parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.POS ? parm.net : 0,
                    DescriptionAr = generalData.invoiceNameAr + " - " + parm.InvoiceCode,
                    DescriptionEn = generalData.invoiceNameEn + " - " + parm.InvoiceCode,
                    FinancialAccountId = generalData.person.FinancialAccountId,
                    JournalEntryId = parm.journalEntryId != null ? parm.journalEntryId.Value : 0

                });
            }
            //Permissions
            if (parm.DocType == DocumentType.AddPermission || parm.DocType == DocumentType.ExtractPermission)
            {


                InvoiceJournalEntryDetails.Add(new JournalEntryDetail()
                {
                    Credit = parm.DocType == DocumentType.AddPermission ? 0 : parm.net,
                    Debit = parm.DocType == DocumentType.AddPermission ? parm.net : 0,
                    DescriptionAr = generalData.invoiceNameAr + " - " + parm.InvoiceCode,
                    DescriptionEn = generalData.invoiceNameEn + " - " + parm.InvoiceCode,
                    FinancialAccountId = parm.DocType == DocumentType.AddPermission ?
                                                            generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.AddPermission && x.ReceiptElemntID == (int)FA_Nature.Debit).FirstOrDefault().FinancialAccountId ?? 0 :
                                                            generalData.glSettings.Where(x => x.RecieptsType == (int)DocumentType.ExtractPermission && x.ReceiptElemntID == (int)FA_Nature.Credit).FirstOrDefault().FinancialAccountId ?? 0,
                    JournalEntryId = parm.journalEntryId != null ? parm.journalEntryId.Value : 0


                });
            }
            //VAT
            if (parm.isAllowedVAT)
                if (parm.VAT != 0)
                    InvoiceJournalEntryDetails.Add(new JournalEntryDetail()
                    {
                        Credit = parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase ||
                                                        parm.DocType == DocumentType.Sales ||
                                                        parm.DocType == DocumentType.POS
                                                        ?
                                                        parm.VAT : 0,


                        Debit = parm.DocType == DocumentType.Purchase || parm.DocType == DocumentType.wov_purchase ||
                                                        parm.DocType == DocumentType.ReturnSales ||
                                                        parm.DocType == DocumentType.ReturnPOS
                                                        ?
                                                        parm.VAT : 0,



                        DescriptionAr = generalData.invoiceNameAr + " - " + parm.InvoiceCode,
                        DescriptionEn = generalData.invoiceNameEn + " - " + parm.InvoiceCode,
                        FinancialAccountId = VATFA_Id,
                        JournalEntryId = parm.journalEntryId != null ? parm.journalEntryId.Value : 0

                    });




            //invoice
            int test = 0;
            InvoiceJournalEntryDetails.Add(new JournalEntryDetail()
            {
                Credit = parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase ||
                                        parm.DocType == DocumentType.Sales ||
                                        parm.DocType == DocumentType.AddPermission ||
                                        parm.DocType == DocumentType.POS
                                        ?
                                        (generalData.gLGeneralSettingQuery.AddDiscountInJournalEntryForInvoics ? parm.total : parm.total - parm.discount) : 0,




                Debit = parm.DocType == DocumentType.Purchase || parm.DocType == DocumentType.wov_purchase ||
                                        parm.DocType == DocumentType.ReturnSales ||
                                        parm.DocType == DocumentType.ExtractPermission ||
                                        parm.DocType == DocumentType.ReturnPOS
                                        ?
                                        (generalData.gLGeneralSettingQuery.AddDiscountInJournalEntryForInvoics ? parm.total : parm.total - parm.discount) : 0,

                DescriptionAr = generalData.invoiceNameAr + " - " + parm.InvoiceCode,
                DescriptionEn = generalData.invoiceNameEn + " - " + parm.InvoiceCode,
                FinancialAccountId = InvocieFA_Id,
                JournalEntryId = parm.journalEntryId != null ? parm.journalEntryId.Value : 0

            });

            // تكلفه المبيعات 
            if (Lists.SalesTransaction.Where(c => c != (int)DocumentType.ExtractPermission).Contains((int)parm.DocType))
            {
                if (parm.isUpdate)
                {
                    var details = _gLJournalEntrydetailsQuery.TableNoTracking.Where(a => a.JournalEntryId == JEnteryId && a.isCostSales).ToList();
                    List<JournalEntryDetail> invoiceDetails = new List<JournalEntryDetail>();
                    foreach (var item in details)
                    {
                        invoiceDetails.Add(new JournalEntryDetail()
                        {
                            FinancialAccountId = item.FinancialAccountId,
                            Credit = item.Credit,
                            Debit = item.Debit,
                            DescriptionAr = item.DescriptionAr,
                            DescriptionEn = item.DescriptionEn,
                            isCostSales = item.isCostSales,
                            JournalEntryId = item.JournalEntryId // parm.journalEntryId.Value

                        });
                    }
                    InvoiceJournalEntryDetails.AddRange(invoiceDetails);
                }
                else
                {
                    //فى حاله الاضافه
                    var elem = GetJurnalDetailForSalesCost(parm);
                    InvoiceJournalEntryDetails.AddRange(elem);
                }
            }
            if (parm.journalEntryId != null)
                InvoiceJournalEntryDetails.Select(a => a.JournalEntryId = parm.journalEntryId.Value).ToList();

            return InvoiceJournalEntryDetails;
        }

        public async Task<ResponseResult> InvoiceJournalEntryIntegration(PurchasesJournalEntryIntegrationDTO parm)
        {
            var journalEntry = _gLJournalEntryQuery.TableNoTracking.Where(x => x.InvoiceId == parm.invoiceId).FirstOrDefault();
            if (parm.isDelete)
            {
                if (journalEntry == null)
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        Note = Actions.DeleteFailed
                    };

                await _mediator.Send(new BlockJournalEntryReqeust
                {
                    Ids = new int[] { journalEntry.Id }
                });

                return new ResponseResult()
                {
                    Result = Result.Success,
                    Note = Actions.Success
                };
            }

            var generalData = Data(parm.DocType, parm.personId);
            var InvoiceJournalEntryDetails = SetJournalEntryDetails(parm, generalData, journalEntry == null ? 0 : journalEntry.Id);

            IRepositoryActionResult saved = null;
            if (!parm.isUpdate || journalEntry == null)
            {
                saved = await _mediator.Send(new AddJournalEntryRequest
                {
                    JournalEntryDetails = InvoiceJournalEntryDetails,
                    isAuto = true,
                    InvoiceId = parm.invoiceId,
                    FTDate = parm.invDate,
                    BranchId = generalData.userinfo.CurrentbranchId,
                    Notes = generalData.invoiceNameAr + " - " + parm.InvoiceCode + (!string.IsNullOrEmpty(parm.note) ? "_" + parm.note : ""),
                    DocType = (int)parm.DocType,
                });
            }
            else
            {
                saved = await _mediator.Send(new UpdateJournalEntryRequest
                {
                    Id = journalEntry.Id,
                    journalEntryDetails = InvoiceJournalEntryDetails,
                    fromSystem = true,
                    FTDate = parm.invDate,
                    BranchId = generalData.userinfo.CurrentbranchId,
                    Notes = generalData.invoiceNameAr + " - " + parm.InvoiceCode + (!string.IsNullOrEmpty(parm.note) ? "_" + parm.note : "")
                });
            }


            return new ResponseResult()
            {
                Data = saved,
                Result = Result.Success
            };
        }
        public async Task<ResponseResult> SalesInvoiceCostJournalEntryIntegration(InvoiceCostJournalEntryIntegrationDTO parm)
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            string descAr = "";
            string descEn = "";
            if (parm.DocType == DocumentType.Sales)
            {
                descAr = "تكلفه فاتورة بيع رقم _ " + parm.InvoiceCode;
                descAr = "Cost of sales invoice number_ " + parm.InvoiceCode;
            }
            else if (parm.DocType == DocumentType.ReturnSales)
            {
                descAr = "تكلفه مرتجع فاتورة بيع رقم _ " + parm.InvoiceCode;
                descAr = "Cost of ReturnSales invoice number_ " + parm.InvoiceCode;
            }
            var InvoiceCostJournalEntryDetails = new List<JournalEntryDetail>();
            InvoiceCostJournalEntryDetails.Add(new JournalEntryDetail()
            {
                Credit = parm.DocType == DocumentType.Sales ? 0 : parm.Cost,
                Debit = parm.DocType == DocumentType.Sales ? parm.Cost : 0,
                DescriptionAr = descAr,
                DescriptionEn = descEn,
                FinancialAccountId = SalesCostFA_Id
            });
            InvoiceCostJournalEntryDetails.Add(new JournalEntryDetail()
            {
                Credit = parm.DocType == DocumentType.Sales ? parm.Cost : 0,
                Debit = parm.DocType == DocumentType.Sales ? 0 : parm.Cost,
                DescriptionAr = descAr,
                DescriptionEn = descEn,
                FinancialAccountId = stockFA_Id
            });

            //var saved = await _journalEntryBusiness.AddJournalEntry(new JournalEntryParameter()
            //{
            //    JournalEntryDetails = InvoiceCostJournalEntryDetails,
            //    isAuto = true,
            //    InvoiceId = parm.invoiceId,
            //    FTDate = DateTime.Now,
            //    BranchId = userinfo.CurrentbranchId,
            //    Notes = descAr + " - " + parm.InvoiceCode,
            //    DocType = (int)parm.DocType
            //});
            var saved = await _mediator.Send(new AddJournalEntryRequest
            {
                JournalEntryDetails = InvoiceCostJournalEntryDetails,
                isAuto = true,
                InvoiceId = parm.invoiceId,
                FTDate = DateTime.Now,
                BranchId = userinfo.CurrentbranchId,
                Notes = descAr + " - " + parm.InvoiceCode,
                DocType = (int)parm.DocType
            });
            return new ResponseResult()
            {
                Data = saved
            };
        }
        //getlast invoice
        private SelectionData GetLastInvoicesCost(int itemId, int sizeId, double serialize, int branchId)
        {
            var _Lastinvoices = _invoiceDetailsQuery.TableNoTracking.Include(c => c.InvoicesMaster)
                .Where(h => h.InvoicesMaster.BranchId == branchId && h.ItemId == itemId && h.SizeId == (sizeId == 0 ? null : sizeId) && h.InvoicesMaster.Serialize < serialize && h.Cost > 0)
                .OrderBy(a => a.InvoiceId).LastOrDefault();
            if (_Lastinvoices == null)
                return new SelectionData() { Cost = 0, AvgPrice = 0 };
            SelectionData Lastinvoices = new SelectionData
            {
                AvgPrice = _Lastinvoices.AvgPrice,
                Cost = _Lastinvoices.Cost,
                invoiceId = _Lastinvoices.InvoiceId
            };


            return Lastinvoices ?? new SelectionData() { Cost = 0, AvgPrice = 0 };
        }
        public async Task<ResponseResult> invoicePaymentJournalEntryIntegration(invoicePaymentJournalEntryDTO parm)
        {
            var userinfo = await _iUserInformation.GetUserInformation();
            var paymentMethod = _invPaymentMethodsQuery.TableNoTracking.Where(x => x.PaymentMethodId == parm.paymentMethodId).FirstOrDefault();
            var data = Data(parm.DocType, parm.personId, paymentMethod.BankId ?? 0, paymentMethod.SafeId ?? 0);
            string descAr = "";
            string descEn = "";
            string PayTypeAr = "";
            string PayTypeEn = "";
            if (paymentMethod.BankId != null)
            {
                PayTypeAr = " بنك ";
                PayTypeEn = " Bank ";

            }
            else if (paymentMethod.SafeId != null)
            {
                PayTypeAr = " خزينة ";
                PayTypeEn = " Safe ";
            }
            if (parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase)
            {
                descAr = $"سند قبض {PayTypeAr}رقم _ " + parm.RecNumber;
                descAr = $"Cash Reciepts {PayTypeEn}_ " + parm.RecNumber;
            }
            else if (parm.DocType == DocumentType.ReturnSales || parm.DocType == DocumentType.Sales)
            {
                descAr = $"سند صرف {PayTypeAr}_ " + parm.RecNumber;
                descAr = $"Payment Reciepts {PayTypeEn}_ " + parm.RecNumber;
            }

            var invoicePaymentJournalEntryDetails = new List<JournalEntryDetail>();
            invoicePaymentJournalEntryDetails.Add(new JournalEntryDetail()
            {
                Credit = parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase ? 0 : parm.net,
                Debit = parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase ? parm.net : 0,
                DescriptionAr = descAr,
                DescriptionEn = descEn,
                FinancialAccountId = paymentMethod.BankId != null ? data.banks.FinancialAccountId : data.safes.FinancialAccountId
            });
            invoicePaymentJournalEntryDetails.Add(new JournalEntryDetail()
            {
                Credit = parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase ? parm.net : 0,
                Debit = parm.DocType == DocumentType.Sales || parm.DocType == DocumentType.ReturnPurchase || parm.DocType == DocumentType.ReturnWov_purchase ? 0 : parm.net,
                DescriptionAr = descAr,
                DescriptionEn = descEn,
                FinancialAccountId = data.person.FinancialAccountId
            });

            var saved = await _mediator.Send(new AddJournalEntryRequest
            {
                JournalEntryDetails = invoicePaymentJournalEntryDetails,
                isAuto = true,
                InvoiceId = parm.invoiceId,
                ReceiptsId = parm.RecNumber,
                FTDate = DateTime.Now,
                BranchId = userinfo.CurrentbranchId,
                Notes = descAr + " - " + parm.invoiceCode,
                DocType = (int)parm.DocType
            });
            //var saved = await _journalEntryBusiness.AddJournalEntry(new JournalEntryParameter()
            //{
            //    JournalEntryDetails = invoicePaymentJournalEntryDetails,
            //    isAuto = true,
            //    InvoiceId = parm.invoiceId,
            //    ReceiptsId = parm.RecNumber,
            //    FTDate = DateTime.Now,
            //    BranchId = userinfo.CurrentbranchId,
            //    Notes = descAr + " - " + parm.invoiceCode,
            //    DocType = (int)parm.DocType
            //});

            return new ResponseResult()
            {
                Data = saved
            };
        }
        public GenralData Data(DocumentType docType, int? personId, int bankId = 0, int safeId = 0)
        {
            var userinfo = _iUserInformation.GetUserInformation().Result;
            var glSettings = _invoiceQuery.TableNoTracking.Where(x => x.branchId == userinfo.CurrentbranchId);
            var gLGeneralSettingQuery = _GLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
            var person = _invPersonsQuery.TableNoTracking.Where(x => x.Id == personId).FirstOrDefault();
            GLBank bank = null;
            GLSafe safe = null;
            if (bankId != 0)
                bank = _gLBankQuery.TableNoTracking.Where(x => x.Id == bankId).FirstOrDefault();
            if (safeId != 0)
                safe = _gLSafeQuery.TableNoTracking.Where(x => x.Id == safeId).FirstOrDefault();
            string invoiceNameAr = "";
            string invoiceNameEn = "";

            if (docType == DocumentType.Purchase)
            {
                invoiceNameAr = InvoiceTransaction.PurchaseAr;
                invoiceNameEn = InvoiceTransaction.PurchaseEn;
            }
            else if (docType == DocumentType.ReturnPurchase)
            {
                invoiceNameAr = InvoiceTransaction.ReturnPurchaseAr;
                invoiceNameEn = InvoiceTransaction.ReturnPurchaseEn;
            }
            else if (docType == DocumentType.POS)
            {
                invoiceNameAr = InvoiceTransaction.POSAr;
                invoiceNameEn = InvoiceTransaction.POSEn;
            }
            else if (docType == DocumentType.ReturnPOS)
            {
                invoiceNameAr = InvoiceTransaction.returnPOSAr;
                invoiceNameEn = InvoiceTransaction.returnPOSEn;
            }
            else if (docType == DocumentType.Sales)
            {
                invoiceNameAr = InvoiceTransaction.SalesAr;
                invoiceNameEn = InvoiceTransaction.SalesEn;
            }
            else if (docType == DocumentType.ReturnSales)
            {
                invoiceNameAr = InvoiceTransaction.returnSalesAr;
                invoiceNameEn = InvoiceTransaction.returnSalesEn;
            }
            else if (docType == DocumentType.AddPermission)
            {
                invoiceNameAr = InvoiceTransaction.addPermissionAr;
                invoiceNameEn = InvoiceTransaction.addPermissionEn;
            }
            else if (docType == DocumentType.ExtractPermission)
            {
                invoiceNameAr = InvoiceTransaction.ExtractPermissionAr;
                invoiceNameEn = InvoiceTransaction.ExtractPermissionEn;
            }
            else if (docType == DocumentType.wov_purchase)
            {
                invoiceNameAr = InvoiceTransaction.WOVPurchaseAr;
                invoiceNameEn = InvoiceTransaction.WOVPurchaseEn;
            }
            else if (docType == DocumentType.ReturnWov_purchase)
            {
                invoiceNameAr = InvoiceTransaction.WOVReturnPurchaseAr;
                invoiceNameEn = InvoiceTransaction.WOVReturnPurchaseEn;
            }



            return new GenralData()
            {
                userinfo = userinfo,
                glSettings = glSettings,
                person = person,
                banks = bank,
                safes = safe,
                invoiceNameAr = invoiceNameAr,
                invoiceNameEn = invoiceNameEn,
                gLGeneralSettingQuery = gLGeneralSettingQuery

            };
        }

        public List<JournalEntryDetail> GetJurnalDetailForSalesCost(PurchasesJournalEntryIntegrationDTO prameter)
        {

            double totalcost = 0;
            foreach (var item in prameter.InvDetails.Where(c => c.parentItemId == null || c.parentItemId == 0))
            {
                var invCost = GetLastInvoicesCost(item.ItemId, 0, prameter.serialize, prameter.branchId.Value);
                totalcost += invCost.Cost * item.Quantity * item.ConversionFactor;
                if (invCost.Cost == 0)
                {
                    totalcost += ((InvStpItemCardUnitQuery.TableNoTracking.Where(h => h.ItemId == item.ItemId).OrderBy(h => h.ConversionFactor).Select(a => a.PurchasePrice).FirstOrDefault()) * item.Quantity * item.ConversionFactor);
                }
            }
            return preparJournalEnterySalesCost(new JEnteryInvoicedata()
            {
                cost = totalcost,
                invoiceId = prameter.InvDetails.Select(a => a.InvoiceId).FirstOrDefault(),
                invoiceType = (int)prameter.DocType
            });
        }
        public List<JournalEntryDetail> preparJournalEnterySalesCost(JEnteryInvoicedata invoice)
        {
            var _userinfo = _iUserInformation.GetUserInformation().Result;
            List<JournalEntryDetail> journalEntryDetails = new List<JournalEntryDetail>();

            journalEntryDetails.Add(new JournalEntryDetail()//add the main data of journal entery
            {
                //JournalEntryId = journalentry.Id,
                FinancialAccountId = _invoiceQuery.TableNoTracking.Where(h => h.RecieptsType == (int)DocumentType.Purchase && h.branchId == _userinfo.CurrentbranchId).Select(a => a.FinancialAccountId).FirstOrDefault(),// المشتريات 
                Credit = Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost, // total cost
                Debit = !Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost,
                DescriptionAr = purAndSalesSettingNames.SalesCostAr,
                DescriptionEn = purAndSalesSettingNames.SalesCostEn,
                isCostSales = true
            });
            journalEntryDetails.Add(new JournalEntryDetail()//add the main data of journal entery
            {
                //JournalEntryId = journalentry.Id,
                FinancialAccountId = _invoiceQuery.TableNoTracking.Where(h => h.RecieptsType == (int)DocumentType.Inventory && h.branchId == _userinfo.CurrentbranchId).Select(a => a.FinancialAccountId).FirstOrDefault(),// تكلفه المبيعات  
                Debit = Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost, // total cost
                Credit = !Lists.returnSalesInvoiceList.Contains(invoice.invoiceType) ? 0 : invoice.cost,
                DescriptionAr = purAndSalesSettingNames.SalesCostAr,
                DescriptionEn = purAndSalesSettingNames.SalesCostEn,
                isCostSales = true

            });



            return journalEntryDetails;

        }
    }
    public class GenralData
    {
        public UserInformationModel userinfo { get; set; }
        public IQueryable<GLPurchasesAndSalesSettings> glSettings { get; set; }
        public GLGeneralSetting gLGeneralSettingQuery { get; set; }
        public InvPersons person { get; set; }
        public GLBank banks { get; set; }
        public GLSafe safes { get; set; }
        public string invoiceNameAr { get; set; }
        public string invoiceNameEn { get; set; }



    }
}
