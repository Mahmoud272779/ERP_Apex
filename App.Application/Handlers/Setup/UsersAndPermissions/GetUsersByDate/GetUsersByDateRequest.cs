using App.Domain.Models.Security.Authentication.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate
{
    public class GetUsersByDateRequest :GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public DateTime date { get; set; }

    }
}
