#nullable disable
namespace API.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate {  get; set; }
        public int MostFamousMovieId { get; set; }
        public virtual Movie MostFamousMovie { get; set; }
        public virtual List<Movie> WorkedOn { get; set; }
    }
}
