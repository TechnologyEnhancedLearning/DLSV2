-- ============================================
-- Drop if exists (for clean redeploy)
-- ============================================
IF OBJECT_ID('dbo.IndexOptimize', 'P') IS NOT NULL DROP PROCEDURE dbo.IndexOptimize;
IF OBJECT_ID('dbo.DatabaseIntegrityCheck', 'P') IS NOT NULL DROP PROCEDURE dbo.DatabaseIntegrityCheck;
IF OBJECT_ID('dbo.CommandExecute', 'P') IS NOT NULL DROP PROCEDURE dbo.CommandExecute;
IF OBJECT_ID('dbo.CommandLog', 'U') IS NOT NULL DROP TABLE dbo.CommandLog;
GO

-- ============================================
-- CommandLog table
-- ============================================
CREATE TABLE dbo.CommandLog (
    ID INT IDENTITY PRIMARY KEY,
    DatabaseName SYSNAME NULL,
    SchemaName SYSNAME NULL,
    ObjectName SYSNAME NULL,
    ObjectType CHAR(2) NULL,
    IndexName SYSNAME NULL,
    IndexType TINYINT NULL,
    StatisticsName SYSNAME NULL,
    PartitionNumber INT NULL,
    ExtendedInfo XML NULL,
    Command NVARCHAR(MAX) NOT NULL,
    CommandType NVARCHAR(60) NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    ErrorNumber INT NOT NULL,
    ErrorMessage NVARCHAR(MAX) NULL
);
GO

-- ============================================
-- CommandExecute stored procedure
-- ============================================
CREATE PROCEDURE dbo.CommandExecute
    @Command NVARCHAR(MAX),
    @CommandType NVARCHAR(60),
    @DatabaseName SYSNAME = NULL,
    @SchemaName SYSNAME = NULL,
    @ObjectName SYSNAME = NULL,
    @ObjectType CHAR(2) = NULL,
    @IndexName SYSNAME = NULL,
    @IndexType TINYINT = NULL,
    @StatisticsName SYSNAME = NULL,
    @PartitionNumber INT = NULL,
    @ExtendedInfo XML = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @StartTime DATETIME = GETDATE();
    DECLARE @ErrorNumber INT = 0;
    DECLARE @ErrorMessage NVARCHAR(MAX) = NULL;

    BEGIN TRY
        EXEC (@Command);
    END TRY
    BEGIN CATCH
        SET @ErrorNumber = ERROR_NUMBER();
        SET @ErrorMessage = ERROR_MESSAGE();
    END CATCH;

    INSERT INTO dbo.CommandLog (
        DatabaseName, SchemaName, ObjectName, ObjectType, IndexName, IndexType, StatisticsName,
        PartitionNumber, ExtendedInfo, Command, CommandType, StartTime, EndTime, ErrorNumber, ErrorMessage
    )
    VALUES (
        @DatabaseName, @SchemaName, @ObjectName, @ObjectType, @IndexName, @IndexType, @StatisticsName,
        @PartitionNumber, @ExtendedInfo, @Command, @CommandType, @StartTime, GETDATE(), @ErrorNumber, @ErrorMessage
    );

    IF @ErrorNumber <> 0
        RAISERROR(@ErrorMessage, 16, 1);
END
GO

-- ============================================
-- IndexOptimize stored procedure
-- ============================================
CREATE PROCEDURE dbo.IndexOptimize
    @Databases NVARCHAR(MAX) = 'USER_DATABASES',
    @FragmentationMedium TINYINT = 30,
    @FragmentationHigh TINYINT = 70
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @db SYSNAME;
    DECLARE db_cursor CURSOR FOR
    SELECT name FROM sys.databases
    WHERE (@Databases = 'USER_DATABASES' AND database_id > 4)
       OR name = @Databases;

    OPEN db_cursor;
    FETCH NEXT FROM db_cursor INTO @db;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @sql NVARCHAR(MAX) = N'
        USE [' + @db + '];

        DECLARE @schema SYSNAME, @table SYSNAME, @index SYSNAME;
        DECLARE @index_id INT, @frag FLOAT;

        DECLARE c CURSOR FOR
        SELECT s.name, t.name, i.name, i.index_id, ips.avg_fragmentation_in_percent
        FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, ''LIMITED'') ips
        JOIN sys.indexes i ON i.object_id = ips.object_id AND i.index_id = ips.index_id
        JOIN sys.tables t ON t.object_id = ips.object_id
        JOIN sys.schemas s ON s.schema_id = t.schema_id
        WHERE ips.index_id > 0 AND ips.page_count > 100;

        OPEN c;
        FETCH NEXT FROM c INTO @schema, @table, @index, @index_id, @frag;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE @cmd NVARCHAR(MAX);
            SET @cmd = ''ALTER INDEX ['' + @index + ''] ON ['' + @schema + ''].['' + @table + ''] '';

            IF @frag >= ' + CAST(@FragmentationHigh AS NVARCHAR) + '
                SET @cmd += ''REBUILD'';
            ELSE IF @frag >= ' + CAST(@FragmentationMedium AS NVARCHAR) + '
                SET @cmd += ''REORGANIZE'';
            ELSE
                SET @cmd = NULL;

            IF @cmd IS NOT NULL
                EXEC dbo.CommandExecute @Command = @cmd,
                                        @CommandType = ''ALTER INDEX'',
                                        @DatabaseName = ''' + @db + ''',
                                        @SchemaName = @schema,
                                        @ObjectName = @table,
                                        @ObjectType = ''U'',
                                        @IndexName = @index;

            FETCH NEXT FROM c INTO @schema, @table, @index, @index_id, @frag;
        END;

        CLOSE c;
        DEALLOCATE c;
        ';

        EXEC sp_executesql @sql;
        FETCH NEXT FROM db_cursor INTO @db;
    END;

    CLOSE db_cursor;
    DEALLOCATE db_cursor;
END
GO

-- ============================================
-- DatabaseIntegrityCheck stored procedure
-- ============================================
CREATE PROCEDURE dbo.DatabaseIntegrityCheck
    @Databases NVARCHAR(MAX) = 'USER_DATABASES'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @db SYSNAME;
    DECLARE db_cursor CURSOR FOR
    SELECT name FROM sys.databases
    WHERE (@Databases = 'USER_DATABASES' AND database_id > 4)
       OR name = @Databases;

    OPEN db_cursor;
    FETCH NEXT FROM db_cursor INTO @db;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE @cmd NVARCHAR(MAX);
        SET @cmd = 'DBCC CHECKDB([' + @db + ']) WITH NO_INFOMSGS, ALL_ERRORMSGS';

        EXEC dbo.CommandExecute
            @Command = @cmd,
            @CommandType = 'DBCC CHECKDB',
            @DatabaseName = @db;

        FETCH NEXT FROM db_cursor INTO @db;
    END

    CLOSE db_cursor;
    DEALLOCATE db_cursor;
END
GO

-- ============================================
-- Purge command log stored procedure
-- ============================================
CREATE OR ALTER PROCEDURE dbo.sp_purge_commandlog
    @DaysToKeep INT = 30
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @DeleteBefore DATETIME = DATEADD(DAY, -@DaysToKeep, GETDATE());

    DELETE FROM dbo.CommandLog
    WHERE StartTime < @DeleteBefore;
END
GO
