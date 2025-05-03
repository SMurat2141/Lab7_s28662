namespace TravelAgencyApi.Models;

public class Client
{
    public int IdClient { get; set; }
    public string FirstName  { get; set; } = default!;
    public string LastName   { get; set; } = default!;
    public string Email      { get; set; } = default!;
    public string Telephone  { get; set; } = default!;
    public string Pesel      { get; set; } = default!;
}