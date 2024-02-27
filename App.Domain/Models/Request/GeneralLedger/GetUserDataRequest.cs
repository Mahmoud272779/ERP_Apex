using App.Domain.Models.Security.Authentication.Response;
using MediatR;

namespace App.Domain.Models.Security.Authentication.Request
{
    public class GetUserDataRequest :  IRequest<GetUserDataResponse>
    {
        public string SecURL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BranchID { get; set; }
        public string Lang { get; set; }
        public bool IsUseMenu { get; set; }
    }
    
}
