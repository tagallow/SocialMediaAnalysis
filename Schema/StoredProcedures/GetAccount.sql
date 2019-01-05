IF OBJECT_ID ( 'GetAccount', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetAccount
GO
CREATE PROCEDURE [dbo].[GetAccount]
@id uniqueidentifier
AS
SELECT 
	FirstName,
	--LastName, 
	isActive, 
	LocalAccountID,
	PhoneNum,
	email,
	companyName,
	address1,
	address2,
	address3,
	city,
	[state],
	zip 
from 
	Accounts
where
	[LocalAccountID] = @id