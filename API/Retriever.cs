using System;
using System.Collections.Generic;
using System.Linq;

namespace API
{
    public class Retriever
    {
        private readonly IndexBuilder _indexBuilder;

        public Retriever(IndexBuilder indexBuilder)
        {
            _indexBuilder = indexBuilder;
        }
        public List<string> GetAuthorizedAttributes(string clientId, string method, string path)
        {
            // Create the lookup key as a Tuple
            var lookupKey = new Tuple<string, string>(clientId, method);

            // Check if the lookup key exists in the index
            if (!_indexBuilder.Index.ContainsKey(lookupKey))
            {
                throw new KeyNotFoundException("Client authorization not found in index.");
            }

            var authorizedEndpoints = _indexBuilder.Index[lookupKey];

            // Find matching endpoint path (using template matching)
            foreach (var endpointPath in authorizedEndpoints.Keys)
            {
                if (IsTemplateMatch(endpointPath, path))
                {
                    // Return the list of authorized JSONPath attributes
                    return authorizedEndpoints[endpointPath];
                }
            }

            throw new InvalidOperationException("No matching endpoint path found for the given URI.");
        }

        // Basic template matching for paths
        private bool IsTemplateMatch(string template, string path)
        {
            // Replace variable segments in the template with regex pattern to generalize matching
            var regexPattern = "^" + template.Replace("{", "").Replace("}", "").Replace("/", "\\/") + "$";
            return System.Text.RegularExpressions.Regex.IsMatch(path, regexPattern);
        }
    }
}
