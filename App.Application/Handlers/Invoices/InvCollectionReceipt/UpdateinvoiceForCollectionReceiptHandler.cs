using App.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.InvCollectionReceipt
{
    public class UpdateinvoiceForCollectionReceiptHandler : IRequestHandler<UpdateinvoiceForCollectionReceiptRequest, ResponseResult>
    {
        private readonly IRepositoryCommand<InvoiceMaster> InvoiceMasterCommand;
        private readonly IRepositoryQuery<InvoiceMaster> InvoiceMasterQuery;
        private readonly IPaymentMethodsForInvoiceService paymentMethodsForInvoiceService;
        private readonly IRoundNumbers roundNumbers;
        public UpdateinvoiceForCollectionReceiptHandler(IRepositoryCommand<InvoiceMaster> invoiceMasterCommand, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery, IPaymentMethodsForInvoiceService paymentMethodsForInvoiceService, IRoundNumbers roundNumbers)
        {
            InvoiceMasterCommand = invoiceMasterCommand;
            InvoiceMasterQuery = invoiceMasterQuery;
            this.paymentMethodsForInvoiceService = paymentMethodsForInvoiceService;
            this.roundNumbers = roundNumbers;
        }

        /*  public async Task<ResponseResult> Handle(UpdateinvoiceForCollectionReceiptRequest receiptRequest, CancellationToken cancellationToken)
          {
              var invoice = InvoiceMasterQuery.TableNoTracking.Where(a => a.InvoiceId == receiptRequest.invoiceId);
             if(invoice.Count() > 0)
              {
                  var invoice_ = invoice.First();
                  invoice_.Remain = invoice_.Net - receiptRequest.paid;
                  invoice_.Paid = receiptRequest.paid;
                  invoice_.IsCollectionReceipt = true;
                  InvoiceMasterCommand.Update(invoice_);
                  bool res =await InvoiceMasterCommand.SaveAsync();
                  if (res)
                      return new ResponseResult() { Result = Result.Success };
                  else
                      return new ResponseResult() { Result = Result.Failed };

              }
              else
              {
                  return new ResponseResult() {Result=Result.NotExist, ErrorMessageAr = ErrorMessagesAr.InvoiceNotExist, ErrorMessageEn = ErrorMessagesEn.InvoiceNotExist };
              }
          }*/

        public async Task<ResponseResult> Handle(UpdateinvoiceForCollectionReceiptRequest receiptRequest, CancellationToken cancellationToken)
        {
            var invoices = InvoiceMasterQuery.TableNoTracking.Where(a => receiptRequest.invoicesList.Select(e => e.invoiceId).Contains(a.InvoiceId));
            if (invoices.Count() > 0)
            {
                var invoicesMaster = new List<InvoiceMaster>();
                var collectionPaymentMethodsList = new List<CollectionPaymentMethods>();
                foreach (var rec in receiptRequest.invoicesList)
                {

                    var invoice_ = invoices.Where(a => a.InvoiceId == rec.invoiceId).First();


                    invoice_.Paid += roundNumbers.GetRoundNumber(receiptRequest.signal * rec.paid);
                    if (!Lists.storesInvoicesList.Contains(invoice_.InvoiceTypeId))
                    {
                        // determine type of payment
                        if (invoice_.Paid == 0)
                        {
                            invoice_.PaymentType = (int)PaymentType.Delay;
                        }
                        if (invoice_.Paid < invoice_.Net && invoice_.Paid != 0)
                        {
                            invoice_.PaymentType = (int)PaymentType.Partial;
                        }
                        if (invoice_.Paid >= invoice_.Net)
                        {
                            invoice_.PaymentType = (int)PaymentType.Complete;
                        }
                    }
                    invoice_.Remain = roundNumbers.GetRoundNumber(invoice_.Net - invoice_.Paid);

                    invoice_.IsCollectionReceipt = true;

                    if (invoice_.Net == invoice_.Remain || receiptRequest.signal == -1) invoice_.IsCollectionReceipt = false;

                    if (rec.CollectionPaymentMethods != null && rec.CollectionPaymentMethods.Count() > 0)
                    {
                        rec.CollectionPaymentMethods.ForEach(a =>
                                         collectionPaymentMethodsList.Add(new CollectionPaymentMethods()
                                         {
                                             invoiceId = rec.invoiceId,
                                             Cheque = a.Cheque,
                                             PaymentMethodId = a.PaymentMethodId,
                                             Value = a.Value,
                                             branchId = rec.branchId,
                                             signal = rec.signal,
                                         }));
                    }
                    invoicesMaster.Add(invoice_);
                }
                bool res = await InvoiceMasterCommand.UpdateAsyn(invoicesMaster);
                if (collectionPaymentMethodsList.Count() > 0)
                {
                    await paymentMethodsForInvoiceService.updatePaymentMethodsForCollectionReceipt(collectionPaymentMethodsList);

                }
                if (res)
                    return new ResponseResult() { Data = invoicesMaster, Result = Result.Success };
                else
                    return new ResponseResult() { Result = Result.Failed };

            }
            else
            {
                return new ResponseResult() { Result = Result.NotExist, ErrorMessageAr = ErrorMessagesAr.InvoiceNotExist, ErrorMessageEn = ErrorMessagesEn.InvoiceNotExist };
            }
        }
    }
}
