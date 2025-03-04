CREATE FUNCTION [dbo].[ufnGetDepartmentMaximalRateByEmployees]
(	
	@DepartmentID INT
)
RETURNS TABLE 
AS
RETURN 
(
	SELECT
		D.DepartmentId AS [DepartmentId],
		ET.EmployeeId AS [EmployeeId],
		D.Rate AS [MaxInDepartment]
	FROM (
		SELECT
			E.BusinessEntityID AS [EmployeeId],
			DH.DepartmentID AS [DepartmentId],
			MAX(PH.Rate) OVER (PARTITION BY E.BusinessEntityID) AS [Rate]
		FROM HumanResources.Employee AS E
			INNER JOIN HumanResources.EmployeePayHistory AS PH ON PH.BusinessEntityID=E.BusinessEntityID
			INNER JOIN HumanResources.EmployeeDepartmentHistory AS DH ON DH.BusinessEntityID=E.BusinessEntityID
	) AS D
		INNER JOIN (
			SELECT
				E.BusinessEntityID AS [EmployeeId],
				DH.DepartmentID AS [DepartmentId],
				MAX(PH.Rate) OVER (PARTITION BY E.BusinessEntityID) AS [Rate]
			FROM HumanResources.Employee AS E
				INNER JOIN HumanResources.EmployeePayHistory AS PH ON PH.BusinessEntityID=E.BusinessEntityID
				INNER JOIN HumanResources.EmployeeDepartmentHistory AS DH ON DH.BusinessEntityID=E.BusinessEntityID
		) AS ET ON ET.DepartmentId=D.DepartmentId
	WHERE ET.Rate=D.Rate
	GROUP BY D.DepartmentId, D.Rate, ET.EmployeeId
	HAVING D.DepartmentId=@DepartmentID
)