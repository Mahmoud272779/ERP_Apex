using App.Application.Helpers;
using App.Domain.Entities;
using App.Domain.Enums;
using App.Domain.Models.Shared;
using App.Infrastructure;
using App.Infrastructure.Interfaces.Repository;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Application.Helpers.Aliases;

namespace App.Application.Services.HelperService.authorizationServices
{
    public class authorizationService : iAuthorizationService
    {
        private readonly iUserInformation _iUserInformation;
        private readonly IRepositoryQuery<UserAndPermission> _userAndPermissionQuery;
        private readonly IRepositoryQuery<rules> _rulesQuery;
        private readonly IHttpContextAccessor _httpContext;

        public authorizationService(iUserInformation iUserInformation,IRepositoryQuery<UserAndPermission> UserAndPermissionQuery, IRepositoryQuery<rules> rulesQuery,IHttpContextAccessor httpContext)
        {
            _iUserInformation = iUserInformation;
            _userAndPermissionQuery = UserAndPermissionQuery;
            _rulesQuery = rulesQuery;
            _httpContext = httpContext;
        }
        public ResponseResult ReturnUnAuthorized => new ResponseResult()
        {
            Note = Actions.UnAuthorized,
            Result = Enums.Result.UnAuthorized,
            ErrorMessageAr = ErrorMessagesAr.NoPermission,
            ErrorMessageEn = ErrorMessagesEn.NoPermission,
        };
        public async Task<ResponseResult> isAuthorized(int mainFormCode, int subFormCode, Opretion opretion)
        {
            var userInfo = await _iUserInformation.GetUserInformation();
            if (userInfo == null)
                return ReturnUnAuthorized;
            var _userPermission = _rulesQuery
                                    .TableNoTracking
                                    .Where(x => x.permissionListId == userInfo.permissionListId && x.isVisible)
                                    .ToList().GroupBy(x => x.subFormCode).Select(x => x.FirstOrDefault());
            var userPermission = _userPermission.Where(x=> x.subFormCode == subFormCode).FirstOrDefault();
            if (userInfo.userId == 1)
                return null;
            if (userPermission == null)
                return ReturnUnAuthorized;
            if (opretion == Opretion.Add)
                return userPermission.isAdd ? null : ReturnUnAuthorized;
            else if (opretion == Opretion.Edit)
                return userPermission.isEdit ? null : ReturnUnAuthorized;
            else if (opretion == Opretion.Open)
                return userPermission.isShow ? null : ReturnUnAuthorized;
            else if (opretion == Opretion.Delete)
                return userPermission.isDelete ? null : ReturnUnAuthorized;
            else if (opretion == Opretion.Print)
                return userPermission.isPrint ? null : ReturnUnAuthorized;
            return ReturnUnAuthorized;
        }
        
    }
}
