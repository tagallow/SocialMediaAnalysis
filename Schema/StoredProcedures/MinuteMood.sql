IF OBJECT_ID ( 'MinuteMood', 'P' ) IS NOT NULL 
    DROP PROCEDURE MinuteMood
GO
CREATE PROCEDURE [dbo].[MinuteMood]
	@keyword NVARCHAR(50),
	@startTime datetime,
	@endTime datetime
AS

declare @keywordID int

set @keywordID = (select ID from keywords where keyword = @keyword)

SELECT 
	posCount=(
		SELECT 
			Count(*) 
		FROM 
			Social_Media_Data 
		WHERE 
			moodBit = 1 and 
			keywordID = @keywordID and 
			datePosted >= @startTime and
			datePosted < @endTime), 
	negCount=(
		SELECT 
			Count(*) 
		FROM 
			Social_Media_Data 
		WHERE 
			moodBit = 0 and 
			keywordID = @keywordID and 
			datePosted >= @startTime and
			datePosted < @endTime)
