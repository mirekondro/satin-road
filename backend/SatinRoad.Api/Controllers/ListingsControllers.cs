using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatinRoad.Api.Auth;
using SatinRoad.Api.Contracts;
using SatinRoad.Core.Listings;

namespace SatinRoad.Api.Controllers;

[ApiController]
[Route("api/listings")]
public class ListingsController(ListingService service) : ControllerBase
{
//public
    [HttpGet]
    [ProducesResponseType<List<ListingView>>(StatusCodes.Status200OK)]
    public Task<List<ListingView>> GetAll([FromQuery] int? categoryId) =>
        service.GetActiveAsync(categoryId);

    [HttpGet("{id:int}")]
    [ProducesResponseType<ListingView>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ListingView> GetById(int id) =>
        service.GetByIdAsync(id);

// signed it user
    [Authorize]
    [HttpGet("mine")]
    [ProducesResponseType<List<ListingView>>(StatusCodes.Status200OK)]
    public Task<List<ListingView>> GetMine() =>
        service.GetMineAsync(User.GetUserId());

    [Authorize]
    [HttpPost]
    [ProducesResponseType<ListingView>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ListingView>> Create(ListingRequest request)
    {
        var created = await service.CreateAsync(User.GetUserId(), ToInput(request));
        return Created($"/api/listings/{created.Id}", created);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    [ProducesResponseType<ListingView>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ListingView> Update(int id, ListingRequest request) =>
        service.UpdateAsync(User.GetUserId(), id, ToInput(request));

    [Authorize]
    [HttpPatch("{id:int}/stock")]
    [ProducesResponseType<ListingView>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ListingView> UpdateStock(int id, StockRequest request) =>
        service.UpdateStockAsync(User.GetUserId(), id, request.Stock);

    [Authorize]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(int id)
    {
        await service.DeactivateAsync(User.GetUserId(), id);
        return NoContent();
    }

    private static ListingInput ToInput(ListingRequest r) =>
        new(r.CategoryId, r.Title, r.Description, r.Price, r.Stock);
}