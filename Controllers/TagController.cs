using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tabloid.Data;

namespace Tabloid.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // [Authorize(Roles = "Admin")]
    public class TagController : ControllerBase
    {
        private TabloidDbContext _dbContext;

        public TagController(TabloidDbContext context)
        {
            _dbContext = context;
        }

        //Get all tags
        [HttpGet]
        public IActionResult GetAllTags()
        {
            var tags = _dbContext.Tags
                .OrderBy(t => t.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name
                })
                .ToList();

            return Ok(tags);
        }


        //Get tag by id
        [HttpGet("{id}")]
        public IActionResult GetTagById(int id)
        {
            var tag = _dbContext.Tags
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.Name
                })
                .FirstOrDefault();

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

        //Post a new tag
        [HttpPost]
        public IActionResult CreateTag(Tag newTag)
        {
            if (string.IsNullOrWhiteSpace(newTag.Name))
            {
                return BadRequest("Tag name cannot be empty");
            }

            _dbContext.Tags.Add(newTag);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetTagById), new { id = newTag.Id }, newTag);
        }
    }
}