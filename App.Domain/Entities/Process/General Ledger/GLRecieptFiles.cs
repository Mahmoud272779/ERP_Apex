using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLRecieptFiles
    {
        public int Id { get; set; }
        public int RecieptId { get; set; }
        public string FileLink { get; set; }
        public string FileName { get; set; }
        public string FileExtensions { get; set; }
        public virtual GlReciepts reciepts { get; set; }
    }
}
