IF OBJECT_ID ( 'ToggleAnalysis', 'P' ) IS NOT NULL 
    DROP PROCEDURE ToggleAnalysis
GO
CREATE PROCEDURE [dbo].[ToggleAnalysis]
AS

IF EXISTS (
	SELECT 
		name,
		value
	FROM 
		[dbo].[settings] 
	WHERE 
		[dbo].[settings].[name] = 'sentimentAnalysis' AND
		[dbo].[settings].[value] = 'true')
BEGIN
    UPDATE
        [dbo].[settings]
    SET
        [dbo].[settings].[value] = 'false'
    WHERE
        [dbo].[settings].[name] = 'sentimentAnalysis'
END
ELSE
BEGIN
    UPDATE
        [dbo].[settings]
    SET
        [dbo].[settings].[value] = 'true'
    WHERE
        [dbo].[settings].[name] = 'sentimentAnalysis'
END