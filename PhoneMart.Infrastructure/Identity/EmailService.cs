using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using PhoneMart.Application.Contracts.Email;

namespace PhoneMart.Infrastructure.Identity;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string bodyHtml, CancellationToken ct = default)
    {
        var smtpHost = _config["EmailSettings:SmtpHost"]!;
        var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]!);
        var smtpUser = _config["EmailSettings:SmtpUser"]!;
        var smtpPass = _config["EmailSettings:SmtpPass"]!;
        var fromEmail = _config["EmailSettings:FromEmail"]!;
        var fromName = _config["EmailSettings:FromName"]!;

        using var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = subject,
            Body = bodyHtml,
            IsBodyHtml = true
        };
        message.To.Add(to);

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        // SmtpClient cancellation direct no support; still ok for most cases
        await client.SendMailAsync(message);
    }
}
