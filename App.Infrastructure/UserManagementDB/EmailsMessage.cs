using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class EmailsMessage
    {
        public int Id { get; set; }
        public string ActivationSubject { get; set; }
        public string ActivationBody { get; set; }
    }
}
