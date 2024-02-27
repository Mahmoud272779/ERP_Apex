using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using App.Domain.Models.Security.Authentication.Response;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class FundsBankSafeRequest
    {
        public FundsBankSafeRequest()
        {
            FundsDetails_B_S = new List<FundBankSafeDetails>();
        }

        public DateTime DocDate { get; set; }
        public int SafeBankId { get; set; }
        public string Notes { get; set; }
        public bool IsBank { get; set; }
        //public bool IsSafe { get; set; }
        //   public int BranchId { get; set; }

        public List<FundBankSafeDetails> FundsDetails_B_S { get; set; }
    }

    public class UpdateFundsBankSafeRequest
    {
        public UpdateFundsBankSafeRequest()
        {
            FundsDetails_B_S = new List<FundBankSafeDetails>();
        }
        public int DocumentId { get; set; }
        //public int Code { get; set; }
        public DateTime DocDate { get; set; }
        public int SafeBankId { get; set; }
        public string Notes { get; set; }
        public bool IsBank { get; set; }
        //public bool IsSafe { get; set; }
        //   public int BranchId { get; set; }

        public List<FundBankSafeDetails> FundsDetails_B_S { get; set; }
    }


    public class FundBankSafeDetails
    {
        public int paymentMethod { get; set; }
        public double? Debtor { get; set; }
        public double? Creditor { get; set; }
        //public string? ChequeNumber { get; set; }
    }
    public class Delete
    {
        public int[] Ids { get; set; }
    }
    

    public class fundsSearch : GeneralPageSizeParameter
    {
        
        public int? id { get; set; }
        [Required]
        public bool IsBank { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public string? SafeOrBankList { get; set; }
        public string? searchCriteria { get; set; }

    }
}
