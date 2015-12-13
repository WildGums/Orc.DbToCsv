namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    [ContentProperty("Value")]
    public class StringProjectProperty : ProjectProperty
    {
        public string Value { get; set; }
    }
}