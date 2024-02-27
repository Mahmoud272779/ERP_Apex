using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Response.General
{
    public class GetUserNotificationsResponseObject
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string titleEn { get; set; }
        public string descEn { get; set; }
        public bool hasRedirection { get; set; }
        public string routeURL { get; set; }
        public bool seen { get; set; }
        public int notificationType { get; set; }
        public string date { get; set; }
    }
    public class GetUserNotificationsResponseMasterObject
    {
        public List<GetUserNotificationsResponseObject> Items { get; set; }
        public int notificationNotSeenCount { get; set; }
    }

    public class ResponseSignalR
    {
        public int Id { get; set; }
        public int notificationTypeId { get; set; }
        public string title { get; set; }
        public string desc { get; set; }
        public string titleEn { get; set; }
        public string descEn { get; set; }
        public string RouteURL { get; set; }
        public int notificationNotSeenCount { get; set; }
        public string date { get; set; }
        public bool hasRedirection { get; set; }
    }
}
