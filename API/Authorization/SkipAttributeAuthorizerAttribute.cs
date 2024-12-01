namespace API.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SkipAttributeAuthorizerAttribute : Attribute
    {
    }
}
