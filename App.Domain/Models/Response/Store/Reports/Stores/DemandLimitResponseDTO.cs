using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Stores
{
    public class DemandLimitResponseDTO
    {
        public int itemId { get; set; }
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public double DemandLimitNum { get; set; }
        public double balance { get; set; }
        public double RequiredLimitBalance { get; set; }
        public string unitNameAr { get; set; }
        public string unitNameEn { get; set; }
    }


    public class DemandLimitResponse
    {
        public List<DemandLimitResponseDTO> data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
    }
}
