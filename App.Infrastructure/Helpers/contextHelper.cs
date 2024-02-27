using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace App.Infrastructure
{
    public class contextHelper
    {
        public static Dictionary<string, string> DecodingToken(string token)
        {
                var TokenInfo = new Dictionary<string, string>();
            try
            {


                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(token);
                var claims = jwtSecurityToken.Claims.ToList();

                foreach (var claim in claims)
                {
                    TokenInfo.Add(claim.Type, claim.Value);
                }

            }
            catch (Exception)
            {

            }
                return TokenInfo;
           
        }
        public static string getToken(IHttpContextAccessor _httpContext)
        {
            var token = _httpContext.HttpContext.GetTokenAsync("access_token").Result;
            return token;
        }
        public static string GetDBName(string Token)
        {
            if(string.IsNullOrEmpty(Token))
            {
                return string.Empty;
            }
            var username = DecodingToken(Token);
            return username["DBname"];
        }
        public static string CompanyLogin(string Token)
        {
            if (Token == null)
                return Token;
            var username = DecodingToken(Token);
            return username["CL"];
        }
        public static string GetEmployeeId(string Token)
        {
            var username = DecodingToken(Token);
            return username["employeeId"];
        }
        public static string GetUserID(string Token)
        {
            var username = DecodingToken(Token);
            return username["userID"];
        }
        public static string updateNumber(string Token)
        {
            var username = DecodingToken(Token);
            return StringEncryption.DecryptString(username["updateNumber"]);
        }
        public static string isPOSDesktop(string Token)
        {
            var username = DecodingToken(Token);
            return username["isPOSDesktop"];
        }
        public static string isPeriodEnded(string Token)
        {
            var username = DecodingToken(Token);
            return username["isPeriodEnded"];
        }
        public static string checkIsTechnicalSupport(string Token)
        {
            var IsTechnicalSupport = DecodingToken(Token);
            return IsTechnicalSupport["isTechnicalSupport"];
        }
        public static string GetBrowserName(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return string.Empty;
            var uaParser = Parser.GetDefault();
            ClientInfo c = uaParser.Parse(userAgent);
            var Browsername = c.UA.Family.ToString();
            return Browsername;
        }
    }
}
