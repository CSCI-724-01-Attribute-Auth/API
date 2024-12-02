#nullable disable
using System.Text.Json.Serialization;

namespace API.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate {  get; set; }
        public int MostFamousMovieId { get; set; }

        //[JsonIgnore]
        public virtual Movie MostFamousMovie { get; set; }

        //[JsonIgnore]
        public virtual List<Movie> WorkedOn { get; set; }
    }
}
