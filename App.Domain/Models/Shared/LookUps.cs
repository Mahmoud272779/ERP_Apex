namespace App.Domain.Models.Shared
{
    public class LookUps
    {
        public int? ID { get; set; }
        public string Code { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
    }
    public class SupplierLookUp
    {
        public int? ID { get; set; }
        public string Code { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int? SupplierID { get; set; }
    }
}
