using App.Application.Services.Process.Invoices.General_APIs;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.MainData.Payment_Methods.GetPaymentMethodByDate
{
    public class GetPaymentMethodsByDateHandler : IRequestHandler<GetPaymentMethodsByDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPaymentMethods> _paymentMethodQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetPaymentMethodsByDateHandler(IRepositoryQuery<InvPaymentMethods> paymentMethodQuery,IGeneralAPIsService generalAPIsService)
        {
            _paymentMethodQuery = paymentMethodQuery;
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<ResponseResult> Handle(GetPaymentMethodsByDateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var resData = await _paymentMethodQuery.TableNoTracking.Where(q => q.UTime >= request.date).ToListAsync();

                return await generalAPIsService.Pagination(resData, request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                return new ResponseResult { Data = null, Id = null, Result = Result.NotFound };

            }
        }
    }
}
