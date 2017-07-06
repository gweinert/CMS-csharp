using System;
using System.IO;
using MailKit;
using MailKit.Net.Smtp;
using MimeKit;


namespace Server.Models
{
    public static class Email
    {

        public static bool SendMessageSmtp (string login,
                                            string pass,
                                            string toName, 
                                            string toEmail, 
                                            string fromName, 
                                            string fromEmail,
                                            string subject,
                                            string body)
        {
            // Compose a message
            MimeMessage mail = new MimeMessage ();
            mail.From.Add (new MailboxAddress (fromName, fromEmail));
            mail.To.Add (new MailboxAddress (toName, toEmail));
            mail.Subject = subject;
            mail.Body = new TextPart ("plain") {
                Text = body
            };

            // Send it!
            using (var client = new SmtpClient ()) {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s,c,h,e) => true;

                client.Connect ("smtp.mailgun.org", 2525, false);


                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove ("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate (login, pass);

                client.Send (mail);
                client.Disconnect (true);

                return true;
            }
        }
    }

}