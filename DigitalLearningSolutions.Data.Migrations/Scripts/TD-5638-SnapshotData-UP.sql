DECLARE @dbName NVARCHAR(128) = DB_NAME()
DECLARE @defaultPath NVARCHAR(500) = CONVERT(NVARCHAR(500), SERVERPROPERTY('InstanceDefaultDataPath'))
DECLARE @snapshotTime NVARCHAR(12) = FORMAT(GETDATE(), 'yyyyMMddHHmm')

DECLARE @snapSql NVARCHAR(4000) = 'CREATE DATABASE ' + @dbName + '_' + @snapshotTime + ' ON 
( NAME = mbdbx101, FILENAME = ''' + @defaultPath + @dbName + '_' + @snapshotTime + '''),
( NAME = mbdbx101files, FILENAME = ''' + @defaultPath + @dbName + '_filestream1_' + @snapshotTime + ''')
AS SNAPSHOT OF ' + @dbName

EXEC sp_executesql @stmt = @SnapSql
