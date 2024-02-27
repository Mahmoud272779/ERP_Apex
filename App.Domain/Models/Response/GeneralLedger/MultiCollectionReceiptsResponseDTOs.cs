using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.GeneralLedger
{
    public class GetAllMultiCollectionReceiptsResponseDTO
    {
        public int Id { get; set; }
        public string RecieptType { get; set; }
        public string RecieptDate { get; set; }
        public GetAllMultiCollectionReceipts_Authority Authority { get; set; }
        public Benefit Benefit { get; set; }
        public double amount { get; set; }
        public bool canDelete { get; set; } = true;
        public bool isBlock { get; set; }
        public bool isEdit { get; set; }


    }
    public class GetAllMultiCollectionReceipts_Authority
    {
        public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
    public class Benefit
    {
        public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
    ///////////////////////////////////////////////////////////////////

    public class GetByIdMultiCollectionReceiptsResponseDTO
    {
        public int Id { get; set; }
        public string RecieptType { get; set; }
        public bool isSafe { get; set; }
        public safeOrBankObj safeOrBankObj { get; set; }
        public string RecieptDate { get; set; }
        public string PaperNumber { get; set; }
        public int Authority { get; set; }
        public string note { get; set; }
        public Benefit Benefit { get; set; }
        public double amount { get; set; }
        //for print
        public int code { get; set; }
        public int signal { get; set; }
        public bool isBlock { get; set; }
        public bool isAccredit { get; set; }
        public double serialize { get; set; }
        public int subTypeId { get; set; }
        public int branchId { get; set; }
        public string documentDate {get;set;}

        public paymentMethod paymentMethod { get; set; }
        public List<invoice> invoices { get; set; }
        public List<recieptsFiles> recieptsFiles { get; set; }
    }
    public class recieptsFiles
    {
        public int fileId { get; set; }
        public string fileExtensions { get; set; }
        public string fileLink { get; set; }
        public string fileName { get; set; }
    }
    public class safeOrBankObj
    {
        public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
    public class paymentMethod
    {
        public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
    }
    public class invoice
    {
        public int Id { get; set; }
        public string invoiceType { get; set; }
        public double net { get; set; }
        public double invoicePaid { get; set; }
        public double recPaid { get; set; }

    }
    public class MultiCollectionReceiptsPrint: GetByIdMultiCollectionReceiptsResponseDTO
    {
        
        public string safeBankNameAr { get; set; }
        public string safeBankNameEn { get; set; }
        public string benefitNameAr { get; set; }
        public string benefitNameEn { get; set; }
        
    }

}
