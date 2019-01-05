IF OBJECT_ID ( 'InsertAddress', 'P' ) IS NOT NULL 
    DROP PROCEDURE InsertAddress
GO
CREATE PROCEDURE [dbo].[InsertAddress]
@AccountId uniqueidentifier,
@phoneNum NVARCHAR(15),
@email NVARCHAR(50),
@companyName NVARCHAR(50),
@address1 NVARCHAR(50),
@address2 NVARCHAR(50),
@address3 NVARCHAR(50),
@city NVARCHAR(50),
@state NVARCHAR(2),
@zip NVARCHAR(10)
--@country NVARCHAR(20),
--@apartmentNum NVARCHAR(8)
AS
DECLARE @UpdatedDate datetime

SET @UpdatedDate = getutcdate()

UPDATE
    [dbo].[Accounts]
SET
    [dbo].[Accounts].[companyName] = @companyName,
	[dbo].[Accounts].[PhoneNum] = @phoneNum,
	[dbo].[Accounts].[email] = @email,
	[dbo].[Accounts].[address1] = @address1,
	[dbo].[Accounts].[address2] = @address2,
	[dbo].[Accounts].[address3] = @address3,
	[dbo].[Accounts].city = @city,
	[dbo].[Accounts].[state] = @state,
	[dbo].[Accounts].[zip] = @zip,
	[dbo].[Accounts].[DateModified] = @UpdatedDate
	--[dbo].[Accounts].[country] = @country,
	--[dbo].[Accounts].[apartmentNum] = @apartmentNum
WHERE
    [dbo].[Accounts].[LocalAccountID] = @AccountId