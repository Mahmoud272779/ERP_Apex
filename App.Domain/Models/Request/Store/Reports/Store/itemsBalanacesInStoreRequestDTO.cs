using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class itemsBalanacesInStoreRequestDTO : GeneralPageSizeParameter
    {
        [Required]
        public int storeId { get; set; }
        public int? itemId { get; set; }
        public int? catId { get; set; }
        public ItemTypes? itemTypes { get; set; }
    }
}
