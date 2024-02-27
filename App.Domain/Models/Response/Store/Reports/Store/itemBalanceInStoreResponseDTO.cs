using App.Domain.Models.Setup.ItemCard.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Store
{
    public class itemBalanceInStoreResponseDTO : InvItemCardDTO
    {
        public int itemId { get; set; }
        public string itemCode { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int unitId { get; set; }
        public string unitArabicName { get; set; }
        public string unitLatinName { get; set; }
        public double balance { get; set; }
        
    }
    public class itemBalanceInStoreResponse
    {
        public List<itemBalanceInStoreResponseDTO> data { get; set; }
        public Result Result { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
        public string notes { get; set; }
    }
    public class Count
    {
        public int ItemId { get; set; }
    }
}
