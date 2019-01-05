IF OBJECT_ID ( 'GetPositiveCountRange', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetPositiveCountRange
GO
CREATE PROCEDURE [dbo].[GetPositiveCountRange]
	@startDate datetime,
	@endDate datetime,
	@keyword NVARCHAR(50)
AS
SELECT 
	count(*) from Social_Media_Data
WHERE
	[dbo].[Social_Media_Data].[keyword] = @keyword AND
	[dbo].[Social_Media_Data].[mood] = 'positive' AND
	[dbo].[Social_Media_Data].[datePosted] >= @startDate AND
	[dbo].[Social_Media_Data].[datePosted] < @endDate
