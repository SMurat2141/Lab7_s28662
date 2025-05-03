// Data/ITripRepository.cs
using TravelAgencyApi.DTOs;
using System.Threading;

namespace TravelAgencyApi.Data;

public interface ITripRepository
{
    Task<IEnumerable<TripDto>> GetTripsAsync(CancellationToken ct = default);
}