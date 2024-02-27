using App.Domain.Entities.POS;
using App.Domain.Models.Response.POS;
using App.Infrastructure.settings;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.POS
{
    public class POSSessionDetaliesHandler : IRequestHandler<POSSessionDetaliesRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvoicePaymentsMethods> _InvoicePaymentsMethodsQuery;
        private readonly IRepositoryQuery<POSSession> _POSSessionQuery;
        private readonly IRoundNumbers _roundNumbers;

        public POSSessionDetaliesHandler(IRepositoryQuery<InvoicePaymentsMethods> invoicePaymentsMethodsQuery, IRoundNumbers roundNumbers, IRepositoryQuery<POSSession> pOSSessionQuery)
        {
            _InvoicePaymentsMethodsQuery = invoicePaymentsMethodsQuery;
            _roundNumbers = roundNumbers;
            _POSSessionQuery = pOSSessionQuery;
        }

        public async Task<ResponseResult> Handle(POSSessionDetaliesRequest request, CancellationToken cancellationToken)
        {
            var invoices = _InvoicePaymentsMethodsQuery.TableNoTracking
                                    .Include(x=> x.InvoicesMaster)
                                    .Include(x=> x.PaymentMethod)
                                    .Where(x => !x.InvoicesMaster.IsDeleted)
                                    .Where(x => x.InvoicesMaster.POSSessionId == request.SessionId)
                                    .Where(x => x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.POS || x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnPOS)
                                    .ToList();

            var sales = invoices.Where(x => x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.POS)
                .GroupBy(x => x.PaymentMethodId)
                .Select(x => new POSSessionDetaliesResponse_Sales
                {
                    arabicName = x.First().PaymentMethod.ArabicName,
                    latinName = x.First().PaymentMethod.LatinName,
                    count = x.Count(),
                    total = _roundNumbers.GetRoundNumber(x.Sum(s=> s.PaymentMethod.InvoicesPaymentsMethods.Sum(c=> c.Value + (s.InvoicesMaster.Remain<0 ? s.InvoicesMaster.Remain : 0))))
                }).ToList();

            var posReturn = invoices.Where(x => x.InvoicesMaster.InvoiceTypeId == (int)DocumentType.ReturnPOS)
                .GroupBy(x => x.PaymentMethodId)
                .Select(x => new POSSessionDetaliesResponse_Return
                {
                    arabicName = x.First().PaymentMethod.ArabicName,
                    latinName = x.First().PaymentMethod.LatinName,
                    count = x.Count(),
                    total = _roundNumbers.GetRoundNumber(x.Sum(c => c.InvoicesMaster.Net))
                }).ToList();

            var session = _POSSessionQuery.TableNoTracking.Include(x => x.employee).Where(x => x.Id == request.SessionId).FirstOrDefault();
            var res = new POSSessionDetaliesResponse()
            {
                sessionId = request.SessionId,
                employeeId = session.employee.Id,
                sessionCode = session.sessionCode,
                arabicName = session.employee.ArabicName,
                latinName = session.employee.LatinName,
                startDate =session.start.ToString(defultData.datetimeFormat),
                endDate = session.end != null ?session.end.Value.ToString(defultData.datetimeFormat):null,

                POSSessionDetaliesResponse_Sales = sales,
                POSSessionDetaliesResponse_Return = posReturn
            };
            return new ResponseResult()
            {
                Data = res,
                Result = Result.Success
            };
        }
    }
}
