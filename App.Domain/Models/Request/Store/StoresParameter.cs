using App.Domain.Entities.Process;
using App.Domain.Entities.Setup;
using App.Domain.Models.Security.Authentication.Response;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Models.Security.Authentication.Request
{

    public class StoresParameter
    {
        //public StoresParameter()
        //{
        //    branches = new List<branchesPrm>();
        //}
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public string Notes { get; set; }
        [Required]
        public int branchId { get; set; }

        public DateTime UTime { get; set; }
    }
    public class branchesPrm
    {
        public int BranchId { get; set; }

    }
    public class UpdateStoresParameter
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public string Notes { get; set; }
        [Required]
        public int branchId { get; set; }

        public DateTime UTime { get; set; }

    }



    public class StoresSearch : GeneralPageSizeParameter
    {

        public string Name { get; set; }
        public int Status { get; set; }
        public int[] BranchList { get; set; }
    }


    public class AllStoresDto
    {

        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public int[] Branches { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }
        public virtual ICollection<InvoiceMaster> InvoiceMaster { get; set; }
        public virtual ICollection<InvoiceMaster> InvoiceMasterTo { get; set; }
        public virtual ICollection<InvStpItemCardStores> CardStores { get; set; }



    }
    public class BranchesDto
    {
        public int BranchId { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
    }


}
