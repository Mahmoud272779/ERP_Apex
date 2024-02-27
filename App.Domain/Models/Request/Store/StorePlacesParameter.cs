using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class StorePlacesParameter
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }



    public class UpdateStorePlacesParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }


    public class StorePlacesSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }
}
