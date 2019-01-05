IF NOT EXISTS (select * from sys.sysobjects where name = 'KeywordAssignments' and xtype = 'U')
BEGIN
CREATE TABLE [dbo].[KeywordAssignments] (
    [LocalAccountID] uniqueidentifier NOT NULL,
    [KeywordID] INT              NOT NULL,
    CONSTRAINT [PrimaryKey_KeywordAssignments] PRIMARY KEY CLUSTERED ([LocalAccountID] ASC, [KeywordID] ASC),
);
END