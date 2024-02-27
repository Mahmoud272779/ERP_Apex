using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.PrintPermission
{
    public class PermissionsForPrint : IPermissionsForPrint
    {
        private readonly IRepositoryQuery<rules> _rulesQuery;
        public PermissionsForPrint(IRepositoryQuery<rules> rulesQuery)
        {
            _rulesQuery = rulesQuery;
        }
        public async Task<PermissionsResponse> GetPermisions(int permissionListId, int subFormsCode)
        {
            var permission = _rulesQuery.TableNoTracking.Where(r => r.permissionListId == permissionListId && r.subFormCode == subFormsCode)
                .Select(x => new PermissionsResponse()
                {
                    IsPrint = x.isPrint,
                    PermissionListId = permissionListId
                }).FirstOrDefault();
            return permission;
        }
    }
}
