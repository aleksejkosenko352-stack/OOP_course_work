using Kursova.Models;
using Kursova.Repositories;

namespace Kursova.Services
{
    public class MovieService
    {
        private readonly IMovieRepository _repo;
        private readonly ILogger<MovieService> _logger;

        public MovieService(IMovieRepository repo, ILogger<MovieService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // --- CRUD ---
        public Task<IEnumerable<Movie>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Movie?> GetByIdAsync(long id) => _repo.GetByIdAsync(id);
        public Task<long> CreateAsync(Movie movie) => _repo.CreateAsync(movie);
        public Task<bool> UpdateAsync(Movie movie) => _repo.UpdateAsync(movie);
        public Task<bool> DeleteAsync(long id) => _repo.DeleteAsync(id);

        // --- 21–25 ---
        public Task<IEnumerable<Movie>> GetByGenreAsync(string genre) => _repo.GetByGenreAsync(genre);
        public Task<int> GetTotalDurationAsync() => _repo.GetTotalDurationAsync();
        public Task<decimal> GetMovieIncomeAsync(long movieId, decimal ticketPrice = 150)
            => _repo.GetMovieIncomeAsync(movieId, ticketPrice);
        public Task<IEnumerable<(long MovieId, int TicketsSold)>> GetWeeklyStatsAsync()
            => _repo.GetWeeklyStatsAsync();
    }
}
