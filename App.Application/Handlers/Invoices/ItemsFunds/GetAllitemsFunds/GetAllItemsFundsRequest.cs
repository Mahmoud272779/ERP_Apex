using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Security.Authentication.Response.Store.Invoices;
using MediatR;

namespace App.Application.Handlers.Invoices.ItemsFunds.AddItemsFunds
{
    public class GetAllItemsFundsRequest : StoreSearchPagination, IRequest<ResponseResult>
    {

    }
}
