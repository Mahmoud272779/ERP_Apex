using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Domain.Models.Security.Authentication.Request.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IPaymentMethodsForInvoiceService
    {
         Task<Tuple< bool,string,string>> SavePaymentMethods(int invoiceType, List<PaymentList> PaymentMethods, int InvoiceId, int BranchId, double Paid, bool isUpdate ,int roundNumber);
        Task<bool> updatePaymentMethodsForCollectionReceipt(List<CollectionPaymentMethods> request);
        Task<ResponseResult> PaymentValidation(  InvoiceMasterRequest parameter, int invoiceType, double NetAfterRecalculate, int currentBranchId,int? invoiceId, bool isEqualPaidInUpdate = false);
        Task<List<PaymentList>> AutoExtractFromPersonBalanceValidation(InvoiceMasterRequest parameter, int invoiceTypeId, double recalultedNet, bool other_AutoExtractFromCustomerSupplierBalance, bool isEqualPaidInUpdate = false, bool IsRemainChangedForUpdate = false);

    }
}
