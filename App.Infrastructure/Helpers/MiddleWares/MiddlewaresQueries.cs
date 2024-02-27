using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure
{
    public static class MiddlewaresQueries
    {
        public static string usrTokens(string userId) => 
             $"SELECT [Id]"+
             $",[userAccountid]"+
             $",[token]"+
             $",[signinDateTime]"+
             $",[isLogout]"+
             $",[logoutDateTime]"+
             $",[isLocked]"+
             $",[lastActionTime]"+
             $",[stayLoggedin]" +
             $" FROM [signinLogs] where userAccountid = '{userId}'";
        public static string usrTokens_FilterWithToken(string token) =>
             $"SELECT [Id]" +
             $",[userAccountid]" +
             $",[token]" +
             $",[signinDateTime]" +
             $",[isLogout]" +
             $",[logoutDateTime]" +
             $",[isLocked]" +
             $",[lastActionTime]" +
             $" FROM [signinLogs] where token = '{token}'";
        public static string isTokenValid(string Token) => 
             $"SELECT userAccount.[id]" +
             $",userAccount.[username]" +
             $",userAccount.[password]" +
             $",userAccount.[email]" +
             $",userAccount.[isActive]" +
             $",userAccount.[employeesId]" +
             $",userAccount.[permissionListId]" +
             $"FROM [userAccount] " +
             $"join signinLogs on signinLogs.userAccountid = userAccount.id " +
             $"where signinLogs.token = '{Token}'";


        public static string InvGeneralSettings_autoLogoutInMints() => "SELECT [autoLogoutInMints] FROM [InvGeneralSettings]";
        public static string updateIsLocked_signinLogs(int isLocked, int logId) => $"UPDATE [signinLogs] SET [isLocked] = {isLocked} WHERE Id = {logId}";
        public static string updateLastActionTime_signinLogs(int logId) => $"UPDATE [dbo].[signinLogs] SET [lastActionTime] = GETDATE() WHERE Id = {logId}";
    }
}
