using App.Domain.Models.Security.Authentication.Response;

namespace App.Infrastructure.Interfaces
{
    public interface IApplicationUser
    {
        GetUserDataResponse _ApplicationUser { get; set; }

    }
}
