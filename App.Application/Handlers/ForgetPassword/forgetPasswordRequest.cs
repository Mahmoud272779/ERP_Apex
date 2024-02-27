using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.ForgetPassword
{
    public class forgetPasswordRequest : IRequest<ResponseResult>
    {
        public string companyLogin { get; set; }
        public string email { get; set; }
    }
}
