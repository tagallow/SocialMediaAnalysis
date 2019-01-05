IF OBJECT_ID ( 'GetAnnotations', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetAnnotations
GO
CREATE PROCEDURE [dbo].GetAnnotations
@keyword NVARCHAR(50)
AS

select 
    ID,dateCreated,AnnotationTitle,Annotation
from
    DailyRecords
where
    AnnotationTitle is not null or
    Annotation is not null