using App.Application.Handlers.Persons.GetPersonsByDate;
using App.Application.Helpers;
using App.Application.Services.HelperService.SecurityIntegrationServices;
using App.Application.Services.Process.GeneralServices.DeletedRecords;
using App.Application.Services.Process.GeneralServices.SystemHistoryLogsServices;
using App.Application.Services.Process.Invoices.General_APIs;
using App.Domain.Entities;
using App.Domain.Entities.Process.Store;
using App.Domain.Models.Request.General;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;
using static App.Domain.Enums.Enums;
using static App.Domain.Models.Security.Authentication.Request.SharedRequestDTOs;

namespace App.Application
{
    public class permissionService : iPermissionService
    {
        private readonly IRepositoryQuery<permissionList> _permissionListQuery;
        private readonly IRepositoryCommand<permissionList> _permissionListCommand;
        private readonly IRepositoryQuery<userAccount> _userAccountQuery;
        private readonly IRepositoryCommand<userAccount> _userAccountCommand;
        private readonly IRepositoryQuery<rules> _rulesQuery;
        private readonly IRepositoryCommand<rules> _rulesCommand;
        private readonly IRepositoryQuery<UserAndPermission> _userAndPermissionQuery;
        private readonly IRepositoryCommand<UserAndPermission> _userAndPermissionCommand;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;
        private readonly IDeletedRecordsServices _deletedRecords;
        private readonly iUserInformation _iUserInformation;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IGeneralAPIsService generalAPIsService;
        private readonly ISecurityIntegrationService _ISecurityIntegrationService;

        public permissionService(
                                IRepositoryQuery<permissionList> permissionListQuery,
                                IRepositoryCommand<permissionList> permissionListCommand,

                                IRepositoryQuery<userAccount> userAccountQuery,
                                IRepositoryCommand<userAccount> userAccountCommand,

                                IRepositoryQuery<rules> rulesQuery,
                                IRepositoryCommand<rules> rulesCommand,

                                IRepositoryQuery<UserAndPermission> UserAndPermissionQuery,
                                IRepositoryCommand<UserAndPermission> UserAndPermissionCommand,

                                ISystemHistoryLogsService systemHistoryLogsService,
                                IDeletedRecordsServices deletedRecords,

                                iUserInformation iUserInformation,
                                IHttpContextAccessor httpContext,
                                IGeneralAPIsService generalAPIsService,
                                ISecurityIntegrationService iSecurityIntegrationService)
        {
            _permissionListQuery = permissionListQuery;
            _permissionListCommand = permissionListCommand;
            _userAccountQuery = userAccountQuery;
            _userAccountCommand = userAccountCommand;
            _rulesQuery = rulesQuery;
            _rulesCommand = rulesCommand;
            _userAndPermissionQuery = UserAndPermissionQuery;
            _userAndPermissionCommand = UserAndPermissionCommand;
            _systemHistoryLogsService = systemHistoryLogsService;
            _deletedRecords = deletedRecords;
            _iUserInformation = iUserInformation;
            _httpContext = httpContext;
            this.generalAPIsService = generalAPIsService;
            _ISecurityIntegrationService = iSecurityIntegrationService;
        }

        private async Task<ResponseResult> checkListNameIfExst(string nameAr, string nameEn, int? id, bool isEdit)
        {
            var permissionLists = await _permissionListQuery.GetAllAsyn();

            //arabic name
            if (!isEdit)
            {

                if (permissionLists.Where(x => x.arabicName == nameAr).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.ArabicNameExist,
                        Result = Result.Exist
                    };
            }
            else
            {

                if (permissionLists.Where(x => x.arabicName == nameAr && x.Id != id).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.ArabicNameExist,
                        Result = Result.Exist
                    };
            }
            // english name
            if (!isEdit)
            {
                if (permissionLists.Where(x => x.latinName == nameEn).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.EnglishNameExist,
                        Result = Result.Exist
                    };
            }
            else
            {
                if (permissionLists.Where(x => x.latinName == nameEn && x.Id != id).Any())
                    return new ResponseResult()
                    {
                        Note = Actions.EnglishNameExist,
                        Result = Result.Exist
                    };
            }

            return null;
        }

        public async Task<ResponseResult> addPermissionList(addPermissionRequestDto prm)
        {
            if (!string.IsNullOrEmpty(prm.arabicName))
                prm.arabicName = prm.arabicName.Trim();
            if (!string.IsNullOrEmpty(prm.latinName))
                prm.latinName = prm.latinName.Trim();

            var checkName = await checkListNameIfExst(prm.arabicName, prm.latinName, null, false);
            if (checkName != null)
                return checkName;
            if (string.IsNullOrEmpty(prm.arabicName.Trim()))
                return new ResponseResult()
                {
                    Result = Result.RequiredData,
                    Note = Actions.NameIsRequired
                };
            if (string.IsNullOrEmpty(prm.latinName))
                prm.latinName = prm.arabicName;
            prm.arabicName = prm.arabicName.Trim();
            prm.latinName = prm.latinName.Trim();
            prm.UTime = DateTime.Now;

            var table = Mapping.Mapper.Map<addPermissionRequestDto, permissionList>(prm);
            var added = await _permissionListCommand.AddAsync(table);

            if (added)
            {
                var rules = returnSubForms.returnRules(false, table.Id);
                _rulesCommand.AddRange(rules);
                await _rulesCommand.SaveAsync();

            }
            if (added)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.addPermission);
            return new ResponseResult()
            {
                Note = added ? Actions.SavedSuccessfully : Actions.SaveFailed,
                Result = added ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> DeletePermissionLists(deletePermissionRequestDto prm)
        {
            if (prm.Ids.Contains(1))
                return new ResponseResult()
                {
                    Note = Actions.CanNotBeDeleted,
                    Result = Result.CanNotBeDeleted
                };
            if (!prm.Ids.Any())
                return new ResponseResult()
                {
                    Note = Actions.NameIsRequired,
                    Result = Result.RequiredData
                };
            var listsCanNotBeDeleted = _userAndPermissionQuery.TableNoTracking.Include(x => x.permissionList).Where(x => prm.Ids.Contains(x.permissionListId));
            var listsCanBeDeleted = await _permissionListQuery.GetAllAsyn(x => prm.Ids.Contains(x.Id) && !listsCanNotBeDeleted.Select(x => x.permissionListId).ToArray().Contains(x.Id));
            if (!listsCanBeDeleted.Any())
                return new ResponseResult()
                {
                    Data = "permission Lists can not be deleted " + string.Join("-", listsCanNotBeDeleted.Select(x => x.permissionList.arabicName)),
                    Note = Actions.CanNotBeDeleted,
                    Result = Result.RequiredData
                };
            _permissionListCommand.RemoveRange(listsCanBeDeleted);
            var deleted = await _permissionListCommand.SaveAsync();
            if (deleted)
            {
                await _deletedRecords.SetDeletedRecord(listsCanBeDeleted.Select(x=>x.Id).ToList(), 10);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deletePermission);

            }
            return new ResponseResult()
            {
                Data = deleted ? "" : "permission Lists can not be deleted " + string.Join("-", listsCanNotBeDeleted.Select(x => x.permissionList.arabicName)),
                Note = deleted ? Actions.DeletedSuccessfully : Actions.DeleteFailed,
                Result = deleted ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> EditPermissionList(editPermissionRequestDto prm)
        {
            var checkName = await checkListNameIfExst(prm.arabicName, prm.latinName, prm.Id, true);
            if (checkName != null)
                return checkName;

            var checkIfListExist = _permissionListQuery.TableNoTracking.Where(x => x.Id == prm.Id).FirstOrDefault();
            if (checkIfListExist == null)
                return new ResponseResult()
                {
                    Result = Result.NotFound,
                    Note = Actions.NotFound
                };
            if (string.IsNullOrEmpty(prm.arabicName.Trim()))
                return new ResponseResult()
                {
                    Result = Result.RequiredData,
                    Note = Actions.NameIsRequired
                };
            if (string.IsNullOrEmpty(prm.latinName))
                prm.latinName = prm.arabicName;
            prm.arabicName = prm.arabicName.Trim();
            prm.latinName = prm.latinName.Trim();

            checkIfListExist.arabicName = prm.arabicName;
            checkIfListExist.latinName = prm.latinName;
            checkIfListExist.note = prm.note;
            checkIfListExist.UTime = DateTime.Now;

            var added = await _permissionListCommand.UpdateAsyn(checkIfListExist);
            if (added)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editPermission);
            return new ResponseResult()
            {
                Result = added ? Result.Success : Result.Failed,
                Note = added ? Actions.Success : Actions.UpdateFailed,
                Id = prm.Id
            };
        }

        public async Task<ResponseResult> GetAllPermissionLists(getPermissionRequestDto prm)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            var allPermissionsList = _permissionListQuery.TableNoTracking.Include(x => x.UserAndPermission).Include(x => x.rules).OrderByDescending(x => x.Id);


            var users = _userAndPermissionQuery.TableNoTracking
                                .Include(x => x.userAccount)
                                .Include(x => x.userAccount.employees)
                                .Include(x => x.userAccount.employees.EmployeeBranches)
                                .Where(x => (userInfo.userId != 1 ? x.userAccount.id != 1 : true) &&
                                            x.userAccount.employees.EmployeeBranches.Select(d => d.BranchId).Any(c => userInfo.employeeBranches.Contains(c))
                                            );


            var totalCount = allPermissionsList.Count();
            var allPermissionsFilter = allPermissionsList.Where(x => x.Id > 0);
            if (!string.IsNullOrEmpty(prm.SearchCriteria))
                allPermissionsFilter = allPermissionsFilter.Where(x => x.arabicName.Contains(prm.SearchCriteria) || x.latinName.Contains(prm.SearchCriteria));
            var dataCount = allPermissionsFilter.Count();
            allPermissionsFilter = allPermissionsFilter.Skip((prm.PageNumber - 1) * prm.PageSize).Take(prm.PageSize);
            var permissionsCanNotDelete = _userAndPermissionQuery.TableNoTracking.Select(x => x.permissionListId);
            var res = allPermissionsFilter.Select(x => new
            {
                Id = x.Id,
                arabicName = x.arabicName,
                latinName = x.latinName,
                x.note,
                userCount = users.Where(c => c.permissionListId == x.Id).Count(),
                canDelete = permissionsCanNotDelete.Where(c => c == x.Id).Any()
            });

            return new ResponseResult()
            {
                Data = res,
                DataCount = dataCount,
                Result = Result.Success,
                TotalCount = totalCount,
            };
        }

        public async Task<ResponseResult> EditUsersToPermissionList(addUsersToPermissionListRequestDto prm)
        {
            //check PermissionListId
            if (!_permissionListQuery.TableNoTracking.Where(x => x.Id == prm.permissionListId).Any())
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            //check ids 
            if (!prm.userIds.Any() || prm.permissionListId == null || prm.permissionListId == 0)
                return new ResponseResult()
                {
                    Note = Actions.IdIsRequired,
                    Result = Result.RequiredData
                };
            //check the id is not exist in user and permissions
            if (_userAndPermissionQuery.TableNoTracking.Where(x => prm.userIds.Contains(x.userAccountId)).Any())
                return new ResponseResult()
                {
                    Note = Actions.Exist,
                    Result = Result.Exist
                };
            var findUsers = _userAccountQuery.TableNoTracking.Include(x => x.UserAndPermission).Where(x => prm.userIds.Contains(x.id));
            if (!findUsers.Any())
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            var ListusersAndPermissions = new List<UserAndPermission>();
            foreach (var item in findUsers)
            {
                var userAndPermissions = new UserAndPermission()
                {
                    permissionListId = prm.permissionListId,
                    userAccountId = item.id,
                    UTime = DateTime.Now
                };
                ListusersAndPermissions.Add(userAndPermissions);
            }
            _userAndPermissionCommand.AddRange(ListusersAndPermissions);
            var saved = await _userAndPermissionCommand.SaveAsync();
            if (saved)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editPermission);

            //Set Time of change to PermissionList Table
            var permission = _permissionListQuery.TableNoTracking.Where(x => x.Id == prm.permissionListId).FirstOrDefault();
            permission.UTime = DateTime.Now;
            await _permissionListCommand.UpdateAsyn(permission);

            return new ResponseResult()
            {
                Note = saved ? Actions.SavedSuccessfully : Actions.SaveFailed,
                Result = saved ? Result.Success : Result.Failed
            };
        }
        public async Task<ResponseResult> EditRules(editRulesRequestDto prm)
        {
            _rulesCommand.ClearTracking();
            var allRules = _rulesQuery.TableNoTracking.Where(x => prm.updatedRules.Select(d => d.id).Contains(x.Id) && x.permissionListId != 1).ToList();
            if (allRules == null)
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            if (allRules.Where(c => c.permissionListId == 1).Any())
                return new ResponseResult()
                {
                    Note = Actions.DefultDataCanNotbeEdited,
                    Result = Result.Failed
                };
            var cast = allRules.Join(prm.updatedRules, x => x.Id, c => c.id, (x, c) => new
            {
                x.Id,
                c.isPrint,
                c.isAdd,
                c.isDelete,
                c.isShow,
                c.isEdit,
                x.mainFormCode,
                x.permissionListId,
                x.arabicName,
                x.latinName,
                x.permissionList,
                x.subFormCode,
                x.applicationId,
                x.isVisible,
                x.UTime
            });
            var listOfRules = new List<rules>();
            foreach (var rule in cast)
            {
                listOfRules.Add(new rules
                {
                    Id = rule.Id,
                    arabicName = rule.arabicName,
                    isAdd = rule.isAdd,
                    isDelete = rule.isDelete,
                    isEdit = rule.isEdit,
                    isPrint = rule.isPrint,
                    isShow = rule.isShow,
                    latinName = rule.latinName,
                    mainFormCode = rule.mainFormCode,
                    permissionListId = rule.permissionListId,
                    subFormCode = rule.subFormCode,
                    applicationId = rule.applicationId,
                    isVisible = rule.isVisible,
                    UTime = DateTime.Now
                });
            }

            var updated = await _rulesCommand.UpdateAsyn(listOfRules);
            if (updated)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editPermission);

            //Set Time of change to PermissionList Table
            var permission = _permissionListQuery.TableNoTracking.Where(x => x.Id == listOfRules.FirstOrDefault().permissionListId).FirstOrDefault();
            permission.UTime = DateTime.Now;
            await _permissionListCommand.UpdateAsyn(permission);

            return new ResponseResult()
            {
                Note = updated ? Actions.UpdatedSuccessfully : Actions.UpdateFailed,
                Result = updated ? Result.Success : Result.Failed
            };
        }


        public async Task<ResponseResult> GetPermissionListUsers(getPermissionListUsers prm)
        {
            var userInfo = await _iUserInformation.GetUserInformation();

            var users = _userAccountQuery
                                        .TableNoTracking
                                        .Include(x => x.employees)
                                        .Include(x => x.employees.EmployeeBranches)
                                        .Where(x =>
                                                    x.UserAndPermission.Where(d => d.permissionListId == prm.permissionListId).Any() &&
                                                    x.employees.EmployeeBranches.Select(d => d.BranchId).Any(c => userInfo.employeeBranches.Contains(c)) &&
                                                    (userInfo.userId != 1 ? x.id != 1 : true)
                                                    );
            var totalCount = users.Count();
            if (!string.IsNullOrEmpty(prm.SearchCriteria))
                users = users.Where(x => x.employees.Id.ToString().Contains(prm.SearchCriteria) || x.employees.ArabicName.Contains(prm.SearchCriteria) || x.employees.LatinName.Contains(prm.SearchCriteria));
            var dataCount = users.Count();
            if (!string.IsNullOrEmpty(prm.SearchCriteria))
                users = users.OrderBy(x => x.id);
            else
                users = users.OrderByDescending(x => x.id);
            users = users.Skip((prm.pageNumber - 1) * prm.pageSize).Take(prm.pageSize);
            var res = users.Select(x => new
            {
                Id = x.id,
                x.username,
                arabicName = x.employees.ArabicName,
                latinName = x.employees.LatinName
            });

            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success,
                DataCount = dataCount,
                TotalCount = totalCount
            };
        }

        public async Task<ResponseResult> DeletePermissionListUsers(int[] Ids)
        {
            if (Ids.Contains(1))
                return new ResponseResult()
                {
                    Note = Actions.DefultDataCanNotbeDeleted,
                    Result = Result.Failed,
                    ErrorMessageEn = "Can not delete defult data",
                    ErrorMessageAr = "لا يمكن حذف البيانات الاساسية"
                };
            if (!Ids.Any())
                return new ResponseResult()
                {
                    Note = Actions.IdIsRequired,
                    Result = Result.RequiredData
                };
            var userInfo = await _iUserInformation.GetUserInformation();
            Ids = Ids.Where(x => x != userInfo.userId).ToArray();
            if (!Ids.Any())
                return new ResponseResult()
                {
                    Note = Actions.IdIsRequired,
                    Result = Result.Failed,
                    ErrorMessageAr = "لا يمكن حذف الصلاحية لنفس المستخدم",
                    ErrorMessageEn = "You can not delete the current user from the permission list"
                };
            var findUsers = _userAndPermissionQuery.TableNoTracking.Where(x => Ids.Contains(x.userAccountId));
            if (findUsers == null)
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound,
                    ErrorMessageAr = "المستخدم غير موجود",
                    ErrorMessageEn = "User is not exist"
                };
            var permissionListIds = findUsers.ToList();
            _userAndPermissionCommand.RemoveRange(findUsers);
            bool deleted = await _userAndPermissionCommand.SaveChanges() > 0 ? true : false ;


            if (deleted)
            {
               
                //Set Time of change to PermissionList Table
                var permission = _permissionListQuery.TableNoTracking.Where(x => x.Id == permissionListIds.FirstOrDefault().permissionListId).FirstOrDefault();
                permission.UTime = DateTime.Now;
                await _permissionListCommand.UpdateAsyn(permission);
                //Fill Delete Record Table
                await _deletedRecords.SetDeletedRecord(permissionListIds.Select(x=>x.Id).ToList(), 11);
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.deletePermission);
            }


            return new ResponseResult()
            {
                Note = deleted ? Actions.DeletedSuccessfully : Actions.DeleteFailed,
                Result = deleted ? Result.Success : Result.Failed
            };
        }

        public async Task<ResponseResult> GetUsersHaveNotPermissions()
        {
            var usersUsed = _userAndPermissionQuery.TableNoTracking.Select(x => x.userAccountId).ToArray();
            var usersNotUsed = _userAccountQuery.TableNoTracking.Include(x => x.employees).Where(x => !usersUsed.Contains(x.id) && x.isActive == true);
            var res = usersNotUsed.Select(x => new
            {
                x.id,
                x.username,
                arabicName = x.employees.ArabicName,
                latinName = x.employees.LatinName
            });
            return new ResponseResult()
            {
                Data = res,
                Note = Actions.Success,
                Result = Result.Success,
                TotalCount = res.Count(),
                DataCount = res.Count()
            };
        }

        public async Task<ResponseResult> GetMainForms(int permissionListId)
        {
            var companyInfo = await _ISecurityIntegrationService.getCompanyInformation();

            var MainForms = returnMainForms
                .mainForms()
                .Where(c=> companyInfo.apps.Select(v=> v.Id).ToArray().Contains( c.appId) || c.appId == (int)applicationIds.Genral);

            var subFormsCount = returnSubForms.returnRules().Where(x => x.isVisible && (companyInfo.apps.Select(v => v.Id).ToArray().Contains(x.applicationId) || x.applicationId == (int)applicationIds.Genral));

            var rules = _rulesQuery.TableNoTracking
                .Where(x => x.permissionListId == permissionListId && x.isVisible && (companyInfo.apps.Select(v => v.Id).ToArray().Contains(x.applicationId) || x.applicationId == (int)applicationIds.Genral));
            if(permissionListId != 1)
            {
                var newRules = subFormsCount.Where(c => !rules.Select(v => v.subFormCode).Contains(c.subFormCode)).ToList();
                if (newRules.Any())
                {
                    var rulesNotExist = subFormsCount.Where(x => !rules.Select(c => c.subFormCode).ToArray().Contains(x.subFormCode)).ToList();
                    if (rulesNotExist.Any() && permissionListId != 1)
                    {
                        newRules.ForEach(x =>
                        {
                            x.permissionListId = permissionListId;
                        });
                        await _rulesCommand.AddAsync(newRules);
                        rules = _rulesQuery.TableNoTracking.Where(x => x.permissionListId == permissionListId && x.isVisible);
                    }
                }
            }
            if (!rules.Any())
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            var res = MainForms.Select(x => new
            {
                x.Id,
                x.arabicName,
                x.latinName,
                x.appId,
                isShow = !rules.Where(c => c.mainFormCode == x.Id && c.isShow == false).Any(),
                isAdd = !rules.Where(c => c.mainFormCode == x.Id && c.isAdd == false).Any(),
                isEdit = !rules.Where(c => c.mainFormCode == x.Id && c.isEdit == false).Any(),
                isDelete = !rules.Where(c => c.mainFormCode == x.Id && c.isDelete == false).Any(),
                isPrint = !rules.Where(c => c.mainFormCode == x.Id && c.isPrint == false).Any(),
                addCheckedCount = rules.Where(c => c.mainFormCode == x.Id && c.isAdd == true).Count(),
                editCheckedCount = rules.Where(c => c.mainFormCode == x.Id && c.isEdit == true).Count(),
                deleteCheckedCount = rules.Where(c => c.mainFormCode == x.Id && c.isDelete == true).Count(),
                printCheckedCount = rules.Where(c => c.mainFormCode == x.Id && c.isPrint == true).Count(),
                showCheckedCount = rules.Where(c => c.mainFormCode == x.Id && c.isShow == true).Count(),
                isAllChecked = !rules.Where(c => c.mainFormCode == x.Id && (c.isShow == false || c.isDelete == false || c.isPrint == false || c.isAdd == false || c.isEdit == false)).Any(),
                subPages = rules.Where(c => c.mainFormCode == x.Id && (companyInfo.apps.Select(v => v.Id).ToArray().Contains(c.applicationId) || c.applicationId == (int)applicationIds.Genral))
                
                .OrderBy(c => c.subFormCode)
                .Select(c => new
                {
                    id = c.Id,
                    mainFormCode = c.mainFormCode,
                    subFormCode = c.subFormCode,
                    arabicName = c.arabicName,
                    latinName = c.latinName,
                    permissionListId = c.permissionListId,
                    isShow = c.isShow,
                    isAdd = c.isAdd,
                    isEdit = c.isEdit,
                    isDelete = c.isDelete,
                    isPrint = c.isPrint,
                    isAllChecked = rules.Where(d => d.Id == c.Id).Where(d =>
                                                                        d.isAdd == true &&
                                                                        d.isEdit == true &&
                                                                        d.isDelete == true &&
                                                                        d.isShow == true &&
                                                                        d.isPrint == true).Any()
                }).OrderBy(v => Math.Abs(v.id))
            });
            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success,
                Note = Actions.Success
            };
        }

        public async Task<ResponseResult> GetSubForms(int permissionListId, int MainFormCode)
        {
            var rules = _rulesQuery.TableNoTracking.Where(x => x.permissionListId == permissionListId && x.mainFormCode == MainFormCode);
            var res = rules.Select(x => new
            {
                id = x.Id,
                mainFormCode = x.mainFormCode,
                subFormCode = x.subFormCode,
                arabicName = x.arabicName,
                latinName = x.latinName,
                permissionListId = x.permissionListId,
                isShow = x.isShow,
                isAdd = x.isAdd,
                isEdit = x.isEdit,
                isDelete = x.isDelete,
                isPrint = x.isPrint,
                isAllChecked = rules.Where(c => c.Id == x.Id).Where(c =>
                                                                        c.isAdd == true &&
                                                                        c.isEdit == true &&
                                                                        c.isDelete == true &&
                                                                        c.isShow == true &&
                                                                        c.isPrint == true).Any()
            });
            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success,
                Note = Actions.Success
            };

        }

        public async Task<ResponseResult> UpdateSubForms()
        {
            var subForms = returnSubForms.returnRules();
            var DBSubForms = _rulesQuery.TableNoTracking;
            _rulesCommand.RemoveRange(DBSubForms);
            await _rulesCommand.SaveAsync();

            var permissionListsIds = _permissionListQuery.TableNoTracking;
            var listOfRules = new List<rules>();
            foreach (var item in permissionListsIds)
            {
                foreach (var subFormsItem in subForms)
                {
                    listOfRules.Add(new rules
                    {
                        permissionListId = item.Id,
                        arabicName = subFormsItem.arabicName,
                        applicationId = subFormsItem.applicationId,
                        isVisible = subFormsItem.isVisible,
                        latinName = subFormsItem.latinName,
                        mainFormCode = subFormsItem.mainFormCode,
                        subFormCode = subFormsItem.subFormCode,

                        isAdd = item.Id == 1 ? true : false,
                        isDelete = item.Id == 1 ? true : false,
                        isEdit = item.Id == 1 ? true : false,
                        isPrint = item.Id == 1 ? true : false,
                        isShow = item.Id == 1 ? true : false
                    });
                }
            }
            _rulesCommand.AddRange(listOfRules);
            var saved = await _rulesCommand.SaveAsync();
            if (saved)
                await _systemHistoryLogsService.SystemHistoryLogsService(Domain.Enums.SystemActionEnum.editPermission);
            return new ResponseResult()
            {
                Result = saved ? Result.Success : Result.Failed
            };

        }

        public async Task<ResponseResult> GetPermissionListByDate(DateTime date, int PageNumber, int PageSize)
        {
            var resData = await _permissionListQuery.TableNoTracking
                .Where(q => q.UTime >= date )
            .Include(x=>x.UserAndPermission)
            .Include(x=>x.rules).ToListAsync();

            return await generalAPIsService.Pagination(resData, PageNumber, PageSize);

            //return await _mediator.Send(new GetPersonsByDateRequest { date = date });
        }
        public async Task<ResponseResult> GetUsersToPermissionListByDate(DateTime date)
        {
            var data = await _userAndPermissionQuery.TableNoTracking.Where(q => q.UTime >= date).ToListAsync();
            return new ResponseResult() { Data = data, Id = null, Result = Result.Success };
            //return await _mediator.Send(new GetPersonsByDateRequest { date = date });
        }
        public async Task<ResponseResult> GetRulesByDate(DateTime date)
        {
            var data = await _rulesQuery.TableNoTracking.Where(q => q.UTime >= date).ToListAsync();
            return new ResponseResult() { Data = data, Id = null, Result = Result.Success };
        }
    }
}
