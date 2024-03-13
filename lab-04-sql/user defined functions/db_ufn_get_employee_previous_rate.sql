USE [AdventureWorks2017]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnGetEmployeePreviousRate]    Script Date: 3/13/2024 2:25:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[ufnGetEmployeePreviousRate]
(
	@BusinessEntitiId INT
)
RETURNS MONEY
AS
BEGIN

	DECLARE @Rate MONEY

	SELECT
		@Rate = PV.Rate
	FROM HumanResources.EmployeePayHistory AS PV
	WHERE PV.BusinessEntityID=@BusinessEntitiId
	ORDER BY PV.RateChangeDate DESC
	OFFSET 1 ROW FETCH NEXT 1 ROWS ONLY

	RETURN @Rate

END
