using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using App.Domain.Models.Shared;
using MediatR;
using App.Application.Handlers.SignalRHandler;
using App.Infrastructure.settings;
using App.Application.Helpers.DemandLimitNotificationSystem;
using Hangfire;
using SelectPdf;

namespace App.Api.Controllers.AllowedApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMediator _imediator;
        private readonly IDemandLimitNotificationService _demandLimitNotificationService;

        public HelpersController(Microsoft.Extensions.Configuration.IConfiguration configuration, IMediator mediator, IDemandLimitNotificationService demandLimitNotificationService)
        {
            _configuration = configuration;
            _imediator = mediator;
            _demandLimitNotificationService = demandLimitNotificationService;
        }
        [AllowAnonymous]
        [HttpGet("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string FileName)
        {
            var fullPath = System.IO.Path.Combine(_configuration["ApplicationSetting:FilesRootPath"], FileName);

            
            var fileBytesArr = System.IO.File.ReadAllBytes(fullPath);

            var extension = System.IO.Path.GetExtension(fullPath);
            var applicationContant = string.Empty;
            switch (extension)
            {
                case ".docx":
                    applicationContant = " application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case ".xlsx":
                    applicationContant = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case ".svg":
                    applicationContant = "image/svg+xml";
                    break;
                case ".pdf":
                    applicationContant = "application/pdf";
                    break;
                case ".Jpeg":
                    applicationContant = "image/jpeg";
                    break;
                case ".zip":
                    applicationContant = "application/zip";
                    break;
            }


            return File(fileBytesArr, applicationContant, $"{FileName}");
        }

        //[AllowAnonymous]
        //[HttpGet("DownloadExcel")]
        //public async Task<IActionResult> DownloadExcel(string FileName)
        //{

           

        //}


           

        [AllowAnonymous]
        [HttpGet("ChangeUpdateNumber")]
        public async Task<IActionResult> ChangeUpdateNumber([FromQuery]int updateNumber, [FromQuery] string Password)
        {
            if(Password != Defults.ChangeUpdateNumberValuePassword)
                return BadRequest(ModelState);
            var path = Path.Combine(Environment.CurrentDirectory, "wwwroot", "updateNumber.txt");
            var isFileExist = System.IO.File.Exists(path);
            if (!isFileExist)
            {
                System.IO.File.Create(path).Close();
            }
            System.IO.File.WriteAllText(path, updateNumber.ToString());
            return Ok();
        }
        [Authorize]
        [HttpPost("SetSignalRConnectionId")]
        public async Task<IActionResult> SetSignalRConnectionId([FromBody] SignalRHandlerRequest request)
        {
            var response = await _imediator.Send(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("RunNotificationForDemandLimit")]
        public async Task<IActionResult> RunNotificationForDemandLimit([FromBody] string SecurityKey)
        {
            if(SecurityKey != defultData.userManagmentApplicationSecurityKey) return BadRequest("Security Key is not correct");
            RecurringJob.AddOrUpdate(()=> _demandLimitNotificationService.DemandLimitNotificationServ().Wait(), Cron.Hourly(60));
            return Ok();
        }
        [AllowAnonymous]
        [HttpPost("stringDecodingic")]
        public async Task<IActionResult> stringDecoding([FromBody] string SecurityKey)
        {
            var stringDecoding = StringEncryption.DecryptString(SecurityKey);
            return Ok(stringDecoding);
        }
    }
    
}
