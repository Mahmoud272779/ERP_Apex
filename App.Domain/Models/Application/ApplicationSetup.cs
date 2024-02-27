namespace App.Domain.Models.Application
{
    public class ApplicationSetup
    {
        public string JWT_Secret { get; set; }
        public string SecURL { get; set; }
        public string GLURL { get; set; }
        public string AcpURL { get; set; }
        public string AcrURL { get; set; }

    }
}
