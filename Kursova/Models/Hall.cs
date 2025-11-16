namespace Kursova.Models
{
    public class Hall
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }

        public List<Screening> Screenings { get; set; }
    }
}
