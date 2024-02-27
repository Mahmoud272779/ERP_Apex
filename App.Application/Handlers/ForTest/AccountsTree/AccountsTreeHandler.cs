//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace App.Application.Handlers.ForTest.AccountsTree
//{
//    internal class AccountsTreeHandler : IRequestHandler<AccountsTreeRequest, bool>
//    {
//        IRepositoryQuery<GLFinancialAccount> _GLFinancialAccountQuery;
//        IRepositoryCommand<GLFinancialAccount> _GLFinancialAccountCommand;

//        IRepositoryQuery<GLFinancialBranch> _GLFinancialBranchQuery;
//        IRepositoryCommand<GLFinancialBranch> _GLFinancialBranchCommand;
//        public AccountsTreeHandler(IRepositoryQuery<GLFinancialAccount> gLFinancialAccountQuery, IRepositoryCommand<GLFinancialAccount> gLFinancialAccountCommand, IRepositoryQuery<GLFinancialBranch> gLFinancialBranchQuery, IRepositoryCommand<GLFinancialBranch> gLFinancialBranchCommand)
//        {
//            _GLFinancialAccountQuery = gLFinancialAccountQuery;
//            _GLFinancialAccountCommand = gLFinancialAccountCommand;
//            _GLFinancialBranchQuery = gLFinancialBranchQuery;
//            _GLFinancialBranchCommand = gLFinancialBranchCommand;
//        }


//        public async Task<bool> Handle(AccountsTreeRequest request, CancellationToken cancellationToken)
//        {
//            var treeBranches = _GLFinancialBranchQuery.TableNoTracking;
//            _GLFinancialBranchCommand.RemoveRange(treeBranches);
//           await _GLFinancialBranchCommand.SaveChanges();
//            var tree = _GLFinancialAccountQuery.TableNoTracking;
//            _GLFinancialAccountCommand.RemoveRange(tree);
//            var ss = await _GLFinancialAccountCommand.SaveChanges();
//            var NewTree = App.Domain.Models.Shared.AccountsTree
//                                    .testDefultGLFinancialAccountList();
//            NewTree.ForEach(x => x.Id = 0);
//            _GLFinancialAccountCommand.AddRangeAsync(NewTree);
//            var newTreebranches = new List<GLFinancialBranch>();
//            foreach (var item in NewTree)
//            {
//                newTreebranches.Add(new GLFinancialBranch { BranchId = 1, FinancialId = item.Id });
//            }
//            _GLFinancialBranchCommand.AddRangeAsync(newTreebranches);
//            return true;
//        }
//    }
//}
