using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;

namespace App.Application.Handlers.Invoices.sales
{
    public class AddSalesRequest : InvoiceMasterRequest, IRequest<ResponseResult>
    {
    }
}
