using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace App.Infrastructure.EmailService
{
    public interface IMailKite
    {
        Task SendEmailAsync(string sender, string senderPassword, string companyName, string email, string employeeName, string subject, string message, string hostName, int PortNumber);
    }
}
