using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class itemsNotSoldResponse
    {
        public Result Result { get; set; }
        public string notes { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
        public List<itemsNotSoldResponseList> itemsNotSoldResponseLists { get; set; }
    }
    public class itemsNotSoldResponseList
    {
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string catNameAr { get; set; }
        public string catNameEn { get; set; }
    }
}
