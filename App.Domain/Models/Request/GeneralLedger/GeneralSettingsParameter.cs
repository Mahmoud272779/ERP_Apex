using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using App.Domain.Entities.Process;
using Microsoft.AspNetCore.Mvc;
namespace App.Domain.Models.Security.Authentication.Request
{
   public class GeneralSettingsParameter
    {
        public bool AutomaticCoding { get; set; }
        public int? MainCode { get; set; }
        public int? SubCode { get; set; }
    }

    public class UpdateGeneralSettingsParameter
    {
        [JsonIgnore]
        public int Id { get; set; }
        public bool isFundsClosed { get; set; }
        public bool isAutoCoding { get; set; }
        public int evaluationMethodOfEndOfPeriodStockType { get; set; }
        public int[] codingLevels { get; set; }
        public bool? isLevelsChanges { get; set; }
        public bool isAutoRecode { get; set; }
        public bool AddDiscountInJournalEntryForInvoics { get; set; }

    }
    public class codingLevels
    {
        public int value { get; set; }
    }

    public class AllAuthoritiesParameter
    {
        public CustomersAcc CustomersAcc { get; set; }
        public SuppliersAcc SuppliersAcc { get; set; }
        public SafesAcc SafesAcc { get; set; }
        public BanksAcc BanksAcc { get; set; }
        public OtherAuthoritiesAcc OtherAuthoritiesAcc { get; set; }
        public SalesManAcc SalesManAcc { get; set; }
        //public SalesManAcc SalesManAcc { get; set; } = new SalesManAcc();
    }
    public class listParameter
    {
        public int FinancialAccountId { get; set; }
        public int Id { get; set; }
    }
    public class CustomersAcc: AcoutntAssign
    {
        public List<listParameter> lstCustomers { get; set; }=new List<listParameter>();

    }
    public class SuppliersAcc : AcoutntAssign
    {
        public List<listParameter> lstSuppliers { get; set; }= new List<listParameter>();   
    }
    public class SafesAcc : AcoutntAssign
    {
        public List<listParameter> lstSafes { get; set; } = new List<listParameter>();
    }
    public class BanksAcc : AcoutntAssign
    {
        public List<listParameter> lstBanks { get; set; } = new List<listParameter>();
    }
    public class OtherAuthoritiesAcc : AcoutntAssign
    {
        public List<listParameter> lstOtherAuthorities { get; set; } = new List<listParameter>();
    }
    public class SalesManAcc : AcoutntAssign
    {
        public List<listParameter> lstSalesMan { get; set; } = new List<listParameter>();
    }

    public class AcoutntAssign
    {
        public bool useDefultAcc{ get; set; }
        public bool useThisAcount { get; set; }
        public bool useUnderThisAcount{ get; set; }
        public int  financialAccId { get; set; }
    }

    public class GLsettingInvoicesParameter
    {
        public List<GLsettingInvoicesData> invoicesSettings { get; set; }  =new List<GLsettingInvoicesData>();
    }
    public class GLsettingInvoicesData
    {
        public int receiptType { get; set; }
        public int ReceiptElemntID { get; set; }
        public int? financialAccountId { get; set; }
    }
    public class updateFinancialAccountRelationSettings
    {
        public int linkingMethodId { get; set; }
        public int financialAccountId { get; set; }
    }
    public class getFinancialAccountRelationResponse
    {
        public int linkingMethodId { get; set; }
        public object financialAccount { get; set; }
    }
    public class getFinancialAccountRelationRequest
    {
        public int entryScreenSettings { get; set; }
        //public int pageNumber { get; set; } = 1;
        //public int pageSize { get; set; } = 100;
    }

}
