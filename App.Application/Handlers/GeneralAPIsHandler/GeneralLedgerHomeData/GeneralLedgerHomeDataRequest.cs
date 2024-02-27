using DocumentFormat.OpenXml.Office2013.Excel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GeneralLedgerHomeData
{
    public class GeneralLedgerHomeDataRequest : IRequest<ResponseResult>
    {
        public DateTime? dateFrom { get; set; }
        public DateTime? dateTo { get; set; }
    }
}
