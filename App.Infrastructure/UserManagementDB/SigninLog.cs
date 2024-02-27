using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class SigninLog
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime SigninTime { get; set; }
        public bool? IsLogout { get; set; }
        public DateTime? LogoutTime { get; set; }
        public int UserId { get; set; }

        public virtual Account User { get; set; }
    }
}
