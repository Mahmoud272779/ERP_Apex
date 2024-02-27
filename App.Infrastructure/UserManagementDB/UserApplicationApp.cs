using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class UserApplicationApp
    {
        public int Id { get; set; }
        public int ReqId { get; set; }
        public int? AppId { get; set; }

        public virtual App App { get; set; }
        public virtual UserApplicationCash Req { get; set; }
    }
}
