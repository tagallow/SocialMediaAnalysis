IF OBJECT_ID ( 'GetKeywords', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetKeywords
GO
CREATE PROCEDURE [dbo].[GetKeywords]

AS
SELECT 
	keyword, isActive, dateModified,ID from keywords