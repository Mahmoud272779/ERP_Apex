using App.Domain.Models.Security.Authentication.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetBranchesForAllUsers
{
    public class GetBranchesForAllUsersRequest :GeneralPageSizeParameter, IRequest<ResponseResult>
    {
    }
}
