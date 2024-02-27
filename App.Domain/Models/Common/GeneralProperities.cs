using App.Domain.Common;

namespace App.Domain.Models.Common
{
    public class GeneralProperities: AuditableEntity
    {
        public int Id { get; set; }
        public string LatinName { get; set; }
        public string ArabicName { get; set; }
    }
}
