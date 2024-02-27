using App.Application.Handlers.Units;
using App.Application.Helpers.Service_helper.History;
using MediatR;

namespace App.Application.Services.Process.Unit
{

    public class UnitsService : BaseClass, IUnitsService
    {
        private readonly IMediator _mediator;

        public UnitsService(IHttpContextAccessor _httpContext, IMediator mediator) : base(_httpContext)
        {
            _mediator = mediator;
        }

        public async Task<ResponseResult> AddUnit(AddUnitRequest parameter)
        {
            return await _mediator.Send(parameter);
        }
        public async Task<ResponseResult> GetListOfUnits(GetListOfUnitsRequest parameters)
        {
            return await _mediator.Send(parameters);
        }
        public async Task<ResponseResult> UpdateUnits(UpdateUnitsRequest parameters)
        {

            return await _mediator.Send(parameters);

        }
        public async Task<ResponseResult> UpdateStatus(UpdateStatusRequest parameters)
        {
            return await _mediator.Send(parameters);

        }
        public async Task<ResponseResult> DeleteUnits(DeleteKitchensRequest parameter)
        {

            return await _mediator.Send(parameter);
        }
        public async Task<ResponseResult> GetUnitHistory(int Code)
        {
           return await _mediator.Send(new GetKitchensHistoryRequest { Code = Code });
        }
        public async Task<ResponseResult> GetUnitsDropDown()
        {
            return await _mediator.Send(new GetUnitsDropDownRequest());
        }

        public async Task<ResponseResult> GetUnitsByDate(GetUnitsByDateRequest parameter)
        {
            return await _mediator.Send(parameter);
        }
    }
}

