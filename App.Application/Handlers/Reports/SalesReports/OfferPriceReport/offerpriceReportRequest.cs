using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.SalesReports.OfferPriceReport
{
    public class offerpriceReportRequest :  PaginationVM, IRequest<ResponseResult>
    {
        public int customerId { get; set; } = 0;
        [AllowNull]
        public string? customerPhone { get; set; }
        public double priceFrom { get; set; } = 0;
        [AllowNull]
        public double? priceTo { get; set; }
        public OfferPriceStatues statues { get; set; } = 0;
        public int salesManId { get; set; } = 0;
        public string branches { get; set; } = "0";
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool? isPrint { get; set; }
    }
    public class OfferPriceReportResponse
    {
        public bool isCustomer { get; set; }
        public string offerPriceDate { get; set; }
        public string code { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string Phone { get; set; }
        public string email { get; set; }
        public string salesmanNameAr { get; set; }
        public string salesmanNameEn { get; set; }
        public double totalPrice { get; set; }
        public int statues { get; set; }

        public int InvoiceId { get; set; }
        public int InvoiceTypeId { get; set; }
        public int PageEnum { get; set; }
        public int PageId { get; set; }
        public int DocTypeId { get; set; }
    }
    public enum OfferPriceStatues
    {
        all = 0,
        accredited = 5,
        notAccredited = 7,
        deleted = 6
    }
}
