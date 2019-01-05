IF OBJECT_ID ( 'GetLatestTwitterID', 'P' ) IS NOT NULL 
    DROP PROCEDURE GetLatestTwitterID
GO
CREATE PROCEDURE [dbo].GetLatestTwitterID
@keyword NVARCHAR (50)
AS

select LatestTwitterID from Keywords where keyword = @keyword

