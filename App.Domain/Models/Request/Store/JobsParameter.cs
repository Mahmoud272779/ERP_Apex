using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class JobsParameter
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }
    public class UpdateJobsParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
    }

    

    public class JobsSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        public int Status { get; set; }
    }
   
}
