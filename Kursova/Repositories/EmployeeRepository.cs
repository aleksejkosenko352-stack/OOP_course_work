using Kursova;
using Kursova.Models;
using Npgsql;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DbConnectionFactory _factory;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(DbConnectionFactory factory, ILogger<EmployeeRepository> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    // 18. Add employee
    public async Task<long> CreateAsync(Employee e)
    {
        const string sql = @"
            INSERT INTO employees (name, role)
            VALUES (@name, @role)
            RETURNING id;
        ";

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            _logger.LogInformation("SQL: {Sql}", sql);

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@name", e.Name);
            cmd.Parameters.AddWithValue("@role", e.Role);

            var id = (long)await cmd.ExecuteScalarAsync();
            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            throw;
        }
    }

    // 19. Get employees
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        const string sql = @"SELECT id, name, role FROM employees ORDER BY id";

        var list = new List<Employee>();

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            await using var r = await cmd.ExecuteReaderAsync();

            while (await r.ReadAsync())
            {
                list.Add(new Employee
                {
                    Id = r.GetInt64(0),
                    Name = r.GetString(1),
                    Role = r.GetString(2)
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employees");
            throw;
        }

        return list;
    }

    // 20. Delete employee
    public async Task<bool> DeleteAsync(long id)
    {
        const string sql = "DELETE FROM employees WHERE id=@id";

        try
        {
            await using var con = _factory.CreateConnection();
            await con.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);

            var affected = await cmd.ExecuteNonQueryAsync();
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting employee");
            throw;
        }
    }
}