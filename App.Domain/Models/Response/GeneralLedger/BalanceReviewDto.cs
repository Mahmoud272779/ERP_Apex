using App.Domain.Entities.Process;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{

    public class Totals
    {




        public class BalanceReviewSearchParameter
        {
            public int FinancialAcountId { get; set; }
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public int TypeId { get; set; }
        }
        public class BalanceReviewFilterData
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string FinancialAccountCode { get; set; }
            public string AutoCode { get; set; }
            public string LatinName { get; set; }
            public string ArabicName { get; set; }
            public bool hasChild { get; set; }
            public DateTime? FDate { get; set; }


            public List<GLJournalEntryDetails> journalDetails { get; set; }


        }
        public class BalanceReviewInsidePeriodDTO : BalanceReviewFilterData
        {
            public double InsidePeriodCreditor { get; set; }
            public double InsidePeriodDebtor { get; set; }
        }
        public class BalanceReviewPrevPeriodDTO : BalanceReviewFilterData
        {
            public double PrevPeriodCreditor { get; set; }
            public double PrevPeriodDebtor { get; set; }
        }
        public class BalanceReviewCollectingPeriodDTO : BalanceReviewFilterData
        {
            public double CollectingPeriodCreditor { get; set; }
            public double CollectingPeriodDebtor { get; set; }
            public double curencyFactor { get; set; }

        }
        public class FinalResponseResult
        {
            public List<finalData> FinalData { get; set; }
            public double TotalPrevCreditor { get; set; }
            public double TotalPrevDebtor { get; set; }
            public double TotalInsideCreditor { get; set; }
            public double TotalInsideDebtor { get; set; }
            public double TotalCollectingCreditor { get; set; }
            public double TotalCollectingDebtor { get; set; }
            public double TotalBalanceDebtor { get; set; }
            public double TotalBalanceCreditor { get; set; } 
        }
        public class finalData
        {
            public int Id { get; set; }
            public int? parentId { get; set; }
            public string autocode { get; set; }
            public string financialCode { get; set; }
            public double pervDeptor { get; set; }
            public double pervCreditor { get; set; }
            public double insideDebtor { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public double insidecreditor { get; set; }
            public double CollectingPeriodDebtor { get; set; }
            public double collectingPeriodcreditor { get; set; }
            public double balanceDebtor { get; set; }
            public double balanceCreditor { get; set; }
            public bool hasChild { get; set; }
            public int Level { get; set; }

        }
        public class TopLevelFinantialAccount
        {
            public int Id { get; set; } 
            public string AccountCode { get; set; } 

        }
    }
}
