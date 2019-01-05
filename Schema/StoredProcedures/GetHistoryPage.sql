IF OBJECT_ID ( 'GetHistoryPage', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetHistoryPage
GO
CREATE PROCEDURE [dbo].[GetHistoryPage]
@keyword NVARCHAR (50),
@page int,
@increment int,
@latestDate datetime
AS
declare @keywordID int
set @keywordID = (select keywords.ID from Keywords where keyword=@keyword);

WITH Result AS( 
	SELECT top 1000
		datePosted,
		sourceID,
		[text],
		moodBit,
		author,
		ROW_NUMBER() OVER (ORDER BY dateCreated DESC) AS R_Number
	FROM 
		Social_Media_Data
	WHERE
		keywordID = @keywordID and
		dateCreated < @latestDate
		)
SELECT 
	*
FROM 
	Result
WHERE 
	R_Number > @page*@increment AND R_Number <= (@page*@increment)+@increment