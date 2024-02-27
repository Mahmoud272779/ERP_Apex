using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Domain.Entities.Process;
using App.Domain.Models.Security.Authentication.Response;
using Newtonsoft.Json;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class PersonRequest
    {

        public string ArabicName { get; set; }
        public string? LatinName { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int? SalesManId { get; set; }
        public string? ResponsibleAr { get; set; }
        public string? ResponsibleEn { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? Email { get; set; }
        public string? TaxNumber { get; set; }
        public string? AddressAr { get; set; }
        public string? AddressEn { get; set; }
        public double? CreditLimit { get; set; }
        public int? CreditPeriod { get; set; }
        public double? DiscountRatio { get; set; }
        public int? SalesPriceId { get; set; }
        public int? LessSalesPriceId { get; set; }
        public bool? AddToAnotherList { get; set; }
        [Required]
        public bool IsSupplier { get; set; }
        public int? FinancialAccountId { get; set; }

        public int[] Branches { get; set; }

        //Data in case of customers
        [StringLength(70)]
        public string? BuildingNumber { get; set; }
        [StringLength(200)]
        public string? StreetName { get; set; }
        [StringLength(70)]
        public string? Neighborhood { get; set; }// الحي
        [StringLength(70)]
        public string? City { get; set; }
        [StringLength(70)]
        public string? Country { get; set; }
        [StringLength(70)]
        public string? PostalNumber { get; set; }
        public DateTime UTime { get; set; }

    }

    public class UpdatePersonRequest
    {

        public int Id { get; set; }
        //public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        [Required]
        public int SalesManId { get; set; }
        public string ResponsibleAr { get; set; }
        public string ResponsibleEn { get; set; }
        public string Phone { get; set; }
        public string? Fax { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public double? CreditLimit { get; set; }
        public int? CreditPeriod { get; set; }
        public double? DiscountRatio { get; set; }
        public int? SalesPriceId { get; set; }
        public int? LessSalesPriceId { get; set; }
        //   public bool IsCustomer { get; set; }
        public bool IsSupplier { get; set; }
        public bool AddToAnotherList { get; set; }
        //  public string CodeT { get; set; }
        public int? FinancialAccountId { get; set; }

        public int[] Branches { get; set; }


        //Data in case of customers
        [StringLength(70)]
        public string? BuildingNumber { get; set; }
        [StringLength(200)]
        public string? StreetName { get; set; }
        [StringLength(70)]
        public string? Neighborhood { get; set; }// الحي
        [StringLength(70)]
        public string? City { get; set; }
        [StringLength(70)]
        public string? Country { get; set; }
        [StringLength(70)]
        public string? PostalNumber { get; set; }

        public DateTime UTime { get; set; }
    }


    public class PersonsSearch : GeneralPageSizeParameter
    {
        public bool? IsSupplier { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        [JsonIgnore]
        public int[] TypeArr { get; set; }
        public string? SalesManId { get; set; }
    }

    
}
