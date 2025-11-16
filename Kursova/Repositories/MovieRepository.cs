using Kursova.Models;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace Kursova.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly DbConnectionFactory _factory;
        private readonly ILogger<MovieRepository> _logger;

        public MovieRepository(DbConnectionFactory factory, ILogger<MovieRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            var list = new List<Movie>();
            const string sql = "SELECT id, title, duration, genre FROM movies";

            try
            {
                _logger.LogInformation("Executing SQL: {Sql}", sql);

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add(new Movie
                    {
                        Id = reader.GetInt64(0),
                        Title = reader.GetString(1),
                        Duration = reader.GetInt32(2),
                        Genre = reader.GetString(3)
                    });
                }

                _logger.LogInformation("Fetched {Count} movies", list.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movies");
                throw;
            }

            return list;
        }

        public async Task<Movie?> GetByIdAsync(long id)
        {
            const string sql = "SELECT id, title, duration, genre FROM movies WHERE id=@id";

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
                    return new Movie
                    {
                        Id = reader.GetInt64(0),
                        Title = reader.GetString(1),
                        Duration = reader.GetInt32(2),
                        Genre = reader.GetString(3)
                    };
                }

                _logger.LogWarning("Movie with id={Id} not found", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie by id={Id}", id);
                throw;
            }

            return null;
        }

        public async Task<long> CreateAsync(Movie movie)
        {
            const string sql = @"
                INSERT INTO movies (title, duration, genre)
                VALUES (@title, @duration, @genre)
                RETURNING id;
            ";

            try
            {
                _logger.LogInformation("Executing SQL: {Sql} with movie={Movie}", sql, movie);

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@title", movie.Title);
                cmd.Parameters.AddWithValue("@duration", movie.Duration);
                cmd.Parameters.AddWithValue("@genre", movie.Genre);

                var id = (long)await cmd.ExecuteScalarAsync();
                _logger.LogInformation("Created movie with id={Id}", id);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie {Movie}", movie);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            const string sql = "DELETE FROM movies WHERE id=@id";

            try
            {
                _logger.LogInformation("Executing SQL: {Sql} with id={Id}", sql, id);

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", id);

                var affected = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Deleted {Count} movie(s)", affected);
                return affected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie id={Id}", id);
                throw;
            }
        }
        public async Task<bool> UpdateAsync(Movie movie)
        {
            const string sql = @"
        UPDATE movies
        SET title=@title, duration=@duration, genre=@genre
        WHERE id=@id";

            try
            {
                _logger.LogInformation("Executing SQL: {Sql} with movie={Movie}", sql, movie);

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", movie.Id);
                cmd.Parameters.AddWithValue("@title", movie.Title);
                cmd.Parameters.AddWithValue("@duration", movie.Duration);
                cmd.Parameters.AddWithValue("@genre", movie.Genre);

                var affected = await cmd.ExecuteNonQueryAsync();
                return affected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie id={Id}", movie.Id);
                throw;
            }
        }

    

    public async Task<IEnumerable<Movie>> GetByGenreAsync(string genre)
        {
            var list = new List<Movie>();
            const string sql = "SELECT id, title, duration, genre FROM movies WHERE genre=@genre";

            try
            {
                _logger.LogInformation("Executing SQL: {Sql} with genre={Genre}", sql, genre);

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@genre", genre);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Movie
                    {
                        Id = reader.GetInt64(0),
                        Title = reader.GetString(1),
                        Duration = reader.GetInt32(2),
                        Genre = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movies by genre={Genre}", genre);
                throw;
            }

            return list;
        }

        // 22. Отримати тривалість усіх фільмів
        public async Task<int> GetTotalDurationAsync()
        {
            const string sql = "SELECT COALESCE(SUM(duration),0) FROM movies";
            try
            {
                _logger.LogInformation("Executing SQL: {Sql}", sql);
                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total movie duration");
                throw;
            }
        }

        // 24. Отримати доходи з фільму (через Tickets, цена фиксированная, например 150)
        public async Task<decimal> GetMovieIncomeAsync(long movieId, decimal ticketPrice = 150)
        {
            const string sql = @"
                SELECT COUNT(*) 
                FROM tickets t
                JOIN screenings s ON t.screening_id = s.id
                WHERE s.movie_id=@movieId
            ";
            try
            {
                _logger.LogInformation("Executing SQL: {Sql} for movieId={MovieId}", sql, movieId);

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@movieId", movieId);

                var count = (long)await cmd.ExecuteScalarAsync();
                return count * ticketPrice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating income for movieId={MovieId}", movieId);
                throw;
            }
        }

        // 25. Отримати статистику за тиждень
        public async Task<IEnumerable<(long MovieId, int TicketsSold)>> GetWeeklyStatsAsync()
        {
            var list = new List<(long MovieId, int TicketsSold)>();
            const string sql = @"
                SELECT s.movie_id, COUNT(*) AS sold
                FROM tickets t
                JOIN screenings s ON t.screening_id = s.id
                WHERE t.purchase_time >= NOW() - INTERVAL '7 days'
                GROUP BY s.movie_id
            ";

            try
            {
                _logger.LogInformation("Executing SQL for weekly stats");

                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    list.Add((reader.GetInt64(0), reader.GetInt32(1)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weekly stats");
                throw;
            }

            return list;
        }
    }


}
