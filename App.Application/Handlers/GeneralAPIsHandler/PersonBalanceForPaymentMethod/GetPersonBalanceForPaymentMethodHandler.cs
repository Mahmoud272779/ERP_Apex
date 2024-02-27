using App.Application.Handlers.InvoicesHelper.GetReceiptBalanceForBenifit;
using App.Application.Handlers.Persons.GetPersonBalance;
using App.Domain.Models.Security.Authentication.Response.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod
{
    public  class GetPersonBalanceForPaymentMethodHandler : IRequestHandler<GetPersonBalanceForPaymentMethodRequest, GetPersonBalanceForPaymentMethodResponse>
    {
        private readonly IMediator _mediator;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> InvoicePaymentMethodsQuery;
        private readonly IRepositoryQuery<InvGeneralSettings> InvGeneralSettingsQuery;
        private readonly IRepositoryQuery<InvoiceMaster> _invoiceMasterQuery;


        public GetPersonBalanceForPaymentMethodHandler(IMediator mediator, IRepositoryQuery<InvoicePaymentsMethods> invoicePaymentMethodsQuery, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsQuery, IRepositoryQuery<InvoiceMaster> invoiceMasterQuery)
        {
            _mediator = mediator;
            InvoicePaymentMethodsQuery = invoicePaymentMethodsQuery;
            InvGeneralSettingsQuery = invGeneralSettingsQuery;
            _invoiceMasterQuery = invoiceMasterQuery;
        }
        public async Task<GetPersonBalanceForPaymentMethodResponse> Handle(GetPersonBalanceForPaymentMethodRequest request, CancellationToken cancellationToken)
        {
            var roundNumber = InvGeneralSettingsQuery.TableNoTracking.First().Other_Decimals;
            double balance = 0;
            int authory = (int)AuthorityTypes.suppliers;

            if (request.invoiceTypeId == (int)DocumentType.Sales)
                authory = (int)AuthorityTypes.customers;
            List<personsForBalanceDto> personsForBalance = new List<personsForBalanceDto>();
            personsForBalance.Add(new personsForBalanceDto() { Id = request.personId });
            var res = await _mediator.Send(new GetReceiptBalanceForBenifitForInvoicesRequest()
            { AuthorityId = authory, persons = personsForBalance, fromGetInvoice = true,BranchId=request.BranchId });
            var data = (personsForBalanceDto)res.Data;

        
            if (request.invoiceTypeId == (int)DocumentType.Sales )
            {
                balance = (data.isCreditor ? data.balance : -data.balance);

            }
            else
            {
                 balance = (!data.isCreditor ? data.balance : -data.balance);

            }

            if (request.invoiceId > 0)  // in update return the old value to balance 
            {
                var oldInvoice= _invoiceMasterQuery.TableNoTracking.Where(a=>a.InvoiceId== request.invoiceId).FirstOrDefault();
                if (oldInvoice.PersonId==request.personId)
                {
                    var oldValue = InvoicePaymentMethodsQuery.TableNoTracking.Where(a => a.InvoiceId == request.invoiceId
                                 && a.PaymentMethodId == (int)PaymentMethod.PersonBalance).Select(a => a.Value);
                    if (oldValue.Any())
                        balance += oldValue.First();
                }
                

            }
            if(request.invoiceTypeId==(int)DocumentType.Sales)
                data.isCreditor = balance > 0 ? true : false;

            int creditorOrDebtor = (data.isCreditor ? 0 : 1);
            var result = new GetPersonBalanceForPaymentMethodResponse()
            {
                balance = Math.Round(balance, roundNumber),
                CreditOrDebit = creditorOrDebtor,
                debitor = Math.Round(data.Debtor, roundNumber),
                creditor = Math.Round(data.Creditor, roundNumber)
            };
            return result;
        }
    }
}
