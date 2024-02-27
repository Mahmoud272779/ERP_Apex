using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers 
{
    public class UpdateAdditionStatusRequest : SharedRequestDTOs.UpdateStatus, IRequest<ResponseResult>
    {
    }
}
