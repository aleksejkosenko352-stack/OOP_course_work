using Kursova;
using Kursova.Models;
using Kursova.Repositories;
using Npgsql;

public class HallRepository : IHallRepository
{
    private readonly DbConnectionFactory _factory;
    private readonly ILogger<HallRepository> _logger;

    public HallRepository(DbConnectionFactory factory, ILogger<HallRepository> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<IEnumerable<Hall>> GetAllAsync()
    {
        var list = new List<Hall>();
        const string sql = "SELECT id, name, seats FROM halls";

        try
        {
            _logger.LogInformation("Executing SQL: {Sql}", sql);

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Hall
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Seats = reader.GetInt32(2)
                });
            }

            return list;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching halls");
            throw;
        }
    }

    public async Task<Hall?> GetByIdAsync(long id)
    {
        const string sql = "SELECT id, name, seats FROM halls WHERE id=@id";

        try
        {
            _logger.LogInformation("Executing SQL: {Sql} with id={Id}", sql, id);

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Hall
                {
                    Id = reader.GetInt64(0),
                    Name = reader.GetString(1),
                    Seats = reader.GetInt32(2)
                };
            }

            _logger.LogWarning("Hall id={Id} not found", id);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hall by id={Id}", id);
            throw;
        }
    }

    public async Task<long> CreateAsync(Hall hall)
    {
        const string sql = @"
            INSERT INTO halls (name, seats)
            VALUES (@name, @seats)
            RETURNING id;
        ";

        try
        {
            _logger.LogInformation("Executing SQL: {Sql} with hall={Hall}", sql, hall);

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@name", hall.Name);
            cmd.Parameters.AddWithValue("@seats", hall.Seats);

            var id = (long)await cmd.ExecuteScalarAsync();
            _logger.LogInformation("Hall created with id={Id}", id);
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hall");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Hall hall)
    {
        const string sql = @"
            UPDATE halls
            SET name=@name, seats=@seats
            WHERE id=@id
        ";

        try
        {
            _logger.LogInformation("Executing SQL: {Sql}", sql);

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", hall.Id);
            cmd.Parameters.AddWithValue("@name", hall.Name);
            cmd.Parameters.AddWithValue("@seats", hall.Seats);

            var affected = await cmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Updated {Count} hall(s)", affected);

            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hall id={Id}", hall.Id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        const string sql = "DELETE FROM halls WHERE id=@id";

        try
        {
            _logger.LogInformation("Executing SQL: {Sql} with id={Id}", sql, id);

            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            var affected = await cmd.ExecuteNonQueryAsync();
            _logger.LogInformation("Deleted {Count} hall(s)", affected);

            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hall id={Id}", id);
            throw;
        }
    }
}