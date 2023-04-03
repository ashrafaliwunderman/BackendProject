using Core.Model.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    internal class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender()
        {
             _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        }
        public void SendEmailAsync(Email email)
        {
            string toName = _configuration.GetValue<string>("Mail:ToName").ToString();
            string fromName = _configuration.GetValue<string>("Mail:FromName").ToString();
            string password = _configuration.GetValue<string>("Mail:Password").ToString();
            string fromEmail = _configuration.GetValue<string>("Mail:FromEmail").ToString();
            string connect = _configuration.GetValue<string>("Mail:Connect").ToString();
            int port = _configuration.GetValue<int>("Mail:Port");

            var message = new MimeMessage();
            message.Body = new TextPart(TextFormat.Text)
            {
                Text = email.Body
            };
            message.To.Add(new MailboxAddress(toName, email.To));
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.Subject = email.Subject;

            try
            {
                using var smtp = new SmtpClient();
                smtp.Connect(connect, port, SecureSocketOptions.StartTls);
                smtp.Authenticate(fromEmail, password);
                smtp.Send(message);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
