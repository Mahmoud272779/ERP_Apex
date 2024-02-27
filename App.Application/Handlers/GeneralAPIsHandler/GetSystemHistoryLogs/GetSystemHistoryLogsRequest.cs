using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.GetSystemHistoryLogs
{
    public class GetSystemHistoryLogsRequest : IRequest<ResponseResult>
    {
        public int pageNumber { get; set; }
        public int pageSize{get;set;}
        public int empId {get;set;}
        public DateTime dateFrom {get;set;}
        public DateTime dateTo { get; set; }
        public bool isPrint { get; set; } = false;
    }
}
