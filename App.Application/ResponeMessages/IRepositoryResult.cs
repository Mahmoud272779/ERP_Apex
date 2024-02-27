using System;
using System.Net;
namespace Attendleave.Erp.Core.APIUtilities
{
    public interface IRepositoryResult
    {
        object Data { get; set; }
        HttpStatusCode Status { get; set; }
        string Message { get; set; }
        bool Success { get; set; }
        string dateTimeNow { get; set; }
        int updateNumber { get; set; }
    }
}
