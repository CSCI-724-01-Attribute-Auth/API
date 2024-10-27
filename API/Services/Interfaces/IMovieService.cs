using API.Models;

namespace API.Services.Interfaces
{
    public interface IMovieService
    {
        public List<Movie> GetAllMovies();
        public Movie? GetMovie(int id);
        public Movie CreateMovie(string title, string description, double totalBudget, double totalCost, DateTime releaseDate, List<int> crewIds);
    }
}
