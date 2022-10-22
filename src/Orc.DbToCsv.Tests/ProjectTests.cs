namespace Orc.DbToCsv.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class ProjectTests
    {
        #region Methods
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
            Assert.AreEqual("Data Source=.\\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False", project.ConnectionString.Value);
            Assert.AreEqual(500, project.MaximumRowsInTable.Value);
            Assert.AreEqual("C:\\Temp\\CustomPath", project.OutputFolder.Value);
            Assert.AreEqual(2, project.Tables.Count);
            Assert.AreEqual("MyTable1", project.Tables[0].Name);
            Assert.AreEqual("Table.csv", project.Tables[0].Csv);
            Assert.AreEqual(string.Empty, project.Tables[0].Output);
            Assert.AreEqual("MyTable1", project.Tables[1].Name);
            Assert.AreEqual("Table1.csv", project.Tables[1].Csv);
            Assert.AreEqual("C:\\Temp\\Xa", project.Tables[1].Output);
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
            Assert.AreEqual("Data Source=.\\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False", project.ConnectionString.Value);
            Assert.AreEqual(500, project.MaximumRowsInTable.Value);
            Assert.AreEqual(2, project.Tables.Count);
            Assert.AreEqual("MyTable1", project.Tables[0].Name);
            Assert.AreEqual("Table.csv", project.Tables[0].Csv);
            Assert.AreEqual(string.Empty, project.Tables[0].Output);
            Assert.AreEqual("MyTable1", project.Tables[1].Name);
            Assert.AreEqual("Table1.csv", project.Tables[1].Csv);
            Assert.AreEqual("C:\\Temp\\Xa", project.Tables[1].Output);
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
            Assert.AreEqual("Data Source=.\\SQLExpress;Initial Catalog=RanttSaaS;Integrated Security=True;Pooling=False", project.ConnectionString.Value);
            Assert.AreEqual(500, project.MaximumRowsInTable.Value);
            Assert.AreEqual(2, project.Tables.Count);
            Assert.AreEqual("MyTable1", project.Tables[0].Name);
            Assert.AreEqual(string.Empty, project.Tables[0].Output);
            Assert.AreEqual("MyTable2", project.Tables[1].Name);
        }
        #endregion
    }
}
