public class AuthorizationRecord
{
    public string ClientId { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public List<string> AuthorizedAttributes { get; set; }
}