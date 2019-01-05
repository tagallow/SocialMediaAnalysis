IF OBJECT_ID ( 'GetAccounts', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetAccounts
GO
CREATE PROCEDURE [dbo].[GetAccounts]

AS
SELECT 
	LocalAccountID,
	FirstName,
	isActive,
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