using System;

namespace DataGenerator
{
    public class Record : IComparable<Record>
    {
        public string Name { get; set; }
        public uint Number { get; set; }

        public override string ToString()
        {
            return string.Format($"{Number}. {Name}\n");
        }
        public int CompareTo(Record other)
        {
            int nameCompare = string.Compare(this.Name.Split(' ')[0], other.Name.Split(' ')[0]);
            if (nameCompare == 0)
            {
                return this.Number.CompareTo(other.Number);
            }
            return nameCompare;
        }
    }
}
