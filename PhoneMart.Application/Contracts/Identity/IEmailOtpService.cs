namespace PhoneMart.Application.Contracts.OTP;

public interface IEmailOtpService
{
    Task SendOtpAsync(string email, CancellationToken ct = default);
    Task<bool> VerifyOtpAsync(string email, string code, CancellationToken ct = default);
}
