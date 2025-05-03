using Microsoft.AspNetCore.Mvc;
using TravelAgencyApi.Data;
using TravelAgencyApi.DTOs;

namespace TravelAgencyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController(IClientRepository repo) : ControllerBase
{
    [HttpGet("{id:int}/trips")]
    public async Task<IActionResult> GetTripsForClient(int id)
    {
        var trips = await repo.GetTripsForClientAsync(id);
        return trips.Any() ? Ok(trips) : NotFound($"Client {id} not found or has no trips.");
    }

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        int id = await repo.AddClientAsync(dto);
        return CreatedAtAction(nameof(GetTripsForClient), new { id }, new { id });
    }

    [HttpPut("{id:int}/trips/{tripId:int}")]
    public async Task<IActionResult> RegisterForTrip(int id, int tripId) =>
        await repo.RegisterClientForTripAsync(id, tripId) ? NoContent() :
            Conflict("Capacity reached or invalid IDs.");

    [HttpDelete("{id:int}/trips/{tripId:int}")]
    public async Task<IActionResult> RemoveFromTrip(int id, int tripId) =>
        await repo.RemoveClientFromTripAsync(id, tripId) ? NoContent() :
            NotFound("Registration not found.");
}