namespace App.Infrastructure.Pagination
{
    public class PageParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SearchCriteria { get; set; }
    }
}
