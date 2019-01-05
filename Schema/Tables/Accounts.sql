IF NOT EXISTS (select * from sys.sysobjects where name = 'Accounts' and xtype = 'U')
begin
	CREATE TABLE [dbo].[Accounts] (
		[LocalAccountID]	uniqueidentifier	NOT NULL,
		[UserID]    INT NULL,
		[FirstName]	   NVARCHAR (50)    NOT NULL,
		[LastName]	   NVARCHAR (50)    NULL,
		[IsActive]     BIT              NOT NULL,
		[DateCreated]  DATETIME         NOT NULL,
		[DateModified] DATETIME         NOT NULL,
		[PhoneNum]     NVARCHAR (15)    NULL,
		[email]        NVARCHAR (50)    NULL,
		[companyName]  NVARCHAR (50)    NULL,
		[address1]     NVARCHAR (50)    NULL,
		[address2]     NVARCHAR (50)    NULL,
		[address3]     NVARCHAR (50)    NULL,
		[city]         NVARCHAR (50)    NULL,
		[state]        NVARCHAR (2)     NULL,
		[zip]          NVARCHAR (10)    NULL,
		CONSTRAINT [PrimaryKey_Accounts] PRIMARY KEY CLUSTERED ([LocalAccountID] ASC)
	);

	CREATE NONCLUSTERED INDEX [IX_Accounts_0]
		ON [dbo].[Accounts]([IsActive] ASC);

	CREATE NONCLUSTERED INDEX [IX_Accounts_1]
		ON [dbo].[Accounts]([DateModified] ASC);
end
