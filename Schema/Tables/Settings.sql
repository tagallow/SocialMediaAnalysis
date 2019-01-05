IF NOT EXISTS (select * from sys.sysobjects where name = 'settings' and xtype = 'U')
BEGIN
	CREATE TABLE [dbo].settings(
		[name] [nvarchar](50) NOT NULL,
		[value][nvarchar](50) NOT NULL
	)
	ALTER TABLE [dbo].settings ADD CONSTRAINT PK_settings PRIMARY KEY CLUSTERED ([name])
	CREATE INDEX IX_settings_name on [dbo].settings (name)
END