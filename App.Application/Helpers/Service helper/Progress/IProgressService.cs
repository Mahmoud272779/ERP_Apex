using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IProgressService
    {
        
        public int progresscount { get; set; }
        public string endTask { get; set; }
        Task<ResponseResult> MainTask();
    }
}
