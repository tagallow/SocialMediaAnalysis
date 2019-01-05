IF OBJECT_ID ( 'CollectTotals2', 'P' ) IS NOT NULL 
	DROP PROCEDURE CollectTotals2
GO
CREATE PROCEDURE [dbo].[CollectTotals2]
@today datetime
AS

DECLARE @startDate datetime;
SET @startDate = CONVERT (date, @today);

DECLARE @dateEnd datetime
SET @dateEnd = dateadd(day,1,@startDate)

DECLARE @posCount int
DECLARE @negCount int
DECLARE @numKeyWords int
DECLARE @currKey int
DECLARE @isActive bit

DECLARE @UpdatedDate datetime
SET @UpdatedDate = getutcdate()

SET @numKeyWords = (SELECT MAX(ID) FROM Keywords)
SET @currKey = 1

WHILE(@currKey <= @numKeyWords)
BEGIN
	set @isActive = (select isActive from keywords where ID = @currKey)
	IF(@isActive = 0)
	BEGIN
		SET @currKey = @currKey + 1
		CONTINUE
	END	
	SET @posCount = 
		(SELECT  
			count(*) FROM Social_Media_Data
		WHERE
			[dbo].[Social_Media_Data].[keywordID] = @currKey AND
			[dbo].[Social_Media_Data].[moodBit] = 1 AND
			[dbo].[Social_Media_Data].[datePosted] >=  @startDate AND
			[dbo].[Social_Media_Data].[datePosted] < @dateEnd)
	
	SET @negCount = 
		(SELECT  
			count(*) FROM Social_Media_Data
		WHERE
			[dbo].[Social_Media_Data].[keywordID] = @currKey AND
			[dbo].[Social_Media_Data].[moodBit] = 0 AND
			[dbo].[Social_Media_Data].[datePosted] >=  @startDate AND
			[dbo].[Social_Media_Data].[datePosted] < @dateEnd)

	IF NOT EXISTS (SELECT * FROM DailyRecords WHERE dateCreated = @startDate AND KeywordID = @currKey)
	BEGIN 
		INSERT INTO DailyRecords (dateCreated,KeywordID,posCount,negCount,dateModified)
		VALUES (@startDate,@currKey,@posCount,@negCount,@UpdatedDate)
	END
	ELSE
	BEGIN
		UPDATE DailyRecords
		SET
			posCount = @posCount,
			negCount = @negCount,
			dateModified = @UpdatedDate
		WHERE
			KeywordId = @currKey AND
			dateCreated = @startDate;
	END


	SET @currKey = @currKey + 1
END
