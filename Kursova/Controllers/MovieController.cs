using Kursova.Models;
using Kursova.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kursova.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MovieController : ControllerBase
    {
        private readonly MovieService _service;
        private readonly ILogger<MovieController> _logger;

        public MovieController(MovieService service, ILogger<MovieController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // --- CRUD ---
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var movie = await _service.GetByIdAsync(id);
            return movie == null ? NotFound() : Ok(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Movie movie)
        {
            var id = await _service.CreateAsync(movie);
            movie.Id = id;
            return CreatedAtAction(nameof(GetById), new { id }, movie);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] Movie movie)
        {
            if (id != movie.Id) return BadRequest();
            var updated = await _service.UpdateAsync(movie);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("genre/{genre}")]
        public async Task<IActionResult> GetByGenre(string genre)
            => Ok(await _service.GetByGenreAsync(genre));

        [HttpGet("duration/total")]
        public async Task<IActionResult> GetTotalDuration()
        {
            var total = await _service.GetTotalDurationAsync();
            return Ok(new { totalDuration = total });
        }

        [HttpGet("{id}/income")]
        public async Task<IActionResult> GetIncome(long id)
        {
            var income = await _service.GetMovieIncomeAsync(id);
            return Ok(new { movieId = id, income });
        }

        [HttpGet("stats/weekly")]
        public async Task<IActionResult> GetWeeklyStats()
        {
            var stats = await _service.GetWeeklyStatsAsync();
            return Ok(stats);
        }
    }
}
