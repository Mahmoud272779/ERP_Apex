using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Posversion
    {
        public int Id { get; set; }
        public string AppVersion { get; set; }
        public string AppLink { get; set; }
        public bool MustUpdate { get; set; }
        public string Notes { get; set; }
    }
}
