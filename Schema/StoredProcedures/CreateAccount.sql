IF OBJECT_ID ( 'CreateAccount', 'P' ) IS NOT NULL 
    DROP PROCEDURE CreateAccount
GO
CREATE PROCEDURE [dbo].[CreateAccount]
@LocalAccountID uniqueidentifier,
@UserID int, 
@FirstName NVARCHAR (50),
@LastName NVARCHAR (50)
AS
SET NOCOUNT ON
DECLARE @DateModified datetime
    SET @DateModified = getutcdate()
    
    IF (@UserID=-1)
		BEGIN
			INSERT INTO [dbo].[Accounts] ([LocalAccountID],[FirstName],[IsActive], [DateCreated], [DateModified])
			SELECT
				@LocalAccountID,
				@FirstName,
				1,
				--@Password,
				@DateModified,
				@DateModified
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[Accounts] ([LocalAccountID],[UserID],[FirstName],[LastName],[IsActive], [DateCreated], [DateModified])
			SELECT
				@LocalAccountID,
				@UserID,
				@FirstName,
				@LastName,
				1,
				--@Password,
				@DateModified,
				@DateModified
		END