namespace Orc.DbToCsv
{
    using System.Windows.Markup;

    /// <summary>
    /// Controls how many backup files to keep per table.
    /// Default value of 0 means no backups are kept.
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class BackupFileCount : Int32ProjectProperty
    {
        public BackupFileCount()
        {
            // Default to not keeping any backups
            Value = 0;
        }
    }

    /// <summary>
    /// Specifies a custom location for backup files.
    /// If not specified, backups are stored in the same location as the output files.
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class BackupLocation : StringProjectProperty
    {
    }

    /// <summary>
    /// Specifies a custom format for backup filenames.
    /// Default format is "{filename}_backup_{timestamp}{extension}".
    /// 
    /// Available placeholders:
    /// {filename} - Original filename without extension
    /// {extension} - Original file extension (includes the dot)
    /// {timestamp} - Current timestamp in yyyyMMddHHmmss format
    /// {index} - Index number (for multiple backups)
    /// </summary>
    [ContentProperty(nameof(Value))]
    public class BackupFormat : StringProjectProperty
    {
        public BackupFormat()
        {
            // Default format
            Value = "{filename}_backup_{timestamp}{extension}";
        }
    }
}