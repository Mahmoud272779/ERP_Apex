using App.Domain.Models.Security.Authentication.Request.Reports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Purchases.AddPurchases
{
    public class AddPurchases_WOVRequest : InvoiceMasterRequest, IRequest<ResponseResult>
    {

    }
}
