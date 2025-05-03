using Microsoft.AspNetCore.Mvc;
using TravelAgencyApi.Data;

namespace TravelAgencyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController(ITripRepository repo) : ControllerBase
{
    /// <summary>GET /api/trips</summary>
    [HttpGet]
    public async Task<IActionResult> GetTrips() =>
        Ok(await repo.GetTripsAsync());
}