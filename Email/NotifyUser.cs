namespace BankOfHogwarts.Email
{
    public class NotifyUser
    {
        public static void NotifyUserByEmail(string rname, string remail, string sub, string body)
        {
            try
            {
                Console.WriteLine("hello");

                // Initialize the EmailNotify class with SMTP server settings
                var emailNotifier = new EmailNotify(
                    smtpServer: "smtp.office365.com",
                    smtpPort: 587,
                    smtpUsername: "prashi1207@outlook.com",
                    smtpPassword: "1207@prashi"
                );

                // Send an email
                emailNotifier.SendEmail(
                    senderName: "Bank Of Hogwarts",
                    senderEmail: "prashi1207@outlook.com",
                    receiverName: rname,
                    receiverEmail: remail,
                    subject: sub,
                    body: body
                );

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending email: {ex.Message}");
                throw; // Rethrow the exception to propagate it back to the controller
            }
        }
    }
}
