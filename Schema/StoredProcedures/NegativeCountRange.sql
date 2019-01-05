IF OBJECT_ID ( 'GetNegativeCountRange', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetNegativeCountRange
GO
CREATE PROCEDURE [dbo].[GetNegativeCountRange]
	@startDate datetime,
	@endDate datetime,
	@keyword NVARCHAR(50)
AS
SELECT 
	count(*) from Social_Media_Data
WHERE
	[dbo].[Social_Media_Data].[keyword] = @keyword AND
	[dbo].[Social_Media_Data].[mood] = 'negative' AND
	[dbo].[Social_Media_Data].[datePosted] >= @startDate AND
	[dbo].[Social_Media_Data].[datePosted] < @endDate
