using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store
{
    public class ExportPersonModel
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }

        public string? LatinName { get; set; }
        public int Type { get; set; }
    }
}
