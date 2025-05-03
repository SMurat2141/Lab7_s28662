namespace TravelAgencyApi.DTOs;

public record TripDto(
    int IdTrip,
    string Name,
    string Description,
    DateTime DateFrom,
    DateTime DateTo,
    int MaxPeople,
    IEnumerable<string> Countries,
    int RegisteredClientsCount);