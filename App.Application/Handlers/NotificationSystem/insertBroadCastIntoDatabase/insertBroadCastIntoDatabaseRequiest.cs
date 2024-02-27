using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.insertBroadCastIntoDatabase
{
    public class insertBroadCastIntoDatabaseRequiest : IRequest<bool>
    {
        public string dbName { get; set; }
        public int emplployeeId { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public bool isSystem { get; set; }
    }
}
