using App.Domain.Entities.Process.Restaurants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.General_Ledger
{
    public class GLPrinter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string LatinName { get; set; } = "";
        public string ArabicName { get; set; } = "";
        public string IP { get; set; } = "";
        public int BranchId { get; set; } = 0;
        public string Notes { get; set; } = "";
        public int Status { get; set; } = 1;
        public bool CanDelete { get; set; } = true;
        public DateTime UTime { get; set; }
        public GLBranch Branchs { get; set; }
        public ICollection<CategoriesPrinters> rstCategoriesPrinters { get; set; }

    }
}
