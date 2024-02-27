using App.Domain.Models.Security.Authentication.Request.Reports;
using App.Domain.Models.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain
{
    public class AddIncommingTransferRequest : InvoiceMasterRequest, IRequest<ResponseResult>
    {

        public int OutgoingInoviceID { get; set; }
    }
}
