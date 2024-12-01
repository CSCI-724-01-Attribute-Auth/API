using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Authorization
{
    public class IndexCache
    {
        public Dictionary<Tuple<string, string>, Dictionary<string, List<string>>> CachedIndex { get; private set; }
        public TableHashResult LastHash = new();

        public IndexCache()
        {
            CachedIndex = new Dictionary<Tuple<string, string>, Dictionary<string, List<string>>>();
            LastHash = new();
        }

        public void UpdateCache(Dictionary<Tuple<string, string>, Dictionary<string, List<string>>> newIndex, TableHashResult newHash)
        {
            CachedIndex = newIndex;
            LastHash = newHash;
        }
    }


    public class IndexBuilder
    {
        private readonly DBContext _dbContext;
        private readonly IndexCache _indexCache;

        public IndexBuilder(DBContext dbContext, IndexCache indexCache)
        {
            _dbContext = dbContext;
            _indexCache = indexCache;
        }

        public Dictionary<Tuple<string, string>, Dictionary<string, List<string>>> GetIndex()
        {
            // Retrieve the latest hash from the database
            var newHash = _dbContext.Set<TableHashResult>()
                .FromSqlRaw("SELECT HASHBYTES('MD5', STRING_AGG(CONCAT(RoleId, Method, Path, CAST(AttributeList AS NVARCHAR(MAX))), '')) AS TableHash FROM AuthorizedAttributesByRole")
                .AsEnumerable()
                .First();

            // If hash matches the last one, return the cached index
            if (_indexCache.LastHash == newHash)
            {
                return _indexCache.CachedIndex;
            }

            // Otherwise, update the cache with a fresh copy from the database
            var updatedIndex = new Dictionary<Tuple<string, string>, Dictionary<string, List<string>>>();

            foreach (var endpoint in _dbContext.AuthorizedAttributesByRole.ToList())
            {
                var indexKey = new Tuple<string, string>(endpoint.RoleId, endpoint.Method);

                if (!updatedIndex.ContainsKey(indexKey))
                {
                    updatedIndex.Add(indexKey, new Dictionary<string, List<string>>());
                }

                updatedIndex[indexKey].Add(endpoint.Path, endpoint.JSONAttributeList);
            }

            // Update the cache with the new dictionary and hash
            _indexCache.UpdateCache(updatedIndex, newHash);

            return updatedIndex;
        }
    }
}
