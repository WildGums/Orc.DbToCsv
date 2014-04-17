Orc.DbToCsv
===========

Extracts data from SQL Server database tables or views and creates corresponding csv files

## Project
Please see projectsample\sample1.iprj for example. 

Project is a xml file defining reusable configuration of import db to csv task. As for today each project 
contains following fields:
ConnectionString - the connection string to the database;
MaximumRowsInTable - the number of of maximum rows read from the table. Helps to stop utility from downloading terrabytes from the database.  
	If the number is zero or less utility reads all records.
Tables - list of tables to import. If the list is empty utilty attempts to read all tables in the catalog defined in the connection string. 

## Orc.DbToCsv.Console
Orc.DbToCsv.Console is a command line tool that helps to process some project and save results to some folder. 
There are two possible command line options:
-p or -project path to the xml file defining import project. If option is not defined utility tries to find first valid in the current folder
-o or -output path to the output folder

Here is the sample:
Orc.DbToCsv.Console.exe -p E:\sample1.iprj -o D:\output


