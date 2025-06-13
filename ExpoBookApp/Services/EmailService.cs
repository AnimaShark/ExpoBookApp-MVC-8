using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using ExpoBookApp.Services;
using Microsoft.AspNetCore.Mvc;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendActivationEmail(string toEmail, string token)
    {
        var fromEmail = _config["Smtp:From"];
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);

        var resetLink = $"https://localhost:7058/account/ConfirmEmail?token={token}";

        var subject = "Verify Account - ExpoBook";
        var body = $@"
            Hello,

            Thank you for registering with ExpoBook!

            We would like you to verify the registration to activate your account.

            Click the link below to activate your account:
            {resetLink}

            If you didn’t register any account with us, please ignore this email.

            Thanks,
            ExpoBook Support Team";

        var message = new MailMessage(fromEmail, toEmail, subject, body);

        using (var smtp = new SmtpClient(smtpHost, smtpPort))
        {
            smtp.EnableSsl = false; // Papercut doesn't use SSL
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = true;
            await Task.Run(() => smtp.Send(message));
        }
    }

    public void SendResetEmail(string toEmail, string token)
    {
        var fromEmail = _config["Smtp:From"];
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);

        var resetLink = $"https://localhost:7058/account/ResetPassword?token={token}";

        var subject = "Reset Your Password - EventBook";
        var body = $@"
            Hello,

            We received a request to reset your password.

            Click the link below to reset your password:
            {resetLink}

            If you didn’t request a password reset, you can safely ignore this email.

            Thanks,
            ExpoBook Support Team";

        var message = new MailMessage(fromEmail, toEmail, subject, body);

        using (var smtp = new SmtpClient(smtpHost, smtpPort))
        {
            smtp.EnableSsl = false; // Papercut doesn't use SSL
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = true;
            smtp.Send(message);
        }
    }
}
