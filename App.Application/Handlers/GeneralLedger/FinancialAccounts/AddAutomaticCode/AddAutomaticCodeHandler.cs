using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class AddAutomaticCodeHandler : IRequestHandler<AddAutomaticCodeRequest, string>
    {
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;

        public AddAutomaticCodeHandler(IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery)
        {
            this.financialAccountRepositoryQuery = financialAccountRepositoryQuery;
        }

        public async Task<string> Handle(AddAutomaticCodeRequest request, CancellationToken cancellationToken)
        {
            var code = financialAccountRepositoryQuery.FindQueryable(q => q.Id > 0);
            if (code.Count() > 0)
            {
                var code2 = financialAccountRepositoryQuery.FindQueryable(q => q.Id > 0).ToList().Last();
                var coo = code2.AccountCode;
                var codee = long.Parse(coo.ToString());
                if (codee == 0)
                {
                }
                var NewCode = codee + 1;
                return NewCode.ToString();
            }
            else
            {
                var NewCode = 1;
                return NewCode.ToString();
            }
        }
    }
}
