using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Helpers.UsermanagerHelper
{
    public class GetDataFromUserManager
    {
        private readonly SqlConnection _con;

        public GetDataFromUserManager(IConfiguration _configuration)
        {
            _con = new SqlConnection(_configuration["ConnectionStrings:UserManagerConnection"]);
        }

    }
}
