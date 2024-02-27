using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class AdditionalPrice
    {
        public AdditionalPrice()
        {
            AdditionalPriceSubscriptions = new HashSet<AdditionalPriceSubscription>();
            OfferPriceAdditionalItems = new HashSet<OfferPriceAdditionalItem>();
            Apps = new HashSet<App>();
        }

        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double YearPrice { get; set; }
        public bool SpecialApps { get; set; }
        public bool Active { get; set; }
        public double MonthPrice { get; set; }

        public virtual ICollection<AdditionalPriceSubscription> AdditionalPriceSubscriptions { get; set; }
        public virtual ICollection<OfferPriceAdditionalItem> OfferPriceAdditionalItems { get; set; }

        public virtual ICollection<App> Apps { get; set; }
    }
}
