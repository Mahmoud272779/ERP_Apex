using App.Domain.Enums;
using App.Domain.Models.Security.Authentication.Response;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.Store.Reports.Store
{
    public class ReviewWarehouseTransfersRequest : PaginationVM
    {
        [Required]
        public int storeFrom { get; set; }
        [Required]
        public int storeTo { get; set; }
        public TransferStatusEnum status { get; set; }
        public DateTime from { get; set; }
        public DateTime to { get; set; }
    }
}
