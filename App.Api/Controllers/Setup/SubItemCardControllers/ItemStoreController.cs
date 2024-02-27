using App.Api.Controllers.BaseController;
using App.Domain.Models.Setup;
using App.Domain.Models.Setup.ItemCard.Request;
using App.Domain.Models.Setup.ItemCard.Response;
using App.Domain.Models.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace App.Api.Controllers.Setup.SubItemCardControllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ItemStoreController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        public ItemStoreController(IMediator mediator) : base(mediator) => _mediator = mediator;
        [Route("ItemStore/GetAll")]
        
        [HttpGet]
        public async Task<ActionResult<List<GetAllItemStoreResponse>>> GetAll(int pageIndex, int pageSize,int ItemId)
        {
            return await _mediator.Send(new GetAllItemStoreRequest(pageIndex, pageSize,ItemId));
        }
        [Route("ItemStore/Add")]
        
        [HttpPost]
        public async Task<ActionResult<ResponseResult>> Add([FromBody] AddItemStoreRequest request)
        {
            return await _mediator.Send(request);
        }
        [Route("ItemStore/Update")]
        
        [HttpPut()]
        public async Task<ActionResult<ResponseResult>> Update([FromBody] UpdateItemStoreRequest request)
        {
            return await _mediator.Send(request);
        }
        [Route("ItemStore/Delete")]
        [HttpDelete()]
        public async Task<ActionResult<ResponseResult>> Delete(DeleteItemStoreRequest request)
        {
            return await _mediator.Send(request);
        }
    }
}
