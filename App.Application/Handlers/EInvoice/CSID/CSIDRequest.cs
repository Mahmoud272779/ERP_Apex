using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.EInvoice.CSID
{
    public class CSIDRequest : IRequest<ResponseResult>
    {
        [Required]
        public string OTP { get; set; }
        public int branchId { get; set; }
    }
}
