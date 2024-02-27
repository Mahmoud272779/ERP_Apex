using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Response.Store.Reports
{
    public class bankAndSafesResponseDTO
    {
        public double totalDebtorTransactionOfOfPeriod { get; set; }
        public double totalCreditorTransactionOfPeriod { get; set; }
        public double totalDebtorBalanceOfPeriod { get; set; }
        public double totalCreditorBalanceOfPeriod { get; set; }
        public double TotalActualDebtorTransaction { get; set; }
        public double TotalActualCreditorTransaction { get; set; }
        public double TotalActualDebtorBalance { get; set; }
        public double TotalActualCreditorBalance { get; set; }
        public List<bankAndSafesResponseData> data { get; set; }
    }
    public class bankAndSafesResponseData
    {
        public int Id { get; set; }
        public DateTime? date { get; set; }
        public DateTime cdate { get; set; }
        public string documentCode { get; set; }
        public string documentTypeAr { get; set; }
        public string documentTypeEn { get; set; }
        public string benfitAr { get; set; }
        public string benefitEn { get; set; }
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public string Notes { get; set; }
        public string rowClassName { get; set; }
        public double debtorTransaction { get; set; }
        public double creditorTransaction { get; set; }
        public double debtorBalance { get; set; }
        public double creditorBalance { get; set; }
        public int subTypeId { get; set; }
        public int docTypeId { get; set; }
        public double Serialize { get; set; }
    }
    public class bankAndSafesResponse
    {
        public bankAndSafesResponseDTO data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int totalCount { get; set; }
        public int dataCount { get; set; }
    }


    //////////////////////////////////////
    public class ExpensesAndReceiptsResponse
    {
        public ExpensesAndReceiptsResponseDTO Data { get; set; }
        public string notes { get; set; }
        public Result Result { get; set; }
        public int totalCount { get; set; }
        public int dataCount { get; set; }
    }
public class ExpensesAndReceiptsResponseDTO
    {
        public double total { get; set; }
        public List<ExpensesAndReceiptsResponseData> data { get; set; }
    }
    public class ExpensesAndReceiptsResponseData
    {
        public int Id { get; set; }
        public int docTypeId { get; set; }
        public string documentCode { get; set; }
        public DateTime date { get; set; }
        public string benfitCode { get; set; }
        public string benfitAr { get; set; }
        public string benfitEn { get; set; }
        public string benefitTypeAr { get; set; }
        public string benefitTypeEn { get; set; }
        public string paymentTypeAr { get; set; }
        public string paymentTypeEn { get; set; }
        public string ChequesNum { get; set; }
        public double amount { get; set; }
        //for print
        public double serialize { get; set; }
        public string DocumentDate { get; set; }
    }
}
