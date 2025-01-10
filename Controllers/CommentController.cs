using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tabloid.Data;
using Tabloid.Models;

namespace Tabloid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CommentController : ControllerBase
    {
        private TabloidDbContext _dbContext;

        public CommentController(TabloidDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("{postId}")]
        [Authorize]
        public IActionResult GetCommentsByPostId(int postId)
        {
            var postComments = _dbContext.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.UserProfile)
            .ToList();

            if (postComments == null || postComments.Count == 0)
            {
                return NoContent();
            }

            return Ok(postComments);
        }


        [HttpPost("{postId}/add")]
        [Authorize]
        public IActionResult CreateComment(int postId, [FromBody] Comment comment, [FromQuery] int userId)
        {
            Posts post = _dbContext.Posts.FirstOrDefault(p => p.Id == postId);
            UserProfile user = _dbContext.UserProfiles.FirstOrDefault(up => up.Id == userId);
            if (post == null)
            {
                return NotFound(new { message = "Post not found" });
            }
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            comment.UserProfileId = userId;
            comment.PostId = postId;
            _dbContext.Comments.Add(comment);
            _dbContext.SaveChanges();

            return Created($"/api/Comment/{comment.Id}", comment);
        }
    }
}