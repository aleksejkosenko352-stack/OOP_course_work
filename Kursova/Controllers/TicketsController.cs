using Kursova.Models;
using Kursova.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kursova.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly TicketService _service;
        private readonly ILogger<TicketController> _logger;

        public TicketController(TicketService service, ILogger<TicketController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // 14. Купить билет
        [HttpPost]
        public async Task<IActionResult> BuyTicket([FromBody] Ticket ticket)
        {
            var id = await _service.BuyTicketAsync(ticket);
            ticket.Id = id;
            return CreatedAtAction(nameof(GetTicketsByScreening), new { screeningId = ticket.ScreeningId }, ticket);
        }

        // 15. Вернуть билет
        [HttpDelete("{id}")]
        public async Task<IActionResult> ReturnTicket(long id)
        {
            var result = await _service.ReturnTicketAsync(id);
            return result ? NoContent() : NotFound();
        }

        // 16. Получить билеты для сеанса
        [HttpGet("screening/{screeningId}")]
        public async Task<IActionResult> GetTicketsByScreening(long screeningId)
        {
            var tickets = await _service.GetTicketsByScreeningAsync(screeningId);
            return Ok(tickets);
        }

        // 17. Получить доступные места для сеанса
        [HttpPost("screening/{screeningId}/available-seats")]
        public async Task<IActionResult> GetAvailableSeats(long screeningId, [FromBody] IEnumerable<string> allSeats)
        {
            var seats = await _service.GetAvailableSeatsAsync(screeningId, allSeats);
            return Ok(seats);
        }
    }
}
