using System.Collections.Generic;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Setup.SubItemCardControllers
{
    public class ItemPartController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ItemPartController(IMediator mediator) : base(mediator) => _mediator = mediator;
        [Route("ItemPart/GetAll")]
        
        [HttpGet]
        public async Task<ActionResult<List<GetAllItemPartResponse>>> GetAll(int pageIndex, int pageSize,int itemId)
        {
            return await _mediator.Send(new GetAllItemPartRequest(pageIndex, pageSize, itemId));
        }
        [Route("ItemPart/Add")]
        
        [HttpPost]
        public async Task<ActionResult<ResponseResult>> Add([FromBody] AddItemPartRequest request)
        {
            return await _mediator.Send(request);
        }
        [Route("ItemPart/Update")]
        
        [HttpPut()]
        public async Task<ActionResult<ResponseResult>> Update([FromBody] UpdateItemPartRequest request)
        {
            return await _mediator.Send(request);
        }
        [Route("ItemPart/Delete")]
        [HttpDelete()]
        public async Task<ActionResult<ResponseResult>> Delete(DeleteItemPartRequest request)
        {
            return await _mediator.Send(request);
        }

    }
}
