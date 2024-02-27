using MediatR;

namespace App.Application.Handlers.Persons
{
    public class SupplierCutomerReportRequest : PersonsSearch, IRequest<WebReport>
    {
        public bool isArabic { get; set; }
        public exportType exportType { get; set; }
        public string ids { get; set; }
        public bool isSearchData { get; set; } = true;
        public int fileId { get; set; }
        public string? SalesManId { get; set; }
    }
}
