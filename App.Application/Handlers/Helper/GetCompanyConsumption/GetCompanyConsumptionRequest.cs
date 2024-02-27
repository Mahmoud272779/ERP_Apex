using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.GetCompanyConsumption
{
    public class GetCompanyConsumptionRequest : IRequest<List<GetCompanyConsumptionResponse>>
    {
    }
    public class GetCompanyConsumptionResponse
    {
        public int Id { get; set; }
        public int count { get; set; }
    }
}
