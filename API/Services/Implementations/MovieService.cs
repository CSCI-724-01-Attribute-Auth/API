using API.Data;
using API.Models;
using API.Services.Interfaces;

namespace API.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly DBContext _dbContext;

        public MovieService(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Movie> GetAllMovies()
        {
            return _dbContext.Movies.ToList();
        }

        public Movie? GetMovie(int id) {
            return _dbContext.Movies.SingleOrDefault(m => m.Id == id);
        }

        public Movie CreateMovie(string title, string description, double totalBudget, double totalCost, DateTime releaseDate, List<int> crewIds)
        {
            foreach (var cId in crewIds)
            {
                if (_dbContext.Persons.SingleOrDefault(p => p.Id == cId) == null)
                {
                    throw new InvalidDataException("There is a crew Id that is not in the set of crew. Please create this person and try again.");
                }
            }

            var toAdd = new Movie
            {
                Id = _dbContext.Movies.Count(),
                Title = title,
                Description = description,
                TotalBudget = totalBudget,
                TotalCost = totalCost,
                ReleaseDate = releaseDate
            };

            _dbContext.CrewMembers.AddRange(crewIds.Select(c => new CrewMember
            {
                MovieId = toAdd.Id,
                PersonId = c
            }));
            _dbContext.Movies.Add(toAdd);
            _dbContext.SaveChanges();

            return toAdd;
        }
    }
}
