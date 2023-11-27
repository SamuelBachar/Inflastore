using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InflaStoreWebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    public UserController(IUserService userService, IEmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<UserLoginDTO>>> Login(UserLoginRequest request)
    {
        ServiceResponse<UserLoginDTO> response = await _userService.Login(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<UserRegisterDTO>>> Register(UserRegisterRequest request)
    {
        ServiceResponse<UserRegisterDTO> responseUserService = await _userService.Register(request);

        if (!responseUserService.Success)
        {
            return BadRequest(responseUserService);
        }

        EmailDTO emailDTO = new EmailDTO
        {
            To = request.Email,
            Subject = "Infla Store - Potvrdenie registrácie",
            Body = "Prosím kliknite na nižšie uvedení link pre potvrdenie registrácie",
            EmailType = EEmailType.Registration
        };

        ServiceResponse<EmailDTO> responseEmailService = await _emailService.SendEmail(emailDTO, responseUserService.Data);

        if (!responseEmailService.Success)
        {
            responseUserService.Success = false;
            responseUserService.Message = responseEmailService.Message;
            responseUserService.ExceptionMessage = responseEmailService.ExceptionMessage;

            return BadRequest(responseUserService);
        }

        return Ok(responseUserService);
    }

    [HttpGet("verify_bkp")]
    public async Task<ActionResult<ServiceResponse<UserVerifyDTO>>> Verify(string token)
    {
        ServiceResponse<UserVerifyDTO> response = await _userService.Verify(token);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("verify")]
    public async Task<ContentResult> VerifyNew(string token)
    {
        ServiceResponse<UserVerifyDTO> response = await _userService.Verify(token);

        string content = await System.IO.File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\VerifyEmailRegistration\\VerifyResponse.html"));

        if (!response.Success)
        {
            content = content.Replace("#Response#", "Neúspešna registrácia");
            content = content.Replace("#Message#", $"{response.Message}");

            if (response.ExceptionMessage.Length > 0)
            {
                content = content.Replace("#Exception#", $"{response.ExceptionMessage}");
            }
            else
            {
                content = content.Replace("#Exception#", "");
            }
        }
        else
        {
            content = content.Replace("#Response#", "Úspešna registrácia");
            content = content.Replace("#Message#", $"{response.Message}");
            content = content.Replace("#Exception#", "");
        }

        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = response.Success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest,
            Content = content
        };
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ServiceResponse<UserForgotPasswordDTO>>> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
    {
        ServiceResponse<UserForgotPasswordDTO> responseUserService = await _userService.ForgotPassword(forgotPasswordRequest);

        if (!responseUserService.Success)
        {
            return BadRequest(responseUserService);
        }

        EmailDTO emailDTO = new EmailDTO
        {
            To = forgotPasswordRequest.Email,
            Subject = "Infla Store - Potvrdenie obnovenia hesla",
            Body = "Prosím kliknite na nižšie uvedení link pre potvrdenie obnovenie hesla",
            EmailType = EEmailType.ResetPassword
        };

        ServiceResponse<EmailDTO> responseEmailService = await _emailService.SendEmail(emailDTO, responseUserService.Data);

        if (!responseEmailService.Success)
        {
            responseUserService.Success = false;
            responseUserService.Message = responseEmailService.Message;
            responseUserService.ExceptionMessage = responseEmailService.ExceptionMessage;

            return BadRequest(responseUserService);
        }

        /* Make sure reset token is not sent back to user for safety reasons */
        if (responseUserService.Data != null)
        {
            responseUserService.Data.PasswordResetToken = string.Empty;
        }

        return Ok(responseUserService);
    }

    [HttpGet("reset-password_bkp")]
    public async Task<ActionResult<ServiceResponse<UserResetPasswordDTO>>> ResetPassword(string resetToken)
    {
        ServiceResponse<UserResetPasswordDTO> response = await _userService.ResetPassword(resetToken);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("reset-password")]
    public async Task<ContentResult> ResetPasswordNew(string resetToken)
    {
        ServiceResponse<UserResetPasswordDTO> response = await _userService.ResetPassword(resetToken);

        string content = await System.IO.File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\ResetPasword\\ResetPasswordResponse.html"));

        if (!response.Success)
        {
            content = content.Replace("#Response#", "Neuspešná zmena hesla");
            content = content.Replace("#Message#", $"{response.Message}");

            if (response.ExceptionMessage.Length > 0)
            {
                content = content.Replace("#Exception#", $"{response.ExceptionMessage}");
            }
            else
            {
                content = content.Replace("#Exception#", "");
            }
        }
        else
        {
            content = content.Replace("#Response#", "Úspešna zmena hesla");
            content = content.Replace("#Message#", $"{response.Message}");
            content = content.Replace("#Exception#", "");
        }

        return new ContentResult
        {
            ContentType = "text/html",
            StatusCode = response.Success ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest,
            Content = content
        };
    }
}

    //https://youtu.be/2Q9Uh-5O8Sk?t=2038 -- toto video je na vybudovanie API bez JWT

    //.net 7 API - treba pozorne pocuvat trosku vynechava kod a mam ho doimplementovat
    // vytvaranie JWT tokenu https://www.youtube.com/watch?v=UwruwHl3BlU&ab_channel=PatrickGod
    //
    // overovanie JWT tokenu ako ?? zrejme toto https://www.youtube.com/watch?v=6sMPvucWNRE&ab_channel=PatrickGod overenie tokenu .net 7 API
    // posielanie JWT tokenu z mobilnej appky https://youtu.be/OnrKktoNJ0o?t=2663

    // vytvaranie JWT tokenu https://www.youtube.com/watch?v=v7q3pEK1EA0&ab_channel=PatrickGod .net 6 API

    // posielanie mailu z .net API 6 https://www.youtube.com/watch?v=PvO_1T0FS_A&ab_channel=PatrickGod
