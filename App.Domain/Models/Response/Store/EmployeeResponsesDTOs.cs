using App.Domain.Entities;
using App.Domain.Entities.Process;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using static App.Domain.Enums.Enums;

namespace App.Domain.Models.Security.Authentication.Response.Store
{
    public class EmployeeResponsesDTOs
    {
        public class GetAll
        {
            public int Id { get; set; }
            public int Code { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public int Status { get; set; }
            public string Notes { get; set; }
            public string ImagePath { get; set; }
            public int[] Branches { get; set; }
            public string BranchNameAr { get; set; }
            public string BranchNameEn { get; set; }
            public int JobId { get; set; }
            public string JobNameAr { get; set; }
            public string JobNameEn { get; set; }
            public int JobStatus { get; set; }
            public bool CanDelete { get; set; }
            public int? FinancialAccountId { get; set; }

            //for print 
            public string StatusAr { get; set; }
            public string StatusEn { get; set; }

            public int? SalesPriceId { get; set; } 


        }
        public class FA
        {
            public int Id { get; set; }
            public string LatinName { get; set; }
            public string ArabicName { get; set; }
        }

        public class EmployeeDto
        {

            public int EmploeeId { get; set; }
            public string ArabicName { get; set; }
            public string LatinName { get; set; }
            public int Status { get; set; }
            public string Notes { get; set; }
            public string branchNameAr { get; set; }
            public string branchNameEn { get; set; }
            public string jobNameAr { get; set; }
            public string jobNameEn { get; set; }
        }
    }
}
