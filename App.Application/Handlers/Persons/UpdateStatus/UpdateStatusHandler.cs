using App.Application.Helpers.Service_helper.History;
using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class UpdateStatusHandler : IRequestHandler<UpdateStatusRequest, ResponseResult>
    {
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly iAuthorizationService _iAuthorizationService;
        private readonly IRepositoryCommand<InvPersons> PersonCommand;
        private readonly IHistory<InvPersonsHistory> history;
        private readonly ISystemHistoryLogsService _systemHistoryLogsService;

        public UpdateStatusHandler(ISystemHistoryLogsService systemHistoryLogsService, IHistory<InvPersonsHistory> history, IRepositoryCommand<InvPersons> personCommand, iAuthorizationService iAuthorizationService, IRepositoryQuery<InvPersons> personQuery)
        {
            _systemHistoryLogsService = systemHistoryLogsService;
            this.history = history;
            PersonCommand = personCommand;
            _iAuthorizationService = iAuthorizationService;
            PersonQuery = personQuery;
        }

        public async Task<ResponseResult> Handle(UpdateStatusRequest parameters, CancellationToken cancellationToken)
        {
            if (parameters.Id.Count() == 0)
                return new ResponseResult() { Data = null, Id = null, Result = Result.RequiredData, Note = Actions.IdIsRequired };

            if (parameters.Status < (int)Status.Active || parameters.Status > (int)Status.Inactive)
            {
                return new ResponseResult { Result = Result.Failed, Note = Actions.InvalidStatus };
            }
            var persons = PersonQuery.TableNoTracking.Where(e => parameters.Id.Contains(e.Id));

            if (persons.Where(x => x.IsSupplier == true).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.Suppliers_Fund, Opretion.Edit);
                if (isAuthorized != null)
                    return isAuthorized;
            }
            if (persons.Where(x => x.IsCustomer == true).Any())
            {
                var isAuthorized = await _iAuthorizationService.isAuthorized((int)MainFormsIds.ItemsFund, (int)SubFormsIds.Customres_Fund, Opretion.Edit);
                if (isAuthorized != null)
                    return isAuthorized;
            }

            var personsList = persons.ToList();

            personsList.Select(e => { e.Status = parameters.Status; return e; }).ToList();
            if (parameters.Id.Contains(1) || parameters.Id.Contains(2))
                personsList.Where(q => q.Id == 1 || q.Id == 2).Select(e => { e.Status = (int)Status.Active; return e; }).ToList();


            var result = await PersonCommand.UpdateAsyn(personsList);

            foreach (var Person in personsList)
            {
                history.AddHistory(Person.Id, Person.LatinName, Person.ArabicName, Aliases.HistoryActions.Update, Aliases.TemporaryRequiredData.UserName);
            }

            if (personsList.Where(x => x.IsSupplier).Any())
                await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.editSupplier);
            else if (personsList.Where(x => x.IsCustomer).Any())
                await _systemHistoryLogsService.SystemHistoryLogsService(SystemActionEnum.editCustomer);


            return new ResponseResult() { Data = null, Id = null, Result = Result.Success };
        }
    }
}
