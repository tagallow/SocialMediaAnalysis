IF OBJECT_ID ( 'GetTimeRangeCounts', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetTimeRangeCounts
GO
CREATE PROCEDURE [dbo].[GetTimeRangeCounts]
	@startTime datetime,
	@endTime datetime,
	@keywordID int
AS

declare @Positive int
declare @Negative int

set @Positive =(select count(*) from Social_Media_Data where keywordID = @keywordID and dateCreated > @startTime and dateCreated < @endTime and moodBit=1 )

set @Negative =(select count(*) from Social_Media_Data where keywordID = @keywordID and dateCreated > @startTime and dateCreated < @endTime and moodBit=0)

select @Positive as Positive,@Negative as Negative

