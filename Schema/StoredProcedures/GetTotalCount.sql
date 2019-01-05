IF OBJECT_ID ( 'GetTotalCount', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetTotalCount
GO
CREATE PROCEDURE [dbo].[GetTotalCount]
	@startDate datetime,
	@endDate datetime,
	@keyword NVARCHAR(50)
AS

DECLARE @keywordID int
set @keywordID = (select ID from Keywords where keyword = @keyword)

SELECT 
	count(*) from Social_Media_Data
WHERE
	[dbo].[Social_Media_Data].[KeywordID] = @keywordID AND
	[dbo].[Social_Media_Data].[datePosted] >= @startDate AND
	[dbo].[Social_Media_Data].[datePosted] < @endDate
