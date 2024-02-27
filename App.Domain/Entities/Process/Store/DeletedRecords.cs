using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.Store
{
    public class DeletedRecords
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public DateTime DTime { get; set; }
        public int RecordID { get; set; }
    }
}
