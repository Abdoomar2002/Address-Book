using AddressBook.Application.DTOs.Auth;
using AddressBook.Application.Interfaces;
using AddressBook.Domain.Entites;
using AddressBook.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AddressBook.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AuthController(
            AppDbContext context,
            ITokenService tokenService,
            IPasswordHasher<AppUser> passwordHasher)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var email = dto.Email.Trim();

            if (await _context.AppUsers.AnyAsync(u => u.Email == email))
            {
                // Email is uniquely indexed; fail here rather than on SaveChanges.
                ModelState.AddModelError(nameof(dto.Email), "Email is already registered.");
                return ValidationProblem(ModelState);
            }

            var user = new AppUser
            {
                FullName = dto.FullName.Trim(),
                Email = email
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            // Add first so EF assigns the key, then build the token before committing:
            // a signing failure must not leave a registered user behind.
            _context.AppUsers.Add(user);
            var response = BuildResponse(user);
            await _context.SaveChangesAsync();

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var email = dto.Email.Trim();
            var user = await _context.AppUsers.SingleOrDefaultAsync(u => u.Email == email);

            if (user is null)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
                await _context.SaveChangesAsync();
            }

            return Ok(BuildResponse(user));
        }

        /// <summary>
        /// Echoes the claims carried by the bearer token. Exists so the JWT pipeline
        /// can be exercised end to end from Swagger.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                fullName = User.FindFirstValue(ClaimTypes.Name),
                email = User.FindFirstValue(ClaimTypes.Email)
            });
        }

        private AuthResponseDto BuildResponse(AppUser user)
        {
            var (token, expiresAt) = _tokenService.CreateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                ExpiresAt = expiresAt
            };
        }
    }
}
