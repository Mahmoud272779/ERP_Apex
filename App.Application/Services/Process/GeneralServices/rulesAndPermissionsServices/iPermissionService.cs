using App.Domain.Models.Request.General;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface iPermissionService
    {
        //PermissionLists
        Task<ResponseResult> addPermissionList(addPermissionRequestDto prm);
        Task<ResponseResult> EditPermissionList(editPermissionRequestDto prm);
        Task<ResponseResult> GetAllPermissionLists(getPermissionRequestDto prm);
        Task<ResponseResult> DeletePermissionLists(deletePermissionRequestDto prm);
        Task<ResponseResult> EditUsersToPermissionList(addUsersToPermissionListRequestDto prm);

        Task<ResponseResult> GetPermissionListUsers(getPermissionListUsers prm);
        Task<ResponseResult> DeletePermissionListUsers(int[] userId);
        Task<ResponseResult> GetUsersHaveNotPermissions();
        Task<ResponseResult> GetPermissionListByDate(DateTime date, int PageNumber, int PageSize);
        Task<ResponseResult> GetUsersToPermissionListByDate(DateTime date);
        Task<ResponseResult> GetRulesByDate(DateTime date);


        //------------------------------------------------------------//
        //Rules
        Task<ResponseResult> EditRules(editRulesRequestDto prm);
        Task<ResponseResult> GetMainForms(int permissionListId);
        Task<ResponseResult> UpdateSubForms();
        Task<ResponseResult> GetSubForms(int permissionListId,int MainFormCode);





    }
}
