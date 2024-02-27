using App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.General;
using App.Infrastructure;
using Attendleave.Erp.Core.APIUtilities;
using Dapper;
using MediatR;
using System.Collections.Immutable;
using System.Data.SqlClient;

namespace App.Application
{
    public class userService :/* BusinessBase<userAccount>,*/ iUserService
    {
        private readonly IRepositoryCommand<userAccount> _userAccountCommand;
        private readonly IRepositoryCommand<signinLogs> _signinLogsCommand;
        private readonly IRepositoryQuery<userAccount> _userAccountQuery;
        private readonly IRepositoryQuery<signalR> _signalRQuery;

        private readonly IRepositoryQuery<GLSafe> _gLSafeQuery;
        private readonly IRepositoryQuery<GLBank> _gLBankQuery;
        private readonly IRepositoryQuery<InvStoreBranch> _invStpStoresQuery;
        private readonly IRepositoryQuery<otherSettings> _otherSettingsQuery;
        private readonly IRepositoryQuery<userBranches> _userBranchesQuery;
        private readonly IRepositoryQuery<GLBranch> _gLBranchQuery;
        private readonly IRepositoryQuery<InvEmployees> _invEmployeesQuery;
        private readonly IRepositoryQuery<InvEmployeeBranch> _invEmployeeBranchQuery;
        private readonly ISecurityIntegrationService _securityIntegrationService;
        private readonly IRepositoryQuery<OtherSettingsStores> _otherSettingsStoresQuery;
        private readonly IRepositoryQuery<OtherSettingsSafes> _otherSettingsSafesQuery;
        private readonly IRepositoryQuery<OtherSettingsBanks> _otherSettingsBanksQuery;
        private readonly IRepositoryQuery<signinLogs> _signinLogsQuery;
        private readonly IMediator _mediator;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly IRepositoryCommand<otherSettings> _otherSettingsCommand;
        private readonly IRepositoryCommand<OtherSettingsStores> _otherSettingsStoresCommand;
        private readonly IRepositoryCommand<OtherSettingsSafes> _otherSettingsSafesCommand;
        private readonly IRepositoryCommand<OtherSettingsBanks> _otherSettingsBanksCommand;
        private readonly IRepositoryCommand<userBranches> _userBranchesCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly iUserInformation _userinformation;
        private readonly IHttpContextAccessor httpContext;
        public userService(
                            IRepositoryQuery<userAccount> userAccountQuery,

                            IRepositoryQuery<GLSafe> GLSafeQuery,
                            IRepositoryQuery<GLBank> GLBankQuery,
                            IRepositoryQuery<InvStoreBranch> InvStpStoresQuery,
                            IRepositoryQuery<otherSettings> otherSettingsQuery,
                            IRepositoryQuery<userBranches> userBranchesQuery,
                            IRepositoryQuery<GLBranch> GLBranchQuery,
                            IRepositoryQuery<InvEmployees> InvEmployeesQuery,
                            IRepositoryQuery<InvEmployeeBranch> _InvEmployeeBranchQuery,
                            ISecurityIntegrationService securityIntegrationService,
                            IRepositoryQuery<OtherSettingsStores> OtherSettingsStoresQuery,
                            IRepositoryQuery<OtherSettingsSafes> OtherSettingsSafesQuery,
                            IRepositoryQuery<OtherSettingsBanks> OtherSettingsBanksQuery,
                            IRepositoryQuery<signinLogs> signinLogsQuery,
                            IMediator mediator,
                            IDeletedRecordsServices deletedRecords,


                            IRepositoryCommand<userAccount> userAccountCommand,
                            IRepositoryCommand<signinLogs> signinLogsCommand,
                            IRepositoryCommand<otherSettings> otherSettingsCommand,
                            IRepositoryCommand<OtherSettingsStores> OtherSettingsStoresCommand,
                            IRepositoryCommand<OtherSettingsSafes> OtherSettingsSafesCommand,
                            IRepositoryCommand<OtherSettingsBanks> OtherSettingsBanksCommand,
                            IRepositoryCommand<userBranches> userBranchesCommand,
                            ISystemHistoryLogsService systemHistoryLogsService,
                            iUserInformation Userinformation,
                            IHttpContextAccessor HttpContext,
                            IRepositoryActionResult repositoryActionResult,
                            IRepositoryQuery<signalR> signalRQuery)/*:base(repositoryActionResult)*/
        {
            _userAccountCommand = userAccountCommand;
            _signinLogsCommand = signinLogsCommand;
            _otherSettingsCommand = otherSettingsCommand;
            _otherSettingsStoresCommand = OtherSettingsStoresCommand;
            _otherSettingsSafesCommand = OtherSettingsSafesCommand;
            _otherSettingsBanksCommand = OtherSettingsBanksCommand;
            _userBranchesCommand = userBranchesCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            _userinformation = Userinformation;
            _userAccountQuery = userAccountQuery;
            _gLSafeQuery = GLSafeQuery;
            _gLBankQuery = GLBankQuery;
            _invStpStoresQuery = InvStpStoresQuery;
            _otherSettingsQuery = otherSettingsQuery;
            _userBranchesQuery = userBranchesQuery;
            _gLBranchQuery = GLBranchQuery;
            _invEmployeesQuery = InvEmployeesQuery;
            _invEmployeeBranchQuery = _InvEmployeeBranchQuery;
            _securityIntegrationService = securityIntegrationService;
            _otherSettingsStoresQuery = OtherSettingsStoresQuery;
            _otherSettingsSafesQuery = OtherSettingsSafesQuery;
            _otherSettingsBanksQuery = OtherSettingsBanksQuery;
            _signinLogsQuery = signinLogsQuery;
            _mediator = mediator;
            _deletedRecords = deletedRecords;
            httpContext = HttpContext;
            _signalRQuery = signalRQuery;
        }

        private async Task<ResponseResult> checkIfValid(int userId, string userName, string password, string email, int employeeId, userAccount table, bool isEdit)
        {
            if (isEdit)
                if (userId == 0 || userId == null)
                    return new ResponseResult()
                    {
                        Note = Actions.IdIsRequired,
                        Result = Result.RequiredData
                    };
            //Check If Username exist
            var users = _userAccountQuery.TableNoTracking;
            if (isEdit)
                users = users.Where(x => x.id != userId);

            var userNameExist = users.Where(x => x.username == userName);
            if (userNameExist.Any())
                return new ResponseResult()
                {
                    Note = Actions.userNameIsExist,
                    Result = Result.Exist
                };
            if (users.Where(x => x.password == password).Any())
                return new ResponseResult()
                {
                    Note = Actions.PasswordExist,
                    Result = Result.Exist
                };
            var emailExist = users.Where(x => x.email == email);
            if (emailExist.Where(x => x.email == email).Any())
                return new ResponseResult()
                {
                    Note = Actions.EmailExist,
                    Result = Result.Exist
                };

            var isEmployeeUsed = _userAccountQuery.TableNoTracking.Where(x => x.employeesId == employeeId && (isEdit ? x.id != userId : true)).Any();
            if (isEmployeeUsed)
                return new ResponseResult()
                {
                    Note = isEdit ? "Emplyee is already Used" : "Emplyee is already have account",
                    Result = Result.Exist
                };
            var checkIfEmpExist = _invEmployeesQuery.TableNoTracking.Where(x => x.Id == employeeId).Any();
            if (!checkIfEmpExist)
                return new ResponseResult()
                {
                    Note = "Employee is not exist",
                    Result = Result.Exist
                };
            if (table == null)
                return new ResponseResult()
                {
                    Note = "error empty table",
                    Result = Result.Failed
                };
            return null;
        }


        public async Task<ResponseResult> addUser(addUsersDto parm)
        {
            var security = await _securityIntegrationService.getCompanyInformation();
            var countOfUsers = _userAccountQuery.GetAll().Count();

            if (!security.isInfinityNumbersOfUsers)
            {
                if (countOfUsers >= security.AllowedNumberOfUser)
                    return new ResponseResult()
                    {
                        Result = Result.MaximumLength,
                        ErrorMessageAr = "تجاوزت الحد الاقصي من عدد المستخدمين",
                        ErrorMessageEn = "You Cant add a new user because you have the maximum of users for your bunlde",
                        Note = Actions.YouHaveTheMaxmumOfUsers
                    };
            }


            if (!stringValidation.CheckEmailFormat(parm.email))
                return new ResponseResult()
                {
                    Note = Actions.invalidEmailFormat,
                    Result = Result.Failed
                };
            var table = Mapping.Mapper.Map<addUsersDto, userAccount>(parm);
            table.employeesId = parm.employeeId;

            if (parm.Status == 1)
                table.isActive = true;
            else
                table.isActive = false;
            var validation = await checkIfValid(0, parm.username, parm.password, parm.email, parm.employeeId, table, false);
            if (validation != null)
                return validation;
            //Set Time of User Creation
            table.UpdateTime = DateTime.Now;
            var existBranches = _invEmployeesQuery.TableNoTracking.Include(x => x.EmployeeBranches).Where(x => x.Id == parm.employeeId).Select(x => x.EmployeeBranches).FirstOrDefault().Select(x => x.BranchId);
            bool added = await _userAccountCommand.AddAsync(table);
            #region add branchess 
            //if (added)
            //{

            //    var _listuserBranches = new List<userBranches>();
            //    foreach (var branchId in existBranches)
            //    {
            //        var _userBranches = new userBranches()
            //        {
            //            GLBranchId = branchId.Id,
            //            userAccountId = table.id
            //        };
            //        _listuserBranches.Add(_userBranches);
            //    }
            //    _userBranchesCommand.AddRange(_listuserBranches);
            //    await _userBranchesCommand.SaveAsync();
            //}
            //add other Settings
            #endregion
            var banksIds = _gLBankQuery.TableNoTracking.Select(x => x.Id);
            var safesIds = _gLSafeQuery.TableNoTracking.Select(x => x.Id);
            var storesIds = _invStpStoresQuery.TableNoTracking.Where(x => existBranches.Contains(x.BranchId)).Select(x => x.StoreId);
            await ChangeOtherSettings(new OtherSettingsDto()
            {
                bankIds = banksIds.ToArray(),
                safeIds = safesIds.ToArray(),
                storeIds = safesIds.ToArray(),
                accredditForAllUsers = true,
                posAddDiscount = true,
                posAllowCreditSales = true,
                posCashPayment = true,
                posEditOtherPersonsInv = true,
                posNetPayment = true,
                posOtherPayment = true,
                posShowOtherPersonsInv = true,
                posShowReportsOfOtherPersons = true,
                purchasesAddDiscount = true,
                purchasesAllowCreditSales = true,
                PurchasesCashPayment = true,
                purchasesEditOtherPersonsInv = true,
                PurchasesNetPayment = true,
                PurchasesOtherPayment = true,
                purchasesShowOtherPersonsInv = true,
                purchasesShowReportsOfOtherPersons = true,
                purchaseShowBalanceOfPerson = true,
                salesAddDiscount = true,
                salesAllowCreditSales = true,
                salesCashPayment = true,
                salesEditOtherPersonsInv = true,
                salesNetPayment = true,
                salesOtherPayment = true,
                salesShowOtherPersonsInv = true,
                salesShowReportsOfOtherPersons = true,
                salesShowBalanceOfPerson = true,
                showAllBranchesInCustomerInfo = true,
                showAllBranchesInSuppliersInfo = true,
                showCustomersOfOtherUsers = true,
                showHistory = true,
                showOfferPricesOfOtherUser = true,
                CollectionReceipts = table.employeesId == 1 ? true : false,
                userAccountId = table.id
            });
            if (added)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addUsers);
            return new ResponseResult()
            {
                Id = table.id,
                Result = added ? Result.Success : Result.Failed
            };
        }
        public async Task<ResponseResult> editUser(editUsersDto parm)
        {
            if (!stringValidation.CheckEmailFormat(parm.email))
                return new ResponseResult()
                {
                    Note = Actions.invalidEmailFormat,
                    Result = Result.Failed
                };

            var user = _userAccountQuery.TableNoTracking.Include(c => c.UserAndPermission).ThenInclude(c => c.permissionList).Where(c => c.id == parm.Id).FirstOrDefault();
            if (parm.Id == 1)
            {
                parm.employeeId = user.employeesId;
                parm.Status = 1;
            }
            var table = Mapping.Mapper.Map(parm, user);
            if (parm.Status == 1)
                table.isActive = true;
            else
                table.isActive = false;
            var validation = await checkIfValid(parm.Id, parm.username, parm.password, parm.email, parm.employeeId, table, true);
            if (validation != null)
                return validation;
            table.employeesId = parm.employeeId;
            table.UpdateTime = DateTime.Now;
            var edited = await _userAccountCommand.UpdateAsyn(table);
            if (edited)
            {
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editUsers);
                SqlConnection con = new SqlConnection(_userAccountQuery.connectionString());
                try
                {
                    con.Open();
                    con.Execute($"update permissionList set UTime = '{DateTime.Now}'");
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    con.Close();
                }
   
            }
            return new ResponseResult()
            {
                Id = table.id,
                Result = edited ? Result.Success : Result.Failed
            };
        }
        public async Task<ResponseResult> deleteUser(deleteUsersDto parm)
        {

            if (parm.Ids.Contains(1))
                return new ResponseResult()
                {
                    Note = Actions.CanNotBeDeleted,
                    Result = Result.CanNotBeDeleted
                };
            if (parm.Ids.Count() < 0)
                return new ResponseResult()
                {
                    Note = "id is required",
                    Result = Result.Failed
                };

            var getUsers = await _userAccountQuery.GetAllAsyn(x => parm.Ids.Contains(x.id));
            if (getUsers.Count() < 0)
                return new ResponseResult()
                {
                    Note = "User Not Exist",
                    Result = Result.NotExist
                };

            _userAccountCommand.RemoveRange(getUsers);
            bool isDeleted = await _userAccountCommand.SaveAsync();
            if (isDeleted)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deleteUsers);

            //Fill The DeletedRecordTable
            var Ids = getUsers.Select(a => a.id);
            _deletedRecords.SetDeletedRecord(Ids.ToList(), 6);

            return new ResponseResult()
            {
                Note = "item deleted",
                Result = isDeleted ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> getAllUsers(getUsersDto parm)
        {
            var userInfo = await _userinformation.GetUserInformation();


            var users = _userAccountQuery.TableNoTracking
                                         .Include(x => x.userBranches)
                                         .Include(x => x.employees)
                                         .Where(x =>
                                         (userInfo.userId != 1 ? x.id != 1 : true) &&
                                         x.employees.EmployeeBranches.Select(c => c.BranchId).ToArray().Any(c => userInfo.employeeBranches.Contains(c))

                                         ).OrderByDescending(x => x.id);


            var employees = _invEmployeesQuery.TableNoTracking.Select(x => new { x.ArabicName, x.LatinName, x.Id });
            var usersFilter = users.Where(x => true);
            if (!string.IsNullOrEmpty(parm.SearchCriteria))
                usersFilter = users.Where(x =>
                                                x.employees.ArabicName.Contains(parm.SearchCriteria) ||
                                                x.employees.LatinName.Contains(parm.SearchCriteria) ||
                                                x.username.Contains(parm.SearchCriteria)
                                                ).OrderBy(x => x.id);


            if (parm.Status > 0)
                usersFilter = users.Where(x => parm.Status == 1 ? x.isActive == true : x.isActive == false).OrderBy(x => x.id);
            var totalCount = users.Count();
            var dataCount = usersFilter.Count();
            usersFilter = usersFilter.Skip((parm.PageNumber - 1) * parm.PageSize).Take(parm.PageSize);

            var usersOnline = _signalRQuery.TableNoTracking.ToList();
            var res = usersFilter
                .ToList()
                .Select(x => new
            {
                x.id,
                x.username,
                x.password,
                x.email,
                isOnline = isOnline(usersOnline,x.employeesId),
                employeeId = employees.Where(d => d.Id == x.employeesId).FirstOrDefault(),
                status = x.isActive ? 1 : 2
            });


            return new ResponseResult()
            {
                Data = res,
                DataCount = dataCount,
                TotalCount = totalCount,
                Result = Result.Success,

            };

        }
        private bool isOnline(List<signalR> signalRs,int employeesId)
        {
            bool isOnline = false;
            var user = signalRs.FirstOrDefault(c => c.InvEmployeesId == employeesId);
            if (user != null)
                isOnline = user.isOnline;

            return isOnline;
        }
        public async Task<ResponseResult> getUserById(int id)
        {
            var user = _userAccountQuery.TableNoTracking.Include(x => x.userBranches).Include(x => x.otherSettings).Include(x => x.employees).Where(x => x.id == id).FirstOrDefault();
            if (user == null)
                return new ResponseResult()
                {
                    Note = "User Not Exist",
                    Result = Result.NotExist
                };
            // edit user selector for the fronend
            return new ResponseResult()
            {
                Data = user,
                Id = user.id,
                Result = Result.Success,
            };
        }

        public async Task<ResponseResult> ChangeOtherSettings(OtherSettingsDto prm)
        {

            if (prm.Id == 1)
                return new ResponseResult()
                {
                    Note = Actions.DefultDataCanNotbeEdited,
                    Result = Result.Failed
                };
            var chechIfUserExist = _userAccountQuery.TableNoTracking.Include(x => x.otherSettings).Where(x => x.id == prm.userAccountId).FirstOrDefault();

            var table = new otherSettings();
            bool saved = false;
            if (chechIfUserExist == null)
                return new ResponseResult()
                {
                    Note = "User Not Exist",
                    Result = Result.NotExist
                };
            if (prm.Id != null)
            {
                var _table = _otherSettingsQuery.TableNoTracking.Where(x => x.Id == (int)prm.Id);
                table = Mapping.Mapper.Map<OtherSettingsDto, otherSettings>(prm);
                table.Id = _table.FirstOrDefault().Id;
                saved = await _otherSettingsCommand.UpdateAsyn(table);
            }
            else
            {
                table = Mapping.Mapper.Map<OtherSettingsDto, otherSettings>(prm);
                saved = await _otherSettingsCommand.AddAsync(table);
            }
            if (saved)
            {
                //Stores
                if (prm.storeIds.Count() > 0)
                {
                    var employeeId = _userAccountQuery.TableNoTracking.Where(x => x.id == prm.userAccountId).FirstOrDefault().employeesId;
                    var userBranches = _invEmployeesQuery.TableNoTracking.Include(x => x.EmployeeBranches).Where(x => x.Id == employeeId).FirstOrDefault().EmployeeBranches.Select(x => x.BranchId).ToArray();
                    var isOtherSettingsStoreExist = _otherSettingsStoresQuery.GetAll().Where(x => x.otherSettingsId == table.Id);

                    var stores = _invStpStoresQuery.TableNoTracking.Where(x => userBranches.Contains(x.BranchId) && prm.storeIds.Contains(x.StoreId));
                    if (isOtherSettingsStoreExist.Any())
                    {
                        _otherSettingsStoresCommand.RemoveRange(isOtherSettingsStoreExist);
                        await _otherSettingsStoresCommand.SaveAsync();
                    }
                    if (stores.Any())
                    {
                        var _ListOtherSettingsStores = new List<OtherSettingsStores>();
                        foreach (var store in stores)
                        {
                            var _OtherSettingsStores = new OtherSettingsStores()
                            {
                                InvStpStoresId = store.StoreId,
                                otherSettingsId = table.Id
                            };
                            _ListOtherSettingsStores.Add(_OtherSettingsStores);
                        }
                        _otherSettingsStoresCommand.AddRange(_ListOtherSettingsStores);
                        var storesSaved = await _otherSettingsStoresCommand.SaveAsync();
                    }
                }
                else
                {
                    var OtherSettingsStoreExist = _otherSettingsStoresQuery.GetAll().Where(x => x.otherSettingsId == table.Id);
                    _otherSettingsStoresCommand.RemoveRange(OtherSettingsStoreExist);
                    await _otherSettingsStoresCommand.SaveAsync();
                }
                //safes
                if (prm.safeIds.Count() > 0)
                {
                    var isOtherSettingsSafesExist = _otherSettingsSafesQuery.GetAll().Where(x => x.otherSettingsId == table.Id);
                    var safes = _gLSafeQuery.TableNoTracking.Where(x => prm.safeIds.Contains(x.Id));
                    if (isOtherSettingsSafesExist.Any())
                    {
                        _otherSettingsSafesCommand.RemoveRange(isOtherSettingsSafesExist);
                        await _otherSettingsSafesCommand.SaveAsync();
                    }
                    if (safes.Any())
                    {
                        var _ListOtherSettingsSafes = new List<OtherSettingsSafes>();
                        foreach (var safe in safes)
                        {
                            var _OtherSettingsStores = new OtherSettingsSafes()
                            {
                                gLSafeId = safe.Id,
                                otherSettingsId = table.Id
                            };
                            _ListOtherSettingsSafes.Add(_OtherSettingsStores);
                        }
                        _otherSettingsSafesCommand.AddRange(_ListOtherSettingsSafes);
                        await _otherSettingsSafesCommand.SaveAsync();
                    }
                }
                else
                {
                    var OtherSettingsSafesExist = _otherSettingsSafesQuery.GetAll().Where(x => x.otherSettingsId == table.Id);
                    _otherSettingsSafesCommand.RemoveRange(OtherSettingsSafesExist);
                    await _otherSettingsSafesCommand.SaveAsync();
                }
                //Banks
                if (prm.safeIds.Count() > 0)
                {
                    var isOtherSettingsBanksExist = _otherSettingsBanksQuery.GetAll().Where(x => x.otherSettingsId == table.Id);
                    var banks = _gLBankQuery.TableNoTracking.Where(x => prm.bankIds.Contains(x.Id));
                    if (isOtherSettingsBanksExist.Any())
                    {
                        _otherSettingsBanksCommand.RemoveRange(isOtherSettingsBanksExist);
                        await _otherSettingsBanksCommand.SaveAsync();
                    }
                    if (banks.Any())
                    {
                        var _ListOtherSettingsBanks = new List<OtherSettingsBanks>();
                        foreach (var bank in banks)
                        {
                            var _OtherSettingsBanks = new OtherSettingsBanks()
                            {
                                gLBankId = bank.Id,
                                otherSettingsId = table.Id
                            };
                            _ListOtherSettingsBanks.Add(_OtherSettingsBanks);
                        }
                        _otherSettingsBanksCommand.AddRange(_ListOtherSettingsBanks);
                        await _otherSettingsBanksCommand.SaveAsync();
                    }
                }
                else
                {
                    var OtherSettingsBanksExist = _otherSettingsBanksQuery.GetAll().Where(x => x.otherSettingsId == table.Id);
                    _otherSettingsBanksCommand.RemoveRange(OtherSettingsBanksExist);
                    await _otherSettingsBanksCommand.SaveAsync();
                }
            }
            _userAccountCommand.ClearTracking();
            var user = _userAccountQuery.TableNoTracking.Where(x => x.employeesId == chechIfUserExist.employeesId).FirstOrDefault();
            user.UpdateTime = DateTime.Now;

            await _userAccountCommand.UpdateAsyn(user);

            await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editUsers);
            return new ResponseResult()
            {
                Result = Result.Success
            };
        }



        public async Task<ResponseResult> GetOtherSettings(int userId)
        {

            var userBranches = _userAccountQuery.TableNoTracking.Include(x => x.employees).Include(x => x.employees.EmployeeBranches).Where(x => x.id == userId).FirstOrDefault().employees.EmployeeBranches.Select(x => x.BranchId).ToArray();
            var userSettings = _otherSettingsQuery.TableNoTracking
                                .Include(x => x.otherSettingsBanks)
                                .Include(x => x.otherSettingsSafes)
                                .Include(x => x.OtherSettingsStores)
                                .Where(x => x.userAccountId == userId);

            if (userSettings.Count() == 0)
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            var settings = userSettings.Select(x => new
            {
                x.posAddDiscount,
                x.posAllowCreditSales,
                x.posEditOtherPersonsInv,
                x.posShowOtherPersonsInv,
                x.posShowReportsOfOtherPersons,
                x.posCashPayment,
                x.posNetPayment,
                x.posOtherPayment,
                x.salesAddDiscount,
                x.salesAllowCreditSales,
                x.salesEditOtherPersonsInv,
                x.salesShowOtherPersonsInv,
                x.salesShowReportsOfOtherPersons,
                x.salesCashPayment,
                x.salesNetPayment,
                x.salesOtherPayment,
                x.salesShowBalanceOfPerson,
                x.salesShowAllPersonBalance,
                x.salesShowDebitorPersonBalance,
                x.salesShowCreditorPersonBaalance,
                x.salesPaymentFromInsideInvoice,
                x.purchasesAddDiscount,
                x.purchasesAllowCreditSales,
                x.purchasesEditOtherPersonsInv,
                x.purchasesShowOtherPersonsInv,
                x.purchasesShowReportsOfOtherPersons,
                x.PurchasesCashPayment,
                x.PurchasesNetPayment,
                x.PurchasesOtherPayment,
                x.purchaseShowBalanceOfPerson,
                x.purchaseShowAllPersonBalance,
                x.purchaseShowDebitorPersonBalance,
                x.purchaseShowCreditorPersonBaalance,
                x.PurchasesPaymentFromInsideInvoice,
                x.PurchasesallowEdidJournalEntry,
                x.showHistory,
                x.accredditForAllUsers,
                x.showCustomersOfOtherUsers,
                x.showOfferPricesOfOtherUser,
                x.showDashboardForAllUsers,
                x.showAllBranchesInCustomerInfo,
                x.showAllBranchesInSuppliersInfo,
                x.canShowAllPOSSessions,
                x.allowCloseCloudPOSSession,
                x.AllowPrintBarcode,
                x.CollectionReceipts
            });
            var stores = _invStpStoresQuery.TableNoTracking.Include(x => x.Store).Where(x => x.Store.Status == 1).GroupBy(x => x.StoreId).Select(y => y.FirstOrDefault()).ToList();
            var banks = _gLBankQuery.TableNoTracking.Include(x => x.BankBranches).Where(x => x.Status == 1); ;
            var safes = _gLSafeQuery.TableNoTracking.Where(x => x.Status == 1);

            var userStores = _otherSettingsStoresQuery.TableNoTracking.Where(x => x.otherSettingsId == userSettings.FirstOrDefault().Id).Select(x => x.InvStpStoresId);
            var userBanks = _otherSettingsBanksQuery.TableNoTracking.Where(x => x.otherSettingsId == userSettings.FirstOrDefault().Id).Select(x => x.gLBankId);
            var userSafes = _otherSettingsSafesQuery.TableNoTracking.Where(x => x.otherSettingsId == userSettings.FirstOrDefault().Id).Select(x => x.gLSafeId);


            var _banks = banks
                .Where(x => x.BankBranches.Select(c => c.BranchId).ToArray().Any(c => userBranches.Contains(c)))
                .Select(x => new
                {
                    x.Id,
                    arabicName = x.ArabicName,
                    latinName = x.LatinName,
                    isSelected = userId == 1 ? true : userBanks.Where(c => c == x.Id).Any()
                });

            var _safes = safes
                .Where(x => userBranches.Contains(x.BranchId))
                .Select(x => new
                {
                    x.Id,
                    arabicName = x.ArabicName,
                    latinName = x.LatinName,
                    isSelected = userId == 1 ? true : userSafes.Where(c => c == x.Id).Any()
                });

            var _stores = stores
                .Where(x => userBranches.Contains(x.BranchId))
                .Select(x => new
                {
                    Id = x.StoreId,
                    arabicName = x.Store.ArabicName,
                    latinName = x.Store.LatinName,
                    isSelected = userId == 1 ? true : userStores.Where(c => c == x.StoreId).Any()
                });


            var _settings = new settings()
            {
                posOtherSettings = settings.Select(x => new
                {
                    x.posAddDiscount,
                    x.posAllowCreditSales,
                    x.posEditOtherPersonsInv,
                    x.posShowOtherPersonsInv,
                    x.posShowReportsOfOtherPersons,
                    x.posCashPayment,
                    x.posNetPayment,
                    x.posOtherPayment,
                    x.canShowAllPOSSessions,
                    x.allowCloseCloudPOSSession
                }).FirstOrDefault(),
                salesOtherSettings = settings.Select(x => new
                {
                    x.salesAddDiscount,
                    x.salesAllowCreditSales,
                    x.salesEditOtherPersonsInv,
                    x.salesShowOtherPersonsInv,
                    x.salesShowReportsOfOtherPersons,
                    x.salesCashPayment,
                    x.salesNetPayment,
                    x.salesOtherPayment,
                    x.AllowPrintBarcode,
                    x.salesShowBalanceOfPerson,
                    x.salesShowAllPersonBalance,
                    x.salesShowDebitorPersonBalance,
                    x.salesShowCreditorPersonBaalance,
                    x.salesPaymentFromInsideInvoice

                }).FirstOrDefault(),
                purchasesOtherSettings = settings.Select(x => new
                {
                    x.purchasesAddDiscount,
                    x.purchasesAllowCreditSales,
                    x.purchasesEditOtherPersonsInv,
                    x.purchasesShowOtherPersonsInv,
                    x.purchasesShowReportsOfOtherPersons,
                    x.purchaseShowBalanceOfPerson,
                    x.purchaseShowAllPersonBalance,
                    x.purchaseShowDebitorPersonBalance,
                    x.purchaseShowCreditorPersonBaalance,
                    x.PurchasesPaymentFromInsideInvoice,
                    x.PurchasesallowEdidJournalEntry
                    //x.PurchasesCashPayment,
                    //x.PurchasesNetPayment,
                    //x.PurchasesOtherPayment
                }).FirstOrDefault(),
                generalOtherSettings = settings.Select(x => new
                {
                    x.showHistory,
                    x.accredditForAllUsers,
                    x.showCustomersOfOtherUsers,
                    x.showOfferPricesOfOtherUser,
                    x.showDashboardForAllUsers,
                    x.CollectionReceipts
                }).FirstOrDefault(),
                branchOtherSettings = settings.Select(x => new
                {
                    x.showAllBranchesInCustomerInfo,
                    x.showAllBranchesInSuppliersInfo
                }).FirstOrDefault()
            };

            var response = new userManagerOtherSettingsResponse()
            {
                settingId = userSettings.FirstOrDefault().Id,
                banks = _banks,
                safes = _safes,
                stores = _stores,
                Settings = _settings
            };
            return new ResponseResult()
            {
                Data = response,
                Result = Result.Success,
                Id = userSettings.FirstOrDefault().Id
            };
        }

        public async Task<ResponseResult> getUserInfoDropDownList(bool isSession = false)
        {
            UserInformationModel userInfo = await _userinformation.GetUserInformation();
            if (userInfo == null)
                return new ResponseResult()
                {
                    Note = Actions.JWTError,
                    Result = Result.Failed
                };
            var safes = _gLSafeQuery.TableNoTracking
                .Where(x => userInfo.userSafes.Contains(x.Id))
                .Where(x => x.BranchId == userInfo.CurrentbranchId)
                .Select(x => new
                {
                    x.Id,
                    x.ArabicName,
                    x.LatinName
                }).ToList();
            var Users = _otherSettingsQuery.TableNoTracking
                        .Include(x => x.userAccount)
                        .Include(x => x.userAccount.employees)
                        .Include(x => x.otherSettingsSafes)
                        .Where(x => x.userAccount.employees.EmployeeBranches.Select(c => c.BranchId).ToArray().Contains(userInfo.CurrentbranchId))
                        .Where(x => (!isSession ? userInfo.otherSettings.accredditForAllUsers : userInfo.otherSettings.canShowAllPOSSessions) ? 1 == 1 : x.userAccount.id == userInfo.userId)
                        .Where(x => userInfo.userId != 1 ? x.Id != 1 : true);
            var resp = Users.Select(x => new
            {
                id = x.userAccount.employeesId,
                arabicName = x.userAccount.employees.ArabicName,
                latinName = x.userAccount.employees.LatinName,
                isSelected = userInfo.userId == x.userAccount.id ? true : false,
                safesIds = safes
            });

            return new ResponseResult()
            {
                Data = resp,
                Result = Result.Success,

            };
        }

        public async Task<ResponseResult> GetUsersByDate(GetUsersByDateRequest parameter)
        {

            return await _mediator.Send(parameter);
        }



    }
    public class getUserInfoDropDownListModel
    {
        public int id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int employeeId { get; set; }
        public object safes { get; set; }
        public bool isSelected { get; set; }
    }

}
