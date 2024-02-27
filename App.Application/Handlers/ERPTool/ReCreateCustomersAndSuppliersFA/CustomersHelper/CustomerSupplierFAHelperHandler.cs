using App.Application.Services.Process.FinancialAccounts;
using App.Domain.Models.Response.Store;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.ERPTool.ReCreateCustomersAndSuppliersFA.CustomersHelper
{
    public class CustomerSupplierFAHelperHandler : IRequestHandler<CustomerSupplierFAHelperRequest, bool>
    {
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly IRepositoryCommand<InvPersons> _InvPersonsCommand;

        private readonly IFinancialAccountBusiness _financialAccountBusiness;


        public CustomerSupplierFAHelperHandler(iGLFinancialAccountRelation iGLFinancialAccountRelation, IRepositoryCommand<InvPersons> invPersonsCommand, IFinancialAccountBusiness financialAccountBusiness)
        {
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            _InvPersonsCommand = invPersonsCommand;
            _financialAccountBusiness = financialAccountBusiness;
        }
        public async Task<bool> Handle(CustomerSupplierFAHelperRequest request, CancellationToken cancellationToken)
        {
            int[] branchs = request.person.PersonBranch.Select(c => c.BranchId).ToArray();
            int[] costCenters = { };


            var GLRelation = await _financialAccountBusiness.AddFinancialAccount(new FinancialAccountParameter()
            {
                AccountCode = null,
                BranchesId = branchs,
                CostCenterId = costCenters,
                CurrencyId = request.FinancialAccount.CurrencyId,
                FA_Nature = request.FinancialAccount.FA_Nature,
                FinalAccount = request.FinancialAccount.FinalAccount,
                IsMain = false,
                Notes = "",
                ParentId = request.newParentId,
                Status = 1,
                ArabicName = request.person.ArabicName,
                LatinName = request.person.LatinName
            });

            //var GLRelation = await _iGLFinancialAccountRelation.GLRelation(request.person.IsSupplier == true ? GLFinancialAccountRelation.supplier : GLFinancialAccountRelation.customer, request.newParentId, request.person.PersonBranch.Select(c => c.BranchId).ToArray(), request.person.ArabicName, request.person.LatinName);
            request.person.FinancialAccountId = (int)GLRelation.Data;
            var saved = await _InvPersonsCommand.UpdateAsyn(request.person);
            return saved;
        }
    }
}
