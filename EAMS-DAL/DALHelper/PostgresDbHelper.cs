using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMS_DAL.DALHelper
{
    public class PostgresDbHelper
    {
        private readonly string? _connectionString;


        public PostgresDbHelper(IConfiguration configuration)
        {
            // Priority: environment variable first, fallback to appsettings.json
            _connectionString = Environment.GetEnvironmentVariable("Postgres")
                                ?? configuration.GetConnectionString("Postgres");
        }

        public async Task<NpgsqlConnection> GetOpenConnectionAsync(CancellationToken cancellationToken)
        {
            var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);
            return conn;
        }
    }
}
