using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class NotificationRequestDTO
    {
        public string title { get; set; }
        public string Desc { get; set; }
        public string titleEn { get; set; }
        public string DescEn { get; set; }
        public int? specialUserId { get; set; }
        public int notificationTypeId { get; set; }
    }
}
