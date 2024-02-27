using App.Application.Handlers.Restaurants;
using App.Application.Handlers.Restaurants.Kitchens;
using App.Application.Handlers.Restaurants.Kitchens.GetListOfKitchens;
using App.Application.Handlers.Restaurants.Kitchens.UpdateKitchens;
using App.Application.Handlers.Units;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Process.RestaurantsServices.KitchensServices
{
    public class KitchensServices : BaseClass, iKitchensServices 
    {
        private readonly IMediator _mediator;

        public KitchensServices(IHttpContextAccessor _httpContext, IMediator mediator) : base(_httpContext) 
        {
            _mediator = mediator;
        }
        public async Task<ResponseResult> AddKitchen( AddKitchensRequest parameter)
        {
            return await _mediator.Send(parameter);
        }

        public async Task<ResponseResult> DeleteKitchens(Handlers.Restaurants.DeleteKitchensRequest parameters)
        {
            return await _mediator.Send(parameters);
        }

        public async Task<ResponseResult> GetKitchensDropDown()
        {
            return await _mediator.Send(new GetKitchensDropDownRequest());
        }

        public async Task<ResponseResult> GetKitchensHistory(int Code)
        {
            return await _mediator.Send(new Handlers.Restaurants.GetKitchensHistoryRequest { Code = Code });
        }

        public async Task<ResponseResult> GetListOfKitchens(GetListOfKitchensRequest parameters)
        {
            return await _mediator.Send(parameters);
        }

        public async Task<ResponseResult> UpdateKitchens(UpdateKitchensRequest parameters)
        {
            return await _mediator.Send(parameters);
        }

        public async Task<ResponseResult> UpdateStatus(Handlers.Restaurants.UpdateStatusRequest parameters)
        {
            return await _mediator.Send(parameters);

        }
    }
}
