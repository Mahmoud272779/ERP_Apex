using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.General
{
    public class ItemsDropdownlistReport
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int itemType { get; set; }
        public int Status { get; set; }
        public List<UnitsForReports> units { get; set; }

    }
    public class UnitsForReports
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public bool isDefult { get; set; }
    }
    public class ItemsDropdownlistInvoice
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int itemType { get; set; }
        public int Status { get; set; }

    }
}
