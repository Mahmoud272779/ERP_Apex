using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports.Sales
{
    public class salesOfCasherResponseDTO
    {
        public double TotalCashPaid { get; set; }
        public double TotalNetPaid { get; set; }
        
        public double TotalChequesPaid { get; set; }
        
        public double TotalReturn { get; set; }
        public double TotalTotal { get; set; }
        public List<ListOfSalesOfCasherResponseDTO> Data { get; set; }
    }
    public class ListOfSalesOfCasherResponseDTO
    {
        public DateTime date { get; set; }
        public string documentCode { get; set; }
        public string documentTypeAR { get; set; }
        public string documentTypeEn { get; set; }
        public string rowClassName { get; set; }
        public double CashPaid { get; set; }
        public double NetPaid { get; set; }
        public double ChequesPaid { get; set; }
        public double Return { get; set; }
        public double Total { get; set; }
        public double Serialize { get; set; }

    }
    public class salesOfCasherResponse
    {
        public salesOfCasherResponseDTO data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int TotalCount { get; set; }
        public int dataCount { get; set; }
    }
}
