using App.Application.Handlers.Invoices.OfferPrice.AddOfferPrice;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services;
using App.Domain.Entities.Process;
using App.Domain.Entities.Process.General;
using App.Infrastructure;
using DocumentFormat.OpenXml.Spreadsheet;
using FastReport.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.ConvertOffline
{ 
    public class addReceiptsForOfflinePosHandler : BaseClass,IRequestHandler<addReceiptsForOfflinePosRequest, bool>
    {
        private readonly IRepositoryQuery<InvPaymentMethods> PaymentsMethodsQuery;
        private readonly IRepositoryCommand<GLRecieptsHistory> ReceiptsHistoryRepositoryCommand;

        private readonly IRepositoryCommand<GlReciepts> ReceiptCommand;
        private readonly IGeneralAPIsService GeneralAPIsService;
        private readonly IHttpContextAccessor httpContext;

        public addReceiptsForOfflinePosHandler(IRepositoryQuery<GlReciepts> receiptQuery,
             IRepositoryCommand<GlReciepts> receiptCommand, 
             IRepositoryQuery<InvPaymentMethods> paymentsMethodsQuery,
             IGeneralAPIsService generalAPIsService, 
             IRepositoryCommand<GLRecieptsHistory> receiptsHistoryRepositoryCommand, 
             IHttpContextAccessor _httpContext) :base(_httpContext)
        {
            ReceiptCommand = receiptCommand;
            PaymentsMethodsQuery = paymentsMethodsQuery;
            GeneralAPIsService = generalAPIsService;
            ReceiptsHistoryRepositoryCommand = receiptsHistoryRepositoryCommand;
            this.httpContext = _httpContext;
        }
        public async Task<bool> Handle(addReceiptsForOfflinePosRequest request, CancellationToken cancellationToken)
        {

            int authorty = AuthorityTypes.customers;
            var recieptsTypeList = new List< string>();
            ResponseResult result = new ResponseResult();
            List<ReceiptsOfflinePos> reciepts = new List<ReceiptsOfflinePos>();
                var safeOrBanks = PaymentsMethodsQuery.TableNoTracking
                    .Where(h => (request.invoicesPaymentMethods.Select(a => a.PaymentMethodId)).Contains(h.PaymentMethodId)).ToList();
                foreach(var Invoice in request.invoiceMasters)
                {
                     var paymentMethodList = request.invoicesPaymentMethods.Where(a=>a.InvoiceId == Invoice.InvoiceId).ToList();
                    if(paymentMethodList !=null && Invoice.Paid>0)
                    {
                        foreach (var item in paymentMethodList)
                        {
                            var safeOrBank = safeOrBanks.Where(a => a.PaymentMethodId == item.PaymentMethodId).First();
                        // var paymentmethod =  InvPaymentMethod.Where(h => h.PaymentMethodId == item.PaymentMethodId).FirstOrDefault();
                        ReceiptsOfflinePos ReceiptsData = new ReceiptsOfflinePos()
                            {
                                SafeID = safeOrBank.SafeId,
                                BankId = safeOrBank.BankId,
                                Amount = Invoice.Net < item.Value ? Invoice.Net : item.Value,
                                RecieptDate = Invoice.InvoiceDate,
                                PaymentMethodId = item.PaymentMethodId,
                                ChequeBankName = "",
                                ChequeNumber = item.Cheque,
                                ChequeDate = Invoice.InvoiceDate,
                                ParentType = Invoice.InvoiceType,
                                ParentTypeId = Invoice.InvoiceTypeId,
                            //Creditor = Invoice.Net < item.Value ? Invoice.Net : item.Value,
                            //Debtor = Invoice.Net < item.Value ? Invoice.Net : item.Value,
                            RecieptTypeId = GetReceiptsType(safeOrBank.SafeId, safeOrBank.BankId, Invoice.InvoiceTypeId),
                                IsAccredit = false,
                                ParentId = Invoice.InvoiceId,
                                Serialize = Invoice.Serialize,
                                Code = Invoice.Code,
                                BenefitId = Invoice.PersonId.Value,
                                Authority = authorty,
                                ReceiptOnly = true,
                                fromInvoice = true,
                                isPartialPaid = Invoice.PaymentType,
                                Notes = Invoice.Notes,
                                  RecieptType = Invoice.InvoiceType,
                            RectypeWithPayment = Invoice.InvoiceType + "-"+ item.PaymentMethodId,
                            PaperNumber = Invoice.InvoiceType,
                            CreationDate = Invoice.InvoiceDate,
                            BranchId = Invoice.BranchId,
                            UserId = Invoice.EmployeeId, 
                            PersonId = Invoice.PersonId

                        };
                        ReceiptsData.Signal=  GeneralAPIsService.GetSignal(ReceiptsData.RecieptTypeId);
                        ReceiptsData.Creditor = ReceiptsData.Amount; // pos
                        ReceiptsData.Debtor = ReceiptsData.Amount;  // r_pos

                        reciepts.Add(ReceiptsData);

                        }

                    }

                    if (Invoice.Remain > 0)
                    {
                    ReceiptsOfflinePos DeferreReceiptsData = new ReceiptsOfflinePos()
                        {
                            SafeID = 1,
                            BankId = null,
                            Amount = Invoice.Remain,
                            RecieptDate = Invoice.InvoiceDate,
                            PaymentMethodId = 1,
                            ChequeBankName = "",
                            ChequeNumber = "",
                            ChequeDate = Invoice.InvoiceDate,
                            ParentType = Invoice.InvoiceType,
                            ParentTypeId = Invoice.InvoiceTypeId,
                            //Creditor = signal < 0 ? Invoice.Remain : 0,
                            //Debtor = signal > 0 ? Invoice.Remain : 0,
                            RecieptTypeId = GetReceiptsType(1, null, Invoice.InvoiceTypeId),
                            IsAccredit = false,
                            ParentId = Invoice.InvoiceId,
                            Serialize = Invoice.Serialize,
                            Code = Invoice.Code,
                            BenefitId = Invoice.PersonId.Value,
                            Deferre = true  ,
                            fromInvoice = true  ,
                            Authority = authorty,
                            ReceiptOnly = true,
                            Notes = Invoice.Notes,
                        RecieptType =Invoice.InvoiceType,
                        RectypeWithPayment = Invoice.InvoiceType+"--1",
                        PaperNumber = Invoice.InvoiceType,
                        CreationDate = Invoice.InvoiceDate,
                        BranchId = Invoice.BranchId,
                        UserId = Invoice.EmployeeId,
                         PersonId = Invoice.PersonId

                    };
                    DeferreReceiptsData.Signal = GeneralAPIsService.GetSignal(DeferreReceiptsData.RecieptTypeId);
                    DeferreReceiptsData.Creditor = DeferreReceiptsData.Signal < 0 ? Invoice.Remain : 0;
                    DeferreReceiptsData.Debtor = DeferreReceiptsData.Signal > 0 ? Invoice.Remain : 0;
                       reciepts.Add(DeferreReceiptsData);


                }
            }
            var MappedData = new List<GlReciepts>();
            Mapping.Mapper.Map(reciepts, MappedData);

        //     MappedData.RectypeWithPayment = parameter.ParentType + "-" + MappedData.PaymentMethodId;
        
        //     MappedData.Creditor = MappedData.Debtor = parameter.Amount;
        //    if (parameter.Deferre || parameter.ParentTypeId == (int)DocumentType.AcquiredDiscount || parameter.ParentTypeId == (int)DocumentType.PermittedDiscount)
        //    {
        //        MappedData.Creditor = MappedData.Signal < 0 ? MappedData.Amount : 0;
        //        MappedData.Debtor = MappedData.Signal > 0 ? MappedData.Amount : 0;
        //    }

       
        //MappedData.RecieptDate = GeneralAPIsService.serverDate(parameter.RecieptDate);
        //        MappedData.CreationDate = GeneralAPIsService.serverDate(parameter.RecieptDate);
        //        MappedData.NoteAR = string.Concat(Set_RecieptType.Item3, " _ ", MappedData.RecieptType);
        //MappedData.NoteEN = Set_RecieptType.Item4 + " _ " + MappedData.RecieptType;



             ReceiptCommand.AddRangeAsync(MappedData);
           //var res= await ReceiptCommand.SaveAsync();

            addHistory(MappedData);
            return true;
        }
        private int GetReceiptsType(int? safeID, int? bankId, int? ParentTypeId)
        {
            if (ParentTypeId == null)
                return 0;
            if ( ParentTypeId == (int)DocumentType.ReturnPOS )
            {
                if (safeID != null && safeID > 0)//recepttype = سند صرف خزينه
                    return (int)DocumentType.SafePayment;
                else if (bankId != null && bankId > 0)
                    return (int)DocumentType.BankPayment;

            }
            if (ParentTypeId == (int)DocumentType.POS)
            {
                if (safeID != null && safeID > 0)//recepttype = سند قبض خزينه
                    return (int)DocumentType.SafeCash;
                else if (bankId != null && bankId > 0)
                    return (int)DocumentType.BankCash;
            }
            return 0;
        }

        private async Task<bool> addHistory(List<GlReciepts> recieptsList)
        {
            var recHistoryList = new List<GLRecieptsHistory>();
            foreach(var reciept in recieptsList)
            {
                var history = new GLRecieptsHistory()
                {
                    employeesId = reciept.UserId,
                    ReceiptsId = reciept.Id,
                    SafeIDOrBank = (reciept.SafeID>0 && reciept.SafeID !=null? reciept.SafeID.Value:reciept.BankId.Value),
                    Code = reciept.Code,
                    RecieptDate = reciept.RecieptDate,
                    Amount = reciept.Amount,
                    AuthorityType = reciept.Authority,
                    PaymentMethodId = reciept.PaymentMethodId,
                    RecieptTypeId = reciept.RecieptTypeId,
                    RecieptType = reciept.RecieptType,
                    Signal = reciept.Signal,
                    Serialize = reciept.Serialize,
                    BranchId = reciept.BranchId,
                    UserId = reciept.UserId,
                    IsAccredit = reciept.IsAccredit,
                    BenefitId = reciept.BenefitId,
                    IsBlock = reciept.IsBlock,
                    ReceiptsAction = HistoryActions.Add,
                    LastDate = DateTime.Now,
                    BrowserName = contextHelper.GetBrowserName(httpContext.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString())
                };
                recHistoryList.Add(history);
            }

            ReceiptsHistoryRepositoryCommand.AddRange(recHistoryList);
         var res=  await  ReceiptsHistoryRepositoryCommand.SaveChanges();
            return res > 0 ? true : false;

        }
    }
}
