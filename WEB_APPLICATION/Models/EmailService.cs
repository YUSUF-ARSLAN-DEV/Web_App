using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace WEB_APPLICATION.Models
{
    public static class EmailService
    {
        /// <summary>
        /// Sends a 6-digit verification code to the given email address via Gmail SMTP.
        /// Reads credentials from Web.config appSettings:
        ///   Gmail:From       — sender Gmail address
        ///   Gmail:Password   — Gmail App Password (NOT the account password)
        ///   Gmail:DisplayName — display name shown in the email client
        /// </summary>
        public static void SendVerificationCode(string toEmail, string code)
        {
            string from = ConfigurationManager.AppSettings["Gmail:From"];
            string password = ConfigurationManager.AppSettings["Gmail:Password"];
            string displayName = ConfigurationManager.AppSettings["Gmail:DisplayName"] ?? "EduNest";

            string subject = "Your EduNest verification code";
            string body = $@"
<!DOCTYPE html>
<html>
<body style=""font-family:Arial,sans-serif;background:#0a0c14;color:#e0e0e0;margin:0;padding:40px"">
  <div style=""max-width:480px;margin:0 auto;background:#12141e;border-radius:16px;padding:40px;border:1px solid #2a2d3e"">
    <div style=""font-size:24px;font-weight:900;margin-bottom:8px;color:#fff"">
      Edu<span style=""color:#c9a84c"">Nest</span>
    </div>
    <h2 style=""font-size:20px;margin:24px 0 8px;color:#fff"">Verify your email address</h2>
    <p style=""color:#9ca3af;font-size:14px;line-height:1.7;margin-bottom:28px"">
      Enter this code to complete your registration. It expires in <strong style=""color:#fff"">10 minutes</strong>.
    </p>
    <div style=""background:#1e2130;border-radius:12px;padding:24px;text-align:center;letter-spacing:12px;
                font-size:36px;font-weight:900;color:#c9a84c;border:1px solid #2a2d3e"">
      {code}
    </div>
    <p style=""color:#6b7280;font-size:12px;margin-top:24px;line-height:1.6"">
      If you didn't create an EduNest account, you can safely ignore this email.
    </p>
  </div>
</body>
</html>";

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from, password);

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(from, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);

                smtp.Send(mail);
            }
        }

        /// <summary>
        /// Sends a welcome email to a new Google sign-in user.
        /// </summary>
        public static void SendWelcomeEmail(string toEmail, string firstName, string role)
        {
            string from = ConfigurationManager.AppSettings["Gmail:From"];
            string password = ConfigurationManager.AppSettings["Gmail:Password"];
            string displayName = ConfigurationManager.AppSettings["Gmail:DisplayName"] ?? "EduNest";

            string roleMessage = role == "instructor"
                ? "You're registered as an <strong style=\"color:#c9a84c\">Instructor</strong>. Start creating courses from your dashboard."
                : "You're registered as a <strong style=\"color:#c9a84c\">Student</strong>. Start exploring courses from your dashboard.";

            string subject = "Welcome to EduNest!";
            string body = $@"
<!DOCTYPE html>
<html>
<body style=""font-family:Arial,sans-serif;background:#0a0c14;color:#e0e0e0;margin:0;padding:40px"">
  <div style=""max-width:480px;margin:0 auto;background:#12141e;border-radius:16px;padding:40px;border:1px solid #2a2d3e"">
    <div style=""font-size:24px;font-weight:900;margin-bottom:8px;color:#fff"">
      Edu<span style=""color:#c9a84c"">Nest</span>
    </div>
    <h2 style=""font-size:20px;margin:24px 0 8px;color:#fff"">Welcome, {firstName}! 🎉</h2>
    <p style=""color:#9ca3af;font-size:14px;line-height:1.7;margin-bottom:16px"">
      Your EduNest account has been created using Google Sign-In.
    </p>
    <p style=""color:#9ca3af;font-size:14px;line-height:1.7;margin-bottom:28px"">
      {roleMessage}
    </p>
    <p style=""color:#6b7280;font-size:12px;margin-top:24px;line-height:1.6"">
      If you didn't create this account, please contact us immediately.
    </p>
  </div>
</body>
</html>";

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from, password);

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(from, displayName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mail.To.Add(toEmail);
                smtp.Send(mail);
            }
        }

        /// <summary>
        /// Generates a cryptographically random 6-digit numeric code.
        /// </summary>
        public static string GenerateCode()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                int value = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
                return value.ToString("D6"); // zero-padded to 6 digits
            }
        }
    }
}
