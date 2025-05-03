using Microsoft.Data.SqlClient;

namespace TravelAgencyApi.Data;

public interface ISqlConnectionFactory
{
    Task<SqlConnection> CreateOpenAsync(CancellationToken ct = default);
}