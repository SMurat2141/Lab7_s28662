using Microsoft.Data.SqlClient;
using TravelAgencyApi.DTOs;

namespace TravelAgencyApi.Data;

public class TripRepository(ISqlConnectionFactory factory) : ITripRepository
{
    public async Task<IEnumerable<TripDto>> GetTripsAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT  t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
                                   c.Name AS Country, COUNT(ct2.IdClient) AS Registered
                           FROM Trip t
                           JOIN Country_Trip  ct  ON ct.IdTrip = t.IdTrip
                           JOIN Country       c   ON c.IdCountry = ct.IdCountry
                           LEFT JOIN Client_Trip ct2 ON ct2.IdTrip = t.IdTrip
                           GROUP BY t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name
                           ORDER BY t.DateFrom DESC;
                           """;

        await using var conn = await factory.CreateOpenAsync(ct);
        await using var cmd  = new SqlCommand(sql, conn);
        await using var rdr  = await cmd.ExecuteReaderAsync(ct);

        var map = new Dictionary<int, Temp>();
        while (await rdr.ReadAsync(ct))
        {
            int id = rdr.GetInt32(0);
            if (!map.TryGetValue(id, out var t))
            {
                t = new Temp(
                    id, rdr.GetString(1), rdr.GetString(2),
                    rdr.GetDateTime(3), rdr.GetDateTime(4),
                    rdr.GetInt32(5), rdr.GetInt32(7));
                map[id] = t;
            }
            t.Countries.Add(rdr.GetString(6));
        }

        return map.Values.Select(t => new TripDto(
            t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo,
            t.MaxPeople, t.Countries, t.Registered));
    }

    private sealed record Temp(
        int IdTrip, string Name, string Description,
        DateTime DateFrom, DateTime DateTo, int MaxPeople, int Registered)
    {
        public List<string> Countries { get; } = [];
    }
}