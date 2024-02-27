using App.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process
{
    public class GLGeneralSetting : AuditableEntity
    {
        public int Id { get; set; }
        public bool isFundsClosed { get; set; }
        public bool AutomaticCoding { get; set; }
        public int? MainCode { get; set; }
        public int? SubCode { get; set; }
        public int evaluationMethodOfEndOfPeriodStockType { get; set; }
        public int DefultAccCustomer { get; set; }
        public int DefultAccEmployee{ get; set; }
        public int DefultAccSalesMan { get; set; }
        public int DefultAccOtherAuthorities { get; set; }
        public int DefultAccBank { get; set; }
        public int DefultAccSafe { get; set; }
        public int DefultAccSupplier { get; set; }
        public int  FinancialAccountIdCustomer { get; set; }
        public int  FinancialAccountIdEmployee { get; set; }
        public int  FinancialAccountIdSalesMan { get; set; }
        public int  FinancialAccountIdOtherAuthorities { get; set; }
        public int  FinancialAccountIdBank { get; set; }
        public int  FinancialAccountIdSafe { get; set; }
        public int FinancialAccountIdSupplier { get; set; }
        public bool AddDiscountInJournalEntryForInvoics { get; set; } = true;
        public ICollection<SubCodeLevels> subCodeLevels { get; set; }

    }
    public class SubCodeLevels
    {
        public int Id { get; set; }
        public int value { get; set; }
        public int GLGeneralSettingId { get; set; }
    }
}
