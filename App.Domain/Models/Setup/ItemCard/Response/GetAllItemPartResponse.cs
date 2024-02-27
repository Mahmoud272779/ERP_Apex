using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Setup.ItemCard.Response
{
    public class GetAllItemPartResponse
    {
        public int ItemId { get; set; }
        public int PartId { get; set; }
        public double Quantity { get; set; }
        public int UnitId { get; set; }
        public virtual PartVM PartDetails { get; set; }
        public virtual UnitVM Unit { get; set; }
    }
    public class PartVM
    {
        public int ItemId { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
    }
    public class UnitVM
    {
        public int UnitId { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
    }

}
