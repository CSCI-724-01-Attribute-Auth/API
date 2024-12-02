#nullable disable

using System.ComponentModel.DataAnnotations.Schema;
// In summary, TableHashResult is a utility class to manage hash data with properties to
//  check if the hash is empty, methods to compare two instances by hash, and a custom hash
//  code generation. This setup might be useful in scenarios where you want to store, compare, or 
// check for empty hash data related to some table or entity.

namespace API.Models
{
    public class TableHashResult
    {
        public TableHashResult()
        {
            TableHash = Array.Empty<byte>();
        }

        public byte[] TableHash { get; set; }

        [NotMapped]
        public bool IsEmpty
        {
            get { return TableHash.Length == 0; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            TableHashResult other = (TableHashResult)obj;

            return TableHash != null && other.TableHash != null && TableHash.SequenceEqual(other.TableHash);
        }

        public override int GetHashCode()
        {
            if (TableHash == null || TableHash.Length == 0)
                return 0;

            return BitConverter.ToInt32(TableHash, 0);
        }
    }

}
