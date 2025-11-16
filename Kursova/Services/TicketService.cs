using Kursova.Models;
using Kursova.Repositories;

namespace Kursova.Services
{
    public class TicketService
    {
        private readonly ITicketRepository _repo;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ITicketRepository repo, ILogger<TicketService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public Task<long> BuyTicketAsync(Ticket ticket)
        {
            _logger.LogInformation("Buying ticket {@Ticket}", ticket);
            return _repo.BuyTicketAsync(ticket);
        }

        public Task<bool> ReturnTicketAsync(long ticketId)
        {
            _logger.LogInformation("Returning ticket id={TicketId}", ticketId);
            return _repo.ReturnTicketAsync(ticketId);
        }

        public Task<IEnumerable<Ticket>> GetTicketsByScreeningAsync(long screeningId)
        {
            _logger.LogInformation("Getting tickets for screeningId={ScreeningId}", screeningId);
            return _repo.GetTicketsByScreeningAsync(screeningId);
        }

        public Task<IEnumerable<string>> GetAvailableSeatsAsync(long screeningId, IEnumerable<string> allSeats)
        {
            _logger.LogInformation("Getting available seats for screeningId={ScreeningId}", screeningId);
            return _repo.GetAvailableSeatsAsync(screeningId, allSeats);
        }
    }
}
