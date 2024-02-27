using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class Counter
    {
        public string Key { get; set; }
        public int Value { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
