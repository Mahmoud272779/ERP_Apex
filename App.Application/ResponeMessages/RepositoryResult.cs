using App.Domain.Models.Shared;
using System;
using System.Net;
namespace Attendleave.Erp.Core.APIUtilities
{
    public class RepositoryResult : IRepositoryResult
    {
        public object Data { get; set; }
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string dateTimeNow { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
        public int updateNumber { get; set; } = Defults.updateNumber;
    }
}
