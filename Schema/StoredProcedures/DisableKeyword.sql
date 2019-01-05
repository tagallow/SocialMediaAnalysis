IF OBJECT_ID ( 'DisableKeyword', 'P' ) IS NOT NULL 
    DROP PROCEDURE DisableKeyword
GO
CREATE PROCEDURE [dbo].[DisableKeyword]
@keyword NVARCHAR (50)
WITH EXECUTE AS CALLER
AS
DECLARE @UpdatedDate datetime
SET @UpdatedDate = getutcdate()

IF EXISTS (select * from [dbo].[keywords] where [dbo].[keywords].[keyword] = @keyword)
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
    INSERT INTO [dbo].[keywords] ([keyword], [isActive], [dateModified], [dateCreated])
    SELECT
        @keyword,
        0,
        @UpdatedDate,
        @UpdatedDate
END