using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{

    public class CommissionListRequest
    {
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Type { get; set; }
        public double? Ratio { get; set; } = 0;
        public double? Target { get; set; } = 0;
        public List<CommissionSlidesRequest> Slides { get; set; }
    }

    public class CommissionSlidesRequest
    {
        public double SlideFrom { get; set; }
        public double SlideTo { get; set; }
        public double Ratio { get; set; }
        public double Value { get; set; }
        public bool isSetUserRatio { get; set; }

    }

    public class UpdateCommissionListRequest
    {
         
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Type { get; set; }
        public double? Ratio { get; set; }
        public double? Target { get; set; }
        public List<CommissionSlidesRequest> Slides { get; set; }

    }

    public class UpdateActiveCommissionList
    {
        public List<int> CommissionList { get; set; }
        public int Status { get; set; }
    }
    public class CommissionListSearch : GeneralPageSizeParameter
    {
        public string Name { get; set; }
        
    }
    

}
