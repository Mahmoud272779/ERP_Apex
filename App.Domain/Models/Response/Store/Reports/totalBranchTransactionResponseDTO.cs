using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports
{
    public class totalBranchTransactionResponseDTO
    {

        public totalBranchTransactionResponseDTO_Sales totalBranchTransactionResponseDTO_Sales { get; set; }
        public totalBranchTransactionResponseDTO_purchases totalBranchTransactionResponseDTO_purchases { get; set; }

        public List<totalSafesAndBanksTransaction> banksAndSafes { get; set; }

        public totalSafesAndBanksTransaction_Totals BanksSafes_Totals { get; set; }

    }
    public class totalBranchTransactionResponseDTO_Sales
    {
        public List<totalBranchTransaction> totalBranchTransaction { get; set; }
        public totalBranchTransaction_Totals totalBranchTransaction_Totals { get; set; }
    }
    public class totalBranchTransactionResponseDTO_purchases
    {
        public List<totalBranchTransaction> totalBranchTransaction { get; set; }
        public totalBranchTransaction_Totals totalBranchTransaction_Totals { get; set; }
    }

    public class totalBranchTransaction_Totals
    {
        public double totalRemind { get; set; }
        public double totalPartial { get; set; }
        public double totalPaid { get; set; }
        public double totalDiscount { get; set; }
        public double total { get; set; }
        public double vat { get; set; }
        public double net { get; set; }
        public int GroupId { get; set; }
    }
    public class totalSafesAndBanksTransaction_Totals
    {
        public double expenses { get; set; }
        public double payments { get; set; }
    }
    public class totalBranchTransaction_branchesDetails
    {
        public int id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public double totalRemind { get; set; }
        public double totalPartial { get; set; }
        public double totalPaid { get; set; }
        public double totalDiscount { get; set; }
        public double total { get; set; }
        public double vat { get; set; }
        public double net { get; set; }
        public string className { get; set; }
        public int eleType { get; set; } //1 for parent 2 for child
        //For Print
        public int GroupId { get; set; }
}
public class totalBranchTransaction
{
    public int id { get; set; }
    public string arabicName { get; set; }
    public string latinName { get; set; }
    public double totalRemind { get; set; }
    public double totalPartial { get; set; }
    public double totalPaid { get; set; }
    public double totalDiscount { get; set; }
    public double total { get; set; }
    public double vat { get; set; }
    public double net { get; set; }
    public bool isExpanded { get; set; } = false;
    public string className { get; set; }
    public int paymentType { get; set; }
    public int eleType { get; set; } = 1;// 1 for parent 2 for child
        //For Print
        public int GroupId { get; set; }
        public int invoiceTypeId { get; set; }
        public List<totalBranchTransaction_branchesDetails> branchesDetails { get; set; }
}
public class totalSafesAndBanksTransaction_branchesDetails
{
    public int id { get; set; }
    public string arabicName { get; set; }
    public string latinName { get; set; }
    public double expenses { get; set; }
    public double payments { get; set; }

        //for print
        public int GroupId { get; set; }
}


public class totalSafesAndBanksTransaction
{
        public int id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public double expenses { get; set; }
        public double payments { get; set; }
        public int branchId { get; set; }
        public bool isExpanded { get; set; } = false;
        //for print 
        public int Level { get; set; }
        public int GroupId { get; set; }
        public List<totalSafesAndBanksTransaction_branchesDetails> branchesDetails { get; set; }
}
public class totalSafesAndBanksTransaction_Banks : totalSafesAndBanksTransaction
{

}
public class totalSafesAndBanksTransaction_Safes : totalSafesAndBanksTransaction
{

}
public class totalSafesAndBanksTransaction_res
{
    public totalSafesAndBanksTransaction_Banks banks { get; set; }
    public totalSafesAndBanksTransaction_Safes safes { get; set; }
}
public class totalBranchTransactionResponse
{
    public totalBranchTransactionResponseDTO data { get; set; }
    public int dataCount { get; set; }
    public int TotalCount { get; set; }
    public string notes { get; set; }
    public Result Result { get; set; }
}
 public class SalesPurchasesTotals 
    {
      public double  SalesPurchasesTotalRemind { get; set; }

        public double SalesPurchasesTotalPartial { get; set; }

        public double SalesPurchasesTotalPaid { get; set; }
        public double SalesPurchasesTotalDiscount { get; set; }
        public double SalesPurchasesTotal { get; set; }
        public double SalesPurchasesVat { get; set; }
        public double SalesPurchasesNet { get; set; }

        
    }


}
