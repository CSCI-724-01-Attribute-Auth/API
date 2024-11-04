using Newtonsoft.Json.Linq;

public class Mapper
{
    public List<string> MapMovieAttributes(JToken jsonResponse)
    {
        try
        {
            var paths = new HashSet<string>();
            GetJsonPaths(jsonResponse, "$", paths);  // Start recursive path generation from root ($)
            return paths.ToList();
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid JSON input", ex);
        }
    }

    public List<string> MapMovieAttributes(string jsonResponse)
    {
        return MapMovieAttributes(JToken.Parse(jsonResponse));
    }

    private void GetJsonPaths(JToken token, string prefix, HashSet<string> paths)
    {
        if (token == null)
        {
            return;
        }
        if (token is JObject)
        {
            // Iterate through each property in a JSON object
            foreach (var property in token.Children<JProperty>())
            {
                // Recurse into each property with updated JSONPath prefix
                GetJsonPaths(property.Value, $"{prefix}.{property.Name}", paths);
            }
        }
        else if (token is JArray)
        {
            // Use wildcard * for array indices to generalize JSONPath
            GetJsonPaths(token.First, $"{prefix}[*]", paths);  // Process only the first item
        }
        else
        {
            // Base case: leaf node (non-nested data value)
            paths.Add(prefix);  // Add the completed JSONPath to the set
        }
    }
}
