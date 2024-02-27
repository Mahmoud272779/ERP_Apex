using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class OffPriceHistory
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public int UserId { get; set; }
        public int OfferPriceId { get; set; }

        public virtual OfferPrice OfferPrice { get; set; }
        public virtual Account User { get; set; }
    }
}
