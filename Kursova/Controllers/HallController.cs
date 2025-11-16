using Kursova.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HallsController : ControllerBase
{
    private readonly HallService _service;
    private readonly ILogger<HallsController> _logger;

    public HallsController(HallService service, ILogger<HallsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // 6. Get all halls
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GET /api/halls");
        var halls = await _service.GetAllAsync();
        return Ok(halls);
    }

    // Get by id (optional but useful)
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        _logger.LogInformation("GET /api/halls/{Id}", id);
        var hall = await _service.GetByIdAsync(id);
        if (hall == null)
            return NotFound();
        return Ok(hall);
    }

    // 5. Add a hall
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Hall hall)
    {
        _logger.LogInformation("POST /api/halls");
        var id = await _service.CreateAsync(hall);
        hall.Id = id;
        return CreatedAtAction(nameof(Get), new { id }, hall);
    }

    // 7. Update hall
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] Hall hall)
    {
        if (id != hall.Id)
            return BadRequest("ID mismatch");

        _logger.LogInformation("PUT /api/halls/{Id}", id);

        var result = await _service.UpdateAsync(hall);
        if (!result)
            return NotFound();

        return NoContent();
    }

    // 8. Delete hall
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        _logger.LogInformation("DELETE /api/halls/{Id}", id);
        var result = await _service.DeleteAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}
