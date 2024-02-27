using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application
{
    public interface iDBCreation
    {
        public Task<dbCreationResponse> createClientDB(dbCreationRequest parm);
        public Task<string> AddReports(string dbName);
        public Task<string> ReplaceFiles(string dbName);
        public Task<string> getDatabaseScript(string key);
    }
    public class dbCreationResponse
    {
        public bool isCreated{ get; set; }
        public string message { get; set; }
    }
    public class dbCreationRequest
    {
        public string dbName { get; set; }
        public string securityKey { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string vatNo { get; set; }
        public string companyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string CompanyActivity { get; set; }
    }
}
