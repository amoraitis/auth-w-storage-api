using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AuthWithStorage.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using AuthWithStorage.Infrastructure.Repositories;
using AuthWithStorage.Infrastructure.Account;
using AuthWithStorage.Domain.Entities;
using AuthWithStorage.Domain.Queries;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AuthWithStorage.API.Controllers
{
    /// <summary>
    /// Controller for managing account-related actions such as login and logout.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IRepository<User, int, UserSearchQuery> _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly JwtService _jwtService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly JwtOptions _jwtOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="passwordHasher">The password hasher service.</param>
        /// <param name="jwtService">The JWT service.</param>
        /// <param name="jwtOptions">The JWT options.</param>
        public AccountController(
            IRepository<User, int, UserSearchQuery> userRepository, 
            PasswordHasher passwordHasher,
            JwtService jwtService,
            IOptions<JwtOptions> jwtOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            this.httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions.Value;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="request">The login request containing username and password.</param>
        /// <returns>An action result containing the JWT token or an unauthorized response.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var users = await _userRepository.GetAllAsync(new UserSearchQuery { Name = request.Username, PageSize = 1 });
            var user = users.FirstOrDefault();

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Invalid username or password." });
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
            };
            if (user.Permissions != null)
                claims.AddRange(user.Permissions.Select(p => new Claim("Permission", p.ToString())));

            var token = _jwtService.GenerateToken(claims, DateTime.UtcNow.AddHours(_jwtOptions.TokenExpiryInHours));
            
            httpContextAccessor.HttpContext?.Response.Cookies.Append("_auth", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(_jwtOptions.TokenExpiryInHours)
            });

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Logs out the user by invalidating the token or session.
        /// </summary>
        /// <returns>An action result indicating the logout status.</returns>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Delete("_auth");
            return Ok(new { Message = "Logout successful." });
        }
    }
}
