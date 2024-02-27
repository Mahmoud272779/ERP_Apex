using App.Domain.Models.Request.Store.Reports.Sales;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Reports.Purchases.PurchasesTransaction
{
    public class puchasesTransactionRequest : salesTransactionRequestDTO,IRequest<ResponseResult>
    {
    }
}
