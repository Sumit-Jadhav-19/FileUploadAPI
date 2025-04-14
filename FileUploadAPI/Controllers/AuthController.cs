using FileUploadAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FileUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserModel user)
        {
            if (user.Username == "admin" && user.Password == "1234")
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("abcdefghijklmnopqrstuvwxyzrgergbgertrtertfgrtsfdguj"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "yourdomain.com",
                    audience: "yourdomain.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return Unauthorized();
        }

        [HttpGet("secure")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult SecureData()
        {
            return Ok("This is protected data.");
        }
    }
}
