using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.POS
{
    public class checkUserSessionStatusRsponse
    {
        public int userPOSSessionStatus { get; set; }
        public string messageAr { get; set; }
        public string messageEn { get; set; }
    }
    public class GetOpenSessionsPOS
    {
        public int Id { get; set; }
        public int code { get; set; }
        public int employeeId { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int statusId { get; set; }
        public string statusAr { get; set; }
        public string statusEn { get; set; }
        public GetOpenSessionsPOSTotalSales financials { get; set; }

    }
    public class GetOpenSessionsPOSTotalSales
    {
        public double totalSales { get; set; }
        public double totalReturn { get; set; }
        public double totlaVat { get; set; }
        public double net { get; set; }
    }
    public class POSSessionDetaliesResponse
    {
        public int sessionId { get; set; }
        public int sessionCode { get; set; }
        public int employeeId { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public List<POSSessionDetaliesResponse_Sales> POSSessionDetaliesResponse_Sales { get; set; }
        public List<POSSessionDetaliesResponse_Return> POSSessionDetaliesResponse_Return { get; set; }
    }
    public class POSSessionDetaliesResponse_Sales
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int count { get; set; }
        public double total { get; set; }
    }
    public class POSSessionDetaliesResponse_Return
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public int count { get; set; }
        public double total { get; set; }
    }
}

