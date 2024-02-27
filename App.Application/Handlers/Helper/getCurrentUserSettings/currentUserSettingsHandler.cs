using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Helper
{
    public class currentUserSettingsHandler : IRequestHandler<currentUserSettingsRequest, ResponseResult>
    {
        private readonly iUserInformation _userInformation;

        public currentUserSettingsHandler(iUserInformation userInformation)
        {
            _userInformation = userInformation;
        }

        public async Task<ResponseResult> Handle(currentUserSettingsRequest request, CancellationToken cancellationToken)
        {
            var userInfo = await _userInformation.GetUserInformation();
            var data = new currentUserInformation();
            Mapping.Mapper.Map(userInfo.otherSettings, data);
            return new ResponseResult()
            {
                Result = Result.Success,
                Data = data
            };
        }

    }
}
