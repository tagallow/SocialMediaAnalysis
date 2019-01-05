IF OBJECT_ID ( 'UnassignKeyword', 'P' ) IS NOT NULL 
    DROP PROCEDURE UnassignKeyword
GO
CREATE PROCEDURE [dbo].[UnassignKeyword]
@AccountId uniqueidentifier,
@keyword NVARCHAR (50)
AS
DECLARE @UpdatedDate datetime
DECLARE @keywordID int
DECLARE @numAssigned int

SET @UpdatedDate = getutcdate()
set @keywordID = (select ID from keywords where keyword = @keyword)

DELETE FROM
	KeywordAssignments
WHERE
	[KeywordID] = @keywordID AND
	[LocalAccountID] = @AccountId

set @numAssigned = (select count(*) from KeywordAssignments where [KeywordID] = @keywordID)

if(@numAssigned = 0)
BEGIN
	UPDATE
        [dbo].[keywords]
    SET
        [dbo].[keywords].[isActive] = 0,
        [dbo].[keywords].[dateModified] = @UpdatedDate
    WHERE
        [dbo].[keywords].[ID] = @keywordID 
END

select @numAssigned