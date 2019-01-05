IF OBJECT_ID ( 'InsertErrorLog', 'P' ) IS NOT NULL 
	DROP PROCEDURE InsertErrorLog
GO
CREATE PROCEDURE [dbo].[InsertErrorLog]
@exception NVARCHAR(800),
@date DATETIME,
@stackTrace NVARCHAR(2000)
AS

INSERT INTO ErrorLogs ([exception],[dateCreated],[stackTrace])
VALUES (@exception,@date,@stackTrace)