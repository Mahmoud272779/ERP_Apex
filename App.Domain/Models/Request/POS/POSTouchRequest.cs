using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.POS
{
    public class POSTouchRequest
    {
        public double fontSize { get; set; }
        public double categoryImageWidth { get; set; }
        public double categoryImageHeight { get; set; }
        public bool displayItemPrice { get; set; }
        public double itemsImageWidth { get; set; }
        public double itemsImageHeight { get; set; }
    }
}
