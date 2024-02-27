using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.CookiesAppend
{
    public interface ICookiesService
    {
        public Task<Tuple<string, string>> ProjectVersion();
        public Task<bool> isUpToDate();
    }
}
