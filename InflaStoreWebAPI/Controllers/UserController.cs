using Azure.Core;
using InflaStoreWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InflaStoreWebAPI.Controllers;

[Route("api/[controller]")] // todo: change it to THIN contollers with services this is fat controller 
[ApiController]
public class UserController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IConfiguration _configuration;

    public UserController(DataContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterRequest request)
    {
        if (_context.Users.Any(u => u.Email == request.Email))
        {
            return BadRequest("Uživateľ s daným e-mailom už existuje");
        }

        string passwordHashWithSalt = CreatePasswordHashWithSalt(request.Password);

        User user = new User
        {
            Email = request.Email,
            PasswordHashWithSalt = passwordHashWithSalt,
            VerificationToken = CreateRandomToken()
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Uživateľ vytvorený, potvrďte registráciu pomocou správy ktorá bola odoslaná na váš e-mail");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginRequest request)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return BadRequest("Účet s daným e-mialom neexistuje");
        }

        if (!VerifyPasswordHash(request.Password, user.PasswordHashWithSalt))
        {
            return BadRequest("Nesprávne heslo");
        }

        if (user.VerifiedAt == null)
        {
            return BadRequest("Registrácia pre daný účet nebola overená");
        }

        string jwtToken = CreateToken(user);

        return Ok($"Prihlásenie úspešne @{jwtToken}");
    }

    private string CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(string token)
    {
        // todo implement sending email to user
        // todo send aswell user email (les bytes than token)

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

        if (user == null)
        {
            return BadRequest("Nevalidný token");
        }

        user.VerifiedAt = DateTime.Now;
        await _context.SaveChangesAsync();

        return Ok("Uživateľ overený");
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
