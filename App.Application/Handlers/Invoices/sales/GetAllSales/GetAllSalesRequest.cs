using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using MediatR;

namespace App.Application.Handlers.Invoices.sales.GetAllSales
{
    public class GetAllSalesRequest : InvoiceSearchPagination,IRequest<ResponseResult>
    {
    }
}
