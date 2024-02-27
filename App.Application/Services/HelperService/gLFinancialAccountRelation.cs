using App.Application.Handlers.GeneralLedger.FinancialAccounts;
using App.Application.Helpers;
using App.Application.Services.Process.FinancialAccounts;
using App.Domain.Entities;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using AutoMapper.Internal;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;

namespace App.Application.Services.HelperService
{
    public class gLFinancialAccountRelation : iGLFinancialAccountRelation
    {
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _financialAccountRepositoryQuery;
        private readonly IRepositoryQuery<GLIntegrationSettings> _gLIntegrationSettingsQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IMediator _mediator;

        public gLFinancialAccountRelation(
                            IFinancialAccountBusiness financialAccountBusiness,
                            IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery,
                            IRepositoryQuery<GLFinancialAccount> financialAccountRepositoryQuery,
                            IRepositoryQuery<GLIntegrationSettings> GLIntegrationSettingsQuery,
                            iUserInformation iUserInformation
,
                            IMediator mediator
            )
        {
            _financialAccountBusiness = financialAccountBusiness;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            _financialAccountRepositoryQuery = financialAccountRepositoryQuery;
            _gLIntegrationSettingsQuery = GLIntegrationSettingsQuery;
            _iUserInformation = iUserInformation;
            _mediator = mediator;
        }

        public async Task<ResponseResult> personsGLRelation(bool IsSupplier, int FinancialAccountId, int[] Branches, string arabicName, string latinName)
        {
            var generalSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
            int[] costCenters = { };
            if (IsSupplier)
            {
                if (generalSettings.DefultAccSupplier == 1)
                {
                    var account = await _financialAccountRepositoryQuery.GetByIdAsync(FinancialAccountId);
                    if (account == null)
                        return new ResponseResult()
                        {
                            Note = Actions.NotFound,
                            Result = Result.NotFound
                        };
                    if (account.IsMain)
                        return new ResponseResult()
                        {
                            Note = Actions.SaveFailed,
                            Result = Result.Failed
                        };
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Id = account.Id
                    };
                }
                else if (generalSettings.DefultAccSupplier == 2)
                {

                    var financaialAccount = await _financialAccountRepositoryQuery.GetByIdAsync(generalSettings.FinancialAccountIdSupplier);
                    var createFinancialAccount = await _financialAccountBusiness.AddFinancialAccount(new FinancialAccountParameter()
                    {
                        AccountCode = null,
                        BranchesId = Branches,
                        CostCenterId = costCenters,
                        CurrencyId = financaialAccount.CurrencyId,
                        FA_Nature = financaialAccount.FA_Nature,
                        FinalAccount = financaialAccount.FinalAccount,
                        IsMain = false,
                        Notes = "",
                        ParentId = generalSettings.FinancialAccountIdSupplier,
                        Status = 1,
                        ArabicName = arabicName,
                        LatinName = latinName
                    });
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Id = (int)createFinancialAccount.Data
                    };
                }
                else if (generalSettings.DefultAccSupplier == 3)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Id = generalSettings.FinancialAccountIdSupplier
                    };
                }
            }
            else
            {
                if (generalSettings.DefultAccCustomer == 1)
                {
                    var account = await _financialAccountRepositoryQuery.GetByIdAsync(FinancialAccountId);
                    if (account == null)
                        return new ResponseResult()
                        {
                            Note = Actions.NotFound,
                            Result = Result.NotFound
                        };
                    if (account.IsMain)
                        return new ResponseResult()
                        {
                            Note = Actions.SaveFailed,
                            Result = Result.Failed
                        };
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Id = account.Id
                    };
                }
                else if (generalSettings.DefultAccCustomer == 2)
                {
                    var financaialAccount = await _financialAccountRepositoryQuery.GetByIdAsync(generalSettings.FinancialAccountIdCustomer);

                    var createFinancialAccount = await _financialAccountBusiness.AddFinancialAccount(new FinancialAccountParameter()
                    {
                        AccountCode = null,
                        BranchesId = Branches,
                        CostCenterId = costCenters,
                        CurrencyId = financaialAccount.CurrencyId,
                        FA_Nature = financaialAccount.FA_Nature,
                        FinalAccount = financaialAccount.FinalAccount,
                        IsMain = false,
                        Notes = "",
                        ParentId = generalSettings.FinancialAccountIdCustomer,
                        Status = 1,
                        ArabicName = arabicName,
                        LatinName = latinName
                    });
                    var dd = createFinancialAccount.Data;
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Id = (int)createFinancialAccount.Data
                    };

                }
                else if (generalSettings.DefultAccCustomer == 3)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Success,
                        Id = generalSettings.FinancialAccountIdSupplier
                    };
                }
            }
            return new ResponseResult()
            {
                Note = Actions.SaveFailed,
                Result = Result.Failed
            };
        }

        public async Task<ResponseResult> GLRelation(GLFinancialAccountRelation type, int FinancialAccountId, int[] branchId, string arabicName, string latinName)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var glSettings = _gLIntegrationSettingsQuery.TableNoTracking.Where(x => x.GLBranchId == userInfo.CurrentbranchId);

            int defultAcc = 0;
            int financailAccount = 0;
            if (type == GLFinancialAccountRelation.customer)
            {
                defultAcc = glSettings.Where(x=> x.screenId == (int)SubFormsIds.Customers_Sales).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.Customers_Sales).FirstOrDefault().GLFinancialAccountId;
            }
            else if(type == GLFinancialAccountRelation.supplier)
            {
                defultAcc = glSettings.Where(x => x.screenId == (int)SubFormsIds.Suppliers_Purchases).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.Suppliers_Purchases).FirstOrDefault().GLFinancialAccountId;
            }
            else if(type == GLFinancialAccountRelation.bank)
            {
                defultAcc = glSettings.Where(x => x.screenId == (int)SubFormsIds.Banks_MainData).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.Banks_MainData).FirstOrDefault().GLFinancialAccountId;
            }
            else if (type == GLFinancialAccountRelation.safe)
            {
                defultAcc = glSettings.Where(x => x.screenId == (int)SubFormsIds.Safes_MainData).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.Safes_MainData).FirstOrDefault().GLFinancialAccountId;
            }
            else if (type == GLFinancialAccountRelation.employee)
            {
                defultAcc = glSettings.Where(x => x.screenId == (int)SubFormsIds.Employees_MainUnits).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.Employees_MainUnits).FirstOrDefault().GLFinancialAccountId;
            }
            else if (type == GLFinancialAccountRelation.OtherAuthorities)
            {
                defultAcc = glSettings.Where(x => x.screenId == (int)SubFormsIds.OtherAuthorities_MainData).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.OtherAuthorities_MainData).FirstOrDefault().GLFinancialAccountId;
            }
            else if (type == GLFinancialAccountRelation.salesman)
            {
                defultAcc = glSettings.Where(x => x.screenId == (int)SubFormsIds.Salesmen_Sales).FirstOrDefault().linkingMethodId;
                financailAccount = glSettings.Where(x => x.screenId == (int)SubFormsIds.Salesmen_Sales).FirstOrDefault().GLFinancialAccountId;
            }

            if (defultAcc == 1)
            {
                if (string.IsNullOrEmpty(FinancialAccountId.ToString()))
                    return new ResponseResult()
                    {
                        Note = Actions.IdIsRequired,
                        Result = Result.Failed
                    };
                var findAccount = _financialAccountRepositoryQuery.TableNoTracking.FirstOrDefault(c=> c.Id == FinancialAccountId);
                if (findAccount == null)
                    return new ResponseResult()
                    {
                        Note = Actions.NotFound,
                        Result = Result.NotFound
                    };
                if (findAccount.IsMain)
                    return new ResponseResult()
                    {
                        Note = "Financial Account can not be main",
                        Result = Result.Failed
                    };
                return new ResponseResult()
                {
                    Result = Result.Success,
                    Id = FinancialAccountId
                };
            }
            else if (defultAcc == 2)
            {
                int[] branchs = branchId;
                int[] costCenters = { };
                var financaialAccount = await _financialAccountRepositoryQuery.GetByIdAsync(financailAccount);


                var createFinancialAccount = await _financialAccountBusiness.AddFinancialAccount(new FinancialAccountParameter()
                {
                    AccountCode = null,
                    BranchesId = branchs,
                    CostCenterId = costCenters,
                    CurrencyId = financaialAccount.CurrencyId,
                    FA_Nature = financaialAccount.FA_Nature,
                    FinalAccount = financaialAccount.FinalAccount,
                    IsMain = false,
                    Notes = "",
                    ParentId = financailAccount,
                    Status = 1,
                    ArabicName = arabicName,
                    LatinName = latinName
                });
                if (createFinancialAccount.Status != RepositoryActionStatus.Created)
                {
                    return new ResponseResult()
                    {
                        Result = Result.Failed,
                        Note = createFinancialAccount.Message
                    };
                }
                return new ResponseResult()
                {
                    Result = Result.Success,
                    Id = (int)createFinancialAccount.Data
                };
            }
            else if (defultAcc == 3)
            {
                return new ResponseResult()
                {
                    Result = Result.Success,
                    Id = financailAccount
                };
            }
            return new ResponseResult()
            {
                Result = Result.Failed
            };
        }
    }
 }
