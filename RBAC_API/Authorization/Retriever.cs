using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Authorization
{
    public class Retriever
    {
        private readonly IndexCache _indexCache;

        public Retriever(IndexCache indexCache)
        {
            _indexCache = indexCache;
        }
        public List<string> GetAuthorizedAttributes(string roleId, string method, string path)
        {
            // Create the lookup key as a Tuple
            var lookupKey = new Tuple<string, string>(roleId, method);

            Dictionary<string, List<string>>? authorizedEndpoints;

            // Check if the lookup key exists in the index
            if (!_indexCache.CachedIndex.TryGetValue(lookupKey, out authorizedEndpoints))
            {
                //_indexCache.CachedIndex.Keys.ToList().ForEach(Console.WriteLine);
                throw new KeyNotFoundException("Authorization not found in index.");
            }

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
