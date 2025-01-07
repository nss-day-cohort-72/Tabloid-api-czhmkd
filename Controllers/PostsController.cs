using Microsoft.AspNetCore.Mvc;
using Tabloid.Models;
using Tabloid.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Tabloid.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{

    private TabloidDbContext _dbContext;

    public PostsController(TabloidDbContext context)
    {
        _dbContext = context;
    }


    ////Get Endpoints
    //Get all posts that are approved and have a publication date in the past
    [HttpGet("approved")]
    [Authorize]
    public IActionResult GetAllApprovedPosts()
    {
        var posts = _dbContext.Posts
           .Include(p => p.Category)
           .Where(p => p.IsApproved == true && p.PublicationDate < DateTime.Now)
           .OrderByDescending(p => p.PublicationDate)
           .Select(p => new
           {
               p.Id,
               p.Title,
               p.Author,
               p.IsApproved,
               p.PublicationDate,
               Category = p.Category.Name
           })
           .ToList();
        return Ok(posts);
    }



    ////Put Endpoints

}

