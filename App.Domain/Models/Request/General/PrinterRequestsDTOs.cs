using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class PrinterRequestsDTOs
    {

        public class Add
        {
            public string LatinName { get; set; }
            public string ArabicName { get; set; }
            public string IP { get; set; }
            public int BranchId { get; set; }
            public string Notes { get; set; }
            public int Status { get; set; } = 1;
            public DateTime UTime { get; set; }
            public bool CanDelete { get; set; } = true;

        }
        public class Update
        {
            public int? Id { get; set; }
            public int Code { get; set; }
            public string LatinName { get; set; }
            public string ArabicName { get; set; }
            public string IP { get; set; }
            public int BranchId { get; set; }
            public string branchNameAr { get; set; }
            public string branchNameEn { get; set; }
            public string Notes { get; set; }
            public int Status { get; set; }
            public DateTime UTime { get; set; }
            public bool CanDelete { get; set; } = true;

        }
        public class Search
        {

            public int PageSize { get; set; }
            public int PageNumber { get; set; }
            public string SearchCriteria { get; set; }//Represents name or code
            public int Status { get; set; } = 0;
            public int PrinterId { get; set; }
            public string Ids { get; set; }
            public bool IsSearchData { get; set; } = true;

        }
        public class UpdateActive
        {
            public List<int> Id { get; set; }
            public int Status { get; set; }
        }
    }
}
