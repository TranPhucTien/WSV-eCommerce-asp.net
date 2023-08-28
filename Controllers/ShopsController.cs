using Microsoft.AspNetCore.Mvc;
using shopDev.Services;

namespace shopDev.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopsController : Controller
{
    private readonly ShopsService _shopsService;

    public ShopsController(ShopsService shopsService) => _shopsService = shopsService;

    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> Get()
    {
        var existingShop = await _shopsService.GetAsync();

        if (!existingShop.Any())
        {
            return NotFound();
        }

        return Ok(existingShop);
    }
    
    [HttpGet]
    [Route("get/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var existingShop = await _shopsService.GetAsync(id.ToString());

        return Ok(existingShop);
    }
}