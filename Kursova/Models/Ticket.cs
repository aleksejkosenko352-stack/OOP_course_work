namespace Kursova.Models
{
    public class Ticket
    {
        public long Id { get; set; }
        public long ScreeningId { get; set; }
        public string SeatNumber { get; set; }
        public string CustomerName { get; set; }
    }
}
