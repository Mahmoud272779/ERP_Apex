using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.InvoicesHelper.calculatePaymentMethods
{
    public class calculatePaymentMethodsRequest : IRequest<ResponseResult>
    {
        public double[] values { get; set; }
        public double net { get; set; }
    }
}
