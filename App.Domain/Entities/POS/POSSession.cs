using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.POS
{
    public class POSSession
    {
        public int Id { get; set; }
        public int sessionCode { get; set; }
        public int employeeId { get; set; }
        public DateTime start { get; set; }
        public DateTime? end { get; set; }
        public int? sessionClosedById { get; set; }
        public int sessionStatus { get; set; }
        public InvEmployees employee { get; set; }
        public InvEmployees employeeCloseSeassion { get; set; }
        public ICollection<POSSessionHistory>  pOSSessionHistories{ get; set; }
    }
}
