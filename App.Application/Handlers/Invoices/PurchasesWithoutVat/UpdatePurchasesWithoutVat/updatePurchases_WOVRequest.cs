using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers
{
    public class updatePurchases_WOVRequest : UpdateInvoiceMasterRequest,IRequest<ResponseResult>
    {
    }
}
