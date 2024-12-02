#nullable disable

namespace API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double TotalBudget { get; set; }
        public double TotalCost { get; set; }
        public DateTime ReleaseDate { get; set; }

        //[JsonIgnore]
        public virtual List<Person> Crew { get; set; }
    }
}
