using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports
{
    public class ItemsProfitRequest : IRequest<ResponseResult>
    {

        public string branches { get; set; } = "";
     
        public int itemId { get; set; }  
        [Required]
        public int categoryId { get; set; }
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
