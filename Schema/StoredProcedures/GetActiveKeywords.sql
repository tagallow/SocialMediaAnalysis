IF OBJECT_ID ( 'GetActiveKeywords', 'P' ) IS NOT NULL 
	DROP PROCEDURE GetActiveKeywords
GO
CREATE PROCEDURE [dbo].[GetActiveKeywords]

AS
SELECT 
	keyword, isActive, dateModified,ID 
from 
	keywords 
where 
	isActive=1
order by 
	keyword asc
