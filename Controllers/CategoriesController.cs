using Microsoft.AspNetCore.Mvc;
using Tabloid.Models;
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



    ////Put Endpoints

}

