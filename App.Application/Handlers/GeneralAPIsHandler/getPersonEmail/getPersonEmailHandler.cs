using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.GeneralAPIsHandler.getPersonEmail
{
    public class getPersonEmailHandler : IRequestHandler<getPersonEmailRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPersons> PersonRepositorQuery;

        public getPersonEmailHandler(IRepositoryQuery<InvPersons> personRepositorQuery)
        {
            PersonRepositorQuery = personRepositorQuery;
        }

        public async Task<ResponseResult> Handle(getPersonEmailRequest request, CancellationToken cancellationToken)
        {
            var person = await PersonRepositorQuery.GetByIdAsync(request.personId);
            if (person == null)
                return new ResponseResult()
                {
                    Note = Actions.NotFound,
                    Result = Result.NotFound
                };
            return new ResponseResult()
            {
                Data = person.Email,
                Note = Actions.Success,
                Result = Result.Success
            };
        }
    }
}
