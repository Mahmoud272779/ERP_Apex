
using MediatR;
using Org.BouncyCastle.Asn1.Mozilla;

namespace App.Application.Handlers.GeneralLedger.FinancialAccounts
{
    public class GetAccountInformationRequest : IRequest<ResponseResult>
    {
        public int id { get; set; }
    }
}
