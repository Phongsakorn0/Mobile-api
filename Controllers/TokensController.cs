using System.Text;
using System.Security.Claims;

using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDo.Models;

namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TokensController : ControllerBase
{
    private readonly ILogger<TokensController> _logger;

    public TokensController(ILogger<TokensController> logger)
    {
        _logger = logger;
    }
    [HttpPost]
    public IActionResult Post([FromBody] ToDo.DTOs.Login data)
    {
        var db = new ToDoDbContext();

        var user = db.User.Find(Convert.ToInt32(data.Id));
        if (user == null)
        {
         return Unauthorized();
        }

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: data.Password,
            salt: Convert.FromBase64String(user.Salt),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 32
        ));

        if (user.HashedPassword != hashed)
        {
            return Unauthorized();
        }

        var desc = new SecurityTokenDescriptor();
        desc.Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, user.Id.ToString()),
            new Claim(ClaimTypes.Role, "user")
        });
        desc.NotBefore = DateTime.UtcNow;
        desc.Expires = DateTime.UtcNow.AddHours(3);
        desc.IssuedAt = DateTime.UtcNow;
        desc.Issuer = "ToDoAPP";
        desc.Audience = "public";
        desc.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Program.SecurityKey)),
            SecurityAlgorithms.HmacSha256Signature
        );
        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(desc);

        return Ok(new { token = handler.WriteToken(token) });
}
}