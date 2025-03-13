using Microsoft.AspNetCore.Mvc;
using ToDo.Models;
using Microsoft.AspNetCore.Authorization;

namespace ToDoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ActivitiesController : ControllerBase
{
    private readonly ILogger<ActivitiesController> _logger;

    public ActivitiesController(ILogger<ActivitiesController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "user")]
    public IActionResult Get()
    {
        var db = new ToDoDbContext();

        var activities = from a in db.Activity 
                            where a.Userid == User.Identity.Name
                            orderby a.When
                            select new{
                                name = a.Name,
                                when = a.When
                            };
        if (!activities.Any())
        {
            return NoContent();
        }

        return Ok(activities);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "user")]
    public IActionResult Get(uint id)
    {
        var db = new ToDoDbContext();

        var activity = from a in db.Activity
                                    where a.Id == id && a.Userid == User.Identity.Name
                                    select new{
                                        name = a.Name,
                                        when = a.When
                                    };
        if (activity == null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    [HttpPost]
    [Authorize(Roles = "user")]
    public IActionResult Post([FromBody] ToDo.DTOs.Activity data)
    {
        var db = new ToDoDbContext();

        var a = new Activity
        {
            Name = data.Name,
            When = data.When,
            Userid = User.Identity.Name
        };
        db.Activity.Add(a);
        db.SaveChanges();

        return Ok();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "user")]
    public IActionResult Put(int id, [FromBody] ToDo.DTOs.Activity data){
        var db = new ToDoDbContext();

        var activitiy = (from a in db.Activity
                                    where a.Id == id && a.Userid == User.Identity.Name
                                    select a).FirstOrDefault();
        if (activitiy == null)
        {
            return NotFound();
        }
        activitiy.Name = data.Name;
        activitiy.When = data.When;
        db.SaveChanges();

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "user")]
    public IActionResult Delete(uint id)
    {
        var db = new ToDoDbContext();

        var activity = db.Activity.Find(id);
        if (activity == null)
        {
            return NotFound();
        }

        db.Activity.Remove(activity);
        db.SaveChanges();

        return Ok();
    }
}

