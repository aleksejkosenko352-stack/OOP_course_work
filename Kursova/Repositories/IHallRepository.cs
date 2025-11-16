using Kursova.Models;

namespace Kursova.Repositories
{
    public interface IHallRepository
    {
        Task<IEnumerable<Hall>> GetAllAsync();
        Task<Hall?> GetByIdAsync(long id);
        Task<long> CreateAsync(Hall hall);
        Task<bool> UpdateAsync(Hall hall);
        Task<bool> DeleteAsync(long id);
    }
}
