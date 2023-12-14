namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    [ContentProperty(nameof(Value))]
    public class Int32ProjectProperty : ProjectProperty
    {
        public int Value { get; set; }
    }
}
