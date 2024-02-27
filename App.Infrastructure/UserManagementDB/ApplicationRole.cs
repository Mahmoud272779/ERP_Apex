using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class ApplicationRole
    {
        public ApplicationRole()
        {
            AdministrationRoleDetails = new HashSet<AdministrationRoleDetail>();
            ApplicationUsers = new HashSet<ApplicationUser>();
        }

        public string Id { get; set; }
        public string ArabicName { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual ICollection<AdministrationRoleDetail> AdministrationRoleDetails { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
