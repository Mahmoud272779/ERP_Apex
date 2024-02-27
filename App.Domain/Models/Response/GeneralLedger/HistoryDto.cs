using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class HistoryDto : HistoryParameter
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public string userNameAr { get; set; }
        public string userNameEn { get; set; }
        public string TransactionTypeAr { get; set; }
        public string TransactionTypeEn { get; set; }
        public string TransactionType { get; set; }
        public string BrowserName { get; set; }
    }
    public class HistoryResponceDto
    {
        public DateTime Date { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string TransactionTypeAr { get; set; }
        public string TransactionTypeEn { get; set; }
        
        public string BrowserName { get; set; }
    }


}
