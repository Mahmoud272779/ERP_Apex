using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.SalesReports.SalesOfSalesMan
{
    public class SalesOfSalesManRequest : IRequest<ResponseResult>
    {
        [Required]
        public int SalesManID { get; set; }
        //[Required]
        public string branches { get; set; } = "";

        public int PaymentType { get; set; }
        [Required]
        public DateTime dateFrom { get; set; }
        [Required]
        public DateTime dateTo { get; set; }
        //[Required]
        public int PageSize { get; set; }
        //[Required]
        public int PageNumber { get; set; }
        public bool isPrint { get; set; } = false;


    }
}
