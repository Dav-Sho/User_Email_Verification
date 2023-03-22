using System;
using System.Collections.Generic;
using System.Linq;
// using System.Net.Mail;
using System.Threading.Tasks;
// using MailKit.Net.Smtp;
using MimeKit;
namespace User_Email_Verification.Service.Implementation
{
    public class EmailImpl : EmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailImpl(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
            
        }
        public void SendEmail(Message message)
        {
            var emailMessage = createMessage(message);
            Send(emailMessage);
        }

        private MimeMessage createMessage(Message message) {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) {Text = message.Content};

            return emailMessage;
        }

        private void Send(MimeMessage mailMessage) {
            using var smtp = new SmtpClient();

            
            smtp.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");      
            
    
            
        }
    }
}