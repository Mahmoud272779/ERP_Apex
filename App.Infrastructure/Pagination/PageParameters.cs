using System.Collections.Generic;

namespace Attendleave.Erp.ServiceLayer.BaseDto
{
    public class PageParameter 
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchCriteria { get; set; }
        public List<int> SelectedId { get; set; }

    }
    public class PageParameterPermission
    {
        public int []GroupId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchCriteria { get; set; }
        public List<int> SelectedId { get; set; }

    }
    public class PageParameterParents
    {
        public int [] ParentIds { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchCriteria { get; set; }
        public List<int> SelectedId { get; set; }

    }

}
