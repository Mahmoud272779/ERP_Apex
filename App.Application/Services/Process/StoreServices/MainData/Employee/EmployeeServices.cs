using App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate;
using App.Application.Handlers.Units;
using App.Application.Helpers.Service_helper.FileHandler;
using App.Application.Helpers.Service_helper.History;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Printing.InvoicePrint;
using App.Application.Services.Process.FinancialAccounts;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Security.Authentication.Response.Store;
using App.Infrastructure.UserManagementDB;
using Org.BouncyCastle.Ocsp;

namespace App.Application.Services.Process.Employee
{
    public class EmployeeServices : BaseClass, IEmployeeServices
    {
        private readonly IRepositoryQuery<InvEmployees> EmployeesRepositoryQuery;
        private readonly IRepositoryQuery<GLIntegrationSettings> _GLIntegrationSettingsQuery;
        private readonly IRepositoryCommand<InvEmployees> EmployeesRepositoryCommand;

        private readonly IRepositoryQuery<GLBranch> BranchRepositoryQuery;
        private readonly IRepositoryCommand<InvEmployeeBranch> EmployeeBranchRepositoryCommand;
        private readonly IRepositoryQuery<InvEmployeeBranch> EmployeeBranchRepositoryQuery;
        private readonly IRepositoryQuery<GLBranch> branchesRepositoryQuery;
        private readonly IRepositoryQuery<GLFinancialAccount> _gLFinancialAccountQuery;
        private readonly IRepositoryQuery<GLGeneralSetting> _gLGeneralSettingQuery;
        private readonly IRepositoryQuery<userAccount> _useraccountQuery;
        private readonly iGLFinancialAccountRelation _iGLFinancialAccountRelation;
        private readonly IRepositoryQuery<InvoiceMaster> _inviceMasterQuery;
        private readonly IRepositoryQuery<OfferPriceMaster> _OfferPriceMasterQuery;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly IFinancialAccountBusiness _financialAccountBusiness;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iUserInformation _userinformation;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IHistory<InvEmployeesHistory> history;
        private readonly IFileHandler _fileHandler;
        private readonly IPrintService _iprintService;

        private readonly IFilesMangerService _filesMangerService;
        private readonly ICompanyDataService _CompanyDataService;
        private readonly iUserInformation _iUserInformation;
        private readonly IGeneralPrint _iGeneralPrint;

        public IHttpContextAccessor HttpContext { get; }

        public EmployeeServices(IRepositoryQuery<InvEmployees> _EmployeesRepositoryQuery,
                                  IRepositoryCommand<InvEmployees> _EmployeesRepositoryCommand,
                                  IRepositoryQuery<GLBranch> _BranchRepositoryQuery,
                                  IRepositoryCommand<InvEmployeeBranch> _EmployeeBranchRepositoryCommand,
                                  IRepositoryQuery<InvEmployeeBranch> _EmployeeBranchRepositoryQuery,
                                  IRepositoryQuery<GLBranch> _BranchesRepositoryQuery,
                                  IRepositoryQuery<GLFinancialAccount> GLFinancialAccountQuery,
                                  IRepositoryQuery<GLGeneralSetting> GLGeneralSettingQuery,
                                  IRepositoryQuery<userAccount> useraccountQuery,
                                  iGLFinancialAccountRelation iGLFinancialAccountRelation,
                                  IRepositoryQuery<InvoiceMaster> InviceMasterQuery,
                                  IWebHostEnvironment _hostingEnvironment,
                                  iUserInformation Userinformation,
                                  ISecurityIntegrationService securityIntegrationService,
                                  IFinancialAccountBusiness financialAccountBusiness,
                                  ISystemHistoryLogsService systemHistoryLogsService,
                                  IHistory<InvEmployeesHistory> history,
                                  IFileHandler fileHandler,
                                  IHttpContextAccessor _httpContext,
                                  IPrintService iprintService,
                                  IFilesMangerService filesMangerService,
                                  ICompanyDataService companyDataService,
                                  iUserInformation iUserInformation,
                                  IGeneralPrint iGeneralPrint,
                                  IRepositoryQuery<OfferPriceMaster> offerPriceMasterQuery,
                                  IDeletedRecordsServices deletedRecords,
                                  IGeneralAPIsService generalAPIsService,
                                  IRepositoryQuery<GLIntegrationSettings> gLIntegrationSettingsQuery) : base(_httpContext)
        {
            EmployeesRepositoryQuery = _EmployeesRepositoryQuery;
            EmployeesRepositoryCommand = _EmployeesRepositoryCommand;
            BranchRepositoryQuery = _BranchRepositoryQuery;
            EmployeeBranchRepositoryCommand = _EmployeeBranchRepositoryCommand;
            EmployeeBranchRepositoryQuery = _EmployeeBranchRepositoryQuery;
            branchesRepositoryQuery = _BranchesRepositoryQuery;
            _gLFinancialAccountQuery = GLFinancialAccountQuery;
            _gLGeneralSettingQuery = GLGeneralSettingQuery;
            _useraccountQuery = useraccountQuery;
            _iGLFinancialAccountRelation = iGLFinancialAccountRelation;
            _inviceMasterQuery = InviceMasterQuery;
            _financialAccountBusiness = financialAccountBusiness;
            _systemHistoryLogsService = systemHistoryLogsService;
            _userinformation = Userinformation;
            _securityIntegrationService = securityIntegrationService;
            this.history = history;
            _fileHandler = fileHandler;
            HttpContext = _httpContext;
            _iprintService = iprintService;
            _filesMangerService = filesMangerService;
            _CompanyDataService = companyDataService;
            _iUserInformation = iUserInformation;
            _iGeneralPrint = iGeneralPrint;
            _OfferPriceMasterQuery = offerPriceMasterQuery;
            _deletedRecords = deletedRecords;
            this.generalAPIsService = generalAPIsService;
            _GLIntegrationSettingsQuery = gLIntegrationSettingsQuery;
        }
        public async Task<ResponseResult> AddEmployee(EmployeesRequestDTOs.Add parameter)
        {
            var security = await _securityIntegrationService.getCompanyInformation();
            var employeeCount = EmployeesRepositoryQuery.GetAll().Count();
            if (!security.isInfinityNumbersOfEmployees)
            {
                if (employeeCount >= security.AllowedNumberOfEmployee)
                    return new ResponseResult()
                    {
                        Result = Result.MaximumLength,
                        ErrorMessageAr = "تجاوزت الحد الاقصي من عدد الموظفين",
                        ErrorMessageEn = "You Cant add a new employee because you have the maximum of employees for your bunlde",
                        Note = Actions.YouHaveTheMaxmumOfEmployees
                    };
            }
            var checkBranch = await branchesHelper.CheckIsBranchExist(parameter.Branches, branchesRepositoryQuery);
            if (checkBranch != null)
                return checkBranch;
            parameter.LatinName = Helpers.Helpers.IsNullString(parameter.LatinName);
            parameter.ArabicName = Helpers.Helpers.IsNullString(parameter.ArabicName);
            if (string.IsNullOrEmpty(parameter.LatinName))
                parameter.LatinName = parameter.ArabicName;

            if (string.IsNullOrEmpty(parameter.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameter.JobId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.JobIsRequired };

            if (parameter.Branches.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.BranchIsRequired };

            if (parameter.Status < (int)Status.Active || parameter.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }

            var ArabicEmployeeExist = await EmployeesRepositoryQuery.GetByAsync(a => a.ArabicName == parameter.ArabicName);
            if (ArabicEmployeeExist != null)
                return new ResponseResult() { Data = null, Id = ArabicEmployeeExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinEmployeeExist = await EmployeesRepositoryQuery.GetByAsync(a => a.LatinName == parameter.LatinName);
            if (LatinEmployeeExist != null)
                return new ResponseResult() { Data = null, Id = LatinEmployeeExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };

            int NextCode = EmployeesRepositoryQuery.GetMaxCode(e => e.Code) + 1;

            var table = new InvEmployees();
            table.ArabicName = parameter.ArabicName;
            table.LatinName = parameter.LatinName;
            table.Status = parameter.Status;
            table.Notes = parameter.Notes;
            table.JobId = parameter.JobId;
            table.Code = NextCode;
            table.SalesPriceId = parameter.SalesPriceId;
            #region Handling image in case of saving image in the database.
            //if (parameter.Image != null)
            //{
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        parameter.Image.CopyTo(ms);
            //        var fileBytes = ms.ToArray();
            //        table.Image = fileBytes;
            //        table.ImageName = parameter.Image.FileName;
            //    }
            //} 
            #endregion

            if (parameter.Image != null)
            {
                #region old
                //item.ImagePath = fileHandler.SaveImage(img, "ItemCards", true);
                //var path = BasePath;
                //string filePath = Path.Combine("wwwroot/Employees");
                //if (!Directory.Exists(filePath))
                //{
                //    Directory.CreateDirectory(Path.Combine(path, filePath));
                //}
                //filePath = Path.Combine(filePath, DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + parameter.Image.FileName.Replace(" ", ""));
                //string actulePath = Path.Combine(path, filePath);
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    parameter.Image.CopyTo(ms);
                //    Image imgResized = Helpers.Helpers.Resize(Image.FromStream(ms), 250, 250);
                //    imgResized.Save(actulePath);
                //}
                #endregion
                table.ImagePath = _fileHandler.SaveImage(parameter.Image, "Employees", true); /*Constants.LocalServer + filePath;*/
            }
            var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.employee, parameter.FinancialAccountId ?? 0, parameter.Branches, table.ArabicName, table.LatinName);
            if (finanicalAccount.Result != Result.Success)
                return finanicalAccount;
            table.FinancialAccountId = finanicalAccount.Id;
            table.UTime = DateTime.Now;
            EmployeesRepositoryCommand.Add(table);
            var id = (await EmployeesRepositoryQuery.FindByAsyn(e => e.Code == table.Code)).FirstOrDefault().Id;
            foreach (var item in parameter.Branches)
            {
                await EmployeeBranchRepositoryCommand.AddAsync(new InvEmployeeBranch() { BranchId = item, EmployeeId = id });
            }
            history.AddHistory(table.Id, table.LatinName, table.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addEmployee);
            return new ResponseResult() { Data = null, Id = table.Id, Result = Result.Success };

        }



        public async Task<ResponseResult> RaoufTest(EmployeesRequestDTOs.Search parameters, bool isPrint = false)
        {
            var userInfo = await _userinformation.GetUserInformation();
            var res = Enumerable.Empty<InvEmployees>().AsQueryable();
            if (parameters.IsSearcheData)
            {
                res = EmployeesRepositoryQuery.TableNoTracking
                                             .Include(navigationPropertyPath: e => e.EmployeeBranches)
                                             .Include(s => s.Job)
                                             .Include(x => x.FinancialAccount)
                                             .Where(a => parameters.EmployeeId > 0 ? (a.Id == parameters.EmployeeId) : true)
                                             .Where(a => !string.IsNullOrEmpty(parameters.Name) ? (a.Code.ToString().Contains(parameters.Name) || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name)) : true)
                                             .Where(a => parameters.Status != 0 ? a.Status == parameters.Status : true);

            }

            else
            {
                if (parameters.Ids != null)
                {


                    string[] ids = parameters.Ids.Split(",");
                    foreach (var id in ids)
                    {
                        var item = EmployeesRepositoryQuery.TableNoTracking
                                                                .Include(navigationPropertyPath: e => e.EmployeeBranches)
                                                                .Include(s => s.Job)
                                                                .Include(x => x.FinancialAccount)
                                                                .Where(a => (Convert.ToInt32(id) > 0 ? (a.Id == Convert.ToInt32(id)) : false)).FirstOrDefault();
                        res = res.Append(item);
                    }
                }
                else return new ResponseResult { Result = Result.RequiredData };
            }



            res = res.Where(x => x.EmployeeBranches.Any(d => d.BranchId == 1));

            var allFAs = _gLFinancialAccountQuery.TableNoTracking;

            if (parameters.JobList != null && parameters.JobList.Count() > 0)
            {
                res = res.Where(u => parameters.JobList.Contains(u.JobId));
            }
            if (parameters.BranchList != null && parameters.BranchList.Count() > 0)
            {
                res = res.Where(x => x.EmployeeBranches.Any(y => parameters.BranchList.Contains(y.BranchId)));
            }
            if (userInfo.employeeId != 1)
                res = res.Where(x => x.Id != 1);

            var result = res.ToList();
            var count = result.Count();

            result.Where(a => a.Id != 1).Select(a => { a.CanDelete = true; return a; }).ToList();

            var List = Mapping.Mapper.Map<List<InvEmployees>, List<EmployeeResponsesDTOs.GetAll>>(result.ToList());
            var offerPrices = _OfferPriceMasterQuery.TableNoTracking;
            foreach (var employee in List)
            {
                //employee.CanDelete = (Branches2.Select(e => e.ManagerId).ToArray().Contains(employee.Id))||employee.Code==1? false : true;
                employee.CanDelete = offerPrices.Where(x => x.EmployeeId == employee.Id).Any() ? false : true;
            }

            var _List = List.Select(x => new
            {
                x.Id,
                x.Code,
                x.ArabicName,
                x.LatinName,
                x.Status,
                x.Notes,
                x.ImagePath,
                x.Branches,
                x.BranchNameAr,
                x.BranchNameEn,
                x.JobId,
                x.JobNameAr,
                x.JobNameEn,
                x.JobStatus,
                x.CanDelete,
                FinancialAccountId = allFAs.Where(Acc => Acc.Id == x.FinancialAccountId).Select(Acc => new { Acc.Id, Acc.ArabicName, Acc.LatinName }).FirstOrDefault()
            }).ToList();
            if (isPrint)
            {
                return new ResponseResult() { Data = List, DataCount = count, Id = null, Result = List.Any() ? Result.Success : Result.Failed };

            }
            return new ResponseResult() { Data = _List, DataCount = count, Id = null, Result = List.Any() ? Result.Success : Result.Failed };









        }
        public async Task<ResponseResult> GetListOfEmployees(EmployeesRequestDTOs.Search parameters, bool isPrint = false)
        {
            var userInfo = await _userinformation.GetUserInformation();
            var res = Enumerable.Empty<InvEmployees>().AsQueryable();
            if (parameters.IsSearcheData)
            {
                res = EmployeesRepositoryQuery.TableNoTracking
                                             .Include(navigationPropertyPath: e => e.EmployeeBranches)
                                             .Include(s => s.Job)
                                             .Include(x => x.FinancialAccount)
                                             .Where(a => parameters.EmployeeId > 0 ? (a.Id == parameters.EmployeeId) : true)
                                             .Where(a => !string.IsNullOrEmpty(parameters.Name) ? (a.Code.ToString().Contains(parameters.Name) || a.ArabicName.Contains(parameters.Name) || a.LatinName.Contains(parameters.Name)) : true)
                                             .Where(a => parameters.Status != 0 ? a.Status == parameters.Status : true);

            }

            else 
            {
                if (parameters.Ids != null)
                {


                    string[] ids = parameters.Ids.Split(",");
                    foreach (var id in ids)
                    {
                        var item = EmployeesRepositoryQuery.TableNoTracking
                                                                .Include(navigationPropertyPath: e => e.EmployeeBranches)
                                                                .Include(s => s.Job)
                                                                .Include(x => x.FinancialAccount)
                                                                .Where(a => (Convert.ToInt32(id) > 0 ? (a.Id == Convert.ToInt32(id)) : false)).FirstOrDefault();
                        res = res.Append(item);
                    }
                }
                else return new ResponseResult { Result = Result.RequiredData};
            }

            

            res = res.Where(x => x.EmployeeBranches.Select(d => d.BranchId).Any(c => userInfo.employeeBranches.Contains(c)));
            var allFAs = _gLFinancialAccountQuery.TableNoTracking;

            if (parameters.JobList != null && parameters.JobList.Count() > 0)
            {
                res = res.Where(u => parameters.JobList.Contains(u.JobId));
            }
            if (parameters.BranchList != null && parameters.BranchList.Count() > 0)
            {
                res = res.Where(x => x.EmployeeBranches.Any(y => parameters.BranchList.Contains(y.BranchId)));
            }
            if (userInfo.employeeId != 1)
                res = res.Where(x => x.Id != 1);
            res = (string.IsNullOrEmpty(parameters.Name) ? res.OrderByDescending(q => q.Code) : res.OrderBy(a => (a.Code.ToString().Contains(parameters.Name)) ? 0 : 1));
            var result = res.ToList();
            var count = result.Count();

            result = isPrint ? res.ToList() : res.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToList();


            if (parameters.PageNumber <= 0 && parameters.PageNumber <= 0 && isPrint == false)
            {
                return new ResponseResult() { Data = null, DataCount = 0, Id = null, Result = Result.Failed };
            }
            result.Where(a => a.Id != 1).Select(a => { a.CanDelete = true; return a; }).ToList();

            //mapping and handle conditions with mapping profile
            var List = Mapping.Mapper.Map<List<InvEmployees>, List<EmployeeResponsesDTOs.GetAll>>(result.ToList());
            var offerPrices = _OfferPriceMasterQuery.TableNoTracking;
            foreach (var employee in List)
            {
                var Branches2 = branchesRepositoryQuery.GetAll(e => employee.Branches.Contains(e.Id))
                                    .Select(e => new { e.ArabicName, e.LatinName, e.ManagerId }).ToList();
                employee.BranchNameAr = employee.Id == 1 ? "الكل" : string.Join(',', Branches2.Select(e => e.ArabicName).ToArray());
                employee.BranchNameEn = employee.Id == 1 ? "All" : string.Join(',', Branches2.Select(e => e.LatinName).ToArray());
                //employee.CanDelete = (Branches2.Select(e => e.ManagerId).ToArray().Contains(employee.Id))||employee.Code==1? false : true;
                employee.CanDelete = _useraccountQuery.TableNoTracking.Where(d => d.employeesId == employee.Id).Any() || branchesRepositoryQuery.TableNoTracking.Where(e => e.ManagerId == employee.Id).Any() || employee.Id == 1  || offerPrices.Where(x=> x.EmployeeId == employee.Id).Any() ? false : true;

            }
            var _List = List.Select(x => new
            {
                x.Id,
                x.Code,
                x.ArabicName,
                x.LatinName,
                x.Status,
                x.Notes,
                x.ImagePath,
                x.Branches,
                x.BranchNameAr,
                x.BranchNameEn,
                x.JobId,
                x.JobNameAr,
                x.JobNameEn,
                x.JobStatus,
                x.CanDelete,
                x.SalesPriceId,
                FinancialAccountId = allFAs.Where(Acc => Acc.Id == x.FinancialAccountId).Select(Acc => new { Acc.Id, Acc.ArabicName, Acc.LatinName }).FirstOrDefault()
            }).ToList();
            if (isPrint)
            {
                return new ResponseResult() { Data = List, DataCount = count, Id = null, Result = List.Any() ? Result.Success : Result.Failed };

            }
            return new ResponseResult() { Data = _List, DataCount = count, Id = null, Result = List.Any() ? Result.Success : Result.Failed };
            #region
            //var resData = await EmployeesRepositoryQuery.GetAllIncludingAsync(parameters.PageNumber, parameters.PageSize,
            //      a => ((a.Code.ToString().Contains(parameters.name) || string.IsNullOrEmpty(parameters.name) 
            //      || a.ArabicName.Contains(parameters.name) || a.LatinName.Contains(parameters.name))
            //      && (parameters.active == 0 || a.Active == parameters.active)),
            //      e => (parameters.name == "" ? e.OrderByDescending(q => q.EmploeeId) : e.OrderBy(a => a.EmploeeId)), e => e.branch, x => x.job);

            // var list = new List<EmployeeWithImgDto>();

            //foreach (var item in result)
            //{
            //    var dataDto = new EmployeeWithImgDto();
            //    dataDto.EmploeeId = item.EmployeeId;
            //    dataDto.ArabicName = item.ArabicName;
            //    dataDto.LatinName = item.LatinName;

            //    // var branch = BranchRepositoryQuery.Get( item.branch_Id);
            //    dataDto.BranchId = item.branch.BranchId;
            //    dataDto.BranchNameAr = item.branch.NameAr;
            //    dataDto.BranchNameEn = item.branch.NameEn;

            //    // var job = JobRepositoryQuery.Get(item.job_Id);
            //    dataDto.JobId = item.job.JobId;
            //    dataDto.JobNameAr = item.job.ArabicName;
            //    dataDto.JobNameEn = item.job.LatinName;
            //    dataDto.Active = item.Active;
            //    dataDto.Code = item.Code;
            //    dataDto.CanDelete = item.EmployeeId != 1;
            //    if (item.job != null || item.branch != null)
            //        dataDto.CanDelete = false ;

            //    if (parameters.EmployeeId>0)
            //    { 
            //        dataDto.Image = item.Image;
            //        dataDto.ImageName = item.ImageName; 
            //    } 


            //    list.Add(dataDto);
            //}
            //var count = EmployeesRepositoryQuery.Count(
            //    a => ((a.Code.ToString().Contains(parameters.name) || string.IsNullOrEmpty(parameters.name)
            //     || a.ArabicName.Contains(parameters.name) || a.LatinName.Contains(parameters.name))
            //     && (parameters.active == 0 || a.Active == parameters.active)) );
            #endregion
        }
        public async Task<WebReport> EmployeesReport(EmployeesRequestDTOs.Search parameters, exportType exportType, bool isArabic, int fileId = 0)
        {

            var data = await GetListOfEmployees(parameters, true);
           
            var userInfo = await _iUserInformation.GetUserInformation();


            var mainData = (List<EmployeeResponsesDTOs.GetAll>)data.Data;
            foreach(var item in mainData)
            {
                if (item.Status == 1)
                {
                    item.StatusAr = "نشط";
                    item.StatusEn = "Active";

                }
                else if (item.Status == 2)
                {
                    item.StatusAr = "غير نشط";
                    item.StatusEn = "InActive";
                }
            }

            var otherdata = new ReportOtherData()
            {
                EmployeeName = userInfo.employeeNameAr.ToString(),
                EmployeeNameEn = userInfo.employeeNameEn.ToString(),
                Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

            };
            
                

            
               int screenId = (int)SubFormsIds.Employees_MainUnits;
            var tablesNames = new TablesNames()
            {
                FirstListName = "Employees"
            };

            var report = await _iGeneralPrint.PrintReport<object, EmployeeResponsesDTOs.GetAll, object>(null, mainData, null, tablesNames, otherdata
                , screenId, exportType, isArabic,fileId);
            return report;

        }

        public async Task<ResponseResult> UpdateStatus(SharedRequestDTOs.UpdateStatus parameters)
        {

            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }


            var Employees = EmployeesRepositoryQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));
            var EmployeeList = Employees.ToList();
            EmployeeList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1))
                EmployeeList.Where(q => q.Id == 1).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();
            var rssult = await EmployeesRepositoryCommand.UpdateAsyn(EmployeeList);
            foreach (var Employee in EmployeeList)
            {
                history.AddHistory(Employee.Id, Employee.LatinName, Employee.ArabicName, Aliases.HistoryActions.Add, Aliases.TemporaryRequiredData.UserName);
            }
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editEmployee);
            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };

        }


        public async Task<ResponseResult> UpdateEmployees(EmployeesRequestDTOs.Update parameters)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var checkBranch = await branchesHelper.CheckIsBranchExist(parameters.Branches, branchesRepositoryQuery);
            if (checkBranch != null)
                return checkBranch;
            if (parameters.Id == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            parameters.LatinName = Helpers.Helpers.IsNullString(parameters.LatinName);
            parameters.ArabicName = Helpers.Helpers.IsNullString(parameters.ArabicName);
            if (string.IsNullOrEmpty(parameters.LatinName))
                parameters.LatinName = parameters.ArabicName;

            if (string.IsNullOrEmpty(parameters.ArabicName))
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.NameIsRequired };

            if (parameters.JobId == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.JobIsRequired };

            if (parameters.Branches.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.BranchIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }


            var ArabicEmployeesExist = await EmployeesRepositoryQuery.GetByAsync(a => a.ArabicName == parameters.ArabicName && a.Id != parameters.Id);
            if (ArabicEmployeesExist != null)
                return new ResponseResult() { Data = null, Id = ArabicEmployeesExist.Id, Result = Result.Exist, Note = Actions.ArabicNameExist };

            var LatinEmployeesExist = await EmployeesRepositoryQuery.GetByAsync(a => a.LatinName == parameters.LatinName && a.Id != parameters.Id);
            if (LatinEmployeesExist != null)
                return new ResponseResult() { Data = null, Id = LatinEmployeesExist.Id, Result = Result.Exist, Note = Actions.EnglishNameExist };


            var data = (await EmployeesRepositoryQuery.GetAllIncludingAsync(0, 0,
                                a => a.Id == parameters.Id/*, w => w. EmployeeBranches*/)).FirstOrDefault();
            if (data == null)
                return new ResponseResult() { Data = null, Id = null, Result = Result.NoDataFound };

            data.ArabicName = parameters.ArabicName;
            data.LatinName = parameters.LatinName;

            if (data.Id == 1)
                data.Status = (int)Status.Active;
            else
                data.Status = parameters.Status;

            data.Notes = parameters.Notes;
            data.JobId = parameters.JobId;
            data.Code = parameters.Code;
            data.SalesPriceId = parameters.SalesPriceId;
            if (parameters.ChangeImage)
            {
                #region Handling image in case of saving image in the database
                //if (parameters.Image != null)
                //{
                //    using (MemoryStream ms = new MemoryStream())
                //    {
                //        parameters.Image.CopyTo(ms);
                //        var fileBytes = ms.ToArray();
                //        data.Image = fileBytes;
                //        data.ImageName = parameters.Image.FileName;
                //    }
                //}
                //else
                //{
                //    data.Image = null;
                //    data.ImageName = null;
                //} 
                #endregion
                if (parameters.Image != null)
                {
                    #region old
                    //var path = BasePath;
                    //string filePath = Path.Combine("Employees");
                    //if (!Directory.Exists(filePath))
                    //{
                    //    Directory.CreateDirectory(Path.Combine(path, filePath));
                    //}
                    //filePath = Path.Combine(filePath, DateTime.Now.ToString().Replace('/', '-').Replace(':', '-') + Guid.NewGuid().ToString() + parameters.Image.FileName.Replace(" ", ""));
                    //string actulePath = Path.Combine(path, filePath);
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    parameters.Image.CopyTo(ms);
                    //    Image imgResized = Helpers.Helpers.Resize(Image.FromStream(ms), 250, 250);
                    //    imgResized.Save(actulePath);
                    //}
                    //}
                    #endregion
                    data.ImagePath = _fileHandler.SaveImage(parameters.Image, "Employees", true); /*Constants.LocalServer + filePath;*/
                }
                else
                {
                    data.Image = null;
                    data.ImagePath = null;
                }
            }

            if (data.Id != 1)
            {
                var currentBranchId = EmployeeBranchRepositoryQuery.TableNoTracking.Where(e => e.EmployeeId == data.Id && e.current == true).FirstOrDefault();

                await EmployeeBranchRepositoryCommand.DeleteAsync(e => e.EmployeeId == data.Id);
                List<InvEmployeeBranch> employeeBranches = new List<InvEmployeeBranch>();
                foreach (var item in parameters.Branches)
                {
                    employeeBranches.Add(new InvEmployeeBranch() { BranchId = item, EmployeeId = data.Id, current = false });
                }
                employeeBranches.ForEach(x =>
                {
                    if (currentBranchId != null)
                    {
                        if (x.BranchId == currentBranchId.BranchId)
                            x.current = true;
                    }
                });
                //data.EmployeeBranches = employeeBranches;

                data.UTime = DateTime.Now;

                //var GLSettings = _gLGeneralSettingQuery.TableNoTracking.FirstOrDefault();
                var linkingMethod = _GLIntegrationSettingsQuery
                    .TableNoTracking
                    .Where(c => c.GLBranchId == userInfo.CurrentbranchId && c.screenId == (int)SubFormsIds.Employees_MainUnits)
                    .FirstOrDefault().linkingMethodId;


                if (linkingMethod == 1 && data.FinancialAccountId != parameters.FinancialAccountId)
                {
                    var finanicalAccount = await _iGLFinancialAccountRelation.GLRelation(GLFinancialAccountRelation.employee, (int)parameters.FinancialAccountId, parameters.Branches, data.ArabicName, data.LatinName);
                    if (finanicalAccount.Result != Result.Success)
                        return finanicalAccount;
                    data.FinancialAccountId = finanicalAccount.Id;
                }

                await EmployeeBranchRepositoryCommand.AddAsync(employeeBranches);
            }
            await EmployeesRepositoryCommand.UpdateAsyn(data);


            history.AddHistory(data.Id, data.LatinName, data.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editEmployee);
            return new ResponseResult() { Data = null, Id = data.Id, Result = data == null ? Result.Failed : Result.Success };

        }
        public async Task<ResponseResult> DeleteEmployees(SharedRequestDTOs.Delete ListCode)
        {

            UserInformationModel userInfo = await _userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };


            var EmployeesCanBeDeleted = EmployeesRepositoryQuery.TableNoTracking
                .Include(x=> x.OfferPriceMaster)          
                .Where(x => ListCode.Ids.Contains(x.Id) && !x.userAccount.Any())
                .Where(x=> !x.OfferPriceMaster.Any());


            var branches = await branchesRepositoryQuery.Get(e => e.ManagerId, a => a.ManagerId != null && a.ManagerId != 0);
            List<int> deletedList = new List<int>();
            List<int> FA_Ids = new List<int>();
            EmployeesCanBeDeleted.ToList().ForEach(emp =>
            {
                if (!branches.Contains(emp.Id))
                {
                    FA_Ids.Add(emp.FinancialAccountId ?? 0);
                    deletedList.Add(emp.Id);
                    EmployeesRepositoryCommand.DeleteAsync(emp.Id).Wait();
                    EmployeesRepositoryCommand.SaveAsync().Wait();
                }
            });

            var FA_Deleted = await _financialAccountBusiness.DeleteFinancialAccountAsync(new SharedRequestDTOs.Delete()
            {
                userId = userInfo.userId,
                Ids = FA_Ids.ToArray()
            });
            if (deletedList.Any())
            {
                int Count = deletedList.Count();
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteEmployee,Count);

            }

            //Fill The DeletedRecordTable
            _deletedRecords.SetDeletedRecord(deletedList.ToList(), 7);

            return new ResponseResult() { Data = deletedList, Id = null, Result = deletedList.Any() ? Result.Success : Result.NotFound };
        }

        public async Task<ResponseResult> GetEmployeeHistory(int EmployeeId)
        {
            return await history.GetHistory(a => a.EntityId == EmployeeId);
        }

        public async Task<ResponseResult> GetEmployeeDropDown(bool isInUserPage, int? employeeId, string SearchCriteria, int pageSize, int pageNumber, bool isReport = false)
        {
            var userInfo = await _userinformation.GetUserInformation();
            var dropdownlist = EmployeesRepositoryQuery.TableNoTracking
                                            .Include(x => x.EmployeeBranches)
                                            .Where(e => !isReport ? e.Status == (int)Status.Active : true)
                                            .Where(x =>   x.EmployeeBranches.Select(d => d.BranchId).Any(c => userInfo.employeeBranches.Contains(c))   )
                                            .Select(e => new { e.Id, e.ArabicName, e.LatinName, e.Status });
            if (!string.IsNullOrEmpty(SearchCriteria))
            {
                dropdownlist = dropdownlist.Where(x => x.ArabicName.Contains(SearchCriteria) || x.LatinName.Contains(SearchCriteria));
            }



            double MaxPageNumber = dropdownlist.Count() / Convert.ToDouble(pageSize);
            var countofFilter = Math.Ceiling(MaxPageNumber);
            var EndOfData = (countofFilter == pageNumber ? Actions.EndOfData : "");
            //dropdownlist = dropdownlist.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            if (isInUserPage)
            {
                var employeeUsedForUsers = _useraccountQuery.TableNoTracking.Where(x => x.employeesId != employeeId).Select(x => x.employeesId).ToList();

                dropdownlist = dropdownlist.Where(x => !employeeUsedForUsers.Contains(x.Id) && x.Status == 1);
            }
            return new ResponseResult() { Note = EndOfData, Data = dropdownlist, DataCount = dropdownlist.Count(), Result = dropdownlist.Any() ? Result.Success : Result.Failed };
        }

        public async Task<ResponseResult> GetEmployeesByDate(DateTime date, int PageNumber, int PageSize)
        {
            try
            {
                var resData = await EmployeesRepositoryQuery.TableNoTracking.Where(q => q.UTime >= date).ToListAsync();

                return await generalAPIsService.Pagination(resData, PageNumber, PageSize);

            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}
