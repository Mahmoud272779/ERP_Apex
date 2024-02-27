using App.Application.Handlers.GeneralAPIsHandler.DetailedInvoices;
using App.Application.Handlers.GeneralLedger.JournalEntry;
using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Application.Handlers.MultiCollectionReceipts.AddMultiCollectionReceipts;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Application.Services.Process.StoreServices.Invoices.HistoryOfInvoices;
using App.Domain;
using App.Domain.Models;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Response.GeneralLedger;
using App.Domain.Models.Security.Authentication.Response;
using MediatR;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.MultiCollectionReceipts
{
    public class MultiCollectionReceiptsHelper
    {
        private readonly IMediator _mediator;
        private readonly IHistoryInvoiceService HistoryInvoiceService;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery;
        private readonly IRepositoryQuery<GlReciepts> _GlRecieptsQuery;
        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IRepositoryQuery<GLSafe> _GLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _GLBankQuery;
        public MultiCollectionReceiptsHelper(IMediator mediator, IHistoryInvoiceService historyInvoiceService, iUserInformation iUserInformation, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IRepositoryQuery<GlReciepts> glRecieptsQuery, IRepositoryQuery<InvPersons> invPersonsQuery, IRepositoryQuery<GLSafe> gLSafeQuery, IRepositoryQuery<GLBank> gLBankQuery)
        {
            _mediator = mediator;
            HistoryInvoiceService = historyInvoiceService;
            _iUserInformation = iUserInformation;
            _InvoiceMasterQuery = invoiceMasterQuery;
            _GlRecieptsQuery = glRecieptsQuery;
            _InvPersonsQuery = invPersonsQuery;
            _GLSafeQuery = gLSafeQuery;
            _GLBankQuery = gLBankQuery;
        }
        public async Task<ResponseResult> ValidationMethod(AddMultiCollectionReceiptsRequest request, bool isUpdate = false, int recId = 0)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            if (request.Authority != (int)Enums.AuthorityTypes.customers && request.Authority != (int)Enums.AuthorityTypes.suppliers)
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "يجب أن تكون الجهة عميل أو مورد.",
                        MessageEn = "Authority Types should be customer or supplier.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            }
            var _invoices = _InvoiceMasterQuery.TableNoTracking.Where(c => request.invoices.Select(x => x.invoiceId).Contains(c.InvoiceId));
            var invoices = _invoices
                .Where(c => c.InvoiceTypeId == (int)Enums.DocumentType.Sales || c.InvoiceTypeId == (int)Enums.DocumentType.Purchase);
            var invoicesGrouped = request.invoices.GroupBy(c => c.invoiceId);
            foreach (var item in invoicesGrouped)
            {
                if (item.Count() > 1)
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "لا يمكنك اختيار فاتورة واحدة أكثر من مرة.",
                            MessageEn = "You cannot choose one invoice more than one time.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
            }
            if (_invoices.Where(c => c.InvoiceTypeId != (int)Enums.DocumentType.Sales).Any() && request.Authority == (int)Enums.AuthorityTypes.customers)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "يُسمح لك باختيار فواتير المبيعات فقط عند اختيار العملاء.",
                        MessageEn = "You are allowed to choose sales invoices only when choosing customers.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            if (_invoices.Where(c => c.InvoiceTypeId != (int)Enums.DocumentType.Purchase).Any() && request.Authority == (int)Enums.AuthorityTypes.suppliers)
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "يُسمح لك باختيار فواتير المشتريات فقط عند اختيار الموردين.",
                        MessageEn = "You are allowed to choose purchase invoices only when choosing customers.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            if (!invoices.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "يجب اختيار فاتورة واحدة على الأقل للمتابعة.",
                        MessageEn = "At least one invoice must be selected to proceed.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };

            }
            if (invoices.Any(c => c.IsDeleted))
            {
                var errorInvoicesArr = invoices.Where(c => c.IsDeleted).Select(c => c.InvoiceType).ToArray();
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        titleAr = "خطأ في الدفع",
                        titleEn = "Payment Error",
                        MessageAr = $"({string.Join(',', errorInvoicesArr)}) لا يمكن اختيار فاتوره محذوفة",
                        MessageEn = $"You can not choose deleted invioce ({string.Join(',', errorInvoicesArr)})"
                    }
                };

            }
            var invoicesLessThan0 = request.invoices.Where(c => c.amount <= 0);
            if (invoicesLessThan0.Any())
            {
                var errorInvoicesArr = invoices.Where(c => invoicesLessThan0.Select(x => x.invoiceId).Contains(c.InvoiceId)).Select(c => c.InvoiceType).ToArray();
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        titleAr = "خطأ في الدفع",
                        titleEn = "Payment Error",
                        MessageAr = $"({string.Join(',', errorInvoicesArr)}) لا يمكن ان يكون المدفوع ب0 اول اقل",
                        MessageEn = $"the amount can not be 0 or equle 0 ({string.Join(',', errorInvoicesArr)})"
                    }
                };
            }
            var persons = _InvPersonsQuery.TableNoTracking.Where(c => c.Id == request.personId);
            if (request.Authority == (int)Enums.AuthorityTypes.customers)
            {

                if (request.personId != 0)
                {
                    if (!persons.FirstOrDefault().IsCustomer)
                        return new ResponseResult
                        {
                            Result = Result.Failed,
                            Alart = new Alart
                            {
                                AlartType = AlartType.error,
                                type = AlartShow.popup,
                                MessageAr = "يجب عليك اختيار العميل الصحيح.",
                                MessageEn = "You have to choose the correct customer.",
                                titleAr = "خطأ",
                                titleEn = "Error"
                            }
                        };

                }



            }
            else if (request.Authority == (int)Enums.AuthorityTypes.suppliers)
            {

                if (request.personId != 0)
                {
                    if (!persons.FirstOrDefault().IsSupplier)
                        return new ResponseResult
                        {
                            Result = Result.Failed,
                            Alart = new Alart
                            {
                                AlartType = AlartType.error,
                                type = AlartShow.popup,
                                MessageAr = "يجب عليك اختيار المورد الصحيح.",
                                MessageEn = "You have to choose the correct supplier.",
                                titleAr = "خطأ",
                                titleEn = "Error"
                            }
                        };

                }


            }
            if (request.isSafe)
            {
                if (request.PaymentMethodId != (int)Enums.PaymentMethod.paid)
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "يجب ان تكون طريقه الدفع نقدي.",
                            MessageEn = "Payment method  should be paid.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                if (request.safeId == null)
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "الرجاء اختيار الخزينة.",
                            MessageEn = "Please choose safe.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                var safe = _GLSafeQuery.TableNoTracking.FirstOrDefault(c => c.Id == request.safeId);
                if (safe == null)
                {
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "هذه الخزينة غير موجودة.",
                            MessageEn = "This safe does not exist.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                }
                if (safe.BranchId != userInfo.CurrentbranchId)
                {
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "هذه الخزينة غير موجودة في الفرع الحالي.",
                            MessageEn = "This safe does not exist in the current branch.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                }
                if (!userInfo.userSafes.Any(c => c == request.safeId))
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "ليس لديك الصلاحية على هذه الخزينة.",
                            MessageEn = "You don't have permission on this safe.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
            }
            else
            {
                if (request.bankId == null)
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "الرجاء اختيار بنك.",
                            MessageEn = "Please choose bank.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                var bank = _GLBankQuery.TableNoTracking.Include(c => c.BankBranches).FirstOrDefault(c => c.Id == request.bankId);
                if (bank == null)
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "هذا البنك غير موجود.",
                            MessageEn = "This bank does not exist.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                if (!bank.BankBranches.Any(c => c.BranchId == userInfo.CurrentbranchId))
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "هذا البنك غير موجود في الفرع الحالي.",
                            MessageEn = "This bank does not exist in the current branch.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                if (!userInfo.userBanks.Any(c => c == request.bankId))
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "ليس لديك الصلاحية على هذا البنك.",
                            MessageEn = "You don't have permission on this bank.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
                if (request.PaymentMethodId == (int)Enums.PaymentMethod.paid)
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            MessageAr = "طريقة الدفع نقدي غير مسموح بها.",
                            MessageEn = "Payment method paid is not allowed.",
                            titleAr = "خطأ",
                            titleEn = "Error"
                        }
                    };
            }
            if (invoices.Any(c => c.BranchId != userInfo.CurrentbranchId))
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "لا يمكنك إرسال فواتير خاصه بفرع اخر. يجب عليك إرسال فواتير خاصه بالفرع الحالي لكم للمتابعه.",
                        MessageEn = "You cannot invoices of another branch. You must send invoices of your current branch only.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            }
            if (!request.invoices.Any())
            {
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        MessageAr = "لا يمكنك إرسال فواتير فارغة. يجب عليك إرسال فاتورة واحدة أو أكثر للمتابعة.",
                        MessageEn = "You cannot send empty invoices. You must send one or more invoices to proceed.",
                        titleAr = "خطأ",
                        titleEn = "Error"
                    }
                };
            }
            var currentInvoicesRec = _GlRecieptsQuery.TableNoTracking.Where(c => c.MultiCollectionReceiptParentId == recId).Select(c => c.ParentId);
            var checkInvoices = await _mediator.Send(new GetDetailesInvoicesRequest { InvoicesIds = request.invoices.Select(c => c.invoiceId).ToArray() });
            if (checkInvoices.Where(c => c.Remain <= 0 && (isUpdate ? !currentInvoicesRec.Contains(c.InvoiceId) : true)).Any())
            {

                var errorInvoicesArr = checkInvoices.Where(c => c.Remain <= 0).Select(c => c.invoiceType).ToArray();
                return new ResponseResult
                {
                    Result = Result.Failed,
                    Alart = new Alart
                    {
                        AlartType = AlartType.error,
                        type = AlartShow.popup,
                        titleAr = "خطأ في الدفع",
                        titleEn = "Payment Error",
                        MessageAr = $"({string.Join(',', errorInvoicesArr)}) بعض الفواتير تم دفعها بالفعل",
                        MessageEn = $"Some invoices have already been paid ({string.Join(',', errorInvoicesArr)})"
                    }
                };
            }
            if (request.personId != 0)
            {
                if (checkInvoices.Where(c => c.personId != request.personId).Any())
                {
                    var errorInvoicesArr = checkInvoices.Where(c => c.personId != request.personId).Select(c => c.invoiceType).ToArray();
                    return new ResponseResult
                    {
                        Result = Result.Failed,
                        Alart = new Alart
                        {
                            AlartType = AlartType.error,
                            type = AlartShow.popup,
                            titleAr = " لا يمكنك اختيار فواتير شخص آخر",
                            titleEn = "You cannot choose another person's invoices",
                            MessageAr = $"({string.Join(',', errorInvoicesArr)}) لا يُسمح لك باختيار فواتير تنتمي إلى شخص آخر",
                            MessageEn = $"You are not allowed to select invoices belonging to another person ({string.Join(',', errorInvoicesArr)})"
                        }
                    };
                }
            }
            return null;
        }
        public async Task<InvoicePaysObjectBuilderDTO> invoicbuilder(AddMultiCollectionReceiptsRequest request, GlReciepts rec, IQueryable<InvoiceMaster> invoices)
        {
            var invoicesForUpdate = new UpdateinvoiceForCollectionReceiptRequest();
            var listOfInvoicesRecs = new List<GlReciepts>();

            invoicesForUpdate.signal = 1;
            var UpdateinvoiceForCollectionReceiptRequestList = new List<UpdateinvoiceForCollectionReceiptRequestList>();
            foreach (var currentInvoice in invoices.Where(c => request.invoices.Select(x => x.invoiceId).Contains(c.InvoiceId)))
            {
                //var recSerialize = invoices.Where(c => c.Serialize.ToString().StartsWith(currentInvoice.Serialize.ToString()));

                //var currentInvoice = invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId);
                var newRec = new GlReciepts();
                var invoiceAmount = request.invoices.FirstOrDefault(c => c.invoiceId == currentInvoice.InvoiceId).amount;

                newRec.ParentTypeId = currentInvoice.InvoiceTypeId;
                newRec.ParentId = currentInvoice.InvoiceId;
                newRec.MultiCollectionReceiptParentId = rec.Id;
                newRec.Creditor = rec.Signal > 0 ? invoiceAmount : 0;
                newRec.Debtor = rec.Signal < 0 ? invoiceAmount : 0;
                newRec.InvoiceNotes = currentInvoice.Notes;
                newRec.IsAccredit = false;
                newRec.PersonId = currentInvoice.PersonId;
                newRec.Amount = invoiceAmount;
                newRec.NoteAR = currentInvoice.InvoiceType + "_" + $"سند مجمع {(request.isSafe ? "خزينة" : "بنك")}";
                newRec.NoteEN = (request.isSafe ? "Safe" : "bank") + "Multi Collection Receipts_" + currentInvoice.InvoiceType;
                newRec.BenefitId = currentInvoice.PersonId ?? 0;
                newRec.RecieptType = rec.RecieptType;
                newRec.CollectionCode = currentInvoice.InvoiceId;
                newRec.SafeID = rec.SafeID;
                newRec.BankId = rec.BankId;
                newRec.BranchId = rec.BranchId;
                newRec.RectypeWithPayment = rec.RectypeWithPayment;
                newRec.RecieptTypeId = rec.RecieptTypeId;
                newRec.EntryFundId = rec.EntryFundId;
                newRec.Code = rec.Code;
                newRec.PaperNumber = rec.PaperNumber;
                newRec.RecieptDate = rec.RecieptDate;
                newRec.Notes = rec.Notes;
                newRec.Authority = rec.Authority;
                newRec.PaymentMethodId = rec.PaymentMethodId;
                newRec.ChequeNumber = rec.ChequeNumber;
                newRec.ChequeBankName = rec.ChequeBankName;
                newRec.ChequeDate = rec.ChequeDate;
                newRec.Signal = rec.Signal;
                newRec.Serialize = /*currentInvoice.Serialize +.1*/rec.Serialize;
                newRec.UserId = rec.UserId;
                newRec.IsCompined = rec.IsCompined;
                newRec.CompinedParentId = rec.CompinedParentId;
                newRec.CreationDate = rec.CreationDate;
                newRec.otherSalesManId = rec.otherSalesManId;
                newRec.FinancialAccountId = rec.FinancialAccountId;
                newRec.IsIncludeVat = rec.IsIncludeVat;
                newRec.SalesManId = rec.SalesManId;
                newRec.OtherAuthorityId = rec.OtherAuthorityId;
                newRec.IsBlock = rec.IsBlock;
                newRec.SubTypeId = rec.SubTypeId;
                newRec.Deferre = rec.Deferre;
                newRec.isPartialPaid = rec.isPartialPaid;
                newRec.CollectionMainCode = rec.CollectionMainCode;

                listOfInvoicesRecs.Add(newRec);


                var paymentMethods = new List<PaymentMethods>();
                paymentMethods.Add(new PaymentMethods
                {
                    PaymentMethodId = request.PaymentMethodId,
                    Value = invoiceAmount
                });

                UpdateinvoiceForCollectionReceiptRequestList.Add(new UpdateinvoiceForCollectionReceiptRequestList
                {
                    invoiceId = currentInvoice.InvoiceId,
                    paid = invoiceAmount,
                    CollectionPaymentMethods = paymentMethods,
                    branchId = currentInvoice.BranchId,
                    signal = invoicesForUpdate.signal
                });
            }
            invoicesForUpdate.invoicesList = UpdateinvoiceForCollectionReceiptRequestList;

            return new InvoicePaysObjectBuilderDTO
            {
                invoices = invoicesForUpdate,
                recs = listOfInvoicesRecs
            };
        }
        public async Task<List<InvoiceMasterHistory>> historyBuilder(IQueryable<InvoiceMaster> invoices, AddMultiCollectionReceiptsRequest request, GlReciepts rec, int CurrentbranchId)
        {
            var listOfHistory = new List<InvoiceMasterHistory>();
            foreach (var invoice in invoices.Where(c => request.invoices.Select(x => x.invoiceId).Contains(c.InvoiceId)))
            {
                var invoiceHistory = await HistoryInvoiceService
                    .HistoryInvoiceMasterObjectBuilder(
                            CurrentbranchId,
                            invoice.Notes,
                            invoice.BrowserName,
                            "A",
                            null,
                            invoice.BookIndex,
                            invoice.Code,
                            rec.RecieptDate,
                            invoice.InvoiceId,
                            invoice.InvoiceType,
                            invoice.InvoiceTypeId,
                            rec.RecieptTypeId,
                            invoice.IsDeleted,
                            "",
                            invoice.Serialize,
                            invoice.StoreId,
                            request.invoices.FirstOrDefault(c => c.invoiceId == invoice.InvoiceId).amount,
                            0,
                            rec.Id
                            );
                listOfHistory.Add(invoiceHistory);
            }
            return listOfHistory;
        }

        public async Task<bool> DeleteInvoiceMultiCollectionRec(
            int masterRecId,
            bool isSafe,
            int MasterPaymentMethodId,
            IRepositoryCommand<InvoicePaymentsMethods> _InvoicePaymentsMethodsCommand,
            IRepositoryQuery<InvoiceMaster> _InvoiceMasterQuery,
            IRepositoryCommand<InvoiceMaster> _InvoiceMasterCommand,
            IRoundNumbers roundNumbers,
            IRepositoryCommand<InvoiceMasterHistory> InvoiceMasterHistoryRepositoryCommand,
            IRepositoryCommand<GlReciepts> _GlRecieptsCommand,
            IRepositoryQuery<InvoicePaymentsMethods> _InvoicePaymentsMethodsQuery,
            IRepositoryQuery<GlReciepts> _GlRecieptsQuery
            )
        {
            _InvoicePaymentsMethodsCommand.StartTransaction();


            List<InvoicePaymentsMethods> ouput_recInvoices = new List<InvoicePaymentsMethods>();
            var recInvoices = _GlRecieptsQuery.TableNoTracking
                                                    .Where(c => c.RecieptTypeId == (isSafe ? (int)Enums.DocumentType.SafeMultiCollectionReceipts : (int)Enums.DocumentType.BankMultiCollectionReceipts))
                                                    .Where(c => c.MultiCollectionReceiptParentId == masterRecId);

            var invoicesPaymentMethod = _InvoicePaymentsMethodsQuery.TableNoTracking.Where(c => recInvoices.Select(x => x.ParentId).Contains(c.InvoiceId) && c.PaymentMethodId == MasterPaymentMethodId);






            foreach (var item in invoicesPaymentMethod)
            {
                ouput_recInvoices.Add(new InvoicePaymentsMethods
                {
                    InvoicePaymentsMethodId = item.InvoicePaymentsMethodId,
                    PaymentMethodId = item.PaymentMethodId,
                    InvoiceId = item.InvoiceId,
                    Value = item.Value - recInvoices.FirstOrDefault(c => c.ParentId == item.InvoiceId)?.Amount ?? 0,
                    Cheque = item.Cheque,
                    BranchId = item.BranchId,
                    CodeOfflinePOS = item.CodeOfflinePOS
                });
            }
            var updatePaymentMethod = await _InvoicePaymentsMethodsCommand.UpdateAsyn(ouput_recInvoices);
            if (updatePaymentMethod)
            {
                List<InvoiceMaster> output_Invoices = new List<InvoiceMaster>();
                var invoices = _InvoiceMasterQuery.TableNoTracking.Where(c => recInvoices.Select(x => x.ParentId).Contains(c.InvoiceId)).ToList();
                invoices.ForEach(x =>
                {
                    x.Paid = x.Paid - recInvoices.FirstOrDefault(c => c.ParentId == x.InvoiceId)?.Amount ?? 0;
                    if (x.Paid == 0)
                    {
                        x.PaymentType = (int)PaymentType.Delay;
                        x.IsCollectionReceipt = false;
                    }
                    if (x.Paid < x.Net && x.Paid != 0)
                    {
                        x.PaymentType = (int)PaymentType.Partial;
                    }
                    if (x.Paid >= x.Net)
                    {
                        x.PaymentType = (int)PaymentType.Complete;
                    }
                    x.Remain = roundNumbers.GetRoundNumber(x.Net - x.Paid);

                });
                var invoiceUpdated = await _InvoiceMasterCommand.UpdateAsyn(invoices);
                if (invoiceUpdated)
                {
                    var HistoryDeleted = await InvoiceMasterHistoryRepositoryCommand.DeleteAsync(c => c.MultiCollectionReceiptsId == masterRecId);
                    if (HistoryDeleted)
                    {
                        var deleteSubRecs = await _GlRecieptsCommand.DeleteAsync(c => recInvoices.Select(x => x.Id).Contains(c.Id));

                        if (!deleteSubRecs)
                        {
                            _InvoicePaymentsMethodsCommand.Rollback();
                            return false;
                        }
                        try
                        {
                            _InvoicePaymentsMethodsCommand.CommitTransaction();

                        }
                        catch (Exception ex)
                        {
                            _InvoicePaymentsMethodsCommand.Rollback();
                            throw;
                        }
                    }
                    else
                    {
                        _InvoicePaymentsMethodsCommand.Rollback();
                        return false;
                    }
                }
                else
                {
                    _InvoicePaymentsMethodsCommand.Rollback();
                    return false;
                }
            }
            else
            {
                _InvoicePaymentsMethodsCommand.Rollback();
                return false;
            }
            return true;
        }
        public async Task<bool> JournalEntryIntegration(JournalEntryIntegrationDTO request)
        {
            GLJournalEntry JournalEntry = new GLJournalEntry();
            if (request.isUpdate)
            {
                JournalEntry = request.GLJournalEntryQuery.TableNoTracking.FirstOrDefault(c => c.ReceiptsId == request.MastreRec.Id);
                if (JournalEntry == null)
                    request.isUpdate = false;

            }
            AddJournalEntryRequest JEntry = new AddJournalEntryRequest();

            JEntry.BranchId = request.userinfo.CurrentbranchId;
            JEntry.FTDate = request.MastreRec.ChequeDate;
            JEntry.ReceiptsId = request.MastreRec.Id;
            JEntry.Notes = request.note;
            JEntry.isAuto = true;
            JEntry.IsAccredit = true;
            JEntry.JournalEntryDetails.Add(new JournalEntryDetail()
            {
                FinancialAccountId = request.safeOrBankFAId,
                Credit = request.MastreRec.Signal < 0 ? request.MastreRec.Amount : 0,
                Debit = request.MastreRec.Signal > 0 ? request.MastreRec.Amount : 0,
                DescriptionAr = request.MastreRec.Notes,
                DescriptionEn = request.MastreRec.Notes,
            });
            var invoices = request.invoices
                .Include(c => c.Person);
            foreach (var item in request.recInvoices)
            {

                JEntry.JournalEntryDetails.Add(new JournalEntryDetail()
                {
                    FinancialAccountId = invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId).Person.FinancialAccountId,
                    Credit = request.MastreRec.Signal > 0 ? item.paid : 0,
                    Debit = request.MastreRec.Signal < 0 ? item.paid : 0,
                    DescriptionAr = (invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId)?.InvoiceType ?? "") + (!string.IsNullOrEmpty(invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId).Notes) ? "_" + (invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId)?.Notes ?? "") : ""),
                    DescriptionEn = (invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId)?.InvoiceType ?? "") + (!string.IsNullOrEmpty(invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId).Notes) ? "_" + (invoices.FirstOrDefault(c => c.InvoiceId == item.invoiceId)?.Notes ?? "") : ""),
                });
            }
            bool Success = false;
            if (request.isUpdate)
            {
                var deleteDetalies = await request.GLJournalEntryDetailsCommand.DeleteAsync(c => c.JournalEntryId == JournalEntry.Id);
                if (!deleteDetalies)
                    return false;
                UpdateJournalEntryRequest JEntryUpdate = new UpdateJournalEntryRequest();

                JEntryUpdate.BranchId = JournalEntry.BranchId;
                JEntryUpdate.FTDate = request.MastreRec.RecieptDate;
                JEntryUpdate.Id = JournalEntry.Id;
                JEntryUpdate.Notes = request.note;
                JEntryUpdate.fromSystem = true;
                JEntryUpdate.journalEntryDetails = JEntry.JournalEntryDetails;
                var res = await _mediator.Send(JEntryUpdate);
                Success = res.Success;
            }
            else
            {
                var res = await _mediator.Send(JEntry);
                Success = res.Success;
            }
            if (Success)
                return true;
            else
                return false;
        }

        //Models
        public class InvoicePaysObjectBuilderDTO
        {
            public UpdateinvoiceForCollectionReceiptRequest invoices { get; set; }
            public List<GlReciepts> recs { get; set; }
        }
        public class JournalEntryIntegrationDTO
        {
            public GlReciepts MastreRec { get; set; }
            public IMediator mediator { get; set; }
            public List<UpdateinvoiceForCollectionReceiptRequestList> recInvoices { get; set; }
            public IQueryable<InvoiceMaster> invoices { get; set; }
            public UserInformationModel userinfo { get; set; }
            public string note { get; set; }
            public int safeOrBankFAId { get; set; }
            public bool isUpdate { get; set; } = false;
            public IRepositoryQuery<GLJournalEntry> GLJournalEntryQuery { get; set; }
            public IRepositoryCommand<GLJournalEntryDetails> GLJournalEntryDetailsCommand { get; set; }

        }
    }
}
