using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Api.Controllers.BaseController;
using App.Application;
using App.Application.Services.Process.Category;
using App.Domain.Models.Common;
using App.Domain.Models.Security.Authentication.Request;
using App.Domain.Models.Shared;
using Attendleave.Erp.Core.APIUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace App.Api.Controllers.Process
{
    public class progressBarController : ApiStoreControllerBase
    {
        private readonly IHubContext<HubConfig> _hub;
        private readonly TimerManager _timer;
        private readonly IProgressService ProgressService;
        public progressBarController(IProgressService _ProgressService, IHubContext<HubConfig> hub, TimerManager timer,
                        IActionResultResponseHandler ResponseHandler) : base(ResponseHandler)
        {
            ProgressService = _ProgressService;
            _hub = hub;
            _timer = timer;
        }




        [HttpGet("GetData")]

        public async Task<ResponseResult> GetData()
        {

            var result = await ProgressService.MainTask();

            return result;
        }

        //[HttpGet("GetProgressData")]

        //public async Task<ResponseResult> GetProgressData()
        //{

        //    var result = new ResponseResult()
        //    {
        //        Data = App.Application.DI.AppDI.ProgressCount
        //    };

        //    return result;
        //}
        //[HttpGet]
        //    public async Task<ResponseResult> Get()
        //    {
        //        string[] data = new string[] {
        //    "Hello World!",
        //    "Hello Galaxy!",
        //    "Hello Universe!"
        //};

        //        Response.Headers.Add("Content-Type",
        //    "text/event-stream");

        //        for (int i = 0; i < data.Length; i++)
        //        {
        //            await Task.Delay(TimeSpan.FromSeconds(5));
        //            string dataItem = $"data: {data[i]}\n\n";
        //            byte[] dataItemBytes =
        //    ASCIIEncoding.ASCII.GetBytes(dataItem);
        //            await Response.Body.WriteAsync
        //    (dataItemBytes, 0, dataItemBytes.Length);

        //            await Response.Body.FlushAsync();
        //        }
        //        return new ResponseResult() { Data= 10000};
        //    }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            
            if (!_timer.IsTimerStarted)
                _timer.PrepareTimer(() => _hub.Clients.All.SendAsync("TransferChartData", ProgressService.progresscount));
            var result = await ProgressService.MainTask();
            _timer.TimerStop(ProgressService.progresscount);
            return Ok(new { Message = "Request Completed",value=result });
        }
        [HttpGet ("Lengthy")]
        public async Task<IActionResult> Lengthy()
        {
            var result = await ProgressService.MainTask();
            await _hub
                .Clients
                .Group(HubConfig.GROUP_NAME)
                .SendAsync("taskStarted");

            for (int i = 0; i < 100; i++)
            {
                //Thread.Sleep(200);
                Debug.WriteLine($"progress={i + 1}");
                await _hub
                    .Clients
                    .Group(HubConfig.GROUP_NAME)
                    .SendAsync("TransferChartData", i + 1);
            }

            await _hub
                .Clients
                .Group(HubConfig.GROUP_NAME)
                .SendAsync("taskEnded");

            return Ok(result);
        }
    }
}
