using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Company
    {
        public Company()
        {
            ApplicationUsers = new HashSet<ApplicationUser>();
        }

        public int CompanyId { get; set; }
        public string Email { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string LatinAddress { get; set; }
        public string ArabicAddress { get; set; }
        public string LatinActivity { get; set; }
        public string ArabicActivity { get; set; }
        public string Fax { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public byte[] Image { get; set; }
        public int BundleId { get; set; }
        public int EmployeesCount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public bool IsTrial { get; set; }
        public string UniqueName { get; set; }
        public int Multiple { get; set; }

        public virtual Bundle Bundle { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }
}
