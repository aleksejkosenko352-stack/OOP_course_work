using Kursova.Models;

public class ScreeningService
{
    private readonly IScreeningRepository _repo;
    private readonly ILogger<ScreeningService> _logger;

    public ScreeningService(IScreeningRepository repo, ILogger<ScreeningService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public Task<long> CreateAsync(Screening s) => _repo.CreateAsync(s);

    public Task<IEnumerable<Screening>> GetAllAsync() => _repo.GetAllAsync();

    public Task<IEnumerable<Screening>> GetByMovieIdAsync(long movieId)
        => _repo.GetByMovieIdAsync(movieId);

    public Task<bool> UpdateAsync(Screening s) => _repo.UpdateAsync(s);

    public Task<bool> DeleteAsync(long id) => _repo.DeleteAsync(id);
}