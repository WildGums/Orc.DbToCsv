namespace Orc.DbToCsv.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ProjectTests
    {
        [TestCase]
        public void GeneralTest()
        {
            var xaml = @"
<Project xmlns='http://schemas.wildgums.com/orc/dbtocsv'>
  <ConnectionString>Data Source=.\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False</ConnectionString>
  <MaximumRowsInTable>500</MaximumRowsInTable>
  <OutputFolder>C:\Temp\CustomPath</OutputFolder>
  <Project.Tables>
    <Table Name='MyTable1' Csv='Table.csv'/>
    <Table Name='MyTable1' Csv='Table1.csv' Output='C:\Temp\Xa'/>
  </Project.Tables>
</Project>".Replace('\'', '\"');

            var project = Project.Parse(xaml);
            Assert.That(project.ConnectionString.Value, Is.EqualTo("Data Source=.\\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False"));
            Assert.That(project.MaximumRowsInTable.Value, Is.EqualTo(500));
            Assert.That(project.OutputFolder.Value, Is.EqualTo("C:\\Temp\\CustomPath"));
            Assert.That(project.Tables.Count, Is.EqualTo(2));
            Assert.That(project.Tables[0].Name, Is.EqualTo("MyTable1"));
            Assert.That(project.Tables[0].Csv, Is.EqualTo("Table.csv"));
            Assert.That(project.Tables[0].Output, Is.EqualTo(string.Empty));
            Assert.That(project.Tables[1].Name, Is.EqualTo("MyTable1"));
            Assert.That(project.Tables[1].Csv, Is.EqualTo("Table1.csv"));
            Assert.That(project.Tables[1].Output, Is.EqualTo("C:\\Temp\\Xa"));
        }

        [TestCase]
        public void GeneralTest_NoOutputPath()
        {
            var xaml = @"
<Project xmlns='http://schemas.wildgums.com/orc/dbtocsv'>
  <ConnectionString>Data Source=.\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False</ConnectionString>
  <MaximumRowsInTable>500</MaximumRowsInTable>
  <Project.Tables>
    <Table Name='MyTable1' Csv='Table.csv'/>
    <Table Name='MyTable1' Csv='Table1.csv' Output='C:\Temp\Xa'/>
  </Project.Tables>
</Project>".Replace('\'', '\"');

            var project = Project.Parse(xaml);
            Assert.That(project.ConnectionString.Value, Is.EqualTo("Data Source=.\\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False"));
            Assert.That(project.MaximumRowsInTable.Value, Is.EqualTo(500));
            Assert.That(project.Tables.Count, Is.EqualTo(2));
            Assert.That(project.Tables[0].Name, Is.EqualTo("MyTable1"));
            Assert.That(project.Tables[0].Csv, Is.EqualTo("Table.csv"));
            Assert.That(project.Tables[0].Output, Is.EqualTo(string.Empty));
            Assert.That(project.Tables[1].Name, Is.EqualTo("MyTable1"));
            Assert.That(project.Tables[1].Csv, Is.EqualTo("Table1.csv"));
            Assert.That(project.Tables[1].Output, Is.EqualTo("C:\\Temp\\Xa"));
        }

        [TestCase]
        public void GeneralTest_NoSpecificFileName()
        {
            var xaml = @"
<Project xmlns='http://schemas.wildgums.com/orc/dbtocsv'>
  <ConnectionString>Data Source=.\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False</ConnectionString>
  <MaximumRowsInTable>500</MaximumRowsInTable>
  <Project.Tables>
    <Table Name='MyTable1'/>
    <Table Name='MyTable2'/>
  </Project.Tables>
</Project>".Replace('\'', '\"');

            var project = Project.Parse(xaml);
            Assert.That(project.ConnectionString.Value, Is.EqualTo("Data Source=.\\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False"));
            Assert.That(project.MaximumRowsInTable.Value, Is.EqualTo(500));
            Assert.That(project.Tables.Count, Is.EqualTo(2));
            Assert.That(project.Tables[0].Name, Is.EqualTo("MyTable1"));
            Assert.That(project.Tables[0].Output, Is.EqualTo(string.Empty));
            Assert.That(project.Tables[1].Name, Is.EqualTo("MyTable2"));
        }
    }
}
