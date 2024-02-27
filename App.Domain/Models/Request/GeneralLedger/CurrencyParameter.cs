using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public  class CurrencyParameter
    {
        public string LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string CoinsAr { get; set; }
        public string CoinsEn { get; set; }
        [Required]
        public string AbbrAr { get; set; }
        public string AbbrEn { get; set; }
        public int Status { get; set; }
        public string BrowserName { get; set; }
        public string Notes { get; set; }
        [Required]
        public double Factor { get; set; }
        public bool isDefault { get; set; }
        //public DateTime? LastUpdate { get; set; }
    }
    public class UpdateCurrencyParameter
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string LatinName { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string CoinsAr { get; set; }
        public string CoinsEn { get; set; }
        [Required]
        public string AbbrAr { get; set; }
        public string AbbrEn { get; set; }
        public int Status { get; set; }
        public string BrowserName { get; set; }

        public string Notes { get; set; }
        [Required]
        public double Factor { get; set; }
        public bool isDefault { get; set; }
        public string LastUpdate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss", new CultureInfo("en-us"));
    }
    public class UpdateCurrencyFactorList
    {
        public List<UpdateCurrencyFactorParameter> updateCurrencyFactorParameters { get; set; }
    }
    public class UpdateCurrencyFactorParameter
    {
        public int Id { get; set; }
        public double Factor { get; set; }
    }
   
    public class ListOfCurrency
    {
        public string Name { get; set; }
    }
    

    public class currencyRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchCriteria { get; set; }
        public int Status { get; set; }
    }
}
