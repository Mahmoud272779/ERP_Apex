using App.Application.Helpers.UpdateSystem.Updates;
using App.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.UpdatePrintFiles
{
    public class UpdatPrintFiles : IUpdatePrintFiles
    {
        private readonly ClientSqlDbContext _clientcontext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UpdatPrintFiles(ClientSqlDbContext clientcontext, IWebHostEnvironment webHostEnvironment)
        {
            _clientcontext = clientcontext;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<ResponseResult> UpdatePrintFiles()
        {
            try
            {
               await ReportFilesUpdate.UpdatePrintFiles(_clientcontext, _webHostEnvironment);
                await ReportFilesUpdate.AddPrintFiles(_clientcontext, _webHostEnvironment);

                return new ResponseResult { Result = Result.Success };
            }
            catch (Exception ex)
            {
                return new ResponseResult { Result = Result.Failed };

            }
        }
    }
}
