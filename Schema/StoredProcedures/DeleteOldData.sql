IF OBJECT_ID ( 'DeleteOldData', 'P' ) IS NOT NULL 
	DROP PROCEDURE DeleteOldData
GO
CREATE PROCEDURE [dbo].[DeleteOldData]

AS

delete from Social_Media_Data where DateCreated < getdate()-90

--delete from DailyRecords where DateCreated < getdate()-90