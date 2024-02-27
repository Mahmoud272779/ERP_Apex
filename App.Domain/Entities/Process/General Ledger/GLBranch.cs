using App.Domain.Common;
using App.Domain.Entities.POS;
using App.Domain.Entities.Process.General;
using App.Domain.Entities.Process.General_Ledger;
using App.Domain.Entities.Process.Store;
using App.Domain.Entities.Process.Store.EInvoice;
using System.Collections.Generic;
using static App.Domain.Enums.Enums;

namespace App.Domain.Entities.Process
{
    public class GLBranch
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string LatinName { get; set; } = "";
        public string ArabicName { get; set; } = "";
        public string AddressEn { get; set; } = "";
        public string AddressAr { get; set; } = "";
        public string Fax { get; set; } = "";
        public string Phone { get; set; } = "";
        public int Status { get; set; }
        public string Notes { get; set; } = "";
        public int? ManagerId { get; set; }
        public string ManagerPhone { get; set; } = "";
        public string BrowserName { get; set; } = "";
        public bool CanDelete { get; set; }
        public DateTime UTime { get; set; }
        public int? SalesPriceId { get; set; } = (int)SalePricesList.SalePrice1;
        public virtual ICollection<InvStpStores> Stores { get; set; }

        //public ICollection<InvEmployees> Employees { get; set; }
        public ICollection<InvPersons_Branches> PersonBranch { get; set; }

        public  ICollection<InvSalesMan_Branches> SalesManBranch { get; set; }
        public ICollection<GLFinancialAccountBranch> FinancialCosts { get; set; }
        public ICollection<GLSafe> Treasuries { get; set; }
        //public IReadOnlyCollection<GLBanks> Banks { get; set; }
        public ICollection<GLBankBranch> BankBranches { get; set; }
        public virtual ICollection<InvoiceMaster> InvoiceMaster { get; set; }
        public virtual ICollection<OfferPriceMaster> OfferPriceMaster { get; set; }
        public virtual ICollection<POSInvoiceSuspension> POSInvoiceSuspension { get; set; }
        public virtual ICollection<InvEmployeeBranch> EmployeeBranches { get; set; }
        public virtual ICollection<InvStoreBranch> StoreBranches { get; set; }
        public virtual ICollection<userBranches> userBranches { get; set; }
        public virtual ICollection<SystemHistoryLogs> SystemHistoryLogs { get; set; }
        public virtual ICollection<GLPurchasesAndSalesSettings> GLPurchasesAndSalesSettings { get; set; }
        public ICollection<GLIntegrationSettings> gLIntegrationSettings { get; set; }
        public ICollection<GLPrinter> gLPrinter { get; set; }
        public ICollection<InvFundsCustomerSupplier> FundsCustomerSupplier { get; set; }
        public ICollection<CSID> CSID { get; set; }


    }
}
