using App.Domain.Models.Request.Restaurants;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.Restaurants.Kitchens.GetListOfKitchens
{
    public class GetListOfKitchensRequest : KitchensSearch, IRequest<ResponseResult>
    {
    }
}
