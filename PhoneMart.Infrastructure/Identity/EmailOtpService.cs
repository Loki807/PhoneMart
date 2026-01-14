using Microsoft.EntityFrameworkCore;
using PhoneMart.Application.Contracts.Email;
using PhoneMart.Application.Contracts.OTP;
using PhoneMart.Domain.Entities;
using PhoneMart.Persistence.Data;

namespace PhoneMart.Infrastructure.Identity;

public class EmailOtpService : IEmailOtpService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public EmailOtpService(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task SendOtpAsync(string email, CancellationToken ct = default)
    {
        email = email.Trim().ToLower();

        // ✅ Generate 6-digit OTP
        var code = Random.Shared.Next(100000, 999999).ToString();
        var expiresAt = DateTime.UtcNow.AddMinutes(5);

        // ✅ Invalidate old active OTPs (optional but recommended)
        var old = await _db.EmailOtps
            .Where(x => x.Email == email && !x.IsUsed && x.ExpiresAtUtc > DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var item in old)
            item.IsUsed = true;

        var otp = new EmailOtp
        {
            Email = email,
            Code = code,
            ExpiresAtUtc = expiresAt,
            IsUsed = false
        };

        await _db.EmailOtps.AddAsync(otp, ct);
        await _db.SaveChangesAsync(ct);

        var subject = "PhoneMart OTP Verification";
        var body = $@"
<div style='font-family:Arial; line-height:1.5'>
  <h2>PhoneMart OTP</h2>
  <p>Your verification code is:</p>
  <div style='font-size:32px; font-weight:bold; letter-spacing:6px'>{code}</div>
  <p>This OTP expires in <b>5 minutes</b>.</p>
</div>";

        await _email.SendEmailAsync(email, subject, body, ct);
    }

    public async Task<bool> VerifyOtpAsync(string email, string code, CancellationToken ct = default)
    {
        email = email.Trim().ToLower();
        code = code.Trim();

        var now = DateTime.UtcNow;

        var otp = await _db.EmailOtps
            .Where(x => x.Email == email && x.Code == code && !x.IsUsed)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(ct);

        if (otp == null) return false;
        if (otp.ExpiresAtUtc < now) return false;

        otp.IsUsed = true;
        await _db.SaveChangesAsync(ct);

        return true;
    }
}
