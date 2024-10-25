﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace InflaStoreWebAPI.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        #region Login
        public async Task<ServiceResponse<UserLoginDTO>> Login(UserLoginRequest request)
        {
            ServiceResponse<UserLoginDTO> serviceResponse = new ServiceResponse<UserLoginDTO>();
            User? userDB = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (userDB == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Účet s daným e-mialom neexistuje";
                return serviceResponse;
            }

            if (!VerifyPasswordHash(request.Password, userDB.PasswordHashWithSalt))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Nesprávne heslo";
                return serviceResponse;
            }

            if (userDB.VerifiedAt == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Registrácia pre daný účet nebola overená";
                return serviceResponse;
            }

            string jwtToken = CreateJWTToken(userDB);

            UserLoginDTO userLoginDTO = _mapper.Map<UserLoginDTO>(userDB);
            userLoginDTO.JWT = jwtToken;

            serviceResponse.Data = userLoginDTO;

            return serviceResponse;
        }

        private bool VerifyPasswordHash(string requestPassword, string dbPasswordHashWithSalt)
        {
            return BCrypt.Net.BCrypt.Verify(requestPassword, dbPasswordHashWithSalt);
        }

        private string CreateJWTToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value!));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        #endregion

        #region Registration
        public async Task<ServiceResponse<UserRegisterDTO>> Register(UserRegisterRequest request)
        {
            ServiceResponse<UserRegisterDTO> serviceResponse = new ServiceResponse<UserRegisterDTO>();

            if (_context.Users.Any(u => u.Email == request.Email))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Uživateľ s daným e-mailom už existuje";
                return serviceResponse;
            }

            try
            {
                string passwordHashWithSalt = CreatePasswordHashWithSalt(request.Password);

                User dbUser = new User
                {
                    Email = request.Email,
                    PasswordHashWithSalt = passwordHashWithSalt,
                    VerificationToken = CreateRandomToken(),
                    Region = request.Region_Id
                };

                _context.Users.Add(dbUser);
                await _context.SaveChangesAsync();

                UserRegisterDTO userRegisterDTO = _mapper.Map<UserRegisterDTO>(dbUser);
                serviceResponse.Data = userRegisterDTO;
                serviceResponse.Message = "Uživateľ vytvorený, potvrďte registráciu pomocou správy ktorá bola odoslaná na váš e-mail";

                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Počas registrácie uživateľa do databázy nastala chyba";
                serviceResponse.ExceptionMessage = $"{ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}";
            }
            finally
            {
                // todo logger
                if (serviceResponse.ExceptionMessage.Length > 0)
                {

                }
            }

            return serviceResponse;
        }
        #endregion

        // todo doplnit aj user Id aby pri vacsom mnozstve uzivatelov dlho neporovnaval tokeny ( ale user_id musi byt encrypted)
        public async Task<ServiceResponse<UserVerifyDTO>> Verify(string token)
        {
            ServiceResponse<UserVerifyDTO> serviceResponse = new ServiceResponse<UserVerifyDTO>();

            try
            {
                User? userDB = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);

                if (userDB == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Nevalidný token";

                    return serviceResponse;
                }

                userDB.VerifiedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                UserVerifyDTO userVerifyDTO = _mapper.Map<UserVerifyDTO>(userDB);
                serviceResponse.Data = userVerifyDTO;
                serviceResponse.Message = "Registrácia úspešne dokončená, môžte sa prihlásiť v aplikácii Infla Store";

                return serviceResponse;

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Počas verifikácie registrácie uživateľa nastala chyba";
                serviceResponse.ExceptionMessage = $"{ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}";
            }
            finally
            {
                // todo logger
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UserForgotPasswordDTO>> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            ServiceResponse<UserForgotPasswordDTO> serviceResponse = new ServiceResponse<UserForgotPasswordDTO>();

            try
            {
                User? userDB = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordRequest.Email);

                if (userDB != null)
                {

                    /* Temporary storing new password into database
                       After reset-password is sent and processed without error this password will replace password 
                       in PasswordHashWithSalt column (means reseting of password will occur)
                     */

                    userDB.TempResetPasswordHashWithSalt = CreatePasswordHashWithSalt(forgotPasswordRequest.Password);

                    userDB.PasswordResetToken = CreateRandomToken();
                    userDB.ResetTokenExpires = DateTime.Now.AddDays(1);

                    await _context.SaveChangesAsync();

                    UserForgotPasswordDTO forgotPasswordDTO = _mapper.Map<UserForgotPasswordDTO>(userDB);
                    serviceResponse.Data = forgotPasswordDTO;
                    serviceResponse.Message = $"Do vášho e-mailu {forgotPasswordDTO.Email} sme odoslali správu pomocou ktorej si môžete obnoviť heslo do 24 hodín.";
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Účet pre daný e-mail nebol nájdení";
                }

            }
            catch (Exception ex)
            {
                serviceResponse.Data = null;
                serviceResponse.Success = false;
                serviceResponse.Message = "Nastala chyba počas ukladania údajov potrebných pre obnovenú hesla";
                serviceResponse.ExceptionMessage = $"{ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}";
            }
            finally // todo
            {
                if (serviceResponse.ExceptionMessage.Length > 0)
                {
                    // logger
                }
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UserResetPasswordDTO>> ResetPassword(string resetToken)
        {
            ServiceResponse<UserResetPasswordDTO> responseService = new ServiceResponse<UserResetPasswordDTO>();

            try
            {
                User? userDB = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == resetToken);

                if (userDB == null)
                {
                    responseService.Success = false;
                    responseService.Message = "Nevalidný token";
                    return responseService;
                }

                if (userDB.ResetTokenExpires < DateTime.Now)
                {
                    responseService.Success = false;
                    responseService.Message = "Nevalidný token, platnosť tokena pre obnovu hesla vypršala";
                    return responseService;
                }

                if (userDB.TempResetPasswordHashWithSalt == null)
                {
                    responseService.Success = false;
                    responseService.Message = "Link pre resetovanie hesla už raz bol už použitý";
                    return responseService;
                }

                /* Work around for obsolete below
                  string tempResetPasswordHashWithSalt = string.Empty;
                  userDB.TempResetPasswordHashWithSalt.Select(c => c).ToList().ForEach(c => tempResetPasswordHashWithSalt += c);
                */

                userDB.PasswordHashWithSalt = string.Copy(userDB.TempResetPasswordHashWithSalt);
                userDB.TempResetPasswordHashWithSalt = null;
                userDB.PasswordResetToken = null;
                userDB.ResetTokenExpires = null;

                await _context.SaveChangesAsync();

                UserResetPasswordDTO forgotPasswordDTO = _mapper.Map<UserResetPasswordDTO>(userDB);

                responseService.Message = $"Vaše heslo bolo úspešne obnovené";
                responseService.Data = forgotPasswordDTO;

                return responseService;

            }
            catch (Exception ex)
            {
                responseService.Success = false;
                responseService.Message = "Počas ukladania nového hesla nastala chyba";
                responseService.ExceptionMessage = $"{ex.Message} {(ex.InnerException != null ? ex.InnerException.Message : "")}";
            }
            finally
            {
                // todo logger
            }

            return responseService;
        }

        #region Common used

        private string CreateRandomToken()
        {
            string result = string.Empty;

            do
            {
                result = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            } while (_context.Users.Any(u => u.VerificationToken == result));

            return result;
        }

        private string CreatePasswordHashWithSalt(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        #endregion

        public async Task DeleteRegistrationAttempt(string email)
        {
            await _context.Users.Where(u => u.Email == email).ExecuteDeleteAsync();
        }

        public async Task DeleteForgetPasswordAttempt(string email)
        {
            User? userDB = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (userDB != null)
            {
                userDB.TempResetPasswordHashWithSalt = null;
                userDB.PasswordResetToken = null;
                userDB.ResetTokenExpires = null;

                await _context.SaveChangesAsync();
            }
        }
    }
}
