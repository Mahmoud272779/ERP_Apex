using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.CheckIsCustomer
{
    public class checkIsCustomerHandler : IRequestHandler<checkIsCustomerRequest, bool>
    {
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;

        public checkIsCustomerHandler(IRepositoryQuery<InvPersons> invPersonsQuery)
        {
            _invPersonsQuery = invPersonsQuery;
        }

        public async Task<bool> Handle(checkIsCustomerRequest request, CancellationToken cancellationToken)
        {
            var checkPerson = _invPersonsQuery.TableNoTracking.Where(x => x.Id == request.personId);
            if (checkPerson.Any())
                return checkPerson.FirstOrDefault().IsCustomer;
            return false;
        }
    }
}
