using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tabloid.Data;
using Tabloid.Models;
using Tabloid.Models.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace Tabloid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PostTagController : ControllerBase
    {
        private readonly TabloidDbContext _dbContext;

        public PostTagController(TabloidDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Add tags to a post
        [HttpPost]
        public IActionResult AddTagsToPost([FromBody] PostTagDTO dto)
        {
            // Find the post
            var post = _dbContext.Posts.Include(p => p.PostTags).FirstOrDefault(p => p.Id == dto.PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Find the tags
            var tags = _dbContext.Tags.Where(t => dto.TagIds.Contains(t.Id)).ToList();
            if (tags.Count != dto.TagIds.Count)
            {
                return BadRequest("One or more tags do not exist.");
            }

            // Add associations
            foreach (var tag in tags)
            {
                if (!post.PostTags.Any(pt => pt.TagId == tag.Id))
                {
                    post.PostTags.Add(new PostTag { PostId = dto.PostId, TagId = tag.Id });
                }
            }

            _dbContext.SaveChanges();

            // Return success response
            return Ok(new
            {
                Message = $"Tags successfully associated with post: {post.Title}",
                Tags = tags.Select(t => t.Name).ToList()
            });
        }

        // Optional: Remove a tag from a post
        [HttpDelete]
        public IActionResult RemoveTagFromPost([FromBody] PostTagDTO dto)
        {
            // Find the post
            var post = _dbContext.Posts.Include(p => p.PostTags).FirstOrDefault(p => p.Id == dto.PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Remove associations
            foreach (var tagId in dto.TagIds)
            {
                var postTag = post.PostTags.FirstOrDefault(pt => pt.TagId == tagId);
                if (postTag != null)
                {
                    post.PostTags.Remove(postTag);
                }
            }

            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
