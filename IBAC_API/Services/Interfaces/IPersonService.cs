using API.Models;

namespace API.Services.Interfaces
{
    public interface IPersonService
    {
        public List<Person> GetAllPersons();
        public Person? GetPerson(int id);
        public Person CreatePerson(string name, DateTime birthDate, int mostFamousMovieId);
    }
}
