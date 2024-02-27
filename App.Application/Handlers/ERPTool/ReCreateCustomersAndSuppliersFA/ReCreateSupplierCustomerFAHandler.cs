using App.Application.Handlers.ERPTool.ReCreateCustomersAndSuppliersFA.CustomersHelper;
using App.Application.Services.Process.FinancialAccounts;
using App.Domain.Models.Response.Store;
using App.Infrastructure.UserManagementDB;
using DocumentFormat.OpenXml.Drawing;
using MediatR;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ReCreateCustomersAndSuppliersFA.CustomersFA
{
    public class ReCreateSupplierCustomerFAHandler : IRequestHandler<ReCreateSupplierCustomerFARequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> _financialAccountRepositoryQuery;

        private readonly IRepositoryQuery<InvPersons> _InvPersonsQuery;
        private readonly IMediator _mediator;
        public ReCreateSupplierCustomerFAHandler(IRepositoryQuery<InvPersons> invPersonsQuery, IMediator mediator, IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            _InvPersonsQuery = invPersonsQuery;
            _mediator = mediator;
            _financialAccountRepositoryQuery = financialAccountRepositoryQuery;
        }
        public async Task<ResponseResult> Handle(ReCreateSupplierCustomerFARequest request, CancellationToken cancellationToken)
        {
            var _persons = _InvPersonsQuery
                .TableNoTracking
                .Where(c => request.Type == PersonTypes.customer ? c.IsCustomer == true : c.IsSupplier == true)
                .Where(c => c.FinancialAccountId == request.OldAccountId)
                .Include(c => c.PersonBranch);
            var persons = _persons.Take(500)
                .ToList();
            var financaialAccount = await _financialAccountRepositoryQuery.GetByIdAsync(request.newParentId);
            bool status = false;
            while (persons.Any())
            {
                status = await _mediator.Send(new CustomerSupplierFAHelperRequest
                {
                    OldAccountId = request.OldAccountId,
                    newParentId = request.newParentId,
                    person = persons.FirstOrDefault(),
                    Type = request.Type,
                    FinancialAccount = financaialAccount
                });
                if (!status)
                    break;
                persons.Remove(persons.FirstOrDefault());
                Thread.Sleep(400);
            }

            return new ResponseResult
            {
                Result = status ? Result.Success : Result.Failed,
                TotalCount = _persons.Count()
            };
        }
    }
}
