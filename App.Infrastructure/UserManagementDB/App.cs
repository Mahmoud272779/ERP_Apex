using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class App
    {
        public App()
        {
            OfferPriceApps = new HashSet<OfferPriceApp>();
            UserApplicationApps = new HashSet<UserApplicationApp>();
            AdditionalPrices = new HashSet<AdditionalPrice>();
            AppChildren = new HashSet<App>();
            AppParents = new HashSet<App>();
        }

        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public double YealryPrice { get; set; }
        public string Note { get; set; }
        public double MonthlyPrice { get; set; }

        public virtual ICollection<OfferPriceApp> OfferPriceApps { get; set; }
        public virtual ICollection<UserApplicationApp> UserApplicationApps { get; set; }

        public virtual ICollection<AdditionalPrice> AdditionalPrices { get; set; }
        public virtual ICollection<App> AppChildren { get; set; }
        public virtual ICollection<App> AppParents { get; set; }
    }
}
