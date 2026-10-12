using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterDTO registerDTO)
        {
            if (await userManager.FindByNameAsync(registerDTO.username) != null)
                return BadRequest("User exists !!");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = registerDTO.username,
                Email = registerDTO.email,
                Role = "Admin"
            };

            var result = await userManager.CreateAsync(applicationUser, registerDTO.password);
            if (result.Succeeded)
            {
                return Ok("Admin created successfully");
            }
            return BadRequest("erreur");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO login)
        {
            var user = await userManager.FindByNameAsync(login.username);
            if (user == null)
                return Unauthorized("Invalid Credentials");

            if (await userManager.CheckPasswordAsync(user, login.password))
            {
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
                var sc = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: claims,
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: sc
                );

                var _token = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = login.username,
                    role = user.Role,
                    userId = user.Id
                };

                return Ok(_token);
            }
            return Unauthorized("Invalid Credentials");
        }
    }
}