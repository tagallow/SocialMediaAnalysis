IF OBJECT_ID ( 'GetKeywordInfo', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetKeywordInfo
GO
CREATE PROCEDURE [dbo].[GetKeywordInfo]
	@id int
AS
SELECT 
	keyword,
	IsActive,
	DateModified 
FROM
	keywords 
where 
	ID = @id