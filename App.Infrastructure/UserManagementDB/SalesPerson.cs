using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class SalesPerson
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public string CreatedUserId { get; set; }
        public string ModifiedUserId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
