using Attendleave.Erp.Core.APIUtilities;
using AutoMapper;

namespace App.Application.Basic_Process
{
    public class BusinessBase<T> where T : class
    {
        protected readonly IRepositoryActionResult repositoryActionResult;
        protected readonly IMapper Mapper;
        public BusinessBase(IRepositoryActionResult RepositoryActionResult)
        //, IBusinessBaseParameter<T> businessBaseParameter
        {
            repositoryActionResult = RepositoryActionResult;
            //Mapper = businessBaseParameter.Mapper;
        }
        public string PrepareSearchCreteria(string searchCriteria)
        {
            return string.IsNullOrEmpty(searchCriteria) ? string.Empty : searchCriteria.ToLower();
        }
 
    }
}
