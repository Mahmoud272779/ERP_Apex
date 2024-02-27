using App.Domain.Models.Shared;
using App.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UpdateSystem.Services
{
    public interface iUpdateService
    {
       Task UpdateDatabase(ClientSqlDbContext dbContext, IWebHostEnvironment webHostEnvironment ,string UpdateDatabase);
       Task updateFile(ClientSqlDbContext dbContext, int updateFilesNumber);

       
    }
}
