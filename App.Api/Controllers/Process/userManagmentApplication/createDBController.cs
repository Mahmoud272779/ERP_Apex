using App.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Api.Controllers.Process.userManagmentApplication
{
    [Route("api/userManagmentApplication/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class createDBController : ControllerBase
    {
        private readonly iDBCreation _iDBCreation;

        public createDBController(iDBCreation iDBCreation)
        {
            _iDBCreation = iDBCreation;
        }
        [HttpPost("createDB")]
        public async Task<IActionResult> createDB([FromBody] dbCreationRequest parm)
        {
            var create = await _iDBCreation.createClientDB(parm);
            return Ok(create);
        }
        [HttpPost("AddReports")]
        public async Task<IActionResult> creatAddReportseDB([FromBody] string dbName)
        {
            var create = await _iDBCreation.AddReports(dbName);
            return Ok(create);
        }
        [HttpPost("ReplaceFiles")]
        public async Task<IActionResult> ReplaceFiles([FromBody] string dbName)
        {
            var create = await _iDBCreation.ReplaceFiles(dbName);
            return Ok(create);
        }
        [HttpGet("GenerateDatabaseScript")]
        public async Task<IActionResult> GenerateDatabaseScript([FromQuery]string key)
        {
            if (key != "(&**^&*%^&*JHKJH&^%^&JH&%^&BJHBPI*Y!6546465465465465465465465465@KJAHSKBASHHAS")
                return BadRequest();
            var script = await _iDBCreation.getDatabaseScript(key);
            return Ok(script);
        }

    }
}
