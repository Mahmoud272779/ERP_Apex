using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
   public class InvFundsBanksSafesDetails
    {
        public int Id { get; set; }
        //public string? ChequeNumber { get; set; }
        public int DocumentId { get; set; }
        public int PaymentId { get; set; }
        public double  Debtor { get; set; }
        public double Creditor { get; set; }
        public InvFundsBanksSafesMaster FundsMaster_B_S { get; set; }
    }
}
