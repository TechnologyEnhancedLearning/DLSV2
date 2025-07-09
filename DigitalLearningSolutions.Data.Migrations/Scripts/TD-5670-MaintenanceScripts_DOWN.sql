IF OBJECT_ID('dbo.IndexOptimize', 'P') IS NOT NULL DROP PROCEDURE dbo.IndexOptimize;
IF OBJECT_ID('dbo.CommandExecute', 'P') IS NOT NULL DROP PROCEDURE dbo.CommandExecute;
IF OBJECT_ID('dbo.sp_purge_commandlog', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_purge_commandlog;
IF OBJECT_ID('dbo.CommandLog', 'U') IS NOT NULL DROP TABLE dbo.CommandLog;
