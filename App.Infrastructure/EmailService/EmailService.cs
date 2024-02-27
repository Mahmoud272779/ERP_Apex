using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MimeKit.Text;

namespace App.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(subject, message, email);
        }

        public async Task Execute(string subject, string messageBody, string email)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Taif Alalmas", "apexweb@taif-alalmas.com"));// get from company data
            message.To.Add(new MailboxAddress(email, email));

            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = messageBody
            };
            //   message.Attachments= 
            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            await client.ConnectAsync("mail.taif-alalmas.com", 587, SecureSocketOptions.Auto);

            // Note: only needed if the SMTP server requires authentication
            await client.AuthenticateAsync("apexweb@taif-alalmas.com", "Hfp0e?77");

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
    }

    public class MailKite:IMailKite
    {
      
        public Task SendEmailAsync(string sender, string senderPassword, string companyName , string email,string employeeName, string subject, string message,string hostName,int PortNumber)
        {
            return Execute(sender, senderPassword,  companyName, subject, message, email, employeeName, hostName, PortNumber);
        }
        public async Task Execute(string sender,string senderPassword, string companyName,string subject, string messageBody, string email, string employeeName, string hostName, int PortNumber)
            {
            MimeMessage message = new MimeMessage();
            MailboxAddress from = new MailboxAddress(companyName, sender);
            message.From.Add(from);
            MailboxAddress to = new MailboxAddress(employeeName, email);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = messageBody };
            BodyBuilder bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = "<h1>Hello World!</h1>";
            bodyBuilder.TextBody = messageBody;
            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            await client.ConnectAsync(hostName, PortNumber, SecureSocketOptions.Auto);

            // Note: only needed if the SMTP server requires authentication
            await client.AuthenticateAsync(sender, senderPassword);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }


    }

}
//Dim mailMessage As MailMessage = New MailMessage()
//Dim fromMail As MailAddress = New MailAddress("Protectioninfo@taif-alalmas.com"
//mailMessage.From = fromMail

//For i = 0 To dt.Rows.Count - 1
//    mailMessage.To.Add(dt("Email".ToString)
//Next

//mailMessage.Subject = "Registeration"
//mailMessage.Body = emailbody

//Dim smtpClient As SmtpClient = New SmtpClient("mail.taif-alalmas.com", 587) With {
//    .Credentials = New Net.NetworkCredential() With {
//    .UserName = "Protectioninfo@taif-alalmas.com",
//    .Password = "aL3fq@00"
//},
//    .EnableSsl = False
//}
//smtpClient.Send(mailMessage)
//public interface IUditorEmailService
//{
//    Task SendEmailAsync(string email, string subject, string message);
//}
//public class UditorEmailService : IUditorEmailService
//{

//    private readonly IConfiguration _configuration;
//public UditorEmailService(IConfiguration configuration)
//{
//    _configuration = configuration;
//}
//public async Task SendEmailAsync(string email, string subject, string message)
//{
//    using (var client = new SmtpClient())
//    {
//        var credential = new NetworkCredential
//        {
//            UserName = _configuration["EmailSettings:UserName"],
//            Password = _configuration["EmailSettings:Password"]
//        };
//        client.UseDefaultCredentials = true;
//        client.Credentials = credential;
//        client.Host = _configuration["EmailSettings:MailServer"];
//        client.Port = int.Parse(_configuration["EmailSettings:MailPort"]);
//        client.EnableSsl = Convert.ToBoolean(_configuration["EmailSettings:EnableSSL"]);
//        using (var emailMessage = new MailMessage())
//        {
//            emailMessage.To.Add(new MailAddress(email));
//            emailMessage.From = new MailAddress(_configuration["EmailSettings:UserName"]);
//            emailMessage.Subject = subject;
//            emailMessage.Body = message;
//            emailMessage.IsBodyHtml = true;
//            client.Send(emailMessage);
//        }
//    }
//}
//}

