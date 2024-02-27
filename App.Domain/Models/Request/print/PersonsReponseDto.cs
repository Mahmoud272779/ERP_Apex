using App.Domain.Entities.POS;
using App.Domain.Entities.Process;
using App.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.print
{
    public class PersonsReponseDto
    {
        public int id { get; set; }
        public string code { get; set; }
        public string arabicName { get; set; }
        public string? latinName { get; set; }
        public int type { get; set; } //نوع المورد
        public int status { get; set; }// نشط 
        public object SalesManId { get; set; }
        public string responsibleAr { get; set; }
        public string responsibleEn { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string taxNumber { get; set; }
        public string addressAr { get; set; }
        public string addressEn { get; set; }
        public bool addToAnotherList { get; set; }
        public bool isSupplier { get; set; }
        public int[] branches { get; set; }
        public string branchNameAr { get; set; }
        public string branchNameEn { get; set; }
        public double? CreditLimit { get; set; }

        public int? CreditPeriod { get; set; }
        public double? DiscountRatio { get; set; }
        public int? SalesPriceId { get; set; }

        public int? LessSalesPriceId { get; set; }
        public bool CanDelete { get; set; }
        public string BuildingNumber { get; set; }
        public string StreetName { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalNumber { get; set; }


        public object FinancialAccountId { get; set; }

        public string StatusAr { get
            {
                if (status == 1) return "نشط";
                else if (status == 2) return "غير نشط";
                else return "";
            }
        }
        public string StatusEn {
            get
            {
                if (status == 1) return "Active";
                else if (status == 2) return "InActive";
                else return "";
            }
        }
        public string TypeAr {
            get
            {
                if (status == 0) return "الكل";
                else if (status == 1) return "عادى";
                else if (status == 2) return "قطاعي";
                else if (status == 3) return "جملة";
                else if (status == 4) return "نصف جملة";
                else return "";
            }
        }
        public string TypeEn {
            get
            {
                if (status == 0) return "All";
                else if (status == 1) return "Normal";
                else if (status == 2) return "Sectoral";
                else if (status == 3) return "Bulk";
                else if (status == 4) return "Half Bulk";
                else return "";
            }
        }


        public bool isUsedInInvoices { get; set; }










    }
}
