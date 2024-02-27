using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports
{
    public class PaymentsAndDisbursementsResponseDTO
    {
        public double TotalDebitorTransactionOfPeriod { get; set; }
        public double TotalCreditorTransactionOfPeriod { get; set; }
        public double TotalDebitorBalanceOfPeriod { get; set; }
        public double TotalCreditorBalanceOfPeriod { get; set; }

        public double actualTotalDebitorTransaction { get; set; }
        public double actualTotalCreditorTransaction { get; set; }
        public double actualTotalDebitorBalance { get; set; }
        public double actualTotalCreditorBalance{ get; set; }

        public List<PaymentsAndDisbursementsResponseList> list { get; set; }
    }
    public class PaymentsAndDisbursementsResponseList
    {
        public int Id { get; set; }
        public int docTypeId { get; set; }
        public string documentCode { get; set; }
        public string date { get; set; }
        public string typeAr { get; set; }
        public string typeEn { get; set; }
        public string benefitAr { get; set; }
        public string benefitEn { get; set; }
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public string Notes { get; set; }
        public double debitorTransaction { get; set; }
        public double creditorTransaction { get; set; }
        public double debitorBalance { get; set; }
        public double creditorBalance { get; set; }
        public string rowClassName { get; set; }

    }
    public class PaymentsAndDisbursementsResponse
    {
        public PaymentsAndDisbursementsResponseDTO Data { get; set; }
        public int dataCount { get; set; }
        public int totalCount { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
    }
}
