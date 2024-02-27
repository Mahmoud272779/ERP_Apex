using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Infrastructure;
using Castle.Core.Internal;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System.Collections.Generic;
using System.Linq;
using static App.Domain.Enums.Enums;
using DocumentType = App.Domain.Enums.Enums.DocumentType;

namespace App.Application.Services.Process.GLServices.ReceiptBusiness.ReceiptsPaid
{
    internal class CollectionReceipts : ICollectionReceipts
    {
        private readonly IRepositoryCommand<GlReciepts> receiptsCommand;
        private readonly iUserInformation userinformation;
        private readonly IRepositoryQuery<GlReciepts> receiptsQuery;
        private readonly IMediator metdiator;
        private readonly IRepositoryQuery<InvoiceMaster> invoiceMasterQuery;
        private readonly IRepositoryCommand<InvoiceMaster> invoiceMasterCommand;
        private readonly IRepositoryQuery<InvPaymentMethods> PaymentsMethodsQuery;
        private readonly IRepositoryQuery<GLJournalEntry> jounralEnteryQuery;
        private readonly IRepositoryQuery<GLSafe> safeQuery;
        private readonly IRepositoryQuery<GLBank> bankQuery;
        private readonly IRepositoryQuery<InvPersons> PersonsQuery;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand;
        private readonly IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery;


        private readonly IReceiptsService ReceiptsServices;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly IRepositoryQuery<InvoiceMasterHistory> InvoiceMasterHistoryQuery;
        private readonly IRepositoryCommand<InvoiceMasterHistory> InvoiceMasterHistoryCommand;

        public CollectionReceipts(IRepositoryQuery<InvoiceMasterHistory> invoiceMasterHistoryQuery,
            IRepositoryCommand<InvoiceMasterHistory> invoiceMasterHistoryCommand,
            IRepositoryCommand<GlReciepts> ReceiptCommand,
            iUserInformation userinformation,
            IMediator metdiator,
            IRepositoryCommand<InvoiceMaster> InvoiceMasterCommand,
            IRepositoryQuery<InvPaymentMethods> paymentsMethodsQuery,
            IRepositoryQuery<InvoiceMaster> invoiceMasterQuery,
            IReceiptsService receiptsServices,
            IHistoryInvoiceService historyInvoiceService,
            IRepositoryQuery<GLJournalEntry> jounralEnteryQuery,
            IRepositoryCommand<GLJournalEntryDetails> journalEntryDetailsCommand,
            IRepositoryQuery<GLSafe> safeQuery,
            IRepositoryQuery<GLBank> bankQuery,
            IRepositoryQuery<InvPersons> personsQuery,
            IGeneralAPIsService generalAPIsService,
            IRepositoryQuery<GlReciepts> receiptsQuery,
            IRepositoryQuery<GLJournalEntryDetails> journalEntryDetailsQuery)
        {
            receiptsCommand = ReceiptCommand;
            this.userinformation = userinformation;
            this.metdiator = metdiator;
            invoiceMasterCommand = InvoiceMasterCommand;
            PaymentsMethodsQuery = paymentsMethodsQuery;
            this.invoiceMasterQuery = invoiceMasterQuery;
            ReceiptsServices = receiptsServices;
            HistoryInvoiceService = historyInvoiceService;
            this.jounralEnteryQuery = jounralEnteryQuery;
            this.journalEntryDetailsCommand = journalEntryDetailsCommand;
            this.safeQuery = safeQuery;
            this.bankQuery = bankQuery;
            PersonsQuery = personsQuery;
            GeneralAPIsService = generalAPIsService;
            this.receiptsQuery = receiptsQuery;
            InvoiceMasterHistoryQuery = invoiceMasterHistoryQuery;
            InvoiceMasterHistoryCommand = invoiceMasterHistoryCommand;
            this.journalEntryDetailsQuery = journalEntryDetailsQuery;
        }

        public async Task<ResponseResult> AddCollectionReceipts(CollectionReceiptsRequest request)
        {

            try
            {


                var userinfo = await userinformation.GetUserInformation();
                if (!userinfo.otherSettings.CollectionReceipts)
                {
                    return new ResponseResult { Result = Result.Failed, ErrorMessageAr = ErrorMessagesAr.Unauthorized, ErrorMessageEn = ErrorMessagesEn.Unauthorized };
                }
                List<int> ids = new List<int>();
                foreach (var method in request.PaymentMethedIds)
                {
                    if (ids.Contains(method.PaymentMethodId))
                        return new ResponseResult
                        {
                            Alart = new Alart { AlartType = AlartType.information, MessageAr = ErrorMessagesAr.CantRepeatSamePaymentMethod, MessageEn = ErrorMessagesEn.CantRepeatSamePaymentMethod },
                            Result = Result.ReleatedData
                        };// ,ErrorMessageAr = ErrorMessagesAr.CantRepeatSamePaymentMethod, ErrorMessageEn = ErrorMessagesEn.CantRepeatSamePaymentMethod };

                    ids.Add(method.PaymentMethodId);
                }

                int branchId = userinfo.CurrentbranchId;
                var invoice = invoiceMasterQuery.TableNoTracking.Where(h => h.InvoiceId == request.invoiceId && !h.IsDeleted).FirstOrDefault();


                List<GLJournalEntryDetails> DetailsList = new List<GLJournalEntryDetails>();
                var getPaymentmethods = PaymentsMethodsQuery.TableNoTracking.Where(h => (request.PaymentMethedIds.Select(a => a.PaymentMethodId)).Contains(h.PaymentMethodId)).ToList();
                double TotalPaid = request.PaymentMethedIds.Sum(a => a.Value);
                ResponseResult res = validateData(invoice, TotalPaid, request);
                if (res.Result == Result.Failed) { return res; }
                int signal = GeneralAPIsService.GetSignal(invoice.InvoiceTypeId);

                int subtype = (invoice.InvoiceTypeId == (int)DocumentType.Purchase || invoice.InvoiceTypeId == (int)DocumentType.ReturnSales) ? (int)SubType.PaidReceipt : (int)SubType.CollectionReceipt;
                int CollectionmainCode = AutoCollectionMaincode(subtype, branchId, invoice.InvoiceType);

                //add receipts

                foreach (var item in getPaymentmethods)
                {
                    RecieptsRequest rec = await setReceiptsData(request, invoice, item, subtype, CollectionmainCode);
                    res = await ReceiptsServices.AddReceipt(rec);
                    if (res.Result == Result.Success)
                        DetailsList.AddRange(await SetJournalEntery(rec, signal, branchId, res.Id.Value,rec.RecieptTypeId));
                    else
                        return res;

                }



                //update invoice price
                var invoices = new UpdateinvoiceForCollectionReceiptRequest();
                invoices.signal = 1;

                invoices.invoicesList.Add(new UpdateinvoiceForCollectionReceiptRequestList()
                {
                    invoiceId = request.invoiceId,
                    paid = TotalPaid,
                    CollectionPaymentMethods = request.PaymentMethedIds,
                    branchId = branchId,
                    signal = invoices.signal
                });


                var invUpdate = await metdiator.Send(invoices);


                //add invoice history  
                if (res.Result == Result.Success)
                    await HistoryInvoiceService.HistoryInvoiceMaster(invoice.BranchId, invoice.Notes, invoice.BrowserName, "A", null, invoice.BookIndex, invoice.Code
                  , request.RecDate, invoice.InvoiceId, invoice.InvoiceType, invoice.InvoiceTypeId, subtype, invoice.IsDeleted, "", invoice.Serialize, invoice.StoreId, request.PaymentMethedIds.Sum(a => a.Value), CollectionmainCode);

                if (DetailsList.Count > 0)
                {
                    journalEntryDetailsCommand.AddRange(DetailsList);
                    var saved = await journalEntryDetailsCommand.SaveAsync();
                }


                //return new ResponseResult() {  Data = invUpdate, Result = Result.Success, Note = Actions.SavedSuccessfully };

                return new ResponseResult() { Result = Result.Success };
            }
            catch (Exception e)
            {

                return new ResponseResult() { Result = Result.Failed, ErrorMessageAr = "Error AR", ErrorMessageEn = e.Message };
            }

        }













        public async Task<int> PersonFinantialAccId(int type, int ID)
        {

            return (int)await PersonsQuery.TableNoTracking.Where(a => a.Id == ID && (type == AuthorityTypes.customers ? a.IsCustomer == true : a.IsSupplier == true))
                            .Include(h => h.FinancialAccount)
                            .Select(s => s.FinancialAccountId).FirstOrDefaultAsync();

        }
        public int SafeFinancialAccId(RecieptsRequest rec, int branchId)
        {
            if (rec.BankId == null)
                return (int)safeQuery.TableNoTracking.Where(h => h.Id == rec.SafeID.Value && h.BranchId == branchId).Select(h => h.FinancialAccountId).FirstOrDefault();
            else
                return (int)bankQuery.TableNoTracking.Where(h => h.Id == rec.BankId.Value).Select(h => h.FinancialAccountId).FirstOrDefault();



        }
        private ResponseResult validateData(InvoiceMaster invoice, double totalPaid, CollectionReceiptsRequest req)
        {
            if (invoice == null)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageEn = "invoice not found", ErrorMessageAr = "الفاتوره غير موجوده " };
            if (req.PaymentMethedIds == null || req.PaymentMethedIds.Count() == 0)
                return new ResponseResult() { Result = Result.Failed, ErrorMessageEn = "you shoud add payment methods", ErrorMessageAr = "يجب اضافة طريقة سداد " };
            //if (totalPaid > invoice.Net - invoice.Paid)
            //    return new ResponseResult() { Result = Result.Failed, ErrorMessageEn = "paid should be less than invoice or equal invoice remain", ErrorMessageAr = "يحب ان تكون الاجمالى المبلغ اقل من او يساوى المتبقى من الفاتوره " };

            return new ResponseResult() { Result = Result.Success };

        }
        public async Task<RecieptsRequest> setReceiptsData(CollectionReceiptsRequest request, InvoiceMaster Invoice, InvPaymentMethods item, int subtype, int collectionMainCode)
        {
            var paymentmethod = request.PaymentMethedIds.Where(h => h.PaymentMethodId == item.PaymentMethodId).FirstOrDefault();

            RecieptsRequest ReceiptsData = new RecieptsRequest()
            {
                SafeID = item.SafeId != null ? request.SafeId : item.SafeId,
                BankId = item.BankId,
                Amount = paymentmethod.Value,
                RecieptDate = request.RecDate,
                PaymentMethodId = item.PaymentMethodId,
                ChequeBankName = "",
                Notes = paymentmethod.Cheque,
                ChequeNumber = item.Code == 3 ? paymentmethod.Cheque : "",
                ChequeDate = request.RecDate,
                ParentType = Invoice.InvoiceType, // + "-" + CollectionCounter(subtype, Invoice.BranchId, Invoice.InvoiceId, Invoice.InvoiceTypeId),// hamada 
                ParentTypeId = Invoice.InvoiceTypeId,
                Creditor = paymentmethod.Value,
                Debtor = paymentmethod.Value,
                RecieptTypeId = GetReceiptsType(item.SafeId, item.BankId, Invoice.InvoiceTypeId),
                IsAccredit = false,
                ParentId = Invoice.InvoiceId,
                Serialize = GetCollectionReceiptsSerialize(Invoice.InvoiceId,Invoice.Serialize),//Invoice.Serialize,/// update this 
                Code = 0,// Invoice.Code,
                BenefitId = Invoice.PersonId.Value,
                Authority = getauthortyId(Invoice),
                ReceiptOnly = true,
                fromInvoice = true,
                subTypeId = subtype,//(Invoice.InvoiceTypeId==(int)DocumentType.Purchase||Invoice.InvoiceTypeId==(int)DocumentType.ReturnSales)? (int)SubType.PaidReceipt :(int)SubType.CollectionReceipt,
                CollectionMainCode = collectionMainCode,
                isPartialPaid = Invoice.PaymentType,
                InvoiceNotes = request.Notes, //Notes = request.Notes,
                AttachedFile = request.AttachedFile,


            };
            return ReceiptsData;

        }
        private double GetCollectionReceiptsSerialize(int invoiceId, double invoiceSerialize)
        {
            double Serialize = 0;
            var recs = receiptsQuery.TableNoTracking.Where(c => c.ParentId == invoiceId).OrderBy(c => c.Serialize).LastOrDefault();
            if (recs != null)
                Serialize = recs.Serialize + 0.1;
            else
                invoiceSerialize = invoiceSerialize + 0.1;
            return Serialize;

        }
        private int CollectionCounter(int subtypeID, int BranchId, int InvoiceId, int InvoiceTypeId)
        {
            var Code = 1;
            //var PN = receiptsQuery.TableNoTracking.Where(a => a.BranchId == BranchId && a.ParentId == InvoiceId && a.ParentTypeId == InvoiceTypeId && a.Code == 0 );
            Code = receiptsQuery.GetMaxCode(e => e.CollectionCode, a => a.SubTypeId == subtypeID && a.BranchId == BranchId && a.ParentId == InvoiceId && a.ParentTypeId == InvoiceTypeId);

            //rc = (rc!=0 || rc != null)? (int.TryParse(PN.Select(e=>e.PaperNumber).First().Split('-').Last(), out rc) ? rc : 0) : 0 ;
            if (Code != 0 || Code != null)
                ++Code;

            return Code;
        }
        private int getauthortyId(InvoiceMaster Invoice)
        {
            int authorty = 0;

            if (Lists.purchasesInvoicesList.Contains(Invoice.InvoiceTypeId) || Lists.purchasesWithoutVatInvoicesList.Contains(Invoice.InvoiceTypeId))
                authorty = AuthorityTypes.suppliers;
            else if (Lists.salesInvoicesList.Contains(Invoice.InvoiceTypeId) || Lists.POSInvoicesList.Contains(Invoice.InvoiceTypeId))
                authorty = AuthorityTypes.customers;
            return authorty;
        }
        private int GetReceiptsType(int? safeID, int? bankId, int? ParentTypeId)
        {
            if (ParentTypeId == null)
                return 0;
            if (ParentTypeId == (int)DocumentType.Purchase || ParentTypeId == (int)DocumentType.ReturnSales || ParentTypeId == (int)DocumentType.ReturnPOS || ParentTypeId == (int)DocumentType.wov_purchase)
            {
                if (safeID != null && safeID > 0)//recepttype = سند صرف خزينه
                    return (int)DocumentType.SafePayment;
                else if (bankId != null && bankId > 0)
                    return (int)DocumentType.BankPayment;

            }
            if (ParentTypeId == (int)DocumentType.ReturnPurchase || ParentTypeId == (int)DocumentType.Sales || ParentTypeId == (int)DocumentType.POS || ParentTypeId == (int)DocumentType.ReturnWov_purchase)
            {
                if (safeID != null && safeID > 0)//recepttype = سند قبض خزينه
                    return (int)DocumentType.SafeCash;
                else if (bankId != null && bankId > 0)
                    return (int)DocumentType.BankCash;
            }
            return 0;
        }
        public async Task<List<GLJournalEntryDetails>> SetJournalEntery(RecieptsRequest prm, int signal, int branchId, int Id,int DocType)
        {
            var Set_RecieptType = ReceiptsServices.SetRecieptTypeAndDirectoryAndNotes(prm.RecieptTypeId, prm.ParentTypeId);
            try
            {


                int safeFA = SafeFinancialAccId(prm, branchId);
                int personFA = await PersonFinantialAccId(prm.Authority, prm.BenefitId);
                //prm.parentId  is invoiceId


                var journalentryID = await jounralEnteryQuery.TableNoTracking.Where(q => q.InvoiceId == prm.ParentId).FirstOrDefaultAsync();

                var addJournalEntry = await metdiator.Send(new AddJournalEntryRequest
                {

                    BranchId = journalentryID.BranchId,
                    FTDate = prm.RecieptDate,
                    InvoiceId = journalentryID.InvoiceId.Value,
                    isAuto = true,
                    IsAccredit = true,
                    DocType = DocType,
                    ReceiptsId = journalentryID.ReceiptsId.Value,
                    Notes = "سند سداد - " + journalentryID.Notes,
                    CompinedReceiptCode = journalentryID.CompinedReceiptCode.Value,
                    IsCompined = journalentryID.IsCompined,
                    AddWithOutElements = true
                });

                var JournalId = addJournalEntry.Data;

                List<GLJournalEntryDetails> journalEntryDetails = new List<GLJournalEntryDetails>();
                if ((int)addJournalEntry.Data == 0)
                    return journalEntryDetails;

                journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
                {
                    JournalEntryId = (int)addJournalEntry.Data,
                    FinancialAccountId = safeFA,
                    Credit = signal > 0 ? prm.Amount : 0,
                    Debit = signal < 0 ? prm.Amount : 0,
                    DescriptionAr = string.Concat(Set_RecieptType.NoteAr, " _ ", prm.ParentType),
                    DescriptionEn = Set_RecieptType.NoteEn + " _ " + prm.ParentType,
                    isCostSales = true,
                    ReceiptsMainCode = Id

                });
                journalEntryDetails.Add(new GLJournalEntryDetails()//add the main data of journal entery
                {
                    JournalEntryId = (int)addJournalEntry.Data,
                    FinancialAccountId = personFA,
                    Credit = signal < 0 ? prm.Amount : 0,
                    Debit = signal > 0 ? prm.Amount : 0,
                    DescriptionAr = string.Concat(Set_RecieptType.NoteAr, " _ ", prm.ParentType),
                    DescriptionEn = Set_RecieptType.NoteEn + " _ " + prm.ParentType,
                    isCostSales = true,
                    ReceiptsMainCode = Id

                });

                //journalEntryDetailsCommand.AddRange(journalEntryDetails);



                return journalEntryDetails;
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<ResponseResult> DeleteCollectionReceipts(List<int?> Ids)
        {


            var res = await ReceiptsServices.DeleteReciepts(Ids);

            if (res.Result == Result.Success)
            {
                var cReceipts = receiptsQuery.TableNoTracking.Where(a => Ids.Contains(a.Id) && a.ParentId != null && a.SubTypeId != 0).ToList();
                //if (cReceipts.Where(h => h.SubTypeId != 0).Any())
                //    return res;
                //update invoice price

                var invHistory = InvoiceMasterHistoryQuery.TableNoTracking.Where(a => cReceipts.Select(s => s.CollectionMainCode).Contains(a.CollectionMainCode));
                var invoices = new UpdateinvoiceForCollectionReceiptRequest();
                invoices.signal = -1;
                var invHistoryList = new List<InvoiceMasterHistory>();
                invHistoryList.AddRange(invHistory);
                foreach (var rec in cReceipts.GroupBy(a => a.CollectionMainCode))
                {
                    List<PaymentMethods> paymentMethods = new List<PaymentMethods>();
                    foreach (var id in rec)
                    {
                        paymentMethods.Add(new PaymentMethods() { PaymentMethodId = id.PaymentMethodId, Value = id.Amount });
                    }


                    invoices.invoicesList.Add(new UpdateinvoiceForCollectionReceiptRequestList()
                    { invoiceId = rec.First().ParentId.Value, paid = rec.Sum(a => a.Amount), signal = rec.First().Signal, branchId = rec.First().BranchId, CollectionPaymentMethods = paymentMethods });
                    invHistoryList.Where(a => a.CollectionMainCode == rec.First().CollectionMainCode)
                                  .Select(a => a.TotalPrice = a.TotalPrice - rec.Sum(e => e.Amount)).ToList();

                }
                var invHistoryWillDeleted = invHistoryList.Where(a => a.TotalPrice <= 0).ToList();
                var invHistoryWillUpdated = invHistoryList.Where(a => a.TotalPrice > 0).ToList();
                var invUpdate = await metdiator.Send(invoices);
                if (invUpdate.Result == Result.Success)
                {
                    if (invHistoryWillUpdated.Count() > 0)
                        await InvoiceMasterHistoryCommand.UpdateAsyn(invHistoryWillUpdated);
                    if (invHistoryWillDeleted.Count > 0)
                    {
                        InvoiceMasterHistoryCommand.RemoveRange(invHistoryWillDeleted);
                        await InvoiceMasterHistoryCommand.SaveAsync();
                    }
                    var x = journalEntryDetailsQuery.TableNoTracking.Where(h => Ids.Contains(h.ReceiptsMainCode));
                    var DelJenery = await journalEntryDetailsCommand.DeleteAsync(h => Ids.Contains(h.ReceiptsMainCode));
                }
                return new ResponseResult() { Result = Result.Success };
            }
            else
            {
                return res;
            }

        }
        public int AutoCollectionMaincode(int subtypeID, int BranchId, string InvoiceType)
        {
            var Code = 1;
            Code = receiptsQuery.GetMaxCode(e => e.CollectionMainCode.Value, a => a.SubTypeId == subtypeID && a.BranchId == BranchId);
            if (Code != null)
                Code++;

            return Code;
        }
    }

}
