using MediatR;

namespace App.Application.Handlers.Persons
{
    public class DisableCanDeleteRequest : IRequest<bool>
    {
        public int personId { get; set; }   
    }
}
