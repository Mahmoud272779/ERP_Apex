using App.Application.Services.Process.Inv_General_Settings;
using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Infrastructure.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Helpers
{
    public class ReportData<T>
    {
        #region impComment
        //public static string paymentMethod(IGrouping<object, InvoicePaymentsMethods> x )
        //{
        //    string paymentMethod = "";
        //    paymentMethod = (x.First().InvoicesMaster.PaymentType==(int)PaymentType.Partial? "جزئي" : 
        //                     ((x.First().InvoicesMaster.PaymentType == (int)PaymentType.Delay ? "آجل"  :
        //                     (x.Count()>1 ? "أخرى" : x.First().PaymentMethod.ArabicName)
        //                ))); 
        //    return paymentMethod;

        //}
        #endregion

        public static string paymentMethod(IGrouping<object, InvoiceMaster> x)
        {
            string paymentMethod = "";
            paymentMethod = (x.First().PaymentType == (int)PaymentType.Partial ? "جزئي" :
                             (x.First().PaymentType == (int)PaymentType.Delay ? "آجل" : "نقدي"));
            return paymentMethod;

        }
        // Operations for item
        public static double Quantity(IGrouping<object, InvoiceDetails> x, double rptFactor)
        {

            var qyt = x.Sum(a => (a.Quantity * a.Signal * a.ConversionFactor) / rptFactor);
            return qyt;
        }
        public static double Quantity(IGrouping<object, InvoiceDetails> x, double rptFactor, IRoundNumbers _roundNumbers)
        {

            double balance = 0.0;
            foreach (var item in x)
            {
                var value = _roundNumbers.GetRoundNumber((item.Quantity * item.Signal * item.ConversionFactor) / rptFactor, 6);
                balance = _roundNumbers.GetRoundNumber(balance + value, 6);
            }
            return balance;
        }
        public static double Quantity(IGrouping<object, InvoiceDetails> x, double rptFactor, IRoundNumbers _roundNumbers, bool isPurchases)
        {

            double balance = 0.0;
            foreach (var item in x)
            {
                var value = _roundNumbers.GetRoundNumber((item.Quantity * (isPurchases ? item.Signal * 1 : item.Signal * -1) * item.ConversionFactor) / rptFactor, 6);
                balance = _roundNumbers.GetRoundNumber(balance + value, 6);
            }
            return balance;
        }
        public static double Quantity(IGrouping<int, InvoiceDetails> x, double rptFactor)
        {

            var qyt = x.Sum(a => (a.Quantity * a.Signal * a.ConversionFactor) / rptFactor);
            return qyt;
        }
        public static double avgPrice(IGrouping<object, InvoiceDetails> x, double rptFactor)
        {
            return x.Sum(e => e.Quantity * e.Price * e.Signal) /
                               x.Sum(x => x.Quantity * x.ConversionFactor * x.Signal) * rptFactor;
        }
        public static double avgPriceWithoutVat(IGrouping<object, InvoiceDetails> x, double rptFactor)
        {
            return x.Sum(e => e.Quantity * (e.Price / (1 + (e.VatRatio / 100))) * e.Signal) /
                               x.Sum(x => x.Quantity * x.ConversionFactor * x.Signal) * rptFactor;
        }
        public static double Total(IGrouping<object, InvoiceDetails> x)
        {
            return x.Sum(a => a.Price * a.Quantity * a.Signal);
        }
        public static double Total(IGrouping<object, InvoiceDetails> x,bool isPurchases)
        {
            return x.Sum(a => a.Price * a.Quantity * (isPurchases ? a.Signal * 1 : a.Signal * -1));
        }
        public static double Discount(IGrouping<object, InvoiceDetails> x)
        {
            var dis = x.Sum(a => (a.SplitedDiscountValue + a.DiscountValue) * a.Signal);
            return dis;
        }
        public static double Net(IGrouping<object, InvoiceDetails> x)
        {
            return Total(x) - Discount(x);
        }
        public static double Vat(IGrouping<object, InvoiceDetails> x)
        {
            return Net(x) / (x.First().InvoicesMaster.PriceWithVat == true ? 100 + (x.First().VatValue) : 100)
                                    * (x.First().VatValue);
        }
        public static double NetWithVat(IGrouping<object, InvoiceDetails> x)
        {
            return Net(x) + (x.First().InvoicesMaster.PriceWithVat == false ? Vat(x) : 0);
        }

        //total discount for each item in all invoices
        public static double DiscountForEachItem(IGrouping<object, InvoiceDetails> x)
        {
            return x.Sum(a => (a.SplitedDiscountValue + a.DiscountValue) * a.Signal);
        }
        public static double NetForEachItem(IGrouping<object, InvoiceDetails> x)
        {
            return Total(x) - DiscountForEachItem(x);
        }
        public static double VatForEachItem(IGrouping<object, InvoiceDetails> x)
        {
            return Convert.ToDouble(x.Sum(a => a.VatValue * a.Signal));
        }
        public static double SumVatForNet(IGrouping<object, InvoiceDetails> x)
        {
            return Convert.ToDouble(x.Sum(a => a.VatValue * a.Signal * (a.InvoicesMaster.PriceWithVat == true ? 0 : 1)));
        }
        public static double NetWithVatForEachItem(IGrouping<object, InvoiceDetails> x)
        {
            return NetForEachItem(x) - SumVatForNet(x);
        }
    }
}
