using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper.SafesBanksFundService
{
    public class SafesBanksFundServiceRequest : IRequest<bool>
    {
        public List<InvFundsBanksSafesDetails> fundsDetailsList { get; set; }
        public InvFundsBanksSafesMaster table { get; set; }
        public UserInformationModel userInfo { get; set; }
        public bool isUpdate { get; set; }
    }
}
