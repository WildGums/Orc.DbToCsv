-- SQL Script to create tables for importing DetailData.csv and ERPData.csv
-- Run this script in your database before importing the CSV files

USE [YOUR_DATABASE_NAME]; -- Replace with your actual database name
GO

-- Create DetailData table
-- This table stores structural steel manufacturing details
IF OBJECT_ID('dbo.DetailData', 'U') IS NOT NULL
    DROP TABLE dbo.DetailData;
GO

CREATE TABLE dbo.DetailData (
    Assembly NVARCHAR(50),
    AssyKg DECIMAL(18,6),
    AssyLength DECIMAL(18,6),
    AssyQty INT,
    BlackPregal NVARCHAR(50),
    Cope INT,
    CutLength DECIMAL(18,6),
    Description NVARCHAR(255),
    FabRqd NVARCHAR(50),
    Finish NVARCHAR(50),
    HasHoles INT,
    KgPerProfile DECIMAL(18,6),
    KgTotal DECIMAL(18,6),
    Mitre INT,
    ParentPart NVARCHAR(50),
    Part NVARCHAR(50),
    PartType NVARCHAR(50),
    ProcessNote NVARCHAR(255),
    Profile NVARCHAR(255),
    QtyPerAssy INT,
    QtyTotal INT,
    Revision NVARCHAR(10),
    SalesOrder NVARCHAR(50),
    ScopeId INT,
    SideCleat INT,
    Splay INT,
    StdPart INT,
    Stock NVARCHAR(50),
    SurfaceArea DECIMAL(18,6),
    Welding INT,
    WSBs INT
);
GO

-- Create ERPData table
-- This table stores project management and workflow information
IF OBJECT_ID('dbo.ERPData', 'U') IS NOT NULL
    DROP TABLE dbo.ERPData;
GO

CREATE TABLE dbo.ERPData (
    ScopeId INT,
    Phase NVARCHAR(100),
    Stage NVARCHAR(100),
    SalesOrder NVARCHAR(50),
    OnHold INT,
    AssignedTo NVARCHAR(255),
    Customer NVARCHAR(255),
    DeliveryAddress NVARCHAR(500),
    Workflow NVARCHAR(100),
    PromisedDate DATETIME,
    RequestedDate DATETIME,
    Deadline DATETIME,
    Label NVARCHAR(100),
    TotalWeightKg DECIMAL(18,6),
    SoValue DECIMAL(18,2),
    EstimatedFabricationTime INT,
    EstimatedComponents INT,
    EstimatedAssemblies INT,
    Title NVARCHAR(255)
);
GO

-- Create indexes for better performance
CREATE INDEX IX_DetailData_SalesOrder ON dbo.DetailData(SalesOrder);
CREATE INDEX IX_DetailData_ScopeId ON dbo.DetailData(ScopeId);
CREATE INDEX IX_DetailData_Assembly ON dbo.DetailData(Assembly);

CREATE INDEX IX_ERPData_SalesOrder ON dbo.ERPData(SalesOrder);
CREATE INDEX IX_ERPData_ScopeId ON dbo.ERPData(ScopeId);
CREATE INDEX IX_ERPData_Stage ON dbo.ERPData(Stage);

PRINT 'Tables created successfully!';
PRINT 'DetailData table: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' rows affected';
PRINT 'ERPData table: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' rows affected';
GO
