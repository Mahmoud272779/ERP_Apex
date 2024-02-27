using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Security.Authentication.Response
{
    public class GeneralPageSizeParameter
    {
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;

    }
    public class GeneralDateTimeParameter
    {
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }

    }
    public class GeneralPrintFile
    {
        public int FileId { get; set; }
    }
}
