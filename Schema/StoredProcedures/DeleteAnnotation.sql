IF OBJECT_ID ( 'DeleteAnnotation', 'P' ) IS NOT NULL 
	DROP PROCEDURE DeleteAnnotation
GO
CREATE PROCEDURE [dbo].DeleteAnnotation
@id int
AS

update DailyRecords
SET
	Annotation = null,
	AnnotationTitle = null
WHERE
	ID = @id