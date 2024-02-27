using App.Domain.Models.Security.Authentication.Response;
using App.Infrastructure;
using App.Infrastructure.settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace App.Application   
{
    public class authService : iAuthService
    {


        public async Task<JwtAuthResponse> getAuthToken(string employeeId, userInfo userInfo, string RoleID,string databaseName,string EndPeriodOnEndPeriodOn,string companyLogin,bool isPOSDesktop, bool isPeriodEnded, bool isTechnicalSupport)
        {
            var encreptedRuleID = StringEncryption.EncryptString(RoleID);
            var encreptedUserID = StringEncryption.EncryptString(userInfo.userId);
            var dbName = StringEncryption.EncryptString(databaseName);
            var encryptedEmployeeId = StringEncryption.EncryptString(employeeId);
            var CL = StringEncryption.EncryptString(companyLogin);

            var claims = new[]
            {
                new Claim("RoleDetails",encreptedRuleID),
                new Claim("DBname",dbName),
                new Claim("userID",encreptedUserID),
                new Claim("EndPeriodOnEndPeriodOn",EndPeriodOnEndPeriodOn),
                new Claim("employeeId",encryptedEmployeeId),
                new Claim("CL",CL),
                new Claim("isPOSDesktop",(isPOSDesktop? "1":"0")),
                new Claim("isTechnicalSupport",(isTechnicalSupport? "1":"0")),
                new Claim("isPeriodEnded",isPeriodEnded.ToString()),

            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(defultData.JWT_SECURITY_KEY));
            var expiryInHours = DateTime.Now.AddHours(defultData.JWT_TOKEN_VALIDAIT_Time);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = tokenHandler.WriteToken(new JwtSecurityToken
                (
                    issuer: defultData.site,
                    audience: defultData.site,
                    expires: expiryInHours,
                    signingCredentials: credentials,
                    claims: claims
                ));
            if (isTechnicalSupport)
            {
                userInfo.permissionNameAr = "دعم فني";
                userInfo.permissionNameEn = "Technical Support";
                userInfo.arabicName =  "دعم فني";
                userInfo.latinName = "Technical Support";
            }

                return new JwtAuthResponse()
            {
                expires_in = expiryInHours,
                token = token.ToString(),
                UserInfo = userInfo
            };
        }
    }
}
