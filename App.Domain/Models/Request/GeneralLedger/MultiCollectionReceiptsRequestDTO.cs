using App.Domain.Models.Shared;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Request.GeneralLedger
{
    public class Add_Edit_Comman_MultiCollectionReceipts
    {
        [Required]
        public int personId { get; set; }
        [Required]
        public int Authority { get; set; } //  الجهه
        public bool isSafe { get; set; }
        [Required]
        public DateTime docDate { get; set; }
        [Required]
        public int PaymentMethodId { get; set; } // طريقة السداد
        public int? safeId { get; set; }
        public int? bankId { get; set; }
        public string? paperNumber { get; set; }
        public string? ChequeNumber { get; set; } // رقم الشيك
        public string? ChequeBankName { get; set; } // رقم الشيك
        public DateTime ChequeDate { get; set; } // تاريخ الشيك
        public string? note { get; set; }
        public IFormFile[]? AttachedFile { get; set; }
        public List<MultiCollectionReceiptsInvocesRequestDTO> invoices { get; set; }
    }
    public class Add_MultiCollectionReceiptsRequestDTO : Add_Edit_Comman_MultiCollectionReceipts
    {
        
    }
    public class Edit_MultiCollectionReceiptsRequestDTO : Add_Edit_Comman_MultiCollectionReceipts
    {
        [Required]
        public int Id { get; set; }
        public string fileId { get; set; }
    }
    public class Delete_MultiCollectionReceiptsRequestDTO
    {
        [Required]
        public int Id { get; set; }
    }
    public class Get_MultiCollectionReceiptsRequestDTO : PaginationVM
    {
        public string? searchCriteria { get; set; }
        public int? Authority { get; set; } //  الجهه
        public int? personId { get; set; }
    }
    public class GetById_MultiCollectionReceiptsRequestDTO
    {
        public int Id { get; set; }


        //for ptint
        public int receiptType { get; set; } = (int)DocumentType.SafeMultiCollectionReceipts;
    }
}
