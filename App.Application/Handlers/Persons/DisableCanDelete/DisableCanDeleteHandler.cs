using MediatR;
using System.Threading;

namespace App.Application.Handlers.Persons
{
    public class DisableCanDeleteHandler : IRequestHandler<DisableCanDeleteRequest, bool>
    {
        private readonly IRepositoryQuery<InvPersons> PersonQuery;
        private readonly IRepositoryCommand<InvPersons> PersonCommand;

        public DisableCanDeleteHandler(IRepositoryCommand<InvPersons> personCommand)
        {
            PersonCommand = personCommand;
        }

        public async Task<bool> Handle(DisableCanDeleteRequest request, CancellationToken cancellationToken)
        {
            var findPerson = await PersonQuery.GetAsync(request.personId);
            if (findPerson != null)
                if (findPerson.CanDelete)
                {
                    findPerson.CanDelete = false;
                    var saved = await PersonCommand.UpdateAsyn(findPerson);
                    if (saved)
                        return true;
                }
                else
                    return true;
            return false;
        }
    }
}
