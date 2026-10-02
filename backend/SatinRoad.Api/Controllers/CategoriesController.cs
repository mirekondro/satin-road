using Microsoft.AspNetCore.Mvc;
using SatinRoad.Api.Contracts;
using SatinRoad.Core.Categories;
using SatinRoad.Core.Entities;

namespace SatinRoad.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(CategoryService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<List<CategoryDto>>(StatusCodes.Status200OK)]
    public async Task<List<CategoryDto>> GetAll()
    {
        var categories = await service.GetAllAsync();
        return categories.Select(ToDto).ToList();
    }

    [HttpPost]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryDto>> Create(CategoryRequest request)
    {
        var created = await service.CreateAsync(request.Name);
        return Created($"/api/categories/{created.Id}", ToDto(created));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<CategoryDto> Update(int id, CategoryRequest request) =>
        ToDto(await service.UpdateAsync(id, request.Name));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }

    private static CategoryDto ToDto(Category c) => new(c.Id, c.Name);
}