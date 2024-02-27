using App.Domain.Models.Security.Authentication.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.Store.Invoices
{
    public class EntryFundCustomerAndSuppliersResponse
    {
        public DateTime Date { get; set; }
        public GetFundsResult Data { get; set; }
        public int dataCount { get; set; } = 0;
        public int totalCount { get; set; } = 0;
    }
    public class GetFundsResult
    {
        public List<FundsCustomerandSuppliersDto> FundsList { get; set; }
        public int Count { get; set; }
    }
    public class response
    {
        public string Date { get; set; }
        public object FundsList { get; set; }
    }
}
