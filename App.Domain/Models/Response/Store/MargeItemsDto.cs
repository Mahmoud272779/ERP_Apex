using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class MargeItemsDto
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
        public double Quantity { get; set; }
        public double PurchasePrice { get; set; }//سعر الشراء
        public double DiscountValue { get; set; }//قيمه الخصم على مستوى الصنف
        public double DiscountRatio { get; set; }// نسبه الخصم على مستوى الصنف
        public double VatRatio { get; set; }// الضريبه على مستوى الصنف 
        public double VatValue { get; set; }// الضريبه على مستوى الصنف 
        public DateTime? ExpireDate { get; set; }
        public double SplitedDiscount { get; set; } // الخصم الاجمالى الموزع
    }
}
