using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class InvoiceResultCalculateDto
    {
        public InvoiceResultCalculateDto()
        {

            itemsTotalList = new List<ItemsTotalList>();
        }
        public double TotalPrice { get; set; } = 0; // اجمالى سعر الفاتوره
        public double TotalDiscountValue { get; set; } = 0; // اجمالى قيمة خصم الفاتورة
        public double TotalDiscountRatio { get; set; } = 0;  // اجمالى نسبة خصم الفاتورة
        public double TotalAfterDiscount { get; set; } = 0;  // الاجمالى بعد الخصم

        public double TotalVat { get; set; } = 0;  // اجملى الضريبة
        public double Net { get; set; } = 0;   // الصافي
        public double ActualNet { get; set; }  // الصافي بدون تقريب

        public List<ItemsTotalList> itemsTotalList { get; set; } 
    }
  
    public class ItemsTotalList
    {
        public double Price { get; set; }
        public double SplitedDiscountValue { get; set; }  // الخصم الموزع من الخصم الاجمالى
        public double SplitedDiscountRatio { get; set; }  
        public double DiscountValue { get; set; }  // if user send value then calc ratio and opposite
        public double DiscountRatio { get; set; }
        public double VatValue { get; set; }  // قيمة الضريبه المحسوبه من النسبه الضريبيه 
        public double ItemTotal { get; set; }   // for front total without splited discount just discount value on item
        public double TotalWithSplitedDiscount { get; set; } // for reports total with splited discount
        public double  Quantity { get; set; }  // الكمية بعد التقريب علي حسب الفاتورة او السيستم او باركود الميزان
        public double VatValueForPrint { get; set; } // القيمة الضريبية بعد الخصم على الصنف فى طباعة الفاتورة
        public bool isDiscountRatioItem { get; set; }

    }
}
