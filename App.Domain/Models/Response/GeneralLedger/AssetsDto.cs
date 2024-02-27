using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class Total
    {
        public Total()
        {
            incomeListDtos = new List<AssetsDto>();
        }
        public double Totals { get; set; }
        public int TotalType { get; set; }
        public List<AssetsDto> incomeListDtos { get; set; }
    }
    public class AssetsDto
    {
        public AssetsDto()
        {
            incomeListChildDtos = new List<AssetsChildDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double TotalBalance { get; set; }
        public int Type { get; set; }
        public double Totals { get; set; }
        public List<AssetsChildDto> incomeListChildDtos { get; set; }

    }
    public class AssetsChildDto
    {
        public AssetsChildDto()
        {
            Childes = new List<AssetsChildDto>();
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string FinancialAccountCode { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public double TotalBalance { get; set; }
        public int Type { get; set; }
        public double Totals { get; set; }
        public List<AssetsChildDto> Childes { get; set; }
    }
    public class AssetsParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
      //  public AssetsSearchParameter Search { get; set; }
    }
    public class AssetsSearchParameter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
    public class PaginationResponse
    {
        public int ListCount { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public object Data { get; set; }
    }
}
