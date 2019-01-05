IF OBJECT_ID ( 'InsertSentiment', 'P' ) IS NOT NULL 
    DROP PROCEDURE InsertSentiment
GO
CREATE PROCEDURE [dbo].[InsertSentiment]
@source NVARCHAR (50), 
@datePosted DATETIME, 
@mood NVARCHAR (50), 
@prob FLOAT (53), 
@text NVARCHAR (900), 
@keyword NVARCHAR (50), 
@author NVARCHAR (50)
AS
DECLARE @UpdatedDate datetime
DECLARE @keywordID int
DECLARE @sourceID tinyint
DECLARE @moodbit bit

IF(@mood = 'positive')
begin
	set @moodbit = 1
end
ELSE
begin
	set @moodbit = 0
end

set @sourceID = (select ID from DataSourceIDs where [DataSourceIDs].[Source] = @source)

SET @UpdatedDate = getutcdate()
set @keywordID = (select ID from keywords where UPPER([keyword]) = UPPER(@keyword))

INSERT INTO [dbo].[Social_Media_Data] ([sourceID], [datePosted], [prob], [text], [keywordID],[author],[dateCreated],[moodBit])
SELECT
    @sourceID,
    @datePosted,
    @prob,
    @text,
    @keywordID,
    @author,
    @UpdatedDate,
	@moodbit
    
UPDATE
    [dbo].[keywords]
SET
    [dbo].[keywords].[dateModified] = @UpdatedDate
WHERE 
    UPPER([dbo].[keywords].[keyword]) = UPPER(@keyword)

