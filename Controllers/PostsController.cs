using Microsoft.AspNetCore.Mvc;
using Tabloid.Models;
using Tabloid.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Tabloid.Models.DTOs;
using System.Security.Claims;

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

    [HttpGet("myposts")]
    [Authorize]
    public IActionResult GetAllPosts()
    {
        var posts = _dbContext.Posts
             .Include(p => p.Category)
             .Select(p => new
             {
                 p.Id,
                 p.Title,
                 p.Author,
                 p.IsApproved,
                 p.PublicationDate,
                 Category = p.Category.Name
             })
             .OrderByDescending(p => p.PublicationDate)
             .ToList();

        return Ok(posts);
    }

    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetPostDetails(int id)
    {
        var post = _dbContext.Posts
        .Include(p => p.Category)
        .Where(p => p.Id == id)
        .Select(p => new
        {
            p.Id,
            p.Title,
            p.Author,
            p.IsApproved,
            PublicationDate = p.PublicationDate.ToString("MM/dd/yyyy"),
            p.Content,
            p.HeaderImage,
            Category = p.Category.Name
        })
        .FirstOrDefault();

        if (post == null)
        {
            return NotFound(new { message = "Post not found." });
        }

        return Ok(post);
    }

    [HttpPost("newpost")]
    [Authorize]
    public IActionResult CreatePost(CreatePostDTO createPostDTO)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        var userProfile = _dbContext.UserProfiles.FirstOrDefault(up => up.IdentityUserId == userId);

        if (userProfile == null)
        {
            return NotFound(new { message = "User not found." });
        }

        var newPost = new Posts()
        {
            Title = createPostDTO.Title,
            PublicationDate = createPostDTO.PublicationDate ?? DateTime.Now,
            Content = createPostDTO.Content,
            HeaderImage = createPostDTO.HeaderImage,
            CategoryId = createPostDTO.CategoryId,
            Author = userProfile.FullName,
            IsApproved = false
        };

        _dbContext.Posts.Add(newPost);
        _dbContext.SaveChanges();

        return CreatedAtAction("GetPostDetails", new { id = newPost.Id }, newPost);
    }

    [HttpPut("edit/{id}")]
    [Authorize]
    public IActionResult EditPost(int id, EditPostDTO editPostDTO)
    {
        var post = _dbContext.Posts.FirstOrDefault(p => p.Id == id);

        if (post == null)
        {
            return NotFound(new { message = "Post not found." });
        }

        post.Title = editPostDTO.Title;
        post.Content = editPostDTO.Content;
        post.CategoryId = editPostDTO.CategoryId;
        post.HeaderImage = editPostDTO.HeaderImage;
        post.PublicationDate = editPostDTO.PublicationDate ?? post.PublicationDate;

        _dbContext.SaveChanges();

        return NoContent();
    }

    //Delete a post
    [HttpDelete("delete/{id}")]
    [Authorize]
    public IActionResult DeletePost(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var post = _dbContext.Posts.FirstOrDefault(p => p.Id == id);

        if (post == null)
        {
            return NotFound(new { message = "Post not found." });
        }

        var userProfile = _dbContext.UserProfiles.FirstOrDefault(up => up.IdentityUserId == userId);

        if (userProfile == null || post.Author != userProfile.FullName)
        {
            return Forbid();
        }

        _dbContext.Posts.Remove(post);
        _dbContext.SaveChanges();

        return NoContent();
    }

}
