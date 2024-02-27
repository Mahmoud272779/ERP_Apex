using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class EmailSetting
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }
}
