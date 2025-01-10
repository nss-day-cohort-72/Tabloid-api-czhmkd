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
    // [Authorize]
    public class PostTagController : ControllerBase
    {
        private readonly TabloidDbContext _dbContext;

        public PostTagController(TabloidDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{postId}")]
        public IActionResult GetTagsByPostId(int postId)
        {
            var tags = _dbContext.PostTags
                .Where(pt => pt.PostId == postId)
                .Include(pt => pt.Tag)
                .Select(pt => new
                {
                    pt.Tag.Id,
                    pt.Tag.Name
                })
                .ToList();

            return Ok(tags);
        }


        [HttpPost]
        public IActionResult UpdatePostTags([FromBody] PostTagDTO dto)
        {
            var post = _dbContext.Posts.Include(p => p.PostTags).FirstOrDefault(p => p.Id == dto.PostId);
            if (post == null) return NotFound("Post not found.");

            // Get the current tag associations
            var currentTagIds = post.PostTags.Select(pt => pt.TagId).ToList();

            // Determine tags to add
            var tagsToAdd = dto.TagIds.Except(currentTagIds).ToList();
            foreach (var tagId in tagsToAdd)
            {
                post.PostTags.Add(new PostTag { PostId = dto.PostId, TagId = tagId });
            }

            // Determine tags to remove
            var tagsToRemove = currentTagIds.Except(dto.TagIds).ToList();
            foreach (var tagId in tagsToRemove)
            {
                var postTag = post.PostTags.FirstOrDefault(pt => pt.TagId == tagId);
                if (postTag != null)
                {
                    _dbContext.PostTags.Remove(postTag);
                }
            }

            _dbContext.SaveChanges();

            return Ok(new { message = "Tags successfully updated." });
        }

    }
}
