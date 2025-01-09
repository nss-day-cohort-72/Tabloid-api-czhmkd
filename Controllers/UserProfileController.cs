using Microsoft.AspNetCore.Mvc;
using Tabloid.Models;
using Tabloid.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Tabloid.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserProfileController : ControllerBase
{
    private TabloidDbContext _dbContext;
    public UserProfileController(TabloidDbContext context)
    {
        _dbContext = context;
    }
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(_dbContext.UserProfiles.ToList());
    }
    [HttpGet("withroles")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetWithRoles()
    {
        return Ok(_dbContext.UserProfiles
        .Include(up => up.IdentityUser)
        .Select(up => new UserProfile
        {
            Id = up.Id,
            FirstName = up.FirstName,
            LastName = up.LastName,
            Email = up.IdentityUser.Email,
            UserName = up.IdentityUser.UserName,
            IdentityUserId = up.IdentityUserId,
            Roles = _dbContext.UserRoles
            .Where(ur => ur.UserId == up.IdentityUserId)
            .Select(ur => _dbContext.Roles.SingleOrDefault(r => r.Id == ur.RoleId).Name)
            .ToList()
        }));
    }
    [HttpPost("promote/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Promote(string id)
    {
        IdentityRole role = _dbContext.Roles.SingleOrDefault(r => r.Name == "Admin");
        _dbContext.UserRoles.Add(new IdentityUserRole<string>
        {
            RoleId = role.Id,
            UserId = id
        });
        _dbContext.SaveChanges();
        return NoContent();
    }
    [HttpPost("demote/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Demote(string id)
    {
        IdentityRole role = _dbContext.Roles
            .SingleOrDefault(r => r.Name == "Admin");
        IdentityUserRole<string> userRole = _dbContext
            .UserRoles
            .SingleOrDefault(ur =>
                ur.RoleId == role.Id &&
                ur.UserId == id);
        _dbContext.UserRoles.Remove(userRole);
        _dbContext.SaveChanges();
        return NoContent();
    }
    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        UserProfile user = _dbContext
            .UserProfiles
            .Include(up => up.IdentityUser)
            .Select(up => new UserProfile
            {
                Id = up.Id,
                FirstName = up.FirstName,
                LastName = up.LastName,
                Email = up.IdentityUser.Email,
                UserName = up.IdentityUser.UserName,
                ImageLocation = up.ImageLocation,
                IdentityUserId = up.IdentityUserId,
                Roles = _dbContext.UserRoles
            .Where(ur => ur.UserId == up.IdentityUserId)
            .Select(ur => _dbContext.Roles.SingleOrDefault(r => r.Id == ur.RoleId).Name)
            .ToList()
            })
            .SingleOrDefault(up => up.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult EditUser(int id, UserProfile user)
    {
        UserProfile foundUser = _dbContext.UserProfiles
        .Include(up => up.IdentityUser)
        .FirstOrDefault(u => u.Id == id);
        foundUser.FirstName = user.FirstName;
        foundUser.LastName = user.LastName;
        foundUser.UserName = user.UserName;
        foundUser.Email = user.Email;
        foundUser.ImageLocation = user.ImageLocation;
        // Update roles
        var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId == foundUser.IdentityUserId).ToList();
        // Remove existing roles
        _dbContext.UserRoles.RemoveRange(userRoles);
        // Add new roles from the request
        foreach (var roleName in user.Roles)
        {
            var role = _dbContext.Roles.SingleOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                _dbContext.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = foundUser.IdentityUserId,
                    RoleId = role.Id
                });
            }
        }
        _dbContext.SaveChanges();
        return NoContent();
    }
}








