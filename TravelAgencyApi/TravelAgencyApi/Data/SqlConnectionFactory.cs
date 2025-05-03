using Microsoft.Data.SqlClient;

namespace TravelAgencyApi.Data;

public class SqlConnectionFactory(IConfiguration cfg) : ISqlConnectionFactory
{
    private readonly string _cs = cfg.GetConnectionString("PJATKDatabase")!;

    public async Task<SqlConnection> CreateOpenAsync(CancellationToken ct = default)
    {
        var conn = new SqlConnection(_cs);
        await conn.OpenAsync(ct);
        return conn;
    }
}