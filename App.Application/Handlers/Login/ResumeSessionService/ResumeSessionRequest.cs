using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Login.ResumeSessionService
{
    public class ResumeSessionRequest : IRequest<ResponseResult>
    {
        public string userPassword   { get; set; }
    }
}
