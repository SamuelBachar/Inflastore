using AutoMapper;
using InflaStoreWebAPI.Models.DatabaseModels;
using InflaStoreWebAPI.Models.ServiceResponseModel;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
        public async Task<ServiceResponse<UserLoginDTO>> Login(UserLoginRequest request) // todo poriesit UserDTO a mozne mapovanie s auto mapperom
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

            string jwtToken = CreateToken(userDB);

            UserLoginDTO userLoginDTO = _mapper.Map<UserLoginDTO>(userDB);
            userLoginDTO.JWT = jwtToken;

            serviceResponse.Data = userLoginDTO;

            return serviceResponse;
        }

        private bool VerifyPasswordHash(string requestPassword, string dbPasswordHashWithSalt)
        {
            return BCrypt.Net.BCrypt.Verify(requestPassword, dbPasswordHashWithSalt);
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

                User user = new User
                {
                    Email = request.Email,
                    PasswordHashWithSalt = passwordHashWithSalt,
                    VerificationToken = CreateRandomToken()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                UserRegisterDTO userRegisterDTO = _mapper.Map<UserRegisterDTO>(user);
                serviceResponse.Data = userRegisterDTO;
                serviceResponse.Message = "Uživateľ vytvorený, potvrďte registráciu pomocou správy ktorá bola odoslaná na váš e-mail";

                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Počas registrácie uživateľa do databázy nastala chyba";
                serviceResponse.ExceptionMessage = ex.Message;
                return serviceResponse;
            }
            finally
            {
                // todo logger
            }
        }
        #endregion

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
                serviceResponse.ExceptionMessage = ex.Message;

                return serviceResponse;
            }
            finally
            {
                // todo logger
            }
        }

        public async Task<ServiceResponse<User>> ForgotPassword(string email)
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

        public async Task<ServiceResponse<User>> ResetPassword(ResetPasswordRequest request)
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
    }
}
