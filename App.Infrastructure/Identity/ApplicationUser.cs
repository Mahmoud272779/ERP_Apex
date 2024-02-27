namespace App.Infrastructure.Identity
{
    public class ApplicationUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FULL_NAME_ARABIC { get; set; }
        public string FULL_NAME_ENGLISH { get; set; }
        public string SHORT_NAME_ARABIC { get; set; }
        public string SHORT_NAME_ENGLISH { get; set; }
        public string BRANCH_CODE { get; set; }

        public string Token { get; set; }

        public int BRANCH_ID { get; set; }
        public int ModuleId { get; set; }
    }
}
