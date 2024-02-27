using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetAllPurchases
{
    public class GetAllPurchase_WOVRequest : InvoiceSearchPagination,IRequest<ResponseResult>
    {
    }
}
