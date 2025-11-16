using Npgsql;
using System.Data;

namespace Kursova
{
    public class DbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
