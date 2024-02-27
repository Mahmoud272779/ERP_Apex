using App.Domain.Models.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure
{
    public class errorResponse
    {
        public async static void responseUnautorized(HttpContext context,string notes = "")
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.NoPermission,
                ErrorMessageEn = ErrorMessagesEn.NoPermission,
                Result = Domain.Enums.Enums.Result.UnAuthorized,
                Note = notes

            };
            var result = new UnauthorizedObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
        public async static void responseUnUpdated(HttpContext context,string notes = "")
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.systemNotUpdated,
                ErrorMessageEn = ErrorMessagesEn.systemNotUpdated,
                Result = Domain.Enums.Enums.Result.UnAuthorized,
                Note = notes

            };
            var result = new UnauthorizedObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
        public async static void PasswordRequired(HttpContext context, string notes = "")
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.passwordRequired,
                ErrorMessageEn = ErrorMessagesEn.passwordRequired,
                Result = Domain.Enums.Enums.Result.UnAuthorized,
                Note = notes

            };
            var result = new UnauthorizedObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }

        public async static void UserLoggedInFromAnotherPLace(HttpContext context)
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.UserLoggedInFromAnotherPLace,
                ErrorMessageEn = ErrorMessagesEn.UserLoggedInFromAnotherPLace,
                Result = Domain.Enums.Enums.Result.Failed

            };
            var result = new UnauthorizedObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
        public async static void responseConflict(HttpContext context)
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.responseConflict,
                ErrorMessageEn = ErrorMessagesEn.responseConflict,
                Result = Domain.Enums.Enums.Result.Failed
    
            };
            var result = new ConflictObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
        public async static void ReponseUserNotFound(HttpContext context)
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.UserNotFound,
                ErrorMessageEn = ErrorMessagesEn.UserNotFound,
                Result = Domain.Enums.Enums.Result.Failed
            };
            var result = new ConflictObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
        public async static void PeriodEnded(HttpContext context)
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.PeriodEnded,
                ErrorMessageEn = ErrorMessagesEn.PeriodEnded,
                Result = Domain.Enums.Enums.Result.periodEnded
            };
            var result = new OkObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
        public async static void UserIsLocked(HttpContext context)
        {
            var obj = new ResponseResult()
            {
                ErrorMessageAr = ErrorMessagesAr.userIsLocked,
                ErrorMessageEn = ErrorMessagesEn.userIsLocked,
                Result = Domain.Enums.Enums.Result.userLocked
            };
            var result = new ConflictObjectResult(obj);
            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = context
            });
            return;
        }
    }
}
