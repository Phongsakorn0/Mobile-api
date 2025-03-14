using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }
    [HttpPost]
    public IActionResult Post([FromBody] ToDo.DTOs.Register data)
    {
        var db = new ToDoDbContext();

        var user = from u in db.User
                   where u.NationalId == data.NationalId
                   select u;
        if (user.Any())
        {
            return BadRequest();
        }
        byte[] s = new byte[16];
        RandomNumberGenerator.Create().GetBytes(s);

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: data.Password,
            salt: s,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 32
        ));

        var newUser = new User
        {
            NationalId = data.NationalId,
            HashedPassword = hashed,
            Salt = Convert.ToBase64String(s),
            Firstname = data.Firstname,
            Lastname = data.Lastname,
            Tittle = data.Tittle
        };

        db.User.Add(newUser);
        db.SaveChanges();

        return Ok();
    }
    [HttpGet]
    [Authorize(Roles = "user")]
    public IActionResult Get()
    {
        var db = new ToDoDbContext();

        var user = (from u in db.User
                    select new
                    {   
                        firstname = u.Firstname,
                        lastname = u.Lastname,
                        nationalId = User.Identity.Name,
                        tittle = u.Tittle
                    }).FirstOrDefault();
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user});
    }
}