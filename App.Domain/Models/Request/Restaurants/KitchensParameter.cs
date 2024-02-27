using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Request.Restaurants
{
    public class KitchensParameter
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public DateTime UTime { get; set; }
    }
    public class UpdateKitchensParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public DateTime UTime { get; set; }

    }


    public class KitchensSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }
}
