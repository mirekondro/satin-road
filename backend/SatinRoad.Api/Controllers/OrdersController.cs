using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatinRoad.Api.Auth;
using SatinRoad.Api.Contracts;
using SatinRoad.Core.Orders;

namespace SatinRoad.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public class OrdersController(OrderService service) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<PlacedOrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PlacedOrderResponse>> Place(OrderRequest request)
    {
        var result = await service.PlaceOrderAsync(User.GetUserId(), request.ListingId, request.Quantity);
        var o = result.Order;

        return Created($"/api/orders/{o.Id}", new PlacedOrderResponse(
            o.Id, o.ListingId, o.Quantity, o.UnitPrice, o.DiscountApplied, o.Total,
            result.VendorShutDown));
    }

    /// <summary>Orders I bought.</summary>
    [HttpGet("mine")]
    [ProducesResponseType<List<OrderView>>(StatusCodes.Status200OK)]
    public Task<List<OrderView>> Mine() => service.GetMineAsync(User.GetUserId());

    /// <summary>Orders other users bought from me (as a vendor).</summary>
    [HttpGet("sales")]
    [ProducesResponseType<List<OrderView>>(StatusCodes.Status200OK)]
    public Task<List<OrderView>> Sales() => service.GetSalesAsync(User.GetUserId());
}
