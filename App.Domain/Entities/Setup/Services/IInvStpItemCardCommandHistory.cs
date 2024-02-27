using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Setup.Services
{
    public interface IInvStpItemCardCommandHistory
    {
        void AddHistory(InvStpItemCardHistory invStpItemCardHistory);
    }
}
