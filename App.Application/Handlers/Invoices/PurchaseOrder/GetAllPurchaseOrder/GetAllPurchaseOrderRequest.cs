using App.Application.Handlers.Invoices.sales.GetAllSales;
using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using MediatR;

namespace App.Application.Handlers
{
    public class GetAllPurchaseOrderRequest : GetAllOfferPriceRequest, IRequest<ResponseResult>
    {
        
    }
   
}
