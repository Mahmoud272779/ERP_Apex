using App.Domain.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface IPersonHelperService
    {
        public Task<bool> checkIsCustomer(int personId);
        public Task<bool> checkIsSuppler(int personId);
        
    }
}
