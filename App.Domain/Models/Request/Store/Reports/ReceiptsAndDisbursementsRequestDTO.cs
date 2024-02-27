using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.Store.Reports
{
    public class PaymentsAndDisbursementsRequestDTO : GeneralPageSizeParameter
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int authorityTypes { get; set; }
        
        public string? branches { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        [Required]
        public int benefitId { get; set; }
    }
}
