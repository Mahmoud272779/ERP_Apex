using App.Domain.Models.Request.Restaurants;
using MediatR;

namespace App.Application.Handlers.Restaurants.Kitchens
{
    public class AddKitchensRequest : KitchensParameter, IRequest<ResponseResult>
    {
    }
}
