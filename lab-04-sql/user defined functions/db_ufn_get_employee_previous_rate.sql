CREATE FUNCTION [dbo].[ufnGetEmployeePreviousRate]
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
