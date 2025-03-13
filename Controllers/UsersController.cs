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

        var user = db.User.Find(Convert.ToInt32(data.Id));
        if (user != null)
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
            Id = Convert.ToInt32(data.Id),
            HashedPassword = hashed,
            Salt = Convert.ToBase64String(s),
            Firstname = data.Firstname,
            Lastname = data.Lastname,
            NationalId = data.NationalId,
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
                        id = User.Identity.Name,
                        firstname = u.Firstname,
                        lastname = u.Lastname,
                        nationalId = u.NationalId,
                        tittle = u.Tittle
                    }).FirstOrDefault();
        var activities = from a in db.Activity
                         where a.Userid == Convert.ToUInt32(User.Identity.Name)
                         orderby a.When
                         select new
                         {
                             name = a.Name,
                             when = a.When
                         };
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new { user, activities });
    }
}