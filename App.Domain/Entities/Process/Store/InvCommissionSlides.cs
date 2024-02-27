using App.Domain.Common;
using System.Collections.Generic;

namespace App.Domain.Entities.Process
{
   public class InvCommissionSlides 
    {
        public int Id { get; set; }
        public int CommissionId { get; set; }
        public int SlideNo { get; set; }
        public double SlideFrom { get; set; }
        public double SlideTo { get; set; }
        public double Ratio { get; set; }
        public double Value { get; set; }
        public virtual  InvCommissionList CommList { get; set; }
    }
}
