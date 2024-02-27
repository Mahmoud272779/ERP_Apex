using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.GeneralLedger
{
    public class GLIntegrationSettingsDTO
    {
        public int linkingMethodId { get; set; }
        public int screenId { get; set; }
        public int GLFinancialAccountId { get; set; }
        public int? GLBranchId { get; set; }
    }
}
