using App.Application.Helpers.Service_helper.History;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class GetPersonHistoryHandler : IRequestHandler<GetPersonHistoryRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IHistory<InvPersonsHistory> history;
        public GetPersonHistoryHandler(IRepositoryQuery<InvPersons> personQuery, iAuthorizationService iAuthorizationService, IHistory<InvPersonsHistory> history)
        {
            PersonQuery = personQuery;
            _iAuthorizationService = iAuthorizationService;
            this.history = history;
        }



        public async Task<ResponseResult> Handle(GetPersonHistoryRequest request, CancellationToken cancellationToken)
        {
            var person = PersonQuery.TableNoTracking.Where(x => x.Id == request.Code).FirstOrDefault();
            if (person.IsSupplier)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Purchases, (int)SubFormsIds.Suppliers_Purchases, Opretion.Open);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (person.IsCustomer)
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.Sales, (int)SubFormsIds.Customers_Sales, Opretion.Open);
                if (isAuthorized != null)
                    return isAuthorized;
            }

            return await history.GetHistory(a => a.EntityId == request.Code);
        }
    }
}
