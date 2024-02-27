using App.Application.Handlers.Units;
using App.Application.Services.Process.Invoices.General_APIs;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace App.Application.Handlers.Persons.GetPersonsByDate
{
    public class GetPersonsByDateHandler : IRequestHandler<GetPersonsByDateRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPersons> PersonsRepositoryQuery;
        private readonly IGeneralAPIsService generalAPIsService;

        public GetPersonsByDateHandler(IRepositoryQuery<InvPersons> personsRepositoryQuery, IGeneralAPIsService generalAPIsService)
        {
            PersonsRepositoryQuery = personsRepositoryQuery;
            this.generalAPIsService = generalAPIsService;
        }

        public async Task<ResponseResult> Handle(GetPersonsByDateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var resData = await PersonsRepositoryQuery.TableNoTracking.Where(q => q.UTime >= request.date).ToListAsync();

                return await generalAPIsService.Pagination(resData, request.PageNumber, request.PageSize);


            }
            catch (Exception ex)
            {
                return new ResponseResult() { Data = null, Id = null, Result = Result.NotFound };


            }
        }
    }
}
