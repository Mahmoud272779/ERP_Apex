using App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate;
using App.Domain.Models.Request.General;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface iUserService
    {
        Task<ResponseResult> getAllUsers(getUsersDto prm);
        Task<ResponseResult> getUserById(int id);
        Task<ResponseResult> addUser(addUsersDto prm);
        Task<ResponseResult> editUser(editUsersDto prm);
        Task<ResponseResult> deleteUser(deleteUsersDto prm);
        Task<ResponseResult> ChangeOtherSettings(OtherSettingsDto prm);
        Task<ResponseResult> GetOtherSettings(int useeId);
        Task<ResponseResult> getUserInfoDropDownList(bool isSession = false);
        Task<ResponseResult> GetUsersByDate(GetUsersByDateRequest parameter);


    }
}
