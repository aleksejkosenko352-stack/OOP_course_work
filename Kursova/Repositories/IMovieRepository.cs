using Kursova.Models;

namespace Kursova.Repositories
{
    public interface IMovieRepository
    {
        // Базовые CRUD
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie?> GetByIdAsync(long id);
        Task<long> CreateAsync(Movie movie);
        Task<bool> UpdateAsync(Movie movie);
        Task<bool> DeleteAsync(long id);

        // 21. Получить фильмы по жанру
        Task<IEnumerable<Movie>> GetByGenreAsync(string genre);

        // 22. Суммарная длительность всех фильмов
        Task<int> GetTotalDurationAsync();

        // 24. Доходы с фильма
        Task<decimal> GetMovieIncomeAsync(long movieId, decimal ticketPrice = 150);

        // 25. Статистика за неделю: список (MovieId, TicketsSold)
        Task<IEnumerable<(long MovieId, int TicketsSold)>> GetWeeklyStatsAsync();
    }
}
