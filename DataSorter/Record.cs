using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSorter
{
    public class Record : IComparable<Record>
    {
        public string Name { get; set; }
        public uint Number { get; set; }

        public int ChunkIndex { get; set; }

        public int CompareTo(Record y)
        {
            int nameCompare = string.Compare(this.Name.Split(' ')[0], y.Name.Split(' ')[0]);
            if (nameCompare == 0)
            {
                return this.Number.CompareTo(y.Number);
            }
            return nameCompare;
        }

        public override string ToString()
        {
            return $"{Number}. {Name}";
        }

        public static Record ParseRecord(string line)
        {
            //todo add exception. 
            try {
                var recordParams = line.Split(new string[] { ". " }, StringSplitOptions.None);
                return new Record() { Number = uint.Parse(recordParams[0]), Name = recordParams[1] };
            } 
            catch(Exception e)
            {
                throw new Exception("Invalid record parse" + e);
            }
            
        }
    }
}
