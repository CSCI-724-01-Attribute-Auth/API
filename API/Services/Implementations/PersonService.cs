using API.Data;
using API.Models;
using API.Services.Interfaces;

namespace API.Services.Implementations
{
    public class PersonService : IPersonService
    {
        private readonly DBContext _dbContext;

        public PersonService(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Person> GetAllPersons()
        {
            return _dbContext.Persons.ToList();
        }

        public Person? GetPerson(int id) {
            return _dbContext.Persons.SingleOrDefault(m => m.Id == id);
        }

        public Person CreatePerson(string name, DateTime birthDate, int mostFamousMovieId)
        {
            if (_dbContext.Movies.SingleOrDefault(m => m.Id == mostFamousMovieId) == null)
            {
                throw new InvalidDataException("The movie Id associated with this person's most famous movie does not exist. Please create this movie and try again.");
            }

            var toAdd = new Person
            {
                Id = _dbContext.Movies.Count(),
                Name = name,
                BirthDate = birthDate,
                MostFamousMovieId = mostFamousMovieId
            };

            _dbContext.Persons.Add(toAdd);
            _dbContext.SaveChanges();
            return toAdd;
        }
    }
}
