using Kursova.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScreeningsController : ControllerBase
{
    private readonly ScreeningService _service;
    private readonly ILogger<ScreeningsController> _logger;

    public ScreeningsController(ScreeningService service, ILogger<ScreeningsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // 10. Отримати розклад сеансів
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    // 11. Отримати сеанси певного фільму
    [HttpGet("movie/{movieId}")]
    public async Task<IActionResult> GetByMovie(long movieId)
    {
        var list = await _service.GetByMovieIdAsync(movieId);
        return Ok(list);
    }

    // 9. Створити новий сеанс
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Screening s)
    {
        var id = await _service.CreateAsync(s);
        s.Id = id;
        return CreatedAtAction(nameof(GetAll), new { id }, s);
    }

    // 12. Оновити сеанс
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] Screening s)
    {
        if (id != s.Id) return BadRequest("IDs mismatch");

        var ok = await _service.UpdateAsync(s);
        if (!ok) return NotFound();

        return NoContent();
    }

    // 13. Видалити сеанс
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();

        return NoContent();
    }
}