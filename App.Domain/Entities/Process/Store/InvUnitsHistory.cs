using App.Domain.Common;
using App.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvUnitsHistory : HistoryParameter
    {
        public int Id { get; set; }
        [ForeignKey("employeesId")]
        public int employeesId { get; set; } = 1;
        public InvEmployees employees { get; set; }
    }
}
