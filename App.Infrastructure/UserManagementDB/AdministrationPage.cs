using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class AdministrationPage
    {
        public AdministrationPage()
        {
            AdministrationRoleDetails = new HashSet<AdministrationRoleDetail>();
        }

        public int PageId { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string Url { get; set; }
        public string LevelCode { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<AdministrationRoleDetail> AdministrationRoleDetails { get; set; }
    }
}
