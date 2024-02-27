using App.Application;
using App.Domain.Entities.Process;
using App.Domain.Models.Shared;
using App.Infrastructure.Interfaces.Repository;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.Helpers.Progress
{
    public class PersonHelperService : IPersonHelperService
    {
        private readonly IRepositoryQuery<InvPersons> _invPersonsQuery;

        public PersonHelperService(IRepositoryQuery<InvPersons> InvPersonsQuery)
        {
            _invPersonsQuery = InvPersonsQuery;
        }

        public async Task<bool> checkIsCustomer(int personId)
        {
            var checkPerson = _invPersonsQuery.TableNoTracking.Where(x => x.Id == personId);
            if (checkPerson.Any())
                return checkPerson.FirstOrDefault().IsCustomer;
            return false;
        }

        public async Task<bool> checkIsSuppler(int personId)
        {
            var checkPerson = _invPersonsQuery.TableNoTracking.Where(x => x.Id == personId);
            if (checkPerson.Any())
                return checkPerson.FirstOrDefault().IsSupplier;
            return false;
        }
    }
}
