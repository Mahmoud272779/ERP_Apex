using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class SubReqPeriod
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int SubReqId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool? Seen { get; set; }

        public virtual UserApplication Company { get; set; }
        public virtual UserApplicationCash SubReq { get; set; }
    }
}
