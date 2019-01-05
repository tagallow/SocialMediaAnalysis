IF OBJECT_ID ( 'GetUserKeywords', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetUserKeywords
GO
CREATE PROCEDURE [dbo].[GetUserKeywords]
	@id uniqueidentifier
AS

SELECT
	KeywordID
from
	KeywordAssignments
where
	[LocalAccountID] = @id

