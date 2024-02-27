using App.Application.Services.HelperService.EmailServices;
using MediatR;

namespace App.Application.Handlers.GeneralAPIsHandler.SendingEmail
{
    public class SendingEmailRequest : emailRequest, IRequest<ResponseResult>
    {
    }
}
