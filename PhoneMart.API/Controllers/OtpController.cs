using Microsoft.AspNetCore.Mvc;
using PhoneMart.Application.Contracts.OTP;
using PhoneMart.Application.Features.Otp.DTOs;

namespace PhoneMart.WebApi.Controllers;

[ApiController]
[Route("api/otp")]
public class OtpController : ControllerBase
{
    private readonly IEmailOtpService _otp;

    public OtpController(IEmailOtpService otp)
    {
        _otp = otp;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendOtpRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            return BadRequest(new { message = "Email is required" });

        await _otp.SendOtpAsync(req.Email, ct);
        return Ok(new { message = "OTP sent to email ✅" });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Code))
            return BadRequest(new { message = "Email and code are required" });

        var ok = await _otp.VerifyOtpAsync(req.Email, req.Code, ct);
        if (!ok) return BadRequest(new { message = "Invalid or expired OTP ❌" });

        return Ok(new { message = "OTP verified ✅" });
    }
}
