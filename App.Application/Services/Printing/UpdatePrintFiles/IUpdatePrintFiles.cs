using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.UpdatePrintFiles
{
    public interface IUpdatePrintFiles
    {
        Task<ResponseResult> UpdatePrintFiles();
    }
}
