IF NOT EXISTS (select * from sys.sysobjects where name = 'ErrorLogs' and xtype = 'U')
BEGIN
	CREATE TABLE [dbo].[ErrorLogs] (
		[ID]          BIGINT     IDENTITY (1, 1) NOT NULL,
		[dateCreated] DATETIME   NOT NULL,
		[exception]		NVARCHAR(1000) NOT NULL,
		[stackTrace]	NVARCHAR(2000) NULL,
		CONSTRAINT [PrimaryKey_ErrorLogs] PRIMARY KEY CLUSTERED ([ID] ASC)
	);
	CREATE NONCLUSTERED INDEX [IX_ErrorLogs_Created]
		ON [dbo].[ErrorLogs](dateCreated)
END