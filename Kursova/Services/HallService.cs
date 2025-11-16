using Kursova.Models;
using Kursova.Repositories;

public class HallService
{
    private readonly IHallRepository _repo;
    private readonly ILogger<HallService> _logger;

    public HallService(IHallRepository repo, ILogger<HallService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<IEnumerable<Hall>> GetAllAsync()
    {
        _logger.LogInformation("HallService: GetAllAsync");
        return await _repo.GetAllAsync();
    }

    public async Task<Hall?> GetByIdAsync(long id)
    {
        _logger.LogInformation("HallService: GetById {Id}", id);
        return await _repo.GetByIdAsync(id);
    }

    public async Task<long> CreateAsync(Hall hall)
    {
        _logger.LogInformation("HallService: Create {Hall}", hall);
        return await _repo.CreateAsync(hall);
    }

    public async Task<bool> UpdateAsync(Hall hall)
    {
        _logger.LogInformation("HallService: Update {Id}", hall.Id);
        return await _repo.UpdateAsync(hall);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        _logger.LogInformation("HallService: Delete {Id}", id);
        return await _repo.DeleteAsync(id);
    }
}