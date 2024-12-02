#nullable disable
namespace API.Models
{
    public class User
    {
        public string UserId { get; set; }

#nullable enable
        public string? RoleId { get; set; }
#nullable disable

        //[JsonIgnore]
        public virtual Role Role { get; set; }
    }
}
