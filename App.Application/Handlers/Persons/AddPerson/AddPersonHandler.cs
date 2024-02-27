using App.Application.Helpers.Service_helper.History;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons.AddPerson
{
    public class AddPersonHandler : IRequestHandler<personRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<GLBranch> branchesRepositoryQuery;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery;
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly IRepositoryCommand<InvPersons> PersonCommand;
        private readonly IRepositoryCommand<InvPersons_Branches> PersonBranchCommand;
        private readonly IRepositoryCommand<InvFundsCustomerSupplier> FundsCustomerSupplierCommand;
        private readonly IHistory<InvPersonsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        public AddPersonHandler(IRepositoryQuery<GLBranch> branchesRepositoryQuery, ISecurityIntegrationService securityIntegrationService, IRepositoryQuery<InvPersons> personQuery, iUserInformation iUserInformation, IRepositoryQuery<InvGeneralSettings> invGeneralSettingsRepositoryQuery, iGLFinancialAccountRelation iGLFinancialAccountRelation, IRepositoryCommand<InvPersons> personCommand, IRepositoryCommand<InvPersons_Branches> personBranchCommand, IRepositoryCommand<InvFundsCustomerSupplier> fundsCustomerSupplierCommand, IHistory<InvPersonsHistory> history, ISystemHistoryLogsService systemHistoryLogsService)
        {
            this.branchesRepositoryQuery = branchesRepositoryQuery;
            _securityIntegrationService = securityIntegrationService;
            PersonQuery = personQuery;
            _iUserInformation = iUserInformation;
            this.invGeneralSettingsRepositoryQuery = invGeneralSettingsRepositoryQuery;
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            PersonCommand = personCommand;
            PersonBranchCommand = personBranchCommand;
            FundsCustomerSupplierCommand = fundsCustomerSupplierCommand;
            this.history = history;
            _systemHistoryLogsService = systemHistoryLogsService;
        }



        public async Task<ResponseResult> Handle(personRequest parameter, CancellationToken cancellationToken)
        {
            var checkBranch = await branchesHelper.CheckIsBranchExist(parameter.Branches, branchesRepositoryQuery);
            if (checkBranch != null)
                return checkBranch;
            var security = await _securityIntegrationService.getCompanyInformation();
            if (parameter.IsSupplier)
            {
                if (!security.isInfinityNumbersOfSuppliers)
                {
                    var suppliersCount = PersonQuery.TableNoTracking.Where(x => x.IsSupplier == true).Count();
                    if (suppliersCount >= security.AllowedNumberOfSuppliers)
                        return new ResponseResult()
                        {
                            Note = Actions.YouHaveTheMaxmumOfSuppliers,
                            Result = Result.MaximumLength,
                            ErrorMessageAr = "تجاوزت الحد الاقصي من عدد الموردين",
                            ErrorMessageEn = "You Cant add a new supplier because you have the maximum of suppliers for your bunlde"
                        };
                }
            }
            else
            {
                if (!security.isInfinityNumbersOfCustomers)
                {
                    var suppliersCount = PersonQuery.TableNoTracking.Where(x => x.IsCustomer == true).Count();
                    if (suppliersCount >= security.AllowedNumberOfCustomers)
                        return new ResponseResult()
                        {
                            Note = Actions.YouHaveTheMaxmumOfCustomers,
                            Result = Result.MaximumLength,
                            ErrorMessageAr = "تجاوزت الحد الاقصي من عدد العملاء",
                            ErrorMessageEn = "You Cant add a new customer because you have the maximum of customers for your bunlde"
                        };
                }
                if (string.IsNullOrEmpty(parameter.SalesManId.ToString()))
                    return new ResponseResult()
                    {
                        Note = "sales man is required",
                        Result = Result.Failed
                    };
            }
            //This To Remove Add To another List option
            parameter.AddToAnotherList = false;

            var userInfo = await _iUserInformation.GetUserInformation();
            //Check settings
            var settings = await invGeneralSettingsRepositoryQuery.FindAsync(e => e.Id == 1);
            var confirmSupplierPhone = settings.Other_ConfirmeSupplierPhone;
            var confirmCustomerPhone = settings.Other_ConfirmeCustomerPhone;
            if (parameter.IsSupplier)
            {
                if (!userInfo.otherSettings.showAllBranchesInSuppliersInfo)
                    if (userInfo.employeeBranches.Where(x => !parameter.Branches.Contains(x)).Any())
                        return new ResponseResult()
                        {
                            Note = "Error Branch id",
                            Result = Result.Failed
                        };
            }
            else
            {
                if (!userInfo.otherSettings.showAllBranchesInCustomerInfo)
                    if (userInfo.employeeBranches.Where(x => !parameter.Branches.Contains(x)).Any())
                        return new ResponseResult()
                        {
                            Note = "Error Branch id",
                            Result = Result.Failed
                        };
            }
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;

            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            if (parameter.Type < (int)PersonType.Normal || parameter.Type > (int)PersonType.Midmost)
                return new ResponseResult { Result = Result.RequiredData, Note = Actions.TypeIsRequired };

            if (!string.IsNullOrEmpty(parameter.Phone))
            {
                if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
                    return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
            }
            //Applying settings on suppliers and customers 
            if (parameter.IsSupplier && confirmSupplierPhone)
            {
                if (!string.IsNullOrEmpty(parameter.Phone))
                {
                    if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
                        return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
                }
                else
                {
                    return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneIsRequired };
                }
            }

            if (!parameter.IsSupplier && confirmCustomerPhone)
            {
                if (!string.IsNullOrEmpty(parameter.Phone))
                {
                    if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
                        return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
                }
                else
                {
                    return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneIsRequired };
                }
            }

            if (!parameter.IsSupplier && parameter.SalesPriceId > parameter.LessSalesPriceId)
                return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };

            if (parameter.SalesPriceId > 0 && parameter.LessSalesPriceId == null)
                parameter.LessSalesPriceId = parameter.SalesPriceId;
            if (parameter.SalesPriceId == null && parameter.LessSalesPriceId == null)
            {
                parameter.LessSalesPriceId = (int)SalePricesList.SalePrice1;
                parameter.SalesPriceId = (int)SalePricesList.SalePrice1;
            }
            var table = Mapping.Mapper.Map<PersonRequest, InvPersons>(parameter);

            if (parameter.Email == "")
                table.Email = null;

            if (parameter.Fax == "")
                table.Fax = null;
            if (parameter.Phone == "")
                table.Phone = null;

            var PhoneExist = await PersonQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone)
                                       && ((parameter.IsSupplier) ?
                                 (parameter.AddToAnotherList == true ? true : a.IsSupplier == true) :
                                 (parameter.AddToAnotherList == true ? true : a.IsCustomer == true)));

            if (PhoneExist != null)
                return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };

            var EmailExist = await PersonQuery.GetByAsync(a => a.Email == parameter.Email && !string.IsNullOrEmpty(parameter.Email)
                                      && ((parameter.IsSupplier) ?
                                 (parameter.AddToAnotherList == true ? true : a.IsSupplier == true) :
                                 (parameter.AddToAnotherList == true ? true : a.IsCustomer == true)));

            if (EmailExist != null)
                return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };



            int NextCode = 1;
            if (parameter.IsSupplier)
            {
                NextCode = PersonQuery.GetMaxCode(e => e.Code, a => a.IsSupplier == true && a.CodeT == "S") + 1;
                if (parameter.AddToAnotherList == true)
                    table.IsCustomer = true;
                else
                {
                    table.SalesManId = null;
                    table.IsCustomer = false;
                }
                table.IsSupplier = true;
                table.CodeT = "S";
            }
            else
            {
                NextCode = PersonQuery.GetMaxCode(e => e.Code, a => a.IsCustomer == true && a.CodeT == "C") + 1;
                if (parameter.AddToAnotherList == true)
                    table.IsSupplier = true;
                else
                {
                    //table.SalesManId = null;
                    table.IsSupplier = false;
                }
                table.IsCustomer = true;
                table.CodeT = "C";
            }


            //Set New Time to Personal ( Cutomer Or Suplier ) 

            table.UTime = DateTime.Now;

            // GL relation 
            var GLRelation = await _iGLFinancialAccountRelation.GLRelation(parameter.IsSupplier ? GLFinancialAccountRelation.supplier : GLFinancialAccountRelation.customer, parameter.FinancialAccountId ?? 1, parameter.Branches, table.ArabicName, table.LatinName);
            if (GLRelation.Result == Result.Success)
                table.FinancialAccountId = GLRelation.Id;
            else
                return GLRelation;

            table.Code = NextCode;
            table.InvEmployeesId = userInfo.employeeId;
            PersonCommand.Add(table);

            List<InvPersons_Branches> personBranchList = new List<InvPersons_Branches>();

            foreach (var i in parameter.Branches)
            {
                var personBranchItem = new InvPersons_Branches();
                personBranchItem.BranchId = i;
                personBranchItem.PersonId = table.Id;
                personBranchList.Add(personBranchItem);
            }

            PersonBranchCommand.AddRange(personBranchList);
            await PersonBranchCommand.SaveAsync();

            //add new Funds for Custommer or Supplier as dfualt value 
            #region Add Entry Fund
            var branches = branchesRepositoryQuery.TableNoTracking;
            var fundsCustomerSupplier = new List<InvFundsCustomerSupplier>();
            foreach (var item in branches)
            {
                fundsCustomerSupplier.Add(new InvFundsCustomerSupplier
                {
                    Credit = 0,
                    Debit = 0,
                    PersonId = table.Id,
                    branchId = item.Id
                });
            }
            await FundsCustomerSupplierCommand.AddAsync(fundsCustomerSupplier);
            #endregion


            //BackgroundJob.Enqueue(() => Method here );
            // History for persons
            await history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(parameter.IsSupplier ? SystemActionEnum.addSupplier : SystemActionEnum.addCustomer);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };
        }
    }
}
