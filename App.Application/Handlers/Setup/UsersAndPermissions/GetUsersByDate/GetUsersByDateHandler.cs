using App.Application.Handlers.Persons.GetPersonsByDate;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Setup.UsersAndPermissions.GetUsersByDate
{
    internal class GetUsersByDateHandler : IRequestHandler<GetUsersByDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<userAccount> _usersRepositoryQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetUsersByDateHandler(IRepositoryQuery<userAccount> usersRepositoryQuery, IGeneralAPIsService generalAPIsService)
        {
            _usersRepositoryQuery = usersRepositoryQuery;
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<ResponseResult> Handle(GetUsersByDateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var resData = await _usersRepositoryQuery.TableNoTracking
                    .Where(q => q.UpdateTime >= request.date)
                    .Include(s => s.otherSettings).ThenInclude(x=>x.otherSettingsBanks)
                    .Include(s => s.otherSettings).ThenInclude(x=>x.otherSettingsSafes)
                    .Include(s => s.otherSettings).ThenInclude(x => x.OtherSettingsStores)
                    .ToListAsync();


                return await generalAPIsService.Pagination(resData, request.PageNumber, request.PageSize);

                
            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}
