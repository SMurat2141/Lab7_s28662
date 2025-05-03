namespace TravelAgencyApi.Models;

public class Trip
{
    public int IdTrip { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime DateFrom { get; set; }
    public DateTime DateTo   { get; set; }
    public int MaxPeople     { get; set; }
    public List<string> Countries { get; set; } = [];
}