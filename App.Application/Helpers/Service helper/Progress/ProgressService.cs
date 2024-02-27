using App.Application;
using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Helpers.Progress
{
    public class ProgressService : IProgressService
    {
        public int progresscount { get; set; } 
        public string endTask { get; set; }
        public ProgressService()
        {

        }

         

        public async Task<ResponseResult> MainTask()
        {

            int  lenght = 100;
            progresscount = lenght;
            for (int i = 0; i < lenght; i++)
            {
                progresscount --;
                System.Threading.Thread.Sleep(100);
                Console.WriteLine("hamada "+i.ToString());
           //     System.Diagnostics.Debug.WriteLine("hamada " + i.ToString());
                App.Application.DI.AppDI.ProgressCount = i;
                
            }
            endTask = "finshed";
            return new ResponseResult() { Data = lenght+9000  };
        }


    }
}
