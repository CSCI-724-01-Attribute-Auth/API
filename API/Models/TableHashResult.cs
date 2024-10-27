#nullable disable

using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class TableHashResult
    {
        public TableHashResult() { 
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
