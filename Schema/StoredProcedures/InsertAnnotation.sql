IF OBJECT_ID ( 'InsertAnnotation', 'P' ) IS NOT NULL 
	DROP PROCEDURE InsertAnnotation
GO
CREATE PROCEDURE [dbo].[InsertAnnotation]
@keyword NVARCHAR(25),
@title NVARCHAR(50),
@caption NVARCHAR(200),
@date DATETIME
AS

DECLARE @keywordID INT
SET @keywordID = (SELECT id FROM Keywords WHERE @keyword = keyword)

update DailyRecords
SET
	Annotation = @caption,
	AnnotationTitle = @title
WHERE
	KeywordID = @keywordID AND
	dateCreated = @date