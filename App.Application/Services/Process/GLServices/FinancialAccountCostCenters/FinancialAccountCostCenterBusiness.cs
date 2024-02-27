using App.Application.Basic_Process;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Attendleave.Erp.Core.APIUtilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.Process.FinancialAccountCostCenters
{
    public class FinancialAccountCostCenterBusiness : BusinessBase<GLFinancialCost>, IFinancialAccountCostCenterBusiness
    {
        private readonly IRepositoryQuery<GLFinancialCost> financialCostRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLCostCenter> costCenterRepositoryQuery;
        private readonly IMapper mapper;
        private readonly IRepositoryCommand<GLFinancialCost> financialCostRepositoryCommand;
        private readonly IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery;
        public FinancialAccountCostCenterBusiness(
            IRepositoryQuery<GLFinancialCost> FinancialCostRepositoryQuery,
            IRepositoryCommand<GLFinancialCost> FinancialCostRepositoryCommand,
            IMapper Mapper,
            IRepositoryQuery<GLCostCenter> CostCenterRepositoryQuery,
            IRepositoryQuery<GLFinancialAccount> FinancialAccountRepositoryQuery,
            IRepositoryQuery<GLJournalEntryDetails> JournalEntryDetailsQuery,
            IRepositoryActionResult repositoryActionResult) : base(repositoryActionResult)
        {
            financialCostRepositoryQuery = FinancialCostRepositoryQuery;
            financialCostRepositoryCommand = FinancialCostRepositoryCommand;
            financialAccountRepositoryQuery = FinancialAccountRepositoryQuery;
            costCenterRepositoryQuery = CostCenterRepositoryQuery;
            this.JournalEntryDetailsQuery = JournalEntryDetailsQuery;
            mapper = Mapper;
        }
        public async Task<IRepositoryActionResult> AddFinancialAccountCostCenter(FinancialCostParameter parameter)
        {
            try
            {
                var list = new List<GLFinancialCost>();
                var table = new GLFinancialCost();
                table.FinancialAccountId = parameter.FinancialAccountId;
                foreach (var item in parameter.CostCenterId)
                {
                    table.CostCenterId = item;
                    await financialCostRepositoryCommand.AddAsync(table);
                }
                return repositoryActionResult.GetRepositoryActionResult(table.FinancialAccountId, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }
        }
        public async Task<IRepositoryActionResult> AddFinancialAccountsForCostCenter(FinancialForCostParameter parameter)
        {
            try
            {
                
                var table = new List<GLFinancialCost>();
                var financialAccount = new List<int>();
                if (parameter.FinancialAccountId.Count() <= 0)
                {
                   var  financialAccountInCost = financialCostRepositoryQuery.TableNoTracking.Where(g => g.CostCenterId == parameter.CostCenterId)
                                     .Select(a => a.FinancialAccountId).ToList();

                      financialAccount = financialAccountRepositoryQuery.TableNoTracking.Where(q => !financialAccountInCost.Contains(q.Id) 
                                         && q.IsMain == false).Select(a=>a.Id).ToList();
                  

                }
                else
                    financialAccount = parameter.FinancialAccountId.ToList();

                foreach (var item in financialAccount)
                {
                    var data = new GLFinancialCost();

                    data.FinancialAccountId = item;
                   data.CostCenterId=parameter.CostCenterId;
                    table.Add(data);
                }
                  financialCostRepositoryCommand.AddRangeAsync(table);
                await financialCostRepositoryCommand.SaveAsync();
                return repositoryActionResult.GetRepositoryActionResult(financialAccount, RepositoryActionStatus.Created, message: "Saved Successfully");
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }
        }
        public async Task<IRepositoryActionResult> GetFinancialAccountCostCenter(int FinancialAccountId)
        {
            try
            {
                var list = new List<FinancialAccountCostDto>();
                var financialAccount = new FinancialAccountCostDto();
                var finacialcost = await financialCostRepositoryQuery.GetByAsync(g => g.FinancialAccountId == FinancialAccountId);
                financialAccount.FinancialAccountId = finacialcost.FinancialAccountId;
                var financialAccountName = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == financialAccount.FinancialAccountId);
                financialAccount.FInancialAccountName = financialAccountName.LatinName;
                var costs = financialCostRepositoryQuery.FindAll(g => g.FinancialAccountId == finacialcost.FinancialAccountId);

                foreach (var item in costs)
                {
                    var cost = new CostCenterList();
                    cost.CostCenterId = item.CostCenterId;
                    var CostCenterName = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == financialAccount.FinancialAccountId);
                    cost.CostCenterName = CostCenterName.LatinName;
                    financialAccount.costCenterList.Add(cost);
                }

                list.Add(financialAccount);
                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }

        }
        public async Task<IRepositoryActionResult> GetFinancialAccountsForCostCenter(int costCenterId , int pageNumber , int pageSize)
        {
            try
            {
                var list = new List<FinancialAccountForCostDto>();
                var costCenter = new FinancialAccountForCostDto();
                  var financialAccount = await financialCostRepositoryQuery.GetByAsync(g => g.CostCenterId == costCenterId);
               // var financialAccount = await JournalEntryDetailsQuery.GetByAsync(a => a.CostCenterId == costCenterId);

                if (financialAccount != null)
                {

                    costCenter.CostCenterId = financialAccount.CostCenterId ;
                    var costCenterName = await costCenterRepositoryQuery.GetByAsync(q => q.Id == costCenter.CostCenterId);
                    costCenter.CostCenterName = costCenterName.ArabicName;
                    var costs = financialCostRepositoryQuery.FindAll(g => g.CostCenterId == financialAccount.CostCenterId);
                    if (pageSize > 0 && pageNumber > 0)
                    {
                        costs = costs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                    }
                    foreach (var item in costs)
                    {
                        var financial = new FinancialAccountList();
                        financial.FinancialAccountId = item.FinancialAccountId;
                        var financialAccountName = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == financial.FinancialAccountId);
                        financial.FInancialAccountName = financialAccountName.ArabicName;
                        financial.FA_Nature = financialAccountName.FA_Nature;
                        financial.FInancialAccountCode = financialAccountName.AccountCode;
                        financial.Checked = false;
                        costCenter.financialAccountList.Add(financial);
                    }

                    list.Add(costCenter);
                  
                }
                else
                   list.Add(costCenter);
                
                    
                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }

        }
        public async Task<IRepositoryActionResult> GetFinancialAccountsWhichNotFoundForCostCenter(int pageNumber , int pageSize, int costCenterId)
        {
            try
            {
                var list = new List<FinancialAccountNotFoundList>();
                var costs = financialCostRepositoryQuery.FindQueryable(g => g.CostCenterId == costCenterId);
                var empCodes = financialCostRepositoryQuery.FindSelectorQueryable<int>(costs, q => q.FinancialAccountId);
                var financialAccounts = financialAccountRepositoryQuery.FindAll(q => !empCodes.Contains(q.Id) && q.IsMain==false);

                foreach (var item in financialAccounts)
                {
                    var financial = new FinancialAccountNotFoundList();
                    financial.FinancialAccountId = item.Id;
                    financial.FInancialAccountCode = item.AccountCode.Replace(".","");
                    financial.FInancialAccountName = item.ArabicName;
                    financial.FA_Nature = item.FA_Nature;
                    list.Add(financial);
                }
                if (pageSize > 0 && pageNumber > 0)
                {
                    list = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
              

                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");
            }

        }
       
        public async Task<ResponseResult> RemoveFinancialAccountCostCenter(FinancialCostsParameter parameter)
        {

            try
            {
                var financialCost = financialCostRepositoryQuery.FindQueryable(q => q.CostCenterId == parameter.CostCenterId);
                var list = new List<FinancialCostsParameter>();
                 
                 await financialCostRepositoryCommand.DeleteAsync(a => a.CostCenterId == parameter.CostCenterId
                                && (parameter.FinancialAccountId.Count() > 0? parameter.FinancialAccountId.Contains(a.FinancialAccountId):true));

                    financialCostRepositoryCommand.SaveAsync();
                    //foreach (var item in parameter.FinancialAccountId)
                    //{
                    //    var financialCoste = await financialCostRepositoryQuery.GetByAsync(q => q.FinancialAccountId == item && q.CostCenterId == parameter.CostCenterId);
                    //    financialCoste.CostCenterId = parameter.CostCenterId;
                    //    financialCoste.FinancialAccountId = item;
                    //    financialCostRepositoryCommand.Remove(financialCoste);
                    //}
                    //await financialCostRepositoryCommand.SaveAsync();
                
               
                return new ResponseResult { Result = Result.Success ,ErrorMessageAr="تم الحذف بنجاح" , ErrorMessageEn="Deleted succesfuly"}; // repositoryActionResult.GetRepositoryActionResult(RepositoryActionStatus.Deleted, message: "Deleted Successfully");
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = ex, Result = Result.CanNotBeDeleted ,ErrorMessageAr="لم يتم الحذف" ,ErrorMessageEn="can not be deleted"};

            }
        }
        //public async Task<IRepositoryActionResult> RemoveFinancialAccountForCostCenter(FinancialCostsParameter parameter)
        //{

        //    try
        //    {
        //        var financialCost = await financialCostRepositoryQuery.GetByAsync(q => q.CostCenterId == parameter.CostCenterId && q.FinancialAccountId == parameter.FinancialAccountId);
        //        var currency = financialCostRepositoryCommand.DeleteAsync(q => q.FinancialAccountId == financialCost.FinancialAccountId && q.CostCenterId == financialCost.CostCenterId);
        //        return repositoryActionResult.GetRepositoryActionResult(currency.Id, RepositoryActionStatus.Deleted, message: "Deleted Successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return repositoryActionResult.GetRepositoryActionResult(ex);

        //    }
        //}
        public async Task<IRepositoryActionResult> GetFinancialAccountsForCostCenters(int costCenterId)
        {
            try
            {
                var list = new List<FinancialAccountForCostDto>();
                var costCenter = new FinancialAccountForCostDto();
                var finacialcost = await financialCostRepositoryQuery.GetByAsync(g => g.CostCenterId == costCenterId);
                costCenter.CostCenterId = finacialcost.CostCenterId;
                var costCenterName = await costCenterRepositoryQuery.GetByAsync(q => q.Id == costCenter.CostCenterId);
                costCenter.CostCenterName = costCenterName.ArabicName;
                var costs = financialCostRepositoryQuery.FindAll(g => g.CostCenterId == finacialcost.CostCenterId);
                foreach (var item in costs)
                {
                    var financial = new FinancialAccountList();
                    financial.FinancialAccountId = item.FinancialAccountId;
                    var financialAccountName = await financialAccountRepositoryQuery.GetByAsync(q => q.Id == financial.FinancialAccountId);
                    financial.FInancialAccountName = financialAccountName.LatinName;
                    costCenter.financialAccountList.Add(financial);
                }

                list.Add(costCenter);
                return repositoryActionResult.GetRepositoryActionResult(list, RepositoryActionStatus.Ok);
            }
            catch (Exception ex)
            {
                return repositoryActionResult.GetRepositoryActionResult(ex, RepositoryActionStatus.Created, message: "Saved Successfully");

            }

        }
    }
}
