using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class Totalss
    {
        public Totalss()
        {
            incomeListDtos = new List<IncomeListDto>();
        }
        public double Totals { get; set; }
        public int TotalType { get; set; }
        public List<IncomeListDto> incomeListDtos { get; set; }
    }
    public class IncomeListDto
    {
        public IncomeListDto()
        {
            incomeListChildDtos = new List<IncomeListChildDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double TotalBalance { get; set; }
        public int Type { get; set; }
        public double Totals { get; set; }
        public List<IncomeListChildDto>  incomeListChildDtos { get; set; }

    }
    public class IncomeListChildDto
    {
        public IncomeListChildDto()
        {
            Childes = new List<IncomeListChildDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double TotalBalance { get; set; }
        public int Type { get; set; }
        public double Totals { get; set; }
        public List<IncomeListChildDto> Childes { get; set; }
    }
   
    public class IncomeListSearchParameter
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int finalAccount  { get;set; }//طبيعه الحساب الميزانيه وقائمه الدخل
        public int? naturalAccount { get; set; }
    }

    //hamada

    public class IncomeingListFilterData
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string AutoCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public int accountnNatural { get; set; }
        public bool hasChild { get; set; }
        public double AccountBalance { get; set; }
        public double debtor { get; set; }
        public double creditor { get; set; }
        public int Level { get; set; }
        public int GroupId { get; set; }


    }
    public class totalResponseResult  
    { 
        public double totalBalance { get; set; }
        public List<IncomeingListFilterData> IncomingData { get; set; }

    }

    public class BudgetResult
    {
        public List<IncomeingListFilterData> CreditDataList { get; set; }
        public List<IncomeingListFilterData> DebitDataList { get; set; }
        public double totalCreditor { get; set; }
        public double totalDeptor { get; set; }



    }

}
