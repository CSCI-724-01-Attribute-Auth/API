#nullable disable
namespace API.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate {  get; set; }
        public int MostFamousMovie { get; set; }
    }
}
