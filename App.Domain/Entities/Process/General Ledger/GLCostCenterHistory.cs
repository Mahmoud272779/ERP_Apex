using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLCostCenterHistory : GeneralProperities
    {
        public int CostCenterId { get; set; }
        public double InitialBalance { get; set; }
        public int CC_Nature { get; set; }
        public string Notes { get; set; }
        public int? ParentId { get; set; }
        public string LastAction { get; set; }
        public DateTime LastDate { get; set; }
        public string BrowserName { get; set; }
        [ForeignKey("employeesId")] public int employeesId { get; set; } = 1;
        public InvEmployees employees { get; set; }
    }
}
