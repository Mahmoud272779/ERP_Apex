using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Notification
{
    public class NotificationsMaster
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string Desc { get; set; }
        public string titleEn { get; set; }
        public string DescEn{ get; set; }
        public bool isSystem { get; set; }
        public int? pageId { get; set; }
        public int? specialUserId { get; set; }
        public DateTime cDate { get; set; }
        public InvEmployees specialUser { get; set; }
        public string? routeUrl { get; set; }

        //
        public int? insertedById { get; set; }
        public InvEmployees insertedBy { get; set; }
    }
}
