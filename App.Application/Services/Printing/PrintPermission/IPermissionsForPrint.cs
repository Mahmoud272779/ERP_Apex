using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.Printing.PrintPermission
{
    public interface IPermissionsForPrint
    {
        public Task<PermissionsResponse> GetPermisions(int permissionListId, int subFormsCode);

    }
}

public class PermissionsResponse
{
    public bool IsPrint { get; set; }
    public int PermissionListId { get; set; }
}
