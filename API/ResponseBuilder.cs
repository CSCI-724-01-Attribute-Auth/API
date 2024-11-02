using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace API
{
    public class ResponseBuilder
    {
        private readonly Mapper _mapper;
        private TrieNode _head;  // Root node of the Trie

        public ResponseBuilder(Mapper mapper)
        {
            _mapper = mapper;
            _head = null;
        }

        /// <summary>
        /// Builds a filtered response containing only authorized attributes
        /// </summary>
        /// <param name="originalResponse">Original JSON response from service provider</param>
        /// <param name="authorizedAttributes">List of JSONPath expressions for authorized attributes</param>
        /// <returns>Modified JSON with only authorized attributes</returns>
        public string BuildResponse(string originalResponse, List<string> authorizedAttributes)
        {
            try
            {
                // Step 1: Build pattern trie with authorized attributes
                _head = BuildPatternTrie(authorizedAttributes);

                // Step 2: Get all JSONPaths from the original response using Mapper
                var allAttributes = _mapper.MapMovieAttributes(originalResponse);

                // Step 3: Find forbidden attributes by checking against the Trie
                var forbiddenAttributes = new List<string>();
                foreach (var attribute in allAttributes)
                {
                    if (!SearchPatternTrie(_head, attribute))
                    {
                        forbiddenAttributes.Add(attribute);
                    }
                }

                // Step 4: Convert JSON to JToken (DOM)
                var jsonDom = JToken.Parse(originalResponse);

                // Step 5: Remove forbidden attributes
                foreach (var forbiddenPath in forbiddenAttributes)
                {
                    RemoveJsonPath(jsonDom, forbiddenPath);
                }

                // Step 6: Convert back to JSON string
                return jsonDom.ToString(Formatting.Indented);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to build filtered response", ex);
            }
        }

        /// <summary>
        /// Removes a JSON path from a JToken structure
        /// </summary>
        private void RemoveJsonPath(JToken token, string jsonPath)
        {
            try
            {
                // Skip the root token ($)
                var pathParts = jsonPath.Substring(2).Split('.');
                RemoveJsonPathRecursive(token, pathParts, 0);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove JSON path: {jsonPath}", ex);
            }
        }

        private void RemoveJsonPathRecursive(JToken current, string[] pathParts, int depth)
        {
            if (depth == pathParts.Length - 1)
            {
                // We've reached the property to remove
                if (current is JObject obj)
                {
                    var propertyName = pathParts[depth];
                    if (propertyName.Contains("[*]"))
                    {
                        propertyName = propertyName.Substring(0, propertyName.IndexOf("["));
                    }
                    obj.Remove(propertyName);
                }
                return;
            }

            var part = pathParts[depth];
            if (part.Contains("[*]"))
            {
                // Handle array
                part = part.Substring(0, part.IndexOf("["));
                if (current[part] is JArray array)
                {
                    foreach (var item in array)
                    {
                        RemoveJsonPathRecursive(item, pathParts, depth + 1);
                    }
                }
            }
            else if (current[part] != null)
            {
                // Handle object
                RemoveJsonPathRecursive(current[part], pathParts, depth + 1);
            }
        }

        private TrieNode BuildPatternTrie(List<string> patterns)
        {
            // Step 1: Create the root node of the Trie
            TrieNode head = new TrieNode("$"); // Initialize with root token '$'

            // Step 2: Iterate through each authorized attribute pattern
            foreach (var pattern in patterns)
            {
                // Step 3: Tokenize the JSONPath expression
                var tokens = pattern.Substring(2).Split('.'); // Skip the '$.' part

                TrieNode currentNode = head; // Start from the root node

                // Step 4: Traverse the Trie for the current pattern tokens
                foreach (var token in tokens)
                {
                    if (!currentNode.Children.TryGetValue(token, out TrieNode childNode))
                    {
                        // If token does not exist, create a new node
                        childNode = new TrieNode(token);
                        currentNode.Children[token] = childNode; // Add it as a child
                    }
                    // Move to the child node
                    currentNode = childNode;
                }
            }

            // Step 5: Return the head of the Trie
            return head;
        }

        private bool SearchPatternTrie(TrieNode head, string jsonpath)
        {
            // Initialize the current node as the root of the Trie
            TrieNode currentNode = head;

            // Step 1: Tokenize the JSONPath expression, skipping the '$.' prefix
            var tokens = jsonpath.Substring(2).Split('.');

            // Step 2: Iterate through the tokens
            foreach (var token in tokens)
            {
                // Step 3: Check if the current node has the child for the current token
                if (currentNode.Children.TryGetValue(token, out TrieNode childNode))
                {
                    // Move to the child node if found
                    currentNode = childNode;
                }
                else
                {
                    // If the token is not found in the current node's children, return false
                    return false;
                }
            }

            // Step 4: If all tokens were found, return true
            return true;
        }

    }

    /// <summary>
    /// Represents a node in the pattern Trie
    /// </summary>
    public class TrieNode
    {
        public string Token { get; set; }
        public Dictionary<string, TrieNode> Children { get; set; }

        public TrieNode(string token)
        {
            Token = token;
            Children = new Dictionary<string, TrieNode>();
        }
    }
}
