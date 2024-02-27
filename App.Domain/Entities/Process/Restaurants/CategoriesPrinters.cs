using App.Domain.Entities.Process.General_Ledger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Restaurants
{
    public class CategoriesPrinters
    {
        public int CategoryId { get; set; }
        public int PrinterId { get; set;}
        public InvCategories Category { get; set; }
        public GLPrinter Printer { get; set; }
    }
}
