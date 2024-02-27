using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvCommissionList 
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Type { get; set; }
        public double? Ratio { get; set; }
        public double? Target { get; set; }
        public bool CanDelete { get; set; }
       
        public int BranchId { get; set; }

        public virtual ICollection<InvCommissionSlides> Slides { get; set; }
        public virtual ICollection<InvSalesMan> SalesMan { get; set; }

    }
}
