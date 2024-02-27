using App.Domain.Common;

namespace App.Domain.Models.Common
{
    public class HistoryParameter : AuditableEntity
    {
        public int EntityId { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
        public string BrowserName { get; set; }
        public int employeesId { get; set; }

    }
}
