IF OBJECT_ID ( 'GenerateData', 'P' ) IS NOT NULL 
    DROP PROCEDURE GenerateData
GO
CREATE PROCEDURE [dbo].[GenerateData]
	@keyword NVARCHAR(20),
	@startDate datetime,
	@endDate datetime
AS

DECLARE @keywordID int
DECLARE @currDate datetime
DECLARE @posCount int
DECLARE @negCount int
declare @step int

SET @currDate = @startDate

exec DisableKeyword @keyword
SET @keywordID = (select ID from keywords where keyword = @keyword)

delete from DailyRecords where KeywordID = @keywordID

set @posCount = RAND()*100
set @negCount = RAND()*100

while(@currDate < @endDate)
BEGIN
	set @step = rand()*10
	
	if(rand() >=.5)
	begin
		set @posCount = @posCount+@step
	end
	else
	begin
		if(@posCount > @step)
		begin
			set @posCount = @posCount-@step
		end
		else
		begin
			set @posCount = @posCount
		end
	end

	insert into DailyRecords([KeywordID],[posCount],[negCount],[dateCreated])
	values(@keywordID,@posCount,@negCount,@currDate)

	set @currDate = (select DATEADD(day,1,@currDate))
END