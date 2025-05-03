namespace TravelAgencyApi.DTOs;

public record ClientDto(
    int IdClient,
    string FirstName,
    string LastName,
    string Email,
    string Telephone,
    string Pesel);