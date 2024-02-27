using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using MediatR;

namespace App.Application.Handlers.Invoices.sales.GetAllSales
{
    public class GetAllOfferPriceRequest : IRequest<ResponseResult>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        //  public int BranchId { get; set; } = 1;
        public int InvoiceTypeId { get; set; } // for purchase and return purchase

        public OfferPriceSearch Searches { get; set; } = new OfferPriceSearch();
    }
    public class OfferPriceSearch
    {
        public string SearchCriteria { get; set; }
        public int[] SubType { get; set; } // معتمد او غير معتمد او محذوف
        public int?[] PersonId { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public int? itemId { get; set; } = 0;
        public int? categoryId { get; set; } = 0;
    }
}
