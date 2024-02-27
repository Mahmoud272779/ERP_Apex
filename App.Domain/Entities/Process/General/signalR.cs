using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Process.General
{
    public class signalR
    {
        public int Id { get; set; }
        public string connectionId { get; set; }
        public int InvEmployeesId { get; set; }
        public bool isOnline { get; set; }
        public InvEmployees InvEmployees { get; set; }
    }
}
