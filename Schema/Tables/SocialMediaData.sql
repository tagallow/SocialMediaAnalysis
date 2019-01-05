IF NOT EXISTS (select * from sys.sysobjects where name = 'Social_Media_Data' and xtype = 'U')
BEGIN
	CREATE TABLE [dbo].[Social_Media_Data] (
		[ID]          BIGINT     IDENTITY (1, 1) NOT NULL,
		[datePosted]  DATETIME   NOT NULL,
		[prob]        FLOAT (53) NOT NULL,
		[dateCreated] DATETIME   NOT NULL,
		[keywordID]   INT        NOT NULL,
		[sourceID]    TINYINT    NOT NULL,
		[text]		  NVARCHAR(200) NOT NULL,
		[moodBit]     BIT        ,
		[author]	  NVARCHAR(20) NOT NULL,
		CONSTRAINT [PrimaryKey_SocialMediaDataID] PRIMARY KEY CLUSTERED ([ID] ASC)
	);

	CREATE NONCLUSTERED INDEX [IX_SocialMediaData_KMD]
		ON [dbo].[Social_Media_Data]([keywordID],[moodbit],[datePosted])

	CREATE NONCLUSTERED INDEX [IX_SocialMediaData_Created]
		ON [dbo].[Social_Media_Data](dateCreated)
END