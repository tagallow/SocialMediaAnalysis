IF OBJECT_ID ( 'EnableAccount', 'P' ) IS NOT NULL 
    DROP PROCEDURE EnableAccount
GO
CREATE PROCEDURE [dbo].[EnableAccount]
@id uniqueidentifier
WITH EXECUTE AS CALLER
AS
DECLARE @UpdatedDate datetime
SET @UpdatedDate = getutcdate()

IF EXISTS (select * from [dbo].[Accounts] where [dbo].[Accounts].[LocalAccountID] = @id)
BEGIN
    UPDATE
        [dbo].[Accounts]
    SET
        [dbo].[Accounts].[isActive] = 1,
        [dbo].[Accounts].[dateModified] = @UpdatedDate
    WHERE
        [dbo].[Accounts].[LocalAccountID] = @id
END