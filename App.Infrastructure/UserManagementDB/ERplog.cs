using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class ERplog
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string ApiPath { get; set; }
        public DateTime Date { get; set; }
    }
}
