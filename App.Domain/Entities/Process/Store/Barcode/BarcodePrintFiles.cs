using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store.Barcode
{
    public class BarcodePrintFiles
    {
        public int Id { get; set; }
        public string ArabicName { get; set; }
        public string LatineName { get; set; }
        public byte[] File { get; set; }
        public bool IsDefault { get; set; }
    }
}
