#nullable disable

using System.Text.Json;
using System.ComponentModel.DataAnnotations.Schema;
using API.Models;

public class AuthorizedAttributesByRole
{
    public string RoleId { get; set; }

    public string Method { get; set; }

    public string Path { get; set; }

    public string AttributeList { get; set; }

    // This essentially is our new Loader
    [NotMapped]
    public List<string> JSONAttributeList
    {
        get
        {
            return string.IsNullOrEmpty(AttributeList)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(AttributeList);
        }
        set
        {
            AttributeList = JsonSerializer.Serialize(value);
        }
    }

    //[JsonIgnore]
    public virtual Role Role { get; set; }
}
