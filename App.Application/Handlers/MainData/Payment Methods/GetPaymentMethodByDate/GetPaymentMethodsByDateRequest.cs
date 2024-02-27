using App.Domain.Models.Security.Authentication.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.MainData.Payment_Methods.GetPaymentMethodByDate
{
    public class GetPaymentMethodsByDateRequest : GeneralPageSizeParameter, IRequest<ResponseResult>
    {
        public DateTime date { get; set; }
    }

    public class GetPaymentMethodForInvoice
    {
        public List<int>? paymentMeyhodID { get; set; }
        public int? invoiceTypeId { get; set; } = 0;

  
    }
}
