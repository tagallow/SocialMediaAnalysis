IF NOT EXISTS (select * from sys.sysobjects where name = 'DataSourceIDs' and xtype = 'U')
begin
	CREATE TABLE [dbo].[DataSourceIDs] (
		[ID] TINYINT NOT NULL,
		[Source] NVARCHAR (50) NOT NULL,
		CONSTRAINT [PrimaryKey_DataSources] PRIMARY KEY CLUSTERED ([ID] ASC, [Source] ASC)
	);

	INSERT INTO [DataSourceIDs] VALUES ('1', 'twitter');
	INSERT INTO [DataSourceIDs] VALUES ('2', 'facebook link');
	INSERT INTO [DataSourceIDs] VALUES ('3', 'facebook photo');
	INSERT INTO [DataSourceIDs] VALUES ('4', 'facebook status');
	INSERT INTO [DataSourceIDs] VALUES ('5', 'facebook video');
	INSERT INTO [DataSourceIDs] VALUES ('6', 'facebook music');
	INSERT INTO [DataSourceIDs] VALUES ('7', 'google plus');

end