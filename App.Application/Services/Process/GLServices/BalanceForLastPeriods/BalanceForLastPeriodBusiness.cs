using App.Application.Basic_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.BalanceForLastPeriods
{
    public class BalanceForLastPeriodBusiness : BusinessBase<GLBalanceForLastPeriod>, IBalanceForLastPeriodBusiness
    {
        private readonly IRepositoryQuery<GLBalanceForLastPeriod> balanceForLastPeriodRepositoryQuery;
        private readonly IRepositoryCommand<GLBalanceForLastPeriod> balanceForLastPeriodRepositoryCommand;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryCommand<GLFinancialAccount> financialAccountRepositoryCommand;
        private readonly IHttpContextAccessor httpContext;
        public BalanceForLastPeriodBusiness(
            IRepositoryQuery<GLBalanceForLastPeriod> BalanceForLastPeriodRepositoryQuery,
            IRepositoryCommand<GLBalanceForLastPeriod> BalanceForLastPeriodRepositoryCommand,
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryCommand<GLFinancialAccount> FinancialAccountRepositoryCommand,
        IHttpContextAccessor HttpContext,
            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            balanceForLastPeriodRepositoryQuery = BalanceForLastPeriodRepositoryQuery;
            balanceForLastPeriodRepositoryCommand = BalanceForLastPeriodRepositoryCommand;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            financialAccountRepositoryCommand = FinancialAccountRepositoryCommand;
            httpContext = HttpContext;

        }
        public async Task<string> AddAutomaticCode()
        {
            var code = financialAccountRepositoryQuery.GetAll();
            var finan = await financialAccountRepositoryQuery.GetByAsync(q => q.ArabicName == "تكلفة النشاط التجاري");
            int codee = (Convert.ToInt32(code.OrderBy(q => q.AccountCode).Where(q=>q.ParentId==finan.Id).Last().AccountCode));

            var NewCode = codee + 1;
            // var finan = financialAccountRepositoryQuery.FindAll(q => q.AccountCode !=null).Contains(NewCode.ToString());
            return NewCode.ToString();
        }
        public async Task<IRepositoryActionResult> AddBalanceForLastPeriod(BalanceForLastPeriodParameter parameter)
        {

            try
            {
                var table = new GLBalanceForLastPeriod();
                table.BranchId = 1;
                table.Balance = parameter.Balance;
                balanceForLastPeriodRepositoryCommand.Add(table);
       
                return repositoryActionResult.GetRepositoryActionResult(table.Id, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }
        }
        public async Task<IRepositoryActionResult> GetAllBalanceForLastPeriodDropDown()
        {
            try
            {
                var treeData = balanceForLastPeriodRepositoryQuery.GetAll().ToList();
                if (treeData.Count() == 0)
                {
                    return repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.NotFound, message: "Empty data");

                }
                else
                {
                    var List = Mapping.Mapper.Map<List<GLBalanceForLastPeriod>, List<BalanceForLastPeriodDto>>(treeData.ToList());
                    List.First().Name = "رصيد اخر مدة";

                    return repositoryActionResult.GetRepositoryActionResult(List, RepositoryActionStatus.Ok, message: "Ok");

                }
                #region
                //var list = new List<BalanceForLastPeriodDto>();
                //foreach (var item in treeData)
                //{
                //    var balanceForLastDto = new BalanceForLastPeriodDto();
                //    balanceForLastDto.Id = item.Id;
                //    balanceForLastDto.Name = "رصيد اخر مدة";
                //    balanceForLastDto.Balance = item.Balance ;
                //    list.Add(balanceForLastDto);
                //}
                #endregion
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex);

            }
        }
    }
}
