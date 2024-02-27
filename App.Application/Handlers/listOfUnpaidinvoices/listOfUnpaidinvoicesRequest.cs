using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.listOfUnpaidinvoices
{
    public class listOfUnpaidinvoicesRequest : PaginationVM,IRequest<ResponseResult>
    {
        public int? recId { get; set; }
        public string? code { get; set; }
        public int Authority { get; set; }
        public int personId { get; set; }
    }
}
