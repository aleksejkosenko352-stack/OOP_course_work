using Kursova;
using Kursova.Models;
using Npgsql;

public class ScreeningRepository : IScreeningRepository
{
    private readonly DbConnectionFactory _factory;
    private readonly ILogger<ScreeningRepository> _logger;

    public ScreeningRepository(DbConnectionFactory factory, ILogger<ScreeningRepository> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<long> CreateAsync(Screening s)
    {
        const string sql = @"
            INSERT INTO screenings (movie_id, hall_id, start_time)
            VALUES (@movie_id, @hall_id, @start_time)
            RETURNING id;
        ";

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            _logger.LogInformation("SQL: {Sql}", sql);

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@movie_id", s.MovieId);
            cmd.Parameters.AddWithValue("@hall_id", s.HallId);
            cmd.Parameters.AddWithValue("@start_time", s.StartTime);

            var id = (long)await cmd.ExecuteScalarAsync();
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating screening");
            throw;
        }
    }

    public async Task<IEnumerable<Screening>> GetAllAsync()
    {
        const string sql = @"
            SELECT id, movie_id, hall_id, start_time
            FROM screenings
            ORDER BY start_time;
        ";

        var list = new List<Screening>();

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            await using var r = await cmd.ExecuteReaderAsync();

            while (await r.ReadAsync())
            {
                list.Add(new Screening
                {
                    Id = r.GetInt64(0),
                    MovieId = r.GetInt64(1),
                    HallId = r.GetInt64(2),
                    StartTime = r.GetDateTime(3)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading screenings");
            throw;
        }

        return list;
    }

    public async Task<IEnumerable<Screening>> GetByMovieIdAsync(long movieId)
    {
        const string sql = @"
            SELECT id, movie_id, hall_id, start_time
            FROM screenings
            WHERE movie_id = @movieId
            ORDER BY start_time;
        ";

        var list = new List<Screening>();

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@movieId", movieId);

            await using var r = await cmd.ExecuteReaderAsync();

            while (await r.ReadAsync())
            {
                list.Add(new Screening
                {
                    Id = r.GetInt64(0),
                    MovieId = r.GetInt64(1),
                    HallId = r.GetInt64(2),
                    StartTime = r.GetDateTime(3)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading screenings by movie");
            throw;
        }

        return list;
    }

    public async Task<bool> UpdateAsync(Screening s)
    {
        const string sql = @"
            UPDATE screenings
            SET movie_id=@movie_id, hall_id=@hall_id, start_time=@start_time
            WHERE id=@id;
        ";

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", s.Id);
            cmd.Parameters.AddWithValue("@movie_id", s.MovieId);
            cmd.Parameters.AddWithValue("@hall_id", s.HallId);
            cmd.Parameters.AddWithValue("@start_time", s.StartTime);

            var a = await cmd.ExecuteNonQueryAsync();
            return a > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating screening");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        const string sql = "DELETE FROM screenings WHERE id=@id";

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            var a = await cmd.ExecuteNonQueryAsync();
            return a > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting screening");
            throw;
        }
    }
}