namespace App.Domain.Models.Shared
{
    public class SetupVM
    {
        public int ID { get; set; }
        public string CODE { get; set; }
        public string LATIN_NAME { get; set; }
        public string ARABIC_NAME { get; set; }
        public string ACTIVE { get; set; }
        public int REASON_ID { get; set; }
        public int? CHART_OF_ACCOUNT_ID { get; set; }
        public int? COST_CENTER_ID { get; set; }

    }
}
