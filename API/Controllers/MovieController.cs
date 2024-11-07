using API.Data;
using API.Models;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// MovieController provides RESTful endpoints to manage movie data.
namespace API.Controllers
{
    /// <summary>
    /// Controller to manage movie-related operations.
    /// </summary>
    [ApiController]
    [Route("{controller}")]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieController"/> class.
        /// </summary>
        /// <param name="movieService">Service to handle movie operations.</param>
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        /// <summary>
        /// Retrieves all movies.
        /// </summary>
        /// <returns>A list of movies with an HTTP 200 status.</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<Movie>), StatusCodes.Status200OK)]
        public IActionResult All()
        {
            return StatusCode(StatusCodes.Status200OK, new { movies = _movieService.GetAllMovies() });
        }

        /// <summary>
        /// Retrieves a movie by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the movie.</param>
        /// <returns>
        /// HTTP 200 if the movie is found, with the movie details in the response body;
        /// HTTP 204 if the movie is not found.
        /// </returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetMovieById([FromQuery] int id)
        {
            var movie = _movieService.GetMovie(id);

            if (movie == null)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status200OK, new { movie });
        }

        /// <summary>
        /// Creates a new movie.
        /// </summary>
        /// <param name="title">The title of the movie.</param>
        /// <param name="description">A brief description of the movie.</param>
        /// <param name="totalBudget">The budget allocated for the movie.</param>
        /// <param name="totalCost">The cost incurred for the movie.</param>
        /// <param name="releaseDate">The release date of the movie.</param>
        /// <param name="crewIds">A list of crew member IDs associated with the movie.</param>
        /// <returns>
        /// HTTP 201 with the created movie details if the operation is successful;
        /// HTTP 400 if the input data is invalid;
        /// HTTP 500 if there is an internal server error.
        /// </returns>
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

                return StatusCode(StatusCodes.Status201Created, new { movie = createdMovie });
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
