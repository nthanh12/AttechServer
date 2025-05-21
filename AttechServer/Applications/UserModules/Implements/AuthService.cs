using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos;
using AttechServer.Domains.Entities;
using AttechServer.Infrastructures.Persistances;
using AttechServer.Shared.Consts.Exceptions;
using AttechServer.Shared.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json;
using AttechServer.Shared.Utils;
using System.Security.Cryptography;

namespace AttechServer.Applications.UserModules.Implements
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public AuthService(ApplicationDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<TokenApiDto> Login(UserLoginDto userInput)
        {
            try
            {
                _logger.LogInformation($"{nameof(Login)}: input = {JsonSerializer.Serialize(userInput)}");
                var user = _context.Users.FirstOrDefault(x => x.Username == userInput.Username);
                if (user == null)
                {
                    throw new UserFriendlyException(ErrorCode.UserNotFound);
                }
                if (!PasswordHasher.VerifyPassword(userInput.Password, user.Password))
                {
                    throw new UserFriendlyException(ErrorCode.PasswordWrong);
                }

                _logger.LogInformation("Creating JWT token...");
                var newAccessToken = CreateJwt(userInput);
                _logger.LogInformation("JWT token created successfully");

                _logger.LogInformation("Creating refresh token...");
                var newRefreshToken = CreateRefreshToken();
                _logger.LogInformation("Refresh token created successfully");

                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _context.SaveChangesAsync();

                return new TokenApiDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                throw;
            }
        }
        public void RegisterUser(CreateUserDto user)
        {
            _logger.LogInformation($"{nameof(RegisterUser)}: input = {JsonSerializer.Serialize(user)}");
            var check = _context.Users.FirstOrDefault(x => x.Username == user.Username);
            if (check != null)
            {
                throw new UserFriendlyException(ErrorCode.UsernameIsExist);
            }
            if (user.Password.Length < 6)
            {
                throw new UserFriendlyException(ErrorCode.PasswordMustBeLongerThanSixCharacter);
            }
            if (!(Regex.IsMatch(user.Password, "[a-z]") && Regex.IsMatch(user.Password, "[A-Z]") && Regex.IsMatch(user.Password, "[0-9]")))
            {
                throw new UserFriendlyException(ErrorCode.TypeofPasswordMustBeNumberOrString);
            }
            if (!Regex.IsMatch(user.Password, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                throw new UserFriendlyException(ErrorCode.PasswordMustBeContainsSpecifyCharacter);

            _context.Users.Add(new User
            {
                Username = user.Username,
                Password = PasswordHasher.HashPassword(user.Password),
                UserType = user.UserType,
            });
            _context.SaveChanges();
        }
        public TokenApiDto RefreshToken(TokenApiDto input)
        {
            if (input is null)
                throw new UserFriendlyException(ErrorCode.InvalidClientRequest);

            string accessToken = input.AccessToken;
            string refreshToken = input.RefreshToken;

            var principal = GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new UserFriendlyException(ErrorCode.InvalidRefreshToken);

            var newAccessToken = CreateJwt(new UserLoginDto { Username = user.Username, Password = user.Password });
            var newRefreshToken = CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            _context.SaveChangesAsync();

            return new TokenApiDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        private string CreateJwt(UserLoginDto user)
        {
            try
            {
                _logger.LogInformation("Starting JWT token creation...");
                var jwtToken = new JwtSecurityTokenHandler();
                var userId = _context.Users.FirstOrDefault(u => u.Username == user.Username) ?? throw new UserFriendlyException(ErrorCode.UserNotFound);

                _logger.LogInformation("Getting JWT key...");
                var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT")["Key"]!);
                _logger.LogInformation("JWT key retrieved successfully");

                var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, $"{userId.Id}"),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("user_type", userId.UserType.ToString()),
                new Claim("user_id", userId.Id.ToString())
            };
                _logger.LogInformation("Claims created successfully");

                var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
                _logger.LogInformation("Signing credentials created successfully");

                var token = new JwtSecurityToken(
                    expires: DateTime.Now.AddHours(1),
                    claims: claims,
                    signingCredentials: credentials
                );
                _logger.LogInformation("JWT token object created successfully");

                var tokenString = jwtToken.WriteToken(token);
                _logger.LogInformation("JWT token string created successfully");
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during JWT token creation");
                throw;
            }
        }
        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);
            var tokenInUser = _context.Users.Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }
        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT")["Key"]!)),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new UserFriendlyException(ErrorCode.LoginExpired);
            return principal;
        }
    }
}
