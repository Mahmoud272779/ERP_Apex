using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Handlers.NotificationSystem.InsertNotification
{
    public class InsertNotificationRequest : IRequest<ResponseResult>
    {
        public string title { get; set; }
        public string Desc { get; set; }
        public string titleEn { get; set; }
        public string DescEn { get; set; }
        public bool isSystem { get; set; }
        public int? specialUserId { get; set; }
        public DateTime cDate { get; set; }
        public int pageId { get; set; }
        public int notificationTypeId { get; set; }
        public bool hasRedirection { get; set; }
        public string? routeUrl { get; set; }

    }
}
