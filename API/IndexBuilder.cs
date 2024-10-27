using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class IndexBuilder
    {
        private readonly DBContext _dbContext;
        private TableHashResult lastHash = new();
        private readonly Dictionary<Tuple<string, string>, Dictionary<string, List<string>>> _index;

        public IndexBuilder(DBContext dbContext) 
        {
            _dbContext = dbContext;
            _index = new();
        }

        public Dictionary<Tuple<string, string>, Dictionary<string, List<string>>> Index
        {
            get
            {
                var newHash = _dbContext.Set<TableHashResult>()
                    .FromSqlRaw("SELECT HASHBYTES('MD5', STRING_AGG(CONCAT(ClientId, Method, Path, CAST(AttributeList AS NVARCHAR(MAX))), '')) AS TableHash FROM AuthorizedAttributes")
                    .AsEnumerable()
                    .First();

                if (lastHash.Equals(newHash))
                {
                    return _index;
                }

                _index.Clear();

                foreach (var endpoint in _dbContext.AuthorizedAttributes.ToList())
                {
                    var indexKey = new Tuple<string, string>(endpoint.ClientId, endpoint.Method);

                    if (!_index.ContainsKey(indexKey))
                    {
                        _index.Add(indexKey, new());
                    }

                    _index[indexKey].Add(endpoint.Path, endpoint.JSONAttributeList);
                }

                return _index;
            }
        }
    }
}
