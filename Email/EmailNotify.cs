using MailKit.Net.Smtp;
using MimeKit;

namespace BankOfHogwarts.Email
{
    public class EmailNotify
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        // Constructor to initialize SMTP settings
        public EmailNotify(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
        }

        // Method to send an email
        public void SendEmail(string senderName, string senderEmail, string receiverName, string receiverEmail, string subject, string body)
        {
            var email = new MimeMessage();

            // Set email sender and receiver
            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(new MailboxAddress(receiverName, receiverEmail));

            // Set email subject and body
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = body
            };

            // Send the email
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_smtpServer, _smtpPort, false);

                // Authenticate if required
                smtp.Authenticate(_smtpUsername, _smtpPassword);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
