using Kursova.Models;
using Npgsql;
using Microsoft.Extensions.Logging;

namespace Kursova.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DbConnectionFactory _factory;
        private readonly ILogger<TicketRepository> _logger;

        public TicketRepository(DbConnectionFactory factory, ILogger<TicketRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        // 14. Купить билет
        public async Task<long> BuyTicketAsync(Ticket ticket)
        {
            const string sql = @"
            INSERT INTO tickets (screening_id, seat_number, customer_name)
            VALUES (@screeningId, @seatNumber, @customerName)
            RETURNING id;
                ";

            try
            {
                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@screeningId", ticket.ScreeningId);
                cmd.Parameters.AddWithValue("@seatNumber", ticket.SeatNumber);
                cmd.Parameters.AddWithValue("@customerName", ticket.CustomerName);

                var id = (long)await cmd.ExecuteScalarAsync();
                _logger.LogInformation("Ticket purchased: {@Ticket}", ticket);
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error buying ticket {@Ticket}", ticket);
                throw;
            }
        }

        // 15. Вернуть билет
        public async Task<bool> ReturnTicketAsync(long ticketId)
        {
            const string sql = "DELETE FROM tickets WHERE id=@id";

            try
            {
                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", ticketId);

                var affected = await cmd.ExecuteNonQueryAsync();
                _logger.LogInformation("Ticket returned, id={TicketId}", ticketId);
                return affected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning ticket id={TicketId}", ticketId);
                throw;
            }
        }

        // 16. Получить билеты для сеанса
        public async Task<IEnumerable<Ticket>> GetTicketsByScreeningAsync(long screeningId)
        {
            var list = new List<Ticket>();
            const string sql = "SELECT id, screening_id, seat_number, customer_name FROM tickets WHERE screening_id=@screeningId";

            try
            {
                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@screeningId", screeningId);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    list.Add(new Ticket
                    {
                        Id = reader.GetInt64(0),
                        ScreeningId = reader.GetInt64(1),
                        SeatNumber = reader.GetString(2),
                        CustomerName = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tickets for screeningId={ScreeningId}", screeningId);
                throw;
            }

            return list;
        }

        // 17. Получить доступные места для сеанса
        public async Task<IEnumerable<string>> GetAvailableSeatsAsync(long screeningId, IEnumerable<string> allSeats)
        {
            var takenSeats = new HashSet<string>();
            const string sql = "SELECT seat_number FROM tickets WHERE screening_id=@screeningId";

            try
            {
                await using var con = _factory.CreateConnection();
                await con.OpenAsync();

                await using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@screeningId", screeningId);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    takenSeats.Add(reader.GetString(0));
                }

                return allSeats.Where(s => !takenSeats.Contains(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching available seats for screeningId={ScreeningId}", screeningId);
                throw;
            }
        }
    }
}
