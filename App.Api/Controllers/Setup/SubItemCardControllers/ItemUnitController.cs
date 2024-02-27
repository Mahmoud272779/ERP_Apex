using App.Api.Controllers.BaseController;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Api.Controllers.Setup.SubItemCardControllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ItemUnitController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ItemUnitController(IMediator mediator) : base(mediator) => _mediator = mediator;
        [Route("ItemUnit/GetAll")]
        
        [HttpGet]
        public async Task<ActionResult<List<GetAllItemUnitResponse>>> GetAll(int pageIndex, int pageSize,int ItemId)
        {
            return await _mediator.Send(new GetAllItemUnitRequest(pageIndex, pageSize,ItemId));
        }
        [Route("ItemUnit/Add")]
        
        [HttpPost]
        public async Task<ActionResult<ResponseResult>> Add([FromBody] AddItemUnitRequest request)
        {
            return await _mediator.Send(request);
        }
        [Route("ItemUnit/Update")]
        
        [HttpPut()]
        public async Task<ActionResult<ResponseResult>> Update([FromBody] UpdateItemUnitRequest request)
        {
            return await _mediator.Send(request);
        }
        [Route("ItemUnit/Delete")]
        [HttpDelete()]
        public async Task<ActionResult<ResponseResult>> Delete(DeleteItemUnitRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}
