IF OBJECT_ID ( 'Togglekeyword', 'P' ) IS NOT NULL 
    DROP PROCEDURE ToggleKeyword
GO
CREATE PROCEDURE [dbo].[ToggleKeyword]
@keyword NVARCHAR (50)
WITH EXECUTE AS CALLER
AS
DECLARE @UpdatedDate datetime
SET @UpdatedDate = getutcdate()

IF EXISTS (select * from [dbo].[keywords] where [dbo].[keywords].[keyword] = @keyword)
BEGIN
	IF EXISTS (select * from [dbo].[keywords] where [dbo].[keywords].[keyword] = @keyword AND [dbo].[keywords].[isActive] = 1)
	BEGIN
		UPDATE
			[dbo].[keywords]
		SET
			[dbo].[keywords].[isActive] = 0,
			[dbo].[keywords].[dateModified] = @UpdatedDate
		WHERE
			[dbo].[keywords].[keyword] = @keyword
	END
	ELSE
	BEGIN
		UPDATE
				[dbo].[keywords]
			SET
				[dbo].[keywords].[isActive] = 1,
				[dbo].[keywords].[dateModified] = @UpdatedDate
			WHERE
				[dbo].[keywords].[keyword] = @keyword
	END
END
ELSE
BEGIN
    INSERT INTO [dbo].[keywords] ([keyword], [isActive], [dateModified], [dateCreated])
    SELECT
        @keyword,
        1,
        @UpdatedDate,
        @UpdatedDate
END