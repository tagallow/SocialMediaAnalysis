IF OBJECT_ID ( 'DisableAccount', 'P' ) IS NOT NULL 
    DROP PROCEDURE DisableAccount
GO
CREATE PROCEDURE [dbo].[DisableAccount]
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
        [dbo].[Accounts].[isActive] = 0,
        [dbo].[Accounts].[dateModified] = @UpdatedDate
    WHERE
        [dbo].[Accounts].[LocalAccountID] = @id
END