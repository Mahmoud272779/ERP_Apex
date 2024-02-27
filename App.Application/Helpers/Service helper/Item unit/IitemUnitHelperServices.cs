using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.Service_helper.Item_unit
{
    public interface IitemUnitHelperServices
    {
         Task<RptUnitDataResponse> getRptUnitData(int itemId);
    }
}
