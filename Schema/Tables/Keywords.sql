IF NOT EXISTS (select * from sys.sysobjects where name = 'Keywords' and xtype = 'U')
begin
CREATE TABLE [dbo].[Keywords] (
    [ID]           INT           IDENTITY (1, 1) NOT NULL,
    [keyword]      NVARCHAR (50) NOT NULL,
    [isActive]     BIT           NOT NULL,
    [dateModified] DATETIME      NOT NULL,
    [dateCreated]  DATETIME      NOT NULL,
    [LatestTwitterID] BIGINT           NOT NULL,
    CONSTRAINT [PrimaryKey_Keywords] PRIMARY KEY CLUSTERED ([ID] ASC)
);

CREATE NONCLUSTERED INDEX [IX_Keywords2_0]
    ON [dbo].[Keywords]([keyword] ASC);

CREATE NONCLUSTERED INDEX [IX_Keywords_0]
    ON [dbo].[Keywords]([isActive] ASC);

end