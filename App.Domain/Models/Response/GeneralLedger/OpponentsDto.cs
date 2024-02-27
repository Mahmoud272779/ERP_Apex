using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class TotalOpponent
    {
        public TotalOpponent()
        {
            opponentsDtos = new List<OpponentsDto>();
        }
        public double Totals { get; set; }
        public int TotalType { get; set; }
        public List<OpponentsDto> opponentsDtos { get; set; }
    }
    public class OpponentsDto
    { 
    public OpponentsDto()
    {
        opponentsChildDtos = new List<OpponentsChildDto>();
    }
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string FinancialAccountCode { get; set; }
    public string LatinName { get; set; }
    public string ArabicName { get; set; }
    public double TotalBalance { get; set; }
    public int Type { get; set; }
    public double Totals { get; set; }
    public List<OpponentsChildDto>  opponentsChildDtos { get; set; }

    }
  public class OpponentsChildDto
  {
    public OpponentsChildDto()
    {
        Childes = new List<OpponentsChildDto>();
    }
    public int Id { get; set; }
    public int? ParentId { get; set; }
    public string FinancialAccountCode { get; set; }
    public string LatinName { get; set; }
    public string ArabicName { get; set; }
    public double TotalBalance { get; set; }
    public int Type { get; set; }
    public double Totals { get; set; }
    public List<OpponentsChildDto> Childes { get; set; }
  }
    public class OpponentsParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
       // public OpponentsSearchParameter Search { get; set; } = new OpponentsSearchParameter();
    }
    public class OpponentsSearchParameter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }


}
