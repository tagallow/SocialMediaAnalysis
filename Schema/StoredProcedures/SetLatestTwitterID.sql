IF OBJECT_ID ( 'SetLatestTwitterID', 'P' ) IS NOT NULL 
    DROP PROCEDURE SetLatestTwitterID
GO
CREATE PROCEDURE [dbo].SetLatestTwitterID
@keyword NVARCHAR (50),
@twitterID BIGINT
AS
DECLARE @UpdatedDate datetime
--declare @keywordID int

SET @UpdatedDate = getutcdate()
--SET @keywordID = (select id from Keywords where keyword = @keyword)

update Keywords
set LatestTwitterID = @twitterID,dateModified = @UpdatedDate
where keyword = @keyword