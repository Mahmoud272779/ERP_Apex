using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class PaymentMethodsRequest
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int? safeOrBankId { get; set; }
        public int Status { get; set; }

    }

    public class PaymentMethodsSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }

    public class UpdatePaymentMethodsRequest
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        //public int? BankId { get; set; }
        //public int? SafeId { get; set; }
        public int? safeOrBankId { get; set; }

        public int Status { get; set; }

    }

    public class PaymentMethodResponse
    {
        public int PaymentMethodId { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int? safeOrBankId { get; set; }
        public string safeOrBankNameAr { get; set; }
        public  string safeOrBankNameEn { get; set; }
        public int Status { get; set; }
        public bool CanDelete { get; set; } = false;

    }

}
