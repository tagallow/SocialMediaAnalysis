IF OBJECT_ID ( 'EnableKeyword', 'P' ) IS NOT NULL 
    DROP PROCEDURE EnableKeyword
GO
CREATE PROCEDURE [dbo].[EnableKeyword]
@keyword NVARCHAR (50)
AS
DECLARE @UpdatedDate datetime
SET @UpdatedDate = getutcdate()

IF EXISTS (
    select 
        * 
    from 
        [dbo].[keywords] 
    where 
        UPPER([dbo].[keywords].[keyword]) = UPPER(@keyword)
    )
BEGIN
    UPDATE
        [dbo].[keywords]
    SET
        [dbo].[keywords].[isActive] = 1,
        [dbo].[keywords].[dateModified] = @UpdatedDate
    WHERE
        UPPER([dbo].[keywords].[keyword]) = UPPER(@keyword)
END
ELSE
BEGIN
    INSERT INTO [dbo].[keywords] ([keyword], [isActive], [dateModified], [dateCreated],[LatestTwitterID])
    VALUES(@keyword,1,@UpdatedDate,@UpdatedDate,-1)
END