using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Interfaces.Repository
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
