IF OBJECT_ID ( 'GetLocalUserID', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetLocalUserID
GO
CREATE PROCEDURE [dbo].[GetLocalUserID]
	@UserName NVARCHAR(50)
AS

select 
	[LocalAccountID] 
from 
	Accounts 
where 
	[FirstName] = @UserName