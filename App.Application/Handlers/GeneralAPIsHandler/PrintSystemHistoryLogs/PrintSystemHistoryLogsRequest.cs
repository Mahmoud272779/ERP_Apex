using App.Application.Handlers.GeneralAPIsHandler.GetSystemHistoryLogs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.PrintSystemHistoryLogs
{
    public class PrintSystemHistoryLogsRequest:  IRequest< WebReport>
    {
     
        public int empId { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        public exportType exportType { get; set; }
        public bool isArabic { get; set; }
        public int fileId { get; set; } = 0;
    }
}
