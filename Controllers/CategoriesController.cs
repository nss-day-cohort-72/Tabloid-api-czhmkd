using Microsoft.AspNetCore.Mvc;
using Tabloid.Models;
using Tabloid.Models.DTOs;
using Tabloid.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Tabloid.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{

    private TabloidDbContext _dbContext;

    public CategoriesController(TabloidDbContext context)
    {
        _dbContext = context;
    }


    ////Get Endpoints
    //Get all categories if role admin and list them alphabetically
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllCategories()
    {
        var categories = _dbContext.Category
           .OrderBy(c => c.Name)
              .Select(c => new
              {
                  c.Id,
                  c.Name
              })
           .ToList();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetCategoryById(int id)
    {
        var category = _dbContext.Category.FirstOrDefault(c => c.Id == id);
        return Ok(category);
    }

    ///Post Endpoints
    //Post new category
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateCategory(Category category)
    {
        _dbContext.Category.Add(category);
        _dbContext.SaveChanges();
        return Ok(category);
    }

    ////Put Endpoints
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult EditCategory(int id, Category category)
    {
        Category foundCategory = _dbContext.Category.FirstOrDefault(c => c.Id == id);
        foundCategory.Name = category.Name;
        _dbContext.SaveChanges();

        return NoContent();

    }

    ////Delete Endpoints
    //Delete Category
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteCategory(int id)
    {
        Category foundCategory = _dbContext.Category.FirstOrDefault(c => c.Id == id);
        _dbContext.Category.Remove(foundCategory);
        _dbContext.SaveChanges();

        return NoContent();

    }

}

