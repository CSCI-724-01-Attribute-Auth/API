using API.Data;
using API.Models;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

// The MovieService class acts as a centralized service layer for managing Movie entities,
//  performing create, retrieve, and validate operations with a clear separation of concerns. 
// The CreateMovie method includes validation to prevent inconsistent crew assignments, ensuring
// the integrity of the movie-crew relationships. This service would likely be used by other parts 
//of the application to access and manage movie data reliably.

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
            return _dbContext.Movies.Include(m => m.Crew).AsNoTracking().ToList();
        }

        public Movie? GetMovie(int id) {
            return _dbContext.Movies.Include(m => m.Crew).SingleOrDefault(m => m.Id == id);
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
                Title = title,
                Description = description,
                TotalBudget = totalBudget,
                TotalCost = totalCost,
                ReleaseDate = releaseDate
            };
            _dbContext.Movies.Add(toAdd);
            _dbContext.SaveChanges();

            _dbContext.CrewMembers.AddRange(crewIds.Select(c => new CrewMember
            {
                MovieId = toAdd.Id,
                PersonId = c
            }));
            _dbContext.SaveChanges();

            return toAdd;
        }
    }
}
