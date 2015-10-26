namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    [ContentProperty("Value")]
    public class Int32ProjectProperty : ProjectProperty
    {
        public int Value { get; set; }
    }
}