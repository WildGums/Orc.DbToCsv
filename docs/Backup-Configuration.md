# Backup Configuration in Orc.DbToCsv

## Overview

Orc.DbToCsv now includes configurable backup management to prevent the accumulation of unnecessary backup files. By default, the system will not keep old backups unless explicitly configured to do so.

## Configuration Options

You can configure backup behavior in your project file using the following options:

### BackupFileCount

Controls how many backup files to keep per table.

```xml
<BackupFileCount>3</BackupFileCount>
```

- **Value**: Number of most recent backups to keep
- **Default**: 0 (delete all backups)
- **Description**: When set to a positive number, the system will keep that many most recent backups (sorted by creation time) for each table and delete older ones. When set to 0 or less, all backup files are deleted.

### BackupLocation

Specifies a custom location for backup files.

```xml
<BackupLocation>Backups</BackupLocation>
```

- **Value**: Directory path where backups should be stored
- **Default**: Same directory as the output files
- **Description**: Can be absolute or relative to the output folder. If the directory doesn't exist, it will be created.

### BackupFormat

Specifies a custom format for backup filenames.

```xml
<BackupFormat>{filename}_backup_{timestamp}{extension}</BackupFormat>
```

- **Value**: Format string with placeholders
- **Default**: `{filename}_backup_{timestamp}{extension}`
- **Description**: Available placeholders:
  - `{filename}` - Original filename without extension
  - `{extension}` - Original file extension (includes the dot)
  - `{timestamp}` - Current timestamp in yyyyMMddHHmmss format

## Complete Example

Here's a complete example of a project file with backup configuration:

```xml
<Project xmlns="http://schemas.wildgums.com/orc/dbtocsv">
  <ConnectionString>Data Source=.\SQLExpress;Initial Catalog=SampleDB;Integrated Security=True;Pooling=False</ConnectionString>
  <MaximumRowsInTable>1000</MaximumRowsInTable>
  <OutputFolder>C:\Temp\Exports</OutputFolder>
  
  <!-- Keep 3 most recent backups for each table -->
  <BackupFileCount>3</BackupFileCount>
  
  <!-- Store backups in a dedicated 'Backups' folder (relative to output folder) -->
  <BackupLocation>Backups</BackupLocation>
  
  <!-- Custom backup filename format -->
  <BackupFormat>{filename}_backup_{timestamp}{extension}</BackupFormat>
  
  <Project.Tables>
    <Table Name="dbo.Customers" Csv="Customers.csv"/>
    <Table Name="dbo.Orders" Csv="Orders.csv"/>
    <Table Name="dbo.Products" Csv="Products.csv"/>
  </Project.Tables>
</Project>
```

## Default Behavior

If you don't specify any backup configuration, the system will:
- Create a temporary backup of the original file before replacing it
- Immediately delete all backup files after the operation is complete
- Store any backups in the same directory as the output files
- Use the default filename format: `{filename}_backup_{timestamp}{extension}`

To keep backups, set `<BackupFileCount>` to a positive number in your project file.