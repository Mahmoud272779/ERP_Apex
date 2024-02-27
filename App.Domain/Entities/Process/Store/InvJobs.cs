using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvJobs 
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the job 1 if active 2 if inactive
        public string Notes { get; set; }
        public bool CanDelete { get; set; }       
        public ICollection<InvEmployees> Employees { get; set; }

    }
}
