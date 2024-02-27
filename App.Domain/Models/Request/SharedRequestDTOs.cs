using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class SharedRequestDTOs
    {
        public class UpdateStatus
        {

            public int[] Id { get; set; }
            public int Status { get; set; }

        }
        public class Delete
        {
            public bool isSupplierPage { get; set; }
            public int[] Ids { get; set; }
            public int userId { get; set; } = 1;

        }
        public class getDropDownlist
        {
            public string? SearchCriteria { get; set; }
            public int? code { get; set; } = 0;
            public int PageSize { get; set; }
            public int PageNumber { get; set; }
            public bool isPerson { get; set; } = false;
        }
    }
}
