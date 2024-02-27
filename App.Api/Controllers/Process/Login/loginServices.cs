//using System.Threading.Tasks;
//using App.Api.Controllers.BaseController;
//using App.Application.Services.Process.Color;
//using App.Domain.Models.Security.Authentication.Request;
//using App.Domain.Models.Shared;
//using Attendleave.Erp.Core.APIUtilities;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;

//namespace App.Api.Controllers.Process
//{
//    public class loginServices : ApiStoreControllerBase
//    {
//        private readonly IColorsService ColorsService;
//        public loginServices(IColorsService _ColorsService,
//                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
//        {
//            ColorsService = _ColorsService;
//        }

//        [AllowAnonymous]
//        [HttpPost(nameof(AddColor))]
//        public async Task<ResponseResult> AddColor(ColorsParameter parameter)
//        {
//            var result = await ColorsService.AddColor(parameter);
//            return result;
//        }



//        [AllowAnonymous]
//        [HttpGet("GetListOfColors")]

//        public async Task<ResponseResult> GetListOfColors(int PageNumber, int PageSize, string? Name, int status)
//        {
//            ColorsSearch paramters = new ColorsSearch()
//            {

//                PageNumber = PageNumber,
//                PageSize = PageSize,
//                Name = Name,
//                Status = status

//            };
//            var result = await ColorsService.GetListOfColors(paramters);
//            return result;
//        }
//        [AllowAnonymous]
//        [HttpPut(nameof(UpdateColor))]
//        public async Task<ResponseResult> UpdateColor(UpdateColorParameter parameters)
//        {
//            var result = await ColorsService.UpdateColors(parameters);
//            return result;
//        }
//        [AllowAnonymous]
//        [HttpPut(nameof(UpdateActiveColor))]
//        public async Task<ResponseResult> UpdateActiveColor(SharedRequestDTOs.UpdateStatus parameters)
//        {
//            var result = await ColorsService.UpdateStatus(parameters);
//            return result;
//        }
//        [AllowAnonymous]
//        [HttpDelete("DeleteColors")]
//        public async Task<ResponseResult> DeleteColors([FromBody] int[] Id)
//        {
//            SharedRequestDTOs.Delete parameter = new SharedRequestDTOs.Delete()
//            {
//                Ids = Id
//            };
//            var result = await ColorsService.DeleteColors(parameter);
//            return result;

//        }

//        [AllowAnonymous]
//        [HttpGet("GetColorHistory")]
//        public async Task<ResponseResult> GetColorHistory(int Id)
//        {
//            var result = await ColorsService.GetColorHistory(Id);
//            return result;
//        }

//    }
//}
