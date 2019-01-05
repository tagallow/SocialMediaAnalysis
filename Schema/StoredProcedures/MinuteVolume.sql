IF OBJECT_ID ( 'MinuteVolume', 'P' ) IS NOT NULL 
    DROP PROCEDURE MinuteVolume
GO
CREATE PROCEDURE [dbo].[MinuteVolume]
	@keyword NVARCHAR(50),
	@startTime datetime,
	@endTime datetime
AS

declare @keywordID int

set @keywordID = (select ID from keywords where keyword = @keyword)

select 
	count(*) 
from 
	Social_Media_Data 
where 
	datePosted >= @startTime AND 
	datePosted < @endTime AND 
	keywordID = @keywordID
