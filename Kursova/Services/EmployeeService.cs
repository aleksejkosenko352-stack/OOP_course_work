using Kursova.Models;

public class EmployeeService
{
    private readonly IEmployeeRepository _repo;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repo, ILogger<EmployeeService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public Task<long> CreateAsync(Employee e) => _repo.CreateAsync(e);

    public Task<IEnumerable<Employee>> GetAllAsync() => _repo.GetAllAsync();

    public Task<bool> DeleteAsync(long id) => _repo.DeleteAsync(id);
}