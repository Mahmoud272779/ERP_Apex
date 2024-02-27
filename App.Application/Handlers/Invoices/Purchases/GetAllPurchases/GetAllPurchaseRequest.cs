using App.Domain.Models.Security.Authentication.Response.PurchasesDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.GetAllPurchases
{
    public class GetAllPurchaseRequest : InvoiceSearchPagination,IRequest<ResponseResult>
    {
    }
}
