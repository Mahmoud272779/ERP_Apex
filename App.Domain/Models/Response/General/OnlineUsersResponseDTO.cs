using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.General
{
    public class OnlineUsersResponseDTO
    {
        public string companyLogin { get; set; }
        public string databaseName { get; set; }
        public int onlineCount { get; set; }
        public List<OnlineUsers> users { get; set; }
    }
    public class OnlineUsers
    {
        public int UserID { get; set; }
        public int EmployeeId { get; set; }
        public int isTechSupport { get; set; }
    }
}
