using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Entities
{
    public class permissionList
    {
        public int Id { get; set; }
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string? note { get; set; }
        public DateTime UTime { get; set; }
        public ICollection<userAccount> userAccount { get; set; }
        public ICollection<UserAndPermission> UserAndPermission { get; set; }
        public ICollection<rules> rules { get; set; }
    }

    public class UserAndPermission
    {
        public int Id { get; set; }
        public int userAccountId { get; set; }
        public int permissionListId { get; set; }
        public DateTime UTime { get; set; }
        public permissionList permissionList { get; set; }
        public userAccount userAccount { get; set; }

    }
}
