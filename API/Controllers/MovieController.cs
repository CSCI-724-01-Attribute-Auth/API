using API.Data;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("{controller}")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("all")]
        [ProducesResponseType(typeof(List<Movie>), StatusCodes.Status200OK)]
        public IActionResult All()
        {
            return StatusCode(StatusCodes.Status200OK, _movieService.GetAllMovies());
        }

        [HttpGet("")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetMovieById([FromQuery] int id)
        {
            var movie = _movieService.GetMovie(id);

            if (movie == null) {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status200OK, movie);
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateMovie(
            [FromQuery] string title,
            [FromQuery] string description,
            [FromQuery] double totalBudget,
            [FromQuery] double totalCost,
            [FromQuery] DateTime releaseDate,
            [FromQuery] List<int> crewIds
        )
        {
            try
            {
                var createdMovie = _movieService.CreateMovie(title, description, totalBudget, totalCost, releaseDate, crewIds);

                return StatusCode(StatusCodes.Status201Created, createdMovie);
            }
            catch (InvalidDataException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }
    }
}
