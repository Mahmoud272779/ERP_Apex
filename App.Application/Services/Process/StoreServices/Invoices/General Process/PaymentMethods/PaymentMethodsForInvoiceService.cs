using App.Application.Handlers.GeneralAPIsHandler.PersonBalanceForPaymentMethod;
using App.Application.Handlers.Invoices.InvCollectionReceipt;
using App.Application.Handlers.Persons.GetPersonBalance;
using App.Application.Helpers;
using App.Application.Services.Process.GeneralServices.RoundNumber;
using App.Domain.Entities.Process;
using App.Domain.Models.Request.GeneralLedger;
using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Application
{
    public class PaymentMethodsForInvoiceService : IPaymentMethodsForInvoiceService
    {
        private readonly IRepositoryCommand<InvoicePaymentsMethods> PaymentsMethodsCommand;
        private readonly IRepositoryQuery<InvoicePaymentsMethods> PaymentsMethodsQuery;
        private readonly IMediator _mediator;
        private readonly IRoundNumbers roundNumbers;
        private readonly iUserInformation Userinformation;

        public PaymentMethodsForInvoiceService(IRepositoryCommand<InvoicePaymentsMethods> PaymentsMethodsCommand
            , IRepositoryQuery<InvoicePaymentsMethods> PaymentsMethodsQuery,
IMediator mediator,
IRoundNumbers roundNumbers,
iUserInformation userinformation)
        {
            this.PaymentsMethodsCommand = PaymentsMethodsCommand;
            this.PaymentsMethodsQuery = PaymentsMethodsQuery;
            _mediator = mediator;
            this.roundNumbers = roundNumbers;
            Userinformation = userinformation;
        }

        public async Task<Tuple< bool,string ,string>> SavePaymentMethods(int invoiceType , List<PaymentList> PaymentMethods , int InvoiceId,int BranchId,double Paid , bool isUpdate , int roundNumber)
        {
            var saved = true;
            if (Lists.SalesWithOutDeleteInvoicesList.Contains(invoiceType) || Lists.purchasesWithOutDeleteInvoicesList.Contains(invoiceType))
            {


                // Delete Payment methods
                if (isUpdate)
                {
                    await PaymentsMethodsCommand.DeleteAsync(a => a.InvoiceId == InvoiceId);
                    /*    var invoicePaymentList = PaymentsMethodsQuery.FindAll(q => q.InvoiceId == InvoiceId).ToList();

                        if(invoicePaymentList.Count > 0)
                           PaymentsMethodsCommand.RemoveRange(invoicePaymentList);*/

                }

                if (PaymentMethods != null && PaymentMethods.Count() > 0)
                {
                    if (PaymentMethods.Count() > 0)
                    {

                        List<int> ids = new List<int>();
                        var invoicePaymentMethodList = new List<InvoicePaymentsMethods>();
                        foreach (var item in PaymentMethods)
                        {
                            if (ids.Contains(item.PaymentMethodId))
                                return new Tuple<bool, string, string>(false,ErrorMessagesAr.CantRepeatSamePaymentMethod,ErrorMessagesEn.CantRepeatSamePaymentMethod);
                            ids.Add(item.PaymentMethodId);
                            var invoicePaymentMethod = new InvoicePaymentsMethods();
                            invoicePaymentMethod.InvoiceId = InvoiceId;
                            invoicePaymentMethod.BranchId = BranchId;
                            invoicePaymentMethod.Cheque = item.Cheque;
                            invoicePaymentMethod.PaymentMethodId = item.PaymentMethodId;
                            invoicePaymentMethod.Value = Math.Round(item.Value, roundNumber);
                            invoicePaymentMethodList.Add(invoicePaymentMethod);
                        }
                        PaymentsMethodsCommand.AddRange(invoicePaymentMethodList);
                        saved = await PaymentsMethodsCommand.SaveAsync();
                    }

                }
                else if (Paid > 0)
                {
                    var invoicePaymentMethod = new InvoicePaymentsMethods();
                    invoicePaymentMethod.InvoiceId = InvoiceId;
                    invoicePaymentMethod.BranchId = BranchId;
                    invoicePaymentMethod.Cheque = "";
                    invoicePaymentMethod.PaymentMethodId = 1;//نقدي
                    invoicePaymentMethod.Value = Math.Round(Paid, roundNumber);
                    saved = PaymentsMethodsCommand.Add(invoicePaymentMethod);
                    //     saved = await PaymentsMethodsCommand.SaveAsync();

                }
            }
            return new Tuple<bool, string, string>(saved, "","");

        }


        public async Task<bool> updatePaymentMethodsForCollectionReceipt(List<CollectionPaymentMethods> request)
        {

            var paymentList = request.GroupBy(a => new { a.branchId, a.invoiceId, a.PaymentMethodId, a.signal })
                     .Select(a => new { a.Key.branchId, a.Key.invoiceId, a.Key.PaymentMethodId, a.Key.signal, Cheque = a.First().Cheque, Value = a.Sum(e => e.Value) });
            var existMethods = PaymentsMethodsQuery.TableNoTracking
                         .Where(a => paymentList.Select(e => e.invoiceId).Contains(a.InvoiceId)).ToList();

            var invoicePaymentMethodList = new List<InvoicePaymentsMethods>();

            foreach (var method in paymentList)
            {
                var exist = existMethods.Where(a => a.InvoiceId == method.invoiceId && a.PaymentMethodId == method.PaymentMethodId)
                      .Select(a => a.Value = a.Value + (method.signal * method.Value)).ToList();
                if (exist == null || exist.Count() == 0)
                {

                    var invoicePaymentMethod = new InvoicePaymentsMethods();
                    invoicePaymentMethod.InvoiceId = method.invoiceId;
                    invoicePaymentMethod.BranchId = method.branchId;
                    invoicePaymentMethod.Cheque = method.Cheque;
                    invoicePaymentMethod.PaymentMethodId = method.PaymentMethodId;
                    invoicePaymentMethod.Value = method.Value;
                    invoicePaymentMethodList.Add(invoicePaymentMethod);

                }

            }
            var methodsWillUpdated = existMethods.Where(a => a.Value > 0).ToList();
            var methodsWillDeleted = existMethods.Where(a => a.Value <= 0).ToList();

            try
            {
                if (methodsWillUpdated.Count() > 0)
                    await PaymentsMethodsCommand.UpdateAsyn(methodsWillUpdated);
                if (methodsWillDeleted.Count() > 0)
                {
                    PaymentsMethodsCommand.RemoveRange(methodsWillDeleted);
                    await PaymentsMethodsCommand.SaveAsync();
                }

                if (invoicePaymentMethodList.Count() > 0)
                    PaymentsMethodsCommand.AddRangeAsync(invoicePaymentMethodList);
            }
            catch (Exception e)
            {

                throw;
            }

            //var  saved = await PaymentsMethodsCommand.SaveAsync();
            return true;
        }

        public async Task<ResponseResult> PaymentValidation(  InvoiceMasterRequest parameter, int invoiceType, double NetAfterRecalculate, int currentBranchId,int? invoiceId, bool isEqualPaidInUpdate = false)
        {
            if (parameter.isOtherPayment.Value && invoiceType == (int)DocumentType.POS && parameter.Paid > NetAfterRecalculate)
                return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.PaidExceedsTheNet, ErrorMessageEn = ErrorMessagesEn.PaidExceedsTheNet };
            //  NewV

            if (Lists.purchasesInvoicesList.Contains(invoiceType) || Lists.salesInvoicesList.Contains(invoiceType))
            {
                if (parameter.Paid > NetAfterRecalculate && parameter.PaymentsMethods.Count() > 1)
                {
                    return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.CantPaidExceedNetWithmoreThan1mwthod, ErrorMessageEn = ErrorMessagesEn.CantPaidExceedNetWithmoreThan1mwthod };
                }
                if (invoiceType == (int)DocumentType.Sales || invoiceType == (int)DocumentType.Purchase || invoiceType == (int)DocumentType.wov_purchase)
                {

                    if (parameter.PaymentsMethods.Select(a => a.PaymentMethodId).Contains((int)PaymentMethod.PersonBalance) )
                    {

                       

                       var valueFromBalance = parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).First();

                        
                        var request = new GetPersonBalanceForPaymentMethodRequest() {invoiceId=invoiceId,
                            invoiceTypeId = invoiceType, personId = parameter.PersonId.Value, BranchId = currentBranchId };
                        var CurrentBalance = await _mediator.Send(request);
                        if (valueFromBalance.Value > CurrentBalance.balance)
                        {
                            if (!isEqualPaidInUpdate)
                            {


                                if (invoiceType == (int)DocumentType.Sales)
                                    return new ResponseResult()
                                    {
                                        Result = Result.ExceedPersonBalance,
                                        Note = "Exceeded person balance",
                                        ErrorMessageAr = ErrorMessagesAr.PaymentMethodvalueExceededCustomerBalance,
                                        ErrorMessageEn = ErrorMessagesEn.PaymentMethodvalueExceededCustomerBalance
                                    };
                                else
                                    return new ResponseResult()
                                    {
                                        Result = Result.ExceedPersonBalance,
                                        Note = "Exceeded person balance",
                                        ErrorMessageAr = ErrorMessagesAr.PaymentMethodvalueExceededSupplierBalance,
                                        ErrorMessageEn = ErrorMessagesEn.PaymentMethodvalueExceededSupplierBalance
                                    };
                            }
                        }

                        if (valueFromBalance.Value > NetAfterRecalculate)
                        {
                            if (invoiceType == (int)DocumentType.Sales)
                                return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.CustomerBalanceGreaterThanNet, ErrorMessageEn = ErrorMessagesEn.CustomerBalanceGreaterThanNet };
                            else
                                return new ResponseResult() { Result = Result.NotTotalPaid, ErrorMessageAr = ErrorMessagesAr.SupplierBalanceGreaterThanNet, ErrorMessageEn = ErrorMessagesEn.SupplierBalanceGreaterThanNet };

                        }



                        //var remain = NetAfterRecalculate- parameter.Paid;
                        //var PaidValue = parameter.PaymentsMethods.Sum(a=>a.Value);
                        //var RemainOfBalance = CurrentBalance.balance - valueFromBalance.Value;
                        //var RemainOfNet = NetAfterRecalculate - valueFromBalance.Value;
                     //   if ((remain > 0 && PaidValue < CurrentBalance.balance) ||
                       //     (CurrentBalance.balance > NetAfterRecalculate && NetAfterRecalculate > valueFromBalance.Value))
                        //if(RemainOfNet>0 && RemainOfBalance>0)
                        //{
                        //    if (invoiceType == (int)DocumentType.Sales)
                        //        return new ResponseResult { Result = Result.ExceedPersonBalance, ErrorMessageAr = ErrorMessagesAr.CustomerHasBalanceCantDefferPaid, ErrorMessageEn = ErrorMessagesEn.CustomerHasBalanceCantDefferPaid };
                        //    else
                        //        return new ResponseResult { Result = Result.ExceedPersonBalance, ErrorMessageAr = ErrorMessagesAr.SupplierHasBalanceCantDefferPaid, ErrorMessageEn = ErrorMessagesEn.SupplierHasBalanceCantDefferPaid };


                        //}
                    }

                }

            }

            return new ResponseResult() { Result = Result.Success };
        }

        public async Task<List<PaymentList>> AutoExtractFromPersonBalanceValidation( InvoiceMasterRequest parameter,int invoiceTypeId,double recalultedNet,bool other_AutoExtractFromCustomerSupplierBalance, bool isEqualPaidInUpdate = false, bool IsRemainChangedForUpdate=false)
        {
            if (parameter.Paid > 0 && (parameter.TotalDiscountValue > 0 || parameter.TotalDiscountRatio>0))
            {
                parameter.Paid = roundNumbers.GetDefultRoundNumber(recalultedNet);
                parameter.Net = recalultedNet;
            }

            var userInfo = await Userinformation.GetUserInformation();
            parameter.PaymentsMethods.RemoveAll(a => a.Value == 0);
            var request = new GetPersonBalanceForPaymentMethodRequest()
            {
                invoiceId = 0,
                invoiceTypeId = invoiceTypeId,
                personId = parameter.PersonId.Value,
                BranchId = userInfo.CurrentbranchId
            };
            var CurrentBalance = await _mediator.Send(request);
            if (parameter.Paid < recalultedNet && parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault() == null && other_AutoExtractFromCustomerSupplierBalance&&parameter.TotalDiscountValue==0)

            {
                
                if (CurrentBalance.balance > 0 )
                { 
                    

                    if (parameter.Remain > CurrentBalance.balance)
                        {
                            parameter.PaymentsMethods.Add(new PaymentList()
                            {
                                PaymentMethodId = (int)PaymentMethod.PersonBalance,
                                Value = roundNumbers.GetRoundNumber(CurrentBalance.balance),
                                Cheque = null
                            });
                        }
                        else
                        {
                            parameter.PaymentsMethods.Add(new PaymentList()
                            {
                                PaymentMethodId = (int)PaymentMethod.PersonBalance,
                                Value = roundNumbers.GetRoundNumber(parameter.Remain),
                                Cheque = null
                            });
                        }

                        parameter.Paid = parameter.PaymentsMethods.Sum(a => a.Value);
                    
                   
                }
                



            }
            else if (parameter.Paid < recalultedNet && parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault() == null && other_AutoExtractFromCustomerSupplierBalance && parameter.TotalDiscountValue > 0)

            {

                if (CurrentBalance.balance > 0)
                {


                    if (parameter.Net > CurrentBalance.balance)
                    {
                        parameter.PaymentsMethods.Add(new PaymentList()
                        {
                            PaymentMethodId = (int)PaymentMethod.PersonBalance,
                            Value = roundNumbers.GetRoundNumber(CurrentBalance.balance),
                            Cheque = null
                        });
                    }
                    else
                    {
                        parameter.PaymentsMethods.Add(new PaymentList()
                        {
                            PaymentMethodId = (int)PaymentMethod.PersonBalance,
                            Value = roundNumbers.GetRoundNumber(parameter.Net),
                            Cheque = null
                        });
                    }

                    parameter.Paid = parameter.PaymentsMethods.Sum(a => a.Value);


                }




            }

            else if (parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault() != null && other_AutoExtractFromCustomerSupplierBalance &&( parameter.TotalDiscountValue > 0||parameter.TotalDiscountRatio>0))

            {

                if (CurrentBalance.balance > 0)
                {
                    parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault().Value = parameter.Net;

                    //if (parameter.Remain > CurrentBalance.balance)
                    //{
                    //    parameter.PaymentsMethods.Add(new PaymentList()
                    //    {
                    //        PaymentMethodId = (int)PaymentMethod.PersonBalance,
                    //        Value = roundNumbers.GetRoundNumber(CurrentBalance.balance),
                    //        Cheque = null
                    //    });
                    //}
                    //else
                    //{
                    //    parameter.PaymentsMethods.Add(new PaymentList()
                    //    {
                    //        PaymentMethodId = (int)PaymentMethod.PersonBalance,
                    //        Value = roundNumbers.GetRoundNumber(parameter.Remain),
                    //        Cheque = null
                    //    });
                    //}

                    parameter.Paid = parameter.PaymentsMethods.Sum(a => a.Value);


                }




            }
            else if (parameter.Paid < recalultedNet && parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault() != null && other_AutoExtractFromCustomerSupplierBalance && isEqualPaidInUpdate && IsRemainChangedForUpdate)
            {
                if (CurrentBalance.balance > 0)
                {
                    if (parameter.Remain > CurrentBalance.balance)
                    {
                        parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault().Value += CurrentBalance.balance;
                        parameter.Paid += CurrentBalance.balance;
                    }
                    else
                    {
                        parameter.PaymentsMethods.Where(a => a.PaymentMethodId == (int)PaymentMethod.PersonBalance).FirstOrDefault().Value += parameter.Remain;
                        parameter.Paid += parameter.Remain;
                    }
                }
 
            }
            
            return parameter.PaymentsMethods;
        }
    }
}
