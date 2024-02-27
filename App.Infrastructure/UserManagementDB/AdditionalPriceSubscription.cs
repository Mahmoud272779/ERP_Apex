using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class AdditionalPriceSubscription
    {
        public int AdditionalPriceId { get; set; }
        public int SubRequestId { get; set; }
        public int Count { get; set; }

        public virtual AdditionalPrice AdditionalPrice { get; set; }
        public virtual UserApplicationCash SubRequest { get; set; }
    }
}
