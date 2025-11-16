namespace Kursova.Models
{
    public class Screening
    {
        public long Id { get; set; }

        public long MovieId { get; set; }

        public long HallId { get; set; }

        public DateTime StartTime { get; set; }
    }
}
