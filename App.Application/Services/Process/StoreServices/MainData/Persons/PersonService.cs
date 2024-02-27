using App.Application.Handlers.Persons;
using App.Application.Handlers.Persons.GetPersonsByDate;
using MediatR;

namespace App.Application.Services.Process.Persons
{
    public class PersonService : BaseClass, IPersonService
    {
        private readonly IMediator _mediator;
        public PersonService(IHttpContextAccessor _httpContext,
                                IMediator mediator) : base(_httpContext)
        {
          
            _mediator = mediator;
        }

        public async Task<ResponseResult> AddPerson(personRequest parameter)
        {
            return await _mediator.Send(parameter);
            #region Old
            //var checkBranch = await branchesHelper.CheckIsBranchExist(parameter.Branches, branchesRepositoryQuery);
            //if (checkBranch != null)
            //    return checkBranch;
            //var security = await _securityIntegrationService.getCompanyInformation();
            //if (parameter.IsSupplier)
            //{
            //    if (!security.isInfinityNumbersOfSuppliers)
            //    {
            //        var suppliersCount = PersonQuery.TableNoTracking.Where(x => x.IsSupplier == true).Count();
            //        if (suppliersCount >= security.AllowedNumberOfSuppliers)
            //            return new ResponseResult()
            //            {
            //                Note = Actions.YouHaveTheMaxmumOfSuppliers,
            //                Result = Result.MaximumLength,
            //                ErrorMessageAr = "تجاوزت الحد الاقصي من عدد الموردين",
            //                ErrorMessageEn = "You Cant add a new supplier because you have the maximum of suppliers for your bunlde"
            //            };
            //    }
            //}
            //else
            //{
            //    if (!security.isInfinityNumbersOfCustomers)
            //    {
            //        var suppliersCount = PersonQuery.TableNoTracking.Where(x => x.IsCustomer == true).Count();
            //        if (suppliersCount >= security.AllowedNumberOfCustomers)
            //            return new ResponseResult()
            //            {
            //                Note = Actions.YouHaveTheMaxmumOfCustomers,
            //                Result = Result.MaximumLength,
            //                ErrorMessageAr = "تجاوزت الحد الاقصي من عدد العملاء",
            //                ErrorMessageEn = "You Cant add a new customer because you have the maximum of customers for your bunlde"
            //            };
            //    }
            //    if (string.IsNullOrEmpty(parameter.SalesManId.ToString()))
            //        return new ResponseResult()
            //        {
            //            Note = "sales man is required",
            //            Result = Result.Failed
            //        };
            //}
            ////This To Remove Add To another List option
            //parameter.AddToAnotherList = false;

            //var userInfo = _iUserInformation.GetUserInformation();
            ////Check settings
            //var settings = await invGeneralSettingsRepositoryQuery.FindAsync(e => e.Id == 1);
            //var confirmSupplierPhone = settings.Other_ConfirmeSupplierPhone;
            //var confirmCustomerPhone = settings.Other_ConfirmeCustomerPhone;
            //if (parameter.IsSupplier)
            //{
            //    if (!userInfo.otherSettings.showAllBranchesInSuppliersInfo)
            //        if (userInfo.employeeBranches.Where(x => !parameter.Branches.Contains(x)).Any())
            //            return new ResponseResult()
            //            {
            //                Note = "Error Branch id",
            //                Result = Result.Failed
            //            };
            //}
            //else
            //{
            //    if (!userInfo.otherSettings.showAllBranchesInCustomerInfo)
            //        if (userInfo.employeeBranches.Where(x => !parameter.Branches.Contains(x)).Any())
            //            return new ResponseResult()
            //            {
            //                Note = "Error Branch id",
            //                Result = Result.Failed
            //            };
            //}
            //parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            //parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            //if (string.IsNullOrEmpty(parameter.LatinName))
            //    parameter.LatinName = parameter.ArabicName;

            //if (string.IsNullOrEmpty(parameter.ArabicName))
            //    return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            //if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            //{
            //    return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            //}

            //if (parameter.Type < (int)PersonType.Normal || parameter.Type > (int)PersonType.Midmost)
            //    return new ResponseResult { Result = Result.RequiredData, Note = Actions.TypeIsRequired };

            //if (!string.IsNullOrEmpty(parameter.Phone))
            //{
            //    if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
            //        return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
            //}
            ////Applying settings on suppliers and customers 
            //if (parameter.IsSupplier && confirmSupplierPhone)
            //{
            //    if (!string.IsNullOrEmpty(parameter.Phone))
            //    {
            //        if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
            //            return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
            //    }
            //    else
            //    {
            //        return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneIsRequired };
            //    }
            //}

            //if (!parameter.IsSupplier && confirmCustomerPhone)
            //{
            //    if (!string.IsNullOrEmpty(parameter.Phone))
            //    {
            //        if (parameter.Phone.Length < 7 && parameter.Phone.Length > 0)
            //            return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneDigitLessThan7 };
            //    }
            //    else
            //    {
            //        return new ResponseResult { Result = Result.RequiredData, Note = Actions.PhoneIsRequired };
            //    }
            //}

            //if (!parameter.IsSupplier && parameter.SalesPriceId > parameter.LessSalesPriceId)
            //    return new ResponseResult() { Data = null, Id = null, Result = Result.Failed };

            //if (parameter.SalesPriceId > 0 && parameter.LessSalesPriceId == null)
            //    parameter.LessSalesPriceId = parameter.SalesPriceId;
            //if (parameter.SalesPriceId == null && parameter.LessSalesPriceId == null)
            //{
            //    parameter.LessSalesPriceId = (int)SalePricesList.SalePrice1;
            //    parameter.SalesPriceId = (int)SalePricesList.SalePrice1;
            //}
            //var table = Mapping.Mapper.Map<PersonRequest, InvPersons>(parameter);

            //if (parameter.Email == "")
            //    table.Email = null;

            //if (parameter.Fax == "")
            //    table.Fax = null;
            //if (parameter.Phone == "")
            //    table.Phone = null;

            //var PhoneExist = await PersonQuery.GetByAsync(a => a.Phone == parameter.Phone && !string.IsNullOrEmpty(parameter.Phone)
            //                           && ((parameter.IsSupplier) ?
            //                     (parameter.AddToAnotherList == true ? true : a.IsSupplier == true) :
            //                     (parameter.AddToAnotherList == true ? true : a.IsCustomer == true)));

            //if (PhoneExist != null)
            //    return new ResponseResult() { Data = null, Id = PhoneExist.Id, Result = Result.Exist, Note = Actions.PhoneExist };

            //var EmailExist = await PersonQuery.GetByAsync(a => a.Email == parameter.Email && !string.IsNullOrEmpty(parameter.Email)
            //                          && ((parameter.IsSupplier) ?
            //                     (parameter.AddToAnotherList == true ? true : a.IsSupplier == true) :
            //                     (parameter.AddToAnotherList == true ? true : a.IsCustomer == true)));

            //if (EmailExist != null)
            //    return new ResponseResult() { Data = null, Id = EmailExist.Id, Result = Result.Exist, Note = Actions.EmailExist };



            //int NextCode = 1;
            //if (parameter.IsSupplier)
            //{
            //    NextCode = PersonQuery.GetMaxCode(e => e.Code, a => a.IsSupplier == true && a.CodeT == "S") + 1;
            //    if (parameter.AddToAnotherList == true)
            //        table.IsCustomer = true;
            //    else
            //    {
            //        table.SalesManId = null;
            //        table.IsCustomer = false;
            //    }
            //    table.IsSupplier = true;
            //    table.CodeT = "S";
            //}
            //else
            //{
            //    NextCode = PersonQuery.GetMaxCode(e => e.Code, a => a.IsCustomer == true && a.CodeT == "C") + 1;
            //    if (parameter.AddToAnotherList == true)
            //        table.IsSupplier = true;
            //    else
            //    {
            //        //table.SalesManId = null;
            //        table.IsSupplier = false;
            //    }
            //    table.IsCustomer = true;
            //    table.CodeT = "C";
            //}
            //// GL relation 
            //var GLRelation = await _iGLFinancialAccountRelation.GLRelation(parameter.IsSupplier ? GLFinancialAccountRelation.supplier : GLFinancialAccountRelation.customer, parameter.FinancialAccountId ?? 1, parameter.Branches, table.ArabicName, table.LatinName);
            //if (GLRelation.Result == Result.Success)
            //    table.FinancialAccountId = GLRelation.Id;
            //else
            //    return GLRelation;

            //table.Code = NextCode;
            //table.InvEmployeesId = userInfo.employeeId;
            //PersonCommand.Add(table);

            //List<InvPersons_Branches> personBranchList = new List<InvPersons_Branches>();

            //foreach (var i in parameter.Branches)
            //{
            //    var personBranchItem = new InvPersons_Branches();
            //    personBranchItem.BranchId = i;
            //    personBranchItem.PersonId = table.Id;
            //    personBranchList.Add(personBranchItem);
            //}

            //PersonBranchCommand.AddRange(personBranchList);


            ////add new Funds for Custommer or Supplier as dfualt value 
            //#region Add Entry Fund
            //var fundsCustomerSupplier = new InvFundsCustomerSupplier(); //By wesal
            //fundsCustomerSupplier.Credit = 0;
            //fundsCustomerSupplier.Debit = 0;
            //fundsCustomerSupplier.PersonId = table.Id;
            //FundsCustomerSupplierCommand.Add(fundsCustomerSupplier);
            //#endregion


            ////BackgroundJob.Enqueue(() => Method here );
            //// History for persons
            //history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            //await _systemHistoryLogsService.SystemHistoryLogsService(parameter.IsSupplier ? SystemActionEnum.addSupplier : SystemActionEnum.addCustomer);
            //return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };
            #endregion
        }
        public async Task<ResponseResult> GetListOfPersons(PersonsSearch parameters,string ids,bool isSearchData=true,bool isPrint=false)
        {
            return await _mediator.Send(new GetListOfPersonsRequest
            {
                ids = ids,
                isSearchData = isSearchData,
                isPrint = isPrint,
                IsSupplier = parameters.IsSupplier,
                Name = parameters.Name,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TypeArr = parameters.TypeArr,
                Status = parameters.Status,
                SalesManId= parameters.SalesManId,
            });
        }
        public async Task<WebReport> SupplierCutomerReport(PersonsSearch parameters, bool isArabic, exportType exportType, string ids,int fileId=0,  bool isSearchData=true)
        {

            return await _mediator.Send(new SupplierCutomerReportRequest
            {
                isArabic = isArabic,
                exportType = exportType,
                ids = ids,
                isSearchData = isSearchData,
                TypeArr = parameters.TypeArr,
                IsSupplier = parameters.IsSupplier,
                Name = parameters.Name,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                Status = parameters.Status,
                fileId=fileId,
                SalesManId=parameters.SalesManId
                
            });

        }
        public async Task<ResponseResult> UpdatePersons(UpdatePersonsRequest parameters)
        {
            return await _mediator.Send(parameters);
        }
        public async Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters)
        {
            return await _mediator.Send(parameters);
        }
        public async Task<ResponseResult> DeletePersons(DeletePersonsRequest ListCode)
        {
            return await _mediator.Send(ListCode);
        }
        public async Task<ResponseResult> GetPersonHistory(int Code)
        {
            return await _mediator.Send(new GetPersonHistoryRequest { Code = Code });   
        }
        public async Task<ResponseResult> GetPersonsDropDown(GetPersonsDropDownRequest request )
        {
            return await _mediator.Send(request);
        }
        public async Task<ResponseResult> GetAllPersonsDropDown(GetAllPersonsDropDownRequest request)
        {
            return await _mediator.Send(request);
        }
        public async Task<bool> DisableCanDelete(int personId)
        {
            return await _mediator.Send(new DisableCanDeleteRequest { personId = personId });
        }
        public async Task<ResponseResult> GetPersonsByDate(GetPersonsByDateRequest parameter)
        {
            return await _mediator.Send(parameter);
        }
    }
}
