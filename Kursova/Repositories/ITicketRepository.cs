using Kursova.Models;

public interface ITicketRepository
{
    Task<long> BuyTicketAsync(Ticket ticket);
    Task<bool> ReturnTicketAsync(long ticketId);
    Task<IEnumerable<Ticket>> GetTicketsByScreeningAsync(long screeningId);
    Task<IEnumerable<string>> GetAvailableSeatsAsync(long screeningId, IEnumerable<string> allSeats);
}