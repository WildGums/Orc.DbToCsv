# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Overview

Orc.DbToCsv is a .NET library and command-line utility for bidirectional data conversion between databases and CSV files. It supports both exporting data from databases to CSV files and importing CSV files back into database tables. The project is built using .NET 8.0/9.0 and uses the Cake build system.

## Development Commands

### Build
- **Full build**: `.\build.ps1` - Runs the complete Cake build process
- **Restore tools**: `dotnet tool restore` - Restores .NET tools including Cake
- **Manual Cake build**: `dotnet cake` - Run Cake build directly

### Testing
- **Run all tests**: `dotnet test src/Orc.DbToCsv.sln`
- **Run specific test project**: `dotnet test src/Orc.DbToCsv.Tests/Orc.DbToCsv.Tests.csproj`
- **Build specific project**: `dotnet build src/Orc.DbToCsv/Orc.DbToCsv.csproj`

### Console Application
- **Build console app**: `dotnet build src/Orc.DbToCsv.Console/Orc.DbToCsv.Console.csproj`
- **Run console app**: `dotnet run --project src/Orc.DbToCsv.Console/Orc.DbToCsv.Console.csproj`
- **Run with project file**: `dotnet run --project src/Orc.DbToCsv.Console/Orc.DbToCsv.Console.csproj -- -p "path/to/project.iprj"`

### CSV Import/Export Operations
- **Export DB to CSV**: `dotnet run --project src/Orc.DbToCsv.Console/Orc.DbToCsv.Console.csproj -- -p "project.iprj"`
- **Import CSV to DB**: `dotnet run --project src/Orc.DbToCsv.Console/Orc.DbToCsv.Console.csproj -- -p "project.iprj" --import`
- **Import with truncate**: `dotnet run --project src/Orc.DbToCsv.Console/Orc.DbToCsv.Console.csproj -- -p "project.iprj" --import --truncate`

## Project Structure

### Core Components
- **Orc.DbToCsv** - Main library containing the core conversion logic
- **Orc.DbToCsv.Console** - Command-line interface for the library
- **Orc.DbToCsv.Tests** - Unit tests using NUnit and Verify.NET

### Key Classes
- **Project**: XAML-based configuration parser (`.iprj` files) that defines database connections, tables, and export/import settings
- **Importer**: Core export engine that processes projects and converts database data to CSV
- **CsvImporter**: Core import engine that processes CSV files and imports data into database tables
- **DbToCsvExportDescription**: Data model describing individual table export operations
- **CsvToDbImportDescription**: Data model describing individual CSV import operations
- **Options**: Command-line argument parser for console application with import/export mode support

### Architecture Flow
1. Console application parses command-line options
2. Loads project configuration from `.iprj` file (XAML format)
3. Project validates connection strings and table definitions
4. Importer processes each table/view/stored procedure defined in project
5. Uses SqlTableReader from Orc.DataAccess to read data
6. Outputs CSV files using CsvHelper library

## Project Configuration (.iprj files)

The system uses XAML-based project files with `.iprj` extension containing:
- **ConnectionString**: Database connection details
- **MaximumRowsInTable**: Limit for rows to export
- **OutputFolder**: Base directory for CSV output
- **Provider**: Database provider (System.Data.SqlClient, Oracle.ManagedDataAccess.Client, etc.)
- **Tables**: Collection of tables/views/stored procedures to export

### Supported Database Types
- Microsoft SQL Server
- Oracle
- SQLite  
- MySQL
- PostgreSQL
- Firebird

### Table Types
- **Table**: Standard database table
- **View**: Database view
- **StoredProcedure**: Stored procedure with parameters
- **Function**: Database function with parameters
- **Sql**: Custom SQL query

### Table Configuration Properties
- **TruncateTable**: Boolean flag to truncate table before importing (import mode only)
- **Schema**: Database schema name
- **ConnectionString**: Override connection string per table
- **Provider**: Override database provider per table
- **Output**: Output directory path for CSV files
- **Csv**: CSV filename

## CSV Import Features

### Import Modes
- **Standard Import**: Appends CSV data to existing table data
- **Truncate Import**: Clears table data before importing (using TRUNCATE TABLE)

### Import Process
1. Validates CSV file exists
2. Establishes database connection
3. Optionally truncates target table
4. Reads CSV headers and maps to database columns
5. Uses parameterized INSERT statements for data safety
6. Processes data in batches with transaction support
7. Provides progress logging and error handling

## Key Dependencies

- **Catel.Core**: MVVM framework and dependency injection
- **Orc.DataAccess**: Database abstraction layer
- **Orc.Csv**: CSV handling utilities
- **Orc.CommandLine**: Command-line parsing
- **Microsoft.Data.SqlClient**: SQL Server connectivity
- **CsvHelper**: CSV file generation
- **NUnit**: Testing framework

## Build System

Uses Cake build automation with:
- **build.cake**: Main build configuration
- **build.ps1**: PowerShell entry point
- **dotnet-tools.json**: Tool manifest for Cake
- **Directory.Build.props**: Shared MSBuild properties across projects

The build system handles dependencies, compilation, testing, and packaging for the multi-project solution.
