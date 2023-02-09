using System;
using System.IO;
using System.Text;

namespace DataSorter
{
    public class Record : IComparable<Record>
    {
        public string Name { get; set; }
        public uint Number { get; set; }

        public int ChunkIndex { get; set; }

        public int CompareTo(Record other)
        {
            int nameCompare = string.Compare(ReadStringUntilWhiteChar(this.Name), ReadStringUntilWhiteChar(other.Name));
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

        public static string ReadStringUntilWhiteChar(string input)
        {
            StringBuilder sb = new StringBuilder();
            int i = 0;
            while (i < input.Length && !char.IsWhiteSpace(input[i]))
            {
                sb.Append(input[i++]);
            }
            return sb.ToString();
        }
    }
}
