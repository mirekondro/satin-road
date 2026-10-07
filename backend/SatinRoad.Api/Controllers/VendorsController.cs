using Microsoft.AspNetCore.Mvc;
using SatinRoad.Core.Vendors;

namespace SatinRoad.Api.Controllers;

[ApiController]
[Route("api/vendors")]
public class VendorsController(VendorService service) : ControllerBase
{
    /// <summary>Vendors with more than 100 sold orders (hard story #13). Public.</summary>
    [HttpGet("featured")]
    [ProducesResponseType<List<FeaturedVendor>>(StatusCodes.Status200OK)]
    public Task<List<FeaturedVendor>> Featured() => service.GetFeaturedAsync();
}
