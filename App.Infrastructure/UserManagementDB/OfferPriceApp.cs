using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class OfferPriceApp
    {
        public int Id { get; set; }
        public int OfferPriceId { get; set; }
        public int AppId { get; set; }

        public virtual App App { get; set; }
        public virtual OfferPrice OfferPrice { get; set; }
    }
}
