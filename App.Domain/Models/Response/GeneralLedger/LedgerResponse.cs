using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.GeneralLedger
{
    public class LedgerResponse
    {
        public int financialAccountId { get; set; }
        public string financialAccountCode { get; set; }
        public string financialAccountNameAr { get; set; }
        public string financialAccountNameEn { get; set; }
        public int journalEntryId { get; set; }
        public DateTime journalEntryDate { get; set; }
        public string Date { get; set; }
        public string notes { get; set; }
        public string notesEn { get; set; }
        public double PreviousBalance { get; set; }
        public double transCredit { get; set; }
        public double transDebit { get; set; }
        public double balanceCredit { get; set; }
        public double balanceDebit { get; set; }

        //for print
        public string NoDataGroup { get; set; }
        public string PreviousBalanceString { get; set; }

        public int GroupId { get; set; }
    }
  public class FinalLedgerResponse
    {
        public string  FinancialAccountNameAr { get; set; }
        public string  FinancialAccountNameEn { get; set; }
        public double TotalTransCredit { get; set; }
        public double totalTransDebit { get; set; }
        public double totalBalanceCredit { get; set; }
        public double totalBalanceDebit { get; set; }

        //for acount Details report ---- print 
        public string PreviousBalanceString { get; set; }
        public int Count { get; set; } = 1;
       


        public List<LedgerResponse> FinancialAccountList { get; set; }

    }
}
