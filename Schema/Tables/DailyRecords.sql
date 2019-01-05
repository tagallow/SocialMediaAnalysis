IF NOT EXISTS (select * from sys.sysobjects where name = 'DailyRecords' and xtype = 'U')
Begin
	CREATE TABLE [dbo].[DailyRecords] (
		[ID]          INT      IDENTITY (1, 1) NOT NULL,
		[KeywordID]   INT      NOT NULL,
		[posCount]    INT      NOT NULL,
		[negCount]    INT      NOT NULL,
		[dateCreated] DATETIME NOT NULL,
	    [AnnotationTitle] NVARCHAR (50)  NULL,
		[Annotation]      NVARCHAR (200) NULL,
		[dateModified]    DATETIME       NULL,
		CONSTRAINT [PrimaryKey_DailyRecord] PRIMARY KEY CLUSTERED ([ID] ASC)
	);

	CREATE NONCLUSTERED INDEX [IX_DailyRecords_0]
		ON [dbo].[DailyRecords]([dateCreated] ASC);

	CREATE NONCLUSTERED INDEX [IX_DailyRecords_1]
		ON [dbo].[DailyRecords]([KeywordID] ASC);

End