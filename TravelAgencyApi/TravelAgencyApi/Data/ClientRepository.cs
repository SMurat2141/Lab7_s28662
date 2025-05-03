using Microsoft.Data.SqlClient;
using TravelAgencyApi.DTOs;

namespace TravelAgencyApi.Data;

public class ClientRepository(ISqlConnectionFactory factory) : IClientRepository
{
    public async Task<IEnumerable<TripDto>> GetTripsForClientAsync(
        int clientId, CancellationToken ct = default)
    {
        const string sql = """
        SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople,
               c.Name AS Country
        FROM Trip t
        JOIN Client_Trip ct   ON ct.IdTrip = t.IdTrip
        JOIN Country_Trip ctr ON ctr.IdTrip = t.IdTrip
        JOIN Country c        ON c.IdCountry = ctr.IdCountry
        WHERE ct.IdClient = @IdClient;
        """;

        await using var conn = await factory.CreateOpenAsync(ct);
        await using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@IdClient", clientId));

        await using var rdr = await cmd.ExecuteReaderAsync(ct);

        var map = new Dictionary<int, Temp>();
        while (await rdr.ReadAsync(ct))
        {
            int idTrip = rdr.GetInt32(0);
            if (!map.TryGetValue(idTrip, out var t))
            {
                t = new Temp(idTrip, rdr.GetString(1), rdr.GetString(2),
                             rdr.GetDateTime(3), rdr.GetDateTime(4),
                             rdr.GetInt32(5));
                map[idTrip] = t;
            }
            t.Countries.Add(rdr.GetString(6));
        }

        return map.Values.Select(t => new TripDto(
            t.IdTrip, t.Name, t.Description, t.DateFrom,
            t.DateTo, t.MaxPeople, t.Countries, 0));
    }

    public async Task<int> AddClientAsync(
        ClientDto dto, CancellationToken ct = default)
    {
        const string sql = """
        INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel)
        OUTPUT INSERTED.IdClient
        VALUES (@First, @Last, @Email, @Tel, @Pesel);
        """;

        await using var conn = await factory.CreateOpenAsync(ct);
        await using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(new[]
        {
            new SqlParameter("@First", dto.FirstName),
            new SqlParameter("@Last",  dto.LastName),
            new SqlParameter("@Email", dto.Email),
            new SqlParameter("@Tel",   dto.Telephone),
            new SqlParameter("@Pesel", dto.Pesel)
        });

        return (int)await cmd.ExecuteScalarAsync(ct);
    }

    public async Task<bool> RegisterClientForTripAsync(
        int clientId, int tripId, CancellationToken ct = default)
    {
        const string check = """
        SELECT 1 FROM Trip
        WHERE IdTrip = @Trip
          AND MaxPeople > (SELECT COUNT(*) FROM Client_Trip WHERE IdTrip = @Trip);
        """;

        await using var conn = await factory.CreateOpenAsync(ct);
        await using var chk  = new SqlCommand(check, conn);
        chk.Parameters.Add(new SqlParameter("@Trip", tripId));

        if (await chk.ExecuteScalarAsync(ct) is null) return false;

        const string insert = """
        INSERT INTO Client_Trip (IdClient, IdTrip, RegisteredAt)
        VALUES (@Client, @Trip, SYSDATETIME());
        """;

        await using var cmd = new SqlCommand(insert, conn);
        cmd.Parameters.AddRange(new[]
        {
            new SqlParameter("@Client", clientId),
            new SqlParameter("@Trip",   tripId)
        });

        return await cmd.ExecuteNonQueryAsync(ct) == 1;
    }

    public async Task<bool> RemoveClientFromTripAsync(
        int clientId, int tripId, CancellationToken ct = default)
    {
        const string sql = """
        DELETE FROM Client_Trip
        WHERE IdClient = @Client AND IdTrip = @Trip;
        """;

        await using var conn = await factory.CreateOpenAsync(ct);
        await using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(new[]
        {
            new SqlParameter("@Client", clientId),
            new SqlParameter("@Trip",   tripId)
        });

        return await cmd.ExecuteNonQueryAsync(ct) == 1;
    }

    private sealed record Temp(
        int IdTrip, string Name, string Description,
        DateTime DateFrom, DateTime DateTo, int MaxPeople)
    {
        public List<string> Countries { get; } = [];
    }
}
