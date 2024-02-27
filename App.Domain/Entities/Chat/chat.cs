using App.Domain.Entities.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities.Chat
{
    public class chat
    {
        public class chatMessages
        {
            public int Id { get; set; }
            public int fromId { get; set; }
            public int? toId { get; set; }
            public int? groupId { get; set; }
            public string message { get; set; }
            public DateTime date { get; set; }
            public bool seen { get; set; }
            public DateTime seenDate { get; set; }
            public bool isDeleted { get; set; } = false;
            public DateTime deleteDate{ get; set; }
            public bool isPrivate { get; set; }
            public InvEmployees InvEmployeesFrom { get; set; }
            public InvEmployees InvEmployeesTo { get; set; }
            public chatGroups group { get; set; }

        }
        public class chatGroups
        {
            public int Id { get; set; }
            public int groupCreatorId { get; set; }
            public bool allowReply { get; set; }
            public string groupName { get; set; }
            public string groupImage { get; set; }
            public DateTime creationDate { get; set; }
            public bool isEnded { get; set; }
            public bool canExit { get; set; } = true;
            public InvEmployees groupCreator { get; set; }
            public ICollection<chatMessages> chatMessages { get; set; }
            public ICollection<chatGroupMembers> chatGroupMembers { get; set; }
        }
        public class chatGroupMembers
        {
            public int Id { get; set; }
            public int groupId { get; set; }
            public int memberId { get; set; }
            public bool isAdmin { get; set; }
            public InvEmployees invEmployeeMember { get; set; }
            public chatGroups group { get; set; }
        }
    }
}
