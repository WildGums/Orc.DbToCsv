Orc.DbToCsv
===========

[![Join the chat at https://gitter.im/WildGums/Orc.DbToCsv](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/WildGums/Orc.DbToCsv?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This library's sole purpose is to extract data from a MS SQL database into csv files. Each table or view will have a corresponding csv file.

The repository contains a library as well as a command line utility.

Usage
-------

In order to extract the data the library needs a project file.
A project file is simply an xml file with an ".iprj" extension, and looks like:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <ConnectionString>Data Source=localhost;Initial Catalog=Test;Integrated Security=True</ConnectionString>
  <MaximumRowsInTable>500</MaximumRowsInTable>
  <Tables>
    <string>Company</string>
    <string>UserProfile</string>
  </Tables>
</Project>
```

There are three important tags:

- ConnectionString: Standard connection string to connect to the database table.
- MaximumRowsInTable: Will specify the number of rows to retrieve from each table. If the tag is missing or the value is 0 or less, then all rows will be retrieved.
- Tables: Specify the name of each table (or view) you want to extract. If left emtpy, all tables will be extracted.

**Note**: The csv files will have the same name as the table names (except white spaces will be replaced with an underscore.)

Once the project file is setup you can create the csv files using the following command:

```C#
public static void ProcessProject(string projectFilePath, string outputFolderPath, ILogWriter logWriter)
```

Example:

```C#
Importer.ProcessProject(projectFilePath, outputFolderPath, new ConsoleWriter());
```

The parameters are:

- projectFilePath: The path to the .iprj file
- outputFolderPath: The folder path you want to save the csv files to
- logWriter: A log writer object


Command line utility (Orc.DbToCsv.Console)
-------------------------------------------

Orc.DbToCsv.Console is a simple command line utility.

There are two possible command line options:

* -p or -project path to .iprj file. If the option is not defined the utility will try and find a valid project file in the current folder.
* -o or -output path to the output directory where the csv files will be saved to.

Example:

```
Orc.DbToCsv.Console.exe -p E:\sample1.iprj -o D:\output
```

