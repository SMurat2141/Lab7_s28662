using TravelAgencyApi.DTOs;
using System.Threading;

namespace TravelAgencyApi.Data;

public interface IClientRepository
{
    Task<IEnumerable<TripDto>> GetTripsForClientAsync(
        int clientId, CancellationToken ct = default);

    Task<int> AddClientAsync(
        ClientDto dto, CancellationToken ct = default);

    Task<bool> RegisterClientForTripAsync(
        int clientId, int tripId, CancellationToken ct = default);

    Task<bool> RemoveClientFromTripAsync(
        int clientId, int tripId, CancellationToken ct = default);
}