using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;

namespace App.Application.Handlers.Invoices.ItemsFunds.AddItemsFunds
{
    public class DeleteItemsFundRequest : SharedRequestDTOs.Delete, IRequest<ResponseResult>
    {

    }
}
