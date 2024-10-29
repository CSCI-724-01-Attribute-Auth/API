using Newtonsoft.Json.Linq;

public class Mapper
{
    private readonly List<string> paths = new List<string>();

    public List<string> MapMovieAttributes(string jsonResponse)
    {
        paths.Clear();  // Clear previous paths for new processing

        try
        {
            var parsedJson = JToken.Parse(jsonResponse);  // Parse JSON into a JToken structure
            GetJsonPaths(parsedJson, "$");  // Start recursive path generation from root ($)
            return paths;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid JSON input", ex);
        }
    }

    private void GetJsonPaths(JToken token, string prefix)
    {
        if (token is JObject)
        {
            // Iterate through each property in a JSON object
            foreach (var property in token.Children<JProperty>())
            {
                // Recurse into each property with updated JSONPath prefix
                GetJsonPaths(property.Value, $"{prefix}.{property.Name}");
            }
        }
        else if (token is JArray)
        {
            // Use wildcard * for array indices to generalize JSONPath
            foreach (var item in token.Children())
            {
                GetJsonPaths(item, $"{prefix}[*]");
            }
        }
        else
        {
            // Base case: leaf node (non-nested data value)
            paths.Add(prefix);  // Add the completed JSONPath to the list
        }
    }
}

