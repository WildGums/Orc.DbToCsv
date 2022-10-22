namespace Orc.DbToCsv
{
    public class Parameter
    {
        public Parameter()
        {
            Name = string.Empty;
        }

        public string Name { get; set; }
        public string? Value { get; set; }
    }
}
