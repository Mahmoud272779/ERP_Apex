namespace App.Domain.Models.Security.Authentication.Request
{
    public class PurchasesAdditionalCostsParameter
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int AdditionalType { get; set; }
    }
    public class UpdatePurchasesAdditionalCostsParameter
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int AdditionalType { get; set; }
    }
   
}
