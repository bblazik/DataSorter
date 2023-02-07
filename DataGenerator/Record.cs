namespace DataGenerator
{
    public class Record
    {
        public string Name { get; set; }
        public uint Number { get; set; }

        public override string ToString()
        {
            return $"{Number}. {Name}\n";
        }
    }
}
