using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class ApplicationUser
    {
        public ApplicationUser()
        {
            Modules = new HashSet<Module>();
        }

        public string Id { get; set; }
        public bool? IsDefaultUser { get; set; }
        public int CompanyId { get; set; }
        public string RoleId { get; set; }
        public string UserToken { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }

        public virtual Company Company { get; set; }
        public virtual ApplicationRole Role { get; set; }

        public virtual ICollection<Module> Modules { get; set; }
    }
}
