using App.Domain.Common;
using App.Domain.Models.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities.Process
{
   public class InvSizesHistory : HistoryParameter
    {
        public int Id { get; set; }
        [ForeignKey("employeesId")] public int employeesId { get; set; } = 1;
        public InvEmployees employees { get; set; }
    }
}
