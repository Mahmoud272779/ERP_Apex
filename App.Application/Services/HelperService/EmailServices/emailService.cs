using App.Application.Services.HelperService.InvoicePDF;
using App.Domain.Entities.Process;
using App.Domain.Enums;
using App.Infrastructure.Interfaces.Repository;
using App.Infrastructure.UserManagementDB;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Application.Services.HelperService.EmailServices
{
    public class emailService : IEmailService
    {
        private readonly IRepositoryQuery<InvGeneralSettings> gSRepositoryQuery;
        private readonly IConfiguration _configuration;
        private ERP_UsersManagerContext _userManagementcontext;


        public emailService(IRepositoryQuery<InvGeneralSettings> _GSRepositoryQuery, IConfiguration configuration)
        {
            gSRepositoryQuery = _GSRepositoryQuery;
            _configuration = configuration;
            _userManagementcontext = new ERP_UsersManagerContext(_configuration);
        }

        public async Task<string> ForgetPasswordSendEmail(ForgetPasswordEmail parm)
        {
            var settings = _userManagementcontext.EmailSettings.FirstOrDefault();
            if (string.IsNullOrEmpty(settings.Email) || string.IsNullOrEmpty(settings.Host) || string.IsNullOrEmpty(settings.Password) || string.IsNullOrEmpty(settings.Port.ToString()) || string.IsNullOrEmpty(settings.DisplayName))
                return "Faild To send please check email setting and try again";
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(settings.DisplayName, settings.Email));
            message.To.Add(new MailboxAddress(parm.toEmail, parm.toEmail));
            var builder = new BodyBuilder();
            message.Subject = "استعادة كلمة المرور";
            builder.HtmlBody = parm.Body;
            message.Body = builder.ToMessageBody();
            using var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(settings.Email, settings.Password);
            var send = await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return "OK";
            throw new NotImplementedException();
        }

        public async Task<string> SendEmail(emailRequest emailRequest)
        {
            try
            {

                var settings = gSRepositoryQuery.TableNoTracking.FirstOrDefault();
                if (string.IsNullOrEmpty(settings.Email) || string.IsNullOrEmpty(settings.EmailHost) || string.IsNullOrEmpty(settings.EmailPassword) || string.IsNullOrEmpty(settings.EmailPort.ToString()) || string.IsNullOrEmpty(settings.EmailDisplayName))
                    return "Faild To send please check email setting and try again";

                //var _mailSettings = _emailSettingsQuery.TableNoTracking.FirstOrDefault();
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(settings.EmailDisplayName, settings.Email));
                message.To.Add(new MailboxAddress(emailRequest.ToEmail, emailRequest.ToEmail));
                var builder = new BodyBuilder();
                if (emailRequest.Attachments != null)
                {
                    if (emailRequest.Attachments[0] != null)
                    {
                        byte[] fileBytes;
                        foreach (var file in emailRequest.Attachments)
                        {
                            if (file.Length > 0)
                            {
                                using (var ms = new MemoryStream())
                                {
                                    file.CopyTo(ms);
                                    fileBytes = ms.ToArray();
                                }
                                builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                            }
                        }
                    }
                }
                if (emailRequest.isInvoice)
                {
                    builder.Attachments.Add(emailRequest.invoiceCode + ".pdf", emailRequest.invoice);
                }
                message.Subject = emailRequest.subject;
                builder.HtmlBody = emailRequest.body;
                message.Body = builder.ToMessageBody();
                using var client = new SmtpClient
                {
                    ServerCertificateValidationCallback = (s, c, h, e) => true
                };

                if (settings.secureSocketOptions == (int)SecureSocketOptionsEnum.auto)
                    await client.ConnectAsync(settings.EmailHost, settings.EmailPort, SecureSocketOptions.Auto);
                else if (settings.secureSocketOptions == (int)SecureSocketOptionsEnum.StartTlsWhenAvailable)
                    await client.ConnectAsync(settings.EmailHost, settings.EmailPort, SecureSocketOptions.StartTlsWhenAvailable);
                else if (settings.secureSocketOptions == (int)SecureSocketOptionsEnum.none)
                    await client.ConnectAsync(settings.EmailHost, settings.EmailPort, SecureSocketOptions.None);
                else if (settings.secureSocketOptions == (int)SecureSocketOptionsEnum.StartTls)
                    await client.ConnectAsync(settings.EmailHost, settings.EmailPort, SecureSocketOptions.StartTls);
                else if (settings.secureSocketOptions == (int)SecureSocketOptionsEnum.SslOnConnect)
                    await client.ConnectAsync(settings.EmailHost, settings.EmailPort, SecureSocketOptions.SslOnConnect);

                // Note: only needed if the SMTP server requires authentication
                await client.AuthenticateAsync(settings.Email, settings.EmailPassword);
                var send = await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return "OK";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
    }
}
