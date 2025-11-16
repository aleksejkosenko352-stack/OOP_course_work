using Kursova.Models;

public interface IScreeningRepository
{
    Task<long> CreateAsync(Screening s);
    Task<IEnumerable<Screening>> GetAllAsync();
    Task<IEnumerable<Screening>> GetByMovieIdAsync(long movieId);
    Task<bool> UpdateAsync(Screening s);
    Task<bool> DeleteAsync(long id);
}