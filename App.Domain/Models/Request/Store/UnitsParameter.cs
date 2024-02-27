using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class UnitsParameter
    {

        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public DateTime UTime { get; set; }

    }
    public class UpdateUnitsParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public DateTime UTime { get; set; }

    }


    public class UnitsSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }

}
