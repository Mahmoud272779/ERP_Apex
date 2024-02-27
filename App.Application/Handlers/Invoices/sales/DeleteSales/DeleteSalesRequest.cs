using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Invoices.sales.DeleteSales
{
    public class DeleteSalesRequest : SharedRequestDTOs.Delete, IRequest<ResponseResult>
    {
    }
}
