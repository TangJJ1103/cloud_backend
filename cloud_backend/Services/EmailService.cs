using MimeKit;
using MailKit.Net.Smtp;

namespace cloud_backend.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendVerificationEmail(string toEmail, string verificationLink)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("TestForCloud", "tangjj1103@gmail.com"));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = "Verify Your Email";
                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif; text-align: center; padding: 20px;'>
                        <h2 style='color: #333;'>Verify Your Email</h2>
                        <p>Click the button below to verify your email address:</p>
                        <a href='{verificationLink}' style='
                            display: inline-block;
                            padding: 12px 24px;
                            font-size: 16px;
                            color: white;
                            background-color: #007BFF;
                            text-decoration: none;
                            border-radius: 5px;
                        '>Verify Email</a>
                        <p style='margin-top: 20px; font-size: 12px; color: #666;'>
                            If you did not sign up, please ignore this email.
                        </p>
                    </body>
                    </html>";

                message.Body = new TextPart("html") { Text = emailBody };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    string emailUser = _config["EmailSettings:Username"];
                    string emailPass = _config["EmailSettings:Password"];

                    if (string.IsNullOrEmpty(emailUser) || string.IsNullOrEmpty(emailPass))
                    {
                        Console.WriteLine("Email credentials not found.");
                        return false;
                    }

                    await client.AuthenticateAsync(emailUser, emailPass);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                Console.WriteLine("Email sent successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex}");
                return false;
            }
        }
    }
}
