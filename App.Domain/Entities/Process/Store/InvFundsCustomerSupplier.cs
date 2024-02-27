using App.Domain.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class InvFundsCustomerSupplier
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public ICollection<InvPersons> Person { get; set; }
        public int branchId { get; set; }
        public GLBranch branch { get; set; }

        public string LastTransactionAction { get; set; }
        public string AddTransactionUser { get; set; } 
        public string AddTransactionDate { get; set; }
        public string LastTransactionUser { get; set; }
        public string LastTransactionDate { get; set; }
        public string DeleteTransactionUser { get; set; }
        public string DeleteTransactionDate { get; set; }
    }
}
