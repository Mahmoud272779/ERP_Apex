using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response.Store
{
    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int? SalesmanId { get; set; }
        public string SalesmanAr { get; set; }
        public string SalesmanEn { get; set; }
        public string ResponsibleAr { get; set; }
        public string ResponsibleEn { get; set; }

        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public double? CreditLimit { get; set; }
        public int? CreditPeriod { get; set; }
        public double? DiscountRatio { get; set; }
        public int? SalesPriceId { get; set; }
        public int? LessSalesPriceId { get; set; }
        public bool AddToAnotherList { get; set; }
        public bool IsSupplier { get; set; }
        public int[] branches { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }

        public bool CanDelete { get; set; }

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
    public class SupplierResponse
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int? SalesmanId { get; set; }
        public string SalesmanAr { get; set; }
        public string SalesmanEn { get; set; }
        public string ResponsibleAr { get; set; }
        public string ResponsibleEn { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string TaxNumber { get; set; }
        public string AddressAr { get; set; }
        public string AddressEn { get; set; }
        public bool AddToAnotherList { get; set; }
        public bool IsSupplier { get; set; }
        public int[] branches { get; set; }
        public string BranchNameAr { get; set; }
        public string BranchNameEn { get; set; }

        public bool CanDelete { get; set; }
    }

    public class personsForBalanceDto
        {
        public int Id { get; set; }
        public int Code { get; set; }
        public string ArabicName { get; set; }
        public string LatinName { get; set; }
        public int Status { get; set; }
        public string Phone { get; set; }
        public int? SalesManId { get; set; }
        public string salesManNameAr { get; set; }
        public string salesManNameEn { get; set; }
        public string CodeT { get; set; }
        public double? Discount { get; set; }
        public double balance { get; set; }
        public double Creditor { get; set; }
        public double Debtor { get; set; }
        public bool isCreditor { get; set; }
        public int? SalesPriceId { get; set; }

    }
}
