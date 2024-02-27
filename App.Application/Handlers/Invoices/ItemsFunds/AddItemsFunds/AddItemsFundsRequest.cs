using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;

namespace App.Application.Handlers.Invoices.ItemsFunds.AddItemsFunds
{
    public class AddItemsFundsRequest : InvoiceMasterRequest, IRequest<ResponseResult>
    {

    }
}
