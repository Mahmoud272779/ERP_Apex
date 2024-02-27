using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class BranchRequestsDTOs
    {
        public class Add
        {
            public string LatinName { get; set; }
            [Required]
            public string ArabicName { get; set; }
            public string AddressEn { get; set; }
            public string AddressAr { get; set; }
            public string Fax { get; set; }
            public string Phone { get; set; }
            public int Status { get; set; }
            public string Notes { get; set; }
            public int? ManagerId { get; set; }
            public string ManagerPhone { get; set; }
            public int? SalesPriceId { get; set; }

        }
        public class Update
        {
            public int Id { get; set; }
            public string LatinName { get; set; }
            [Required]
            public string ArabicName { get; set; }
            public string AddressEn { get; set; }
            public string AddressAr { get; set; }
            public string Fax { get; set; }
            public string Phone { get; set; }
            public int Status { get; set; }
            public string Notes { get; set; }
            public int? ManagerId { get; set; }
            public string ManagerPhone { get; set; }
            public int? SalesPriceId { get; set; }

        }
        public class Search
        {

            public int PageSize { get; set; }
            public int PageNumber { get; set; }
            public string Name { get; set; }//Represents name or code
            public int? Status { get; set; }
        }
    }


    public class ListOfBranches
    {
        public string Name { get; set; }
    }
    //public class ListOfBranchesIds
    //{
    //    public int Id { get; set; }
    //}
}
