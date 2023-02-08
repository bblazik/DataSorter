using System;

namespace DataSorter
{
    public class Record : IComparable<Record>
    {
        public string Name { get; set; }
        public uint Number { get; set; }

        public int ChunkIndex { get; set; }

        public int CompareTo(Record other)
        {
            int nameCompare = string.Compare(this.Name.Split(' ')[0], other.Name.Split(' ')[0]);
            if (nameCompare == 0)
            {
                return this.Number.CompareTo(other.Number);
            }
            return nameCompare;
        }

        public override string ToString()
        {
            return string.Format($"{Number}. {Name}\n");
        }

        public static Record ParseRecord(string line)
        {
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
