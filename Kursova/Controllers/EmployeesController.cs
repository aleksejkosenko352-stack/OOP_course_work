using Kursova.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly EmployeeService _service;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(EmployeeService service, ILogger<EmployeesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // 19. Get all employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _service.GetAllAsync();
        return Ok(employees);
    }

    // 18. Add employee
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee e)
    {
        var id = await _service.CreateAsync(e);
        e.Id = id;
        return CreatedAtAction(nameof(GetAll), new { id }, e);
    }

    // 20. Delete employee
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();

        return NoContent();
    }
}
