IF OBJECT_ID ( 'GetAllStatistics', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetAllStatistics
GO
CREATE PROCEDURE [dbo].[GetAllStatistics]
	@keyword NVARCHAR(50)
AS

declare @keywordID int

set @keywordID = (select ID from keywords where keyword = @keyword)

select 
	posCount,
	negCount,
	dateCreated,
	AnnotationTitle,
	Annotation
from 
	DailyRecords 
where 
	KeywordID=@keywordID
order by
	dateCreated