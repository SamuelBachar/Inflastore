using Azure.Core;
using InflaStoreWebAPI.Models.DatabaseModels;
using InflaStoreWebAPI.Models.ServiceResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InflaStoreWebAPI.Controllers;

[Route("api/[controller]")] // todo: change it to THIN contollers with services this is fat controller 
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

        ServiceResponse<EmailDTO> responseEmailService = await _emailService.SendEmail(emailDTO);

        if (!responseEmailService.Success)
        {
            responseUserService.Success = false;
            responseUserService.Message = responseEmailService.Message;
            responseUserService.ExceptionMessage = responseEmailService.ExceptionMessage;

            return BadRequest(responseUserService);
        }

        return Ok(responseUserService);
    }

    [HttpPost("verify")]
    public async Task<ActionResult<ServiceResponse<UserVerifyDTO>>> Verify(string token)
    {
        ServiceResponse<UserVerifyDTO> response = await _userService.Verify(token);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return BadRequest("Účet pre daný e-mail nebol nájdení");
        }

        user.PasswordResetToken = CreateRandomToken();
        user.ResetTokenExpires = DateTime.Now.AddDays(1);

        await _context.SaveChangesAsync();

        return Ok("Do vášho e-mailu sme odoslali správu pomocou ktorej si môžete obnoviť heslo do 24 hodín.");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

        if (user == null)
        {
            return BadRequest("Nevalidný token");
        }

        if (user.ResetTokenExpires < DateTime.Now)
        {
            return BadRequest("Nevalidný token, platnosť tokena pre obnovu hesla vypršala");
        }

        string passwordHash = CreatePasswordHashWithSalt(request.Password);

        user.PasswordHashWithSalt = passwordHash;
        user.PasswordResetToken = null;
        user.ResetTokenExpires = null;

        await _context.SaveChangesAsync();

        return Ok("Vaše heslo bolo úspešne obnovené");
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private string CreatePasswordHashWithSalt(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPasswordHash(string requestPassword, string dbPasswordHashWithSalt)
    {
        return BCrypt.Net.BCrypt.Verify(requestPassword, dbPasswordHashWithSalt);
    }

    private string CreateRandomToken()
    {
        string result = string.Empty;

        do
        {
            result = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        } while (_context.Users.Any(u => u.VerificationToken == result));


        return result;
    }

    //https://youtu.be/2Q9Uh-5O8Sk?t=2038 -- toto video je na vybudovanie API bez JWT

    //.net 7 API - treba pozorne pocuvat trosku vynechava kod a mam ho doimplementovat
    // vytvaranie JWT tokenu https://www.youtube.com/watch?v=UwruwHl3BlU&ab_channel=PatrickGod
    //
    // overovanie JWT tokenu ako ?? zrejme toto https://www.youtube.com/watch?v=6sMPvucWNRE&ab_channel=PatrickGod overenie tokenu .net 7 API
    // posielanie JWT tokenu z mobilnej appky https://youtu.be/OnrKktoNJ0o?t=2663

    // vytvaranie JWT tokenu https://www.youtube.com/watch?v=v7q3pEK1EA0&ab_channel=PatrickGod .net 6 API

    // posielanie mailu z .net API 6 https://www.youtube.com/watch?v=PvO_1T0FS_A&ab_channel=PatrickGod
}
