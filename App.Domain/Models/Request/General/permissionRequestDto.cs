using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models.Request.General
{
    public class addPermissionRequestDto
    {
        public string arabicName { get; set; }
        public string latinName { get; set; }
        public string? note { get; set; }
        public DateTime UTime { get; set; }
    }
    public class editPermissionRequestDto : addPermissionRequestDto
    {
        public int Id { get; set; }
    }
    public class deletePermissionRequestDto
    {
        public int[] Ids { get; set; }
    }
    public class getPermissionRequestDto
    {
        public int PageSize { get; set; } = 5;
        public int PageNumber { get; set; } = 1;
        public string SearchCriteria { get; set; }
    }
    public class addUsersToPermissionListRequestDto
    {
        public int permissionListId { get; set; }
        public int[] userIds { get; set; }
    }
    public class updatedRules
    {
        public int id { get; set; }
        public bool isShow { get; set; }
        public bool isAdd { get; set; }
        public bool isEdit { get; set; }
        public bool isDelete { get; set; }
        public bool isPrint { get; set; }

        //public string arabicName { get; set; }
        //public string latinName { get; set; }
        //public string mainFormCode { get; set; }
        //public string subFormCode { get; set; }
        //public int permissionListId { get; set; }
    }
    public class editRulesRequestDto
    {
        public List<updatedRules> updatedRules { get; set; }
    }
    
    public class getPermissionListUsers
    {
        public int permissionListId { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 5;
        public string SearchCriteria { get; set; }
    }

}
