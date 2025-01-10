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


    ////Get Endpoints
    //Get all posts that are approved and have a publication date in the past
    [HttpGet("approved")]
    // [Authorize]
    public IActionResult GetAllApprovedPosts()
    {
        var posts = _dbContext.Posts
            .Include(p => p.Category)
            .Include(p => p.Comments)
                .ThenInclude(c => c.UserProfile) // Include UserProfile for each comment
            .Where(p => p.IsApproved == true && p.PublicationDate < DateTime.Now)
            .OrderByDescending(p => p.PublicationDate)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Author,
                p.IsApproved,
                p.PublicationDate,
                Category = p.Category.Name,
                Comments = p.Comments.Select(c => new
                {
                    c.Id,
                    c.Subject,
                    c.Content,
                    c.CreatedAt,
                    User = new
                    {
                        c.UserProfile.Id,
                        FullName = c.UserProfile.FullName,
                        c.UserProfile.ImageLocation
                    }
                })
            })
            .ToList();

        return Ok(posts);
    }


    // //Get the logged in users posts
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

    //Get a single posts details by Id
    [HttpGet("{id}")]
    [Authorize]
    public IActionResult GetPostDetails(int id)
    {
        var post = _dbContext.Posts
        .Include(p => p.Category)
        .Include(p => p.Comments)
            .ThenInclude(c => c.UserProfile)
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
            Category = p.Category.Name,
            Comments = p.Comments.Select(c => new
            {
                c.Id,
                c.Subject,
                c.Content,
                c.CreatedAt,
                User = new
                {
                    c.UserProfile.Id,
                    c.UserProfile.FullName,
                    c.UserProfile.ImageLocation
                }
            }).ToList()
        })
        .FirstOrDefault();

        if (post == null)
        {
            return NotFound(new { message = "Post not found." });
        }

        return Ok(post);
    }


    ////Post Endpoints
    //Create a new post
    [HttpPost("newpost")]
    [Authorize]
    public IActionResult CreatePost(CreatePostDTO createPostDTO)
    {

        //Get logged in users Identity User Id
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        //Find the user profile by the Identity User Id 
        var userProfile = _dbContext.UserProfiles.FirstOrDefault(up => up.IdentityUserId == userId);

        if (userProfile == null)
        {
            return NotFound(new { message = "User not found." });
        }

        //Create a new post object
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

        //Add the post to the database
        _dbContext.Posts.Add(newPost);
        _dbContext.SaveChanges();

        //return the details of the post
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