using Kursova.Models;

public interface IEmployeeRepository
{
    Task<long> CreateAsync(Employee e);
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<bool> DeleteAsync(long id);
}