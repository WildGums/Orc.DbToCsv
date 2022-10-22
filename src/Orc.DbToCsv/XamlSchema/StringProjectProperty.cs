namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    [ContentProperty(nameof(Value))]
    public class StringProjectProperty : ProjectProperty
    {
        public string? Value { get; set; }
    }
}
