using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.MainData
{
    public class itemsPricesRequestDTO : GeneralPageSizeParameter
    {
        public int? itemId { get; set; }
        public int? catId { get; set; }
        public int? statues { get; set; }
        public ItemTypes? itemTypes { get; set; }

    }
}
