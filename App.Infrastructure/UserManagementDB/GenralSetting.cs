using System;
using System.Collections.Generic;

namespace App.Infrastructure.UserManagementDB
{
    public partial class GenralSetting
    {
        public int Id { get; set; }
        public int NotificateClientForActiveSubscraptionInDays { get; set; }
        public bool? SendSmstoClientForActiveSubscraption { get; set; }
        public bool? SendEmailToClientForActiveSubscraption { get; set; }
        public bool? SendSmsforSalesManAfterActiveSubscrape { get; set; }
        public bool? SendEmailForSalesManAfterActiveSubscrape { get; set; }
        public bool? SendSmstoGenralManagerForActiveSubscraption { get; set; }
        public string GenralManagerEmail { get; set; }
        public bool? SendEmailToSalesManagerForActiveSubscraption { get; set; }
        public string SalesManagerEmail { get; set; }
        public bool? SendEmailToAccountantForActiveSubscraption { get; set; }
        public string AccountantEmail { get; set; }
        public bool? SendEmailToTechncalSupportForActiveSubscraption { get; set; }
        public string TechncalSupportEmail { get; set; }
        public string TrailAccountantEmail { get; set; }
        public string TrailGenralManagerEmail { get; set; }
        public string TrailSalesManagerEmail { get; set; }
        public bool? TrailSendEmailForSalesManAfterActiveSubscrape { get; set; }
        public bool? TrailSendEmailToAccountantForActiveSubscraption { get; set; }
        public bool? TrailSendEmailToSalesManagerForActiveSubscraption { get; set; }
        public bool? TrailSendEmailToTechncalSupportForActiveSubscraption { get; set; }
        public bool? TrailSendSmsforSalesManAfterActiveSubscrape { get; set; }
        public bool? TrailSendSmstoGenralManagerForActiveSubscraption { get; set; }
        public string TrailTechncalSupportEmail { get; set; }
        public int TrailNotificateClientForActiveSubscraptionInDays { get; set; }
        public bool? TrailSendEmailToClientForActiveSubscraption { get; set; }
        public bool? TrailSendSmstoClientForActiveSubscraption { get; set; }
        public string OfferPriceAccountantEmail { get; set; }
        public string OfferPriceGenralManagerEmail { get; set; }
        public string OfferPriceSalesManagerEmail { get; set; }
        public bool? OfferPriceSendEmailToAccountantForActiveSubscraption { get; set; }
        public bool? OfferPriceSendEmailToSalesManagerForActiveSubscraption { get; set; }
        public bool? OfferPriceSendEmailToTechncalSupportForActiveSubscraption { get; set; }
        public bool? OfferPriceSendSmstoGenralManagerForActiveSubscraption { get; set; }
        public string OfferPriceTechncalSupportEmail { get; set; }
    }
}
