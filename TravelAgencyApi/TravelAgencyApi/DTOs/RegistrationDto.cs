namespace TravelAgencyApi.DTOs;

public record RegistrationDto(
    int IdClient,
    int IdTrip,
    DateTime RegisteredAt,
    DateTime? PaymentDate);