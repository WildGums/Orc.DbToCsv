# CSV Import Instructions

This guide explains how to import the CSV data files into your database using the Orc.DbToCsv tool.

## Files Overview

All import files are located in the `Data/` directory:

- **`DetailData.csv`** - Structural steel manufacturing details (31 columns, ~70 rows)
- **`ERPData.csv`** - Project management and workflow information (19 columns, ~8 rows)
- **`ImportData.iprj`** - Project configuration file for the import
- **`ImportData.bat`** - Batch script to run import with truncate (replace mode)
- **`ImportData_Append.bat`** - Batch script to run import without truncate (append mode)
- **`Orc.DbToCsv.Console.exe`** - Published console application
- **`CreateTables.sql`** - SQL script to create the required database tables
- **`IMPORT_INSTRUCTIONS.md`** - This instruction file

## Setup Instructions

### 1. Prepare Your Database

1. Open SQL Server Management Studio (or your preferred SQL tool)
2. Connect to your database server
3. Open the `CreateTables.sql` file
4. **IMPORTANT**: Replace `[YOUR_DATABASE_NAME]` with your actual database name
5. Execute the SQL script to create the tables

### 2. Configure Connection String

1. Open `ImportData.iprj` in a text editor
2. Find this line:
   ```xml
   <ConnectionString>Data Source=YOUR_SERVER;Initial Catalog=YOUR_DATABASE;Integrated Security=True;Pooling=False</ConnectionString>
   ```
3. Replace with your actual connection string. Examples:
   
   **Windows Authentication (Integrated Security):**
   ```xml
   <ConnectionString>Data Source=MYSERVER\SQLEXPRESS;Initial Catalog=MyDatabase;Integrated Security=True;Pooling=False</ConnectionString>
   ```
   
   **SQL Server Authentication:**
   ```xml
   <ConnectionString>Data Source=MYSERVER;Initial Catalog=MyDatabase;User Id=myusername;Password=mypassword;Pooling=False</ConnectionString>
   ```
   
   **Local SQL Express:**
   ```xml
   <ConnectionString>Data Source=.\SQLEXPRESS;Initial Catalog=MyDatabase;Integrated Security=True;Pooling=False</ConnectionString>
   ```

### 3. Run the Import

1. Navigate to the `Data/` directory
2. Choose your import mode:
   - **`ImportData.bat`** - Truncates tables first (clears existing data)
   - **`ImportData_Append.bat`** - Appends to existing data (no truncate)
3. Double-click the chosen batch file
4. The script will:
   - Import both CSV files using the published .exe
   - Show progress and results
   - No build required (uses pre-compiled executable)

## What Gets Imported

### DetailData Table
- **Assembly, Part, Profile** - Component identification
- **AssyKg, KgPerProfile, KgTotal** - Weight information
- **AssyLength, CutLength** - Dimensional data
- **Finish, Stock, PartType** - Manufacturing details
- **SalesOrder, ScopeId** - Project references
- And many more manufacturing-specific columns

### ERPData Table  
- **ScopeId, SalesOrder** - Project identifiers
- **Phase, Stage, Workflow** - Project status information
- **Customer, DeliveryAddress** - Customer details
- **PromisedDate, RequestedDate, Deadline** - Timeline information
- **TotalWeightKg, SoValue** - Project metrics

## Import Features

- **Truncate Tables**: Tables are cleared before importing to avoid duplicates
- **Transaction Safety**: Each import uses database transactions for data integrity
- **Error Handling**: Comprehensive logging and error reporting
- **Progress Tracking**: Shows import progress with batch updates
- **Data Validation**: Handles NULL values and data type conversions

## Troubleshooting

### Common Issues:

1. **Connection String Error**: Verify server name, database name, and credentials
2. **Table Missing Error**: Run the `CreateTables.sql` script first
3. **Permission Error**: Ensure your database user has INSERT permissions
4. **CSV File Not Found**: Verify the Data directory contains the CSV files

### Manual Import Commands:

If the batch file doesn't work, you can run the commands manually from the Data directory:

```bash
# Navigate to Data directory first
cd Data

# Import without truncate (append mode)
Orc.DbToCsv.Console.exe "ImportData.iprj" -i

# Import with truncate (replace mode - recommended)
Orc.DbToCsv.Console.exe "ImportData.iprj" -i -t

# Export data back to CSV (reverse operation)
Orc.DbToCsv.Console.exe "ImportData.iprj"

# Show help (if available)
Orc.DbToCsv.Console.exe -?
```

## Data Verification

After import, verify the data with these SQL queries:

```sql
-- Check record counts
SELECT COUNT(*) AS DetailData_Count FROM dbo.DetailData;
SELECT COUNT(*) AS ERPData_Count FROM dbo.ERPData;

-- View sample data
SELECT TOP 5 * FROM dbo.DetailData;
SELECT TOP 5 * FROM dbo.ERPData;

-- Check for common relationships
SELECT DISTINCT SalesOrder FROM dbo.DetailData;
SELECT DISTINCT SalesOrder FROM dbo.ERPData;
```

## Next Steps

Once the data is imported, you can:
- Query the data using SQL
- Export it back to CSV using the export mode
- Create reports and analysis
- Build applications using the imported data
