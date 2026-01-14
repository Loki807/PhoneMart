namespace PhoneMart.Application.Contracts.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string bodyHtml, CancellationToken ct = default);
}
