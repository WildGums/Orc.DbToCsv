@echo off
echo ===============================================
echo CSV Data Import Tool
echo ===============================================
echo.
echo This batch file will import DetailData.csv and ERPData.csv into your database.
echo Make sure to update the connection string in ImportData.iprj before running.
echo.
echo Files to import:
echo - DetailData.csv (structural steel manufacturing details)
echo - ERPData.csv (project management and workflow information)
echo.
echo Tables will be TRUNCATED before importing (existing data will be cleared).
echo.
pause
echo.
echo Building the project...
dotnet build ..\src\Orc.DbToCsv.sln --configuration Release --verbosity minimal
if %errorlevel% neq 0 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo.
echo Starting CSV import...
echo.
dotnet run --project ..\src\Orc.DbToCsv.Console\Orc.DbToCsv.Console.csproj --framework net8.0-windows --configuration Release -- --project "ImportData.iprj" --import --truncate
echo.
if %errorlevel% neq 0 (
    echo ERROR: Import failed!
) else (
    echo SUCCESS: Data imported successfully!
)
echo.
pause
