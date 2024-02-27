using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class OfferPriceAdditionalItem
    {
        public int Id { get; set; }
        public int OfferPriceId { get; set; }
        public int AdditionalItemId { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }

        public virtual AdditionalPrice AdditionalItem { get; set; }
        public virtual OfferPrice OfferPrice { get; set; }
    }
}
