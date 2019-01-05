IF OBJECT_ID ( 'AssignKeyword', 'P' ) IS NOT NULL 
	DROP PROCEDURE AssignKeyword
GO
CREATE PROCEDURE [dbo].[AssignKeyword]
@AccountId uniqueidentifier,
@keyword NVARCHAR (50)
AS
DECLARE @UpdatedDate datetime
declare @keywordID int

SET @UpdatedDate = getutcdate()

IF EXISTS (select * from [dbo].[keywords] where UPPER([dbo].[keywords].[keyword]) = UPPER(@keyword))
BEGIN
	set @keywordID = (select ID from keywords where UPPER([dbo].[keywords].[keyword]) = UPPER(@keyword))
	insert into dbo.KeywordAssignments([LocalAccountID],KeywordID) 
	values (@AccountId, @keywordID)

	UPDATE
		[dbo].[keywords]
	SET
		[dbo].[keywords].[isActive] = 1,
		[dbo].[keywords].[dateModified] = @UpdatedDate
	WHERE
		[dbo].[keywords].[ID] = @keywordID
END
ELSE
BEGIN
	INSERT INTO [dbo].[keywords] ([keyword], [isActive], [dateModified], [dateCreated],[LatestTwitterID])
	VALUES(@keyword,1,@UpdatedDate,@UpdatedDate,-1)
	
	set @keywordID = (select ID from keywords where keyword = @keyword)
	
	insert into dbo.KeywordAssignments([LocalAccountID],KeywordID) 
	values (@AccountId, @keywordID)
END

--testdata should never be active.
IF @keyword = 'testData'
begin
	UPDATE
		[dbo].[keywords]
	SET
		[dbo].[keywords].[isActive] = 0
	WHERE
		[dbo].[keywords].[ID] = @keywordID
end