using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class OfferPrice
    {
        public OfferPrice()
        {
            OffPriceHistories = new HashSet<OffPriceHistory>();
            OfferPriceAdditionalItems = new HashSet<OfferPriceAdditionalItem>();
            OfferPriceApps = new HashSet<OfferPriceApp>();
        }

        public int Id { get; set; }
        public int Code { get; set; }
        public string CompanyName { get; set; }
        public string CompanyActive { get; set; }
        public string PersonName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public int BundleId { get; set; }
        public double Total { get; set; }
        public double Vat { get; set; }
        public double Net { get; set; }
        public DateTime CDate { get; set; }
        public string CancelReason { get; set; }
        public int? SalesmanId { get; set; }
        public int Statues { get; set; }
        public int Period { get; set; }
        public int PeriodType { get; set; }

        public virtual Bundle Bundle { get; set; }
        public virtual ICollection<OffPriceHistory> OffPriceHistories { get; set; }
        public virtual ICollection<OfferPriceAdditionalItem> OfferPriceAdditionalItems { get; set; }
        public virtual ICollection<OfferPriceApp> OfferPriceApps { get; set; }
    }
}
