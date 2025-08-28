@echo off
echo ===============================================
echo CSV Data Import Tool - APPEND MODE
echo ===============================================
echo.
echo This batch file will import DetailData.csv and ERPData.csv into your database.
echo Make sure to update the connection string in ImportData.iprj before running.
echo.
echo Files to import:
echo - DetailData.csv (structural steel manufacturing details)
echo - ERPData.csv (project management and workflow information)
echo.
echo APPEND MODE: Data will be added to existing tables (no truncate).
echo.
pause
echo.
echo Starting CSV import (append mode)...
echo.
Orc.DbToCsv.Console.exe "ImportData.iprj" -i true
echo.
if %errorlevel% neq 0 (
    echo ERROR: Import failed!
) else (
    echo SUCCESS: Data appended successfully!
)
echo.
pause
