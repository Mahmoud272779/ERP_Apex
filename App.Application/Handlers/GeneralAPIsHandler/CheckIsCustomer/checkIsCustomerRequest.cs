using MediatR;

namespace App.Application.Handlers.InvoicesHelper.CheckIsCustomer
{
    public class checkIsCustomerRequest : IRequest<bool>
    {
        public int personId { get; set; }
    }
}
