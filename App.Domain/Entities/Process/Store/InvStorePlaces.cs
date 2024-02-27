using App.Domain.Common;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
  public  class InvStorePlaces 
    {
        public int Id { get; set; }
        public int Code { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }//Represent the status of the store place 1 if active 2 if inactive
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public virtual ICollection<InvStpItemCardMaster> Items { get; set; }

    }
}
