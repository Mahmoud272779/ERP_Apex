using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class BroadCastCompany
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int BroadCastMasterId { get; set; }

        public virtual BroadCastMaster BroadCastMaster { get; set; }
        public virtual UserApplication Company { get; set; }
    }
}
