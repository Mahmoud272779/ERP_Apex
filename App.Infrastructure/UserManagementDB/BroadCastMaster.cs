using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class BroadCastMaster
    {
        public BroadCastMaster()
        {
            BroadCastCompanies = new HashSet<BroadCastCompany>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CDate { get; set; }
        public int UserId { get; set; }
        public DateTime PushTime { get; set; }
        public bool? ForAll { get; set; }

        public virtual Account User { get; set; }
        public virtual ICollection<BroadCastCompany> BroadCastCompanies { get; set; }
    }
}
