using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class SizesParameter
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }
    public class UpdateSizesParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }

   
    public class SizesSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }
}
