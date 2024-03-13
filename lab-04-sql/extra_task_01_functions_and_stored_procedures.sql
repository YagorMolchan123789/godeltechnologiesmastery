DECLARE @DepartmentRates [UTDepartmentPerEmployeesRate]

INSERT INTO @DepartmentRates
SELECT * FROM dbo.ufnGetDepartmentMaximalRateByEmployees(2) 

SELECT
	D.DepartmentID AS [DepartmentId],
	D.BusinessEntityID AS [EmployeeId],
	D.Rate AS [MaxInDepartment]
FROM @DepartmentRates AS D
GROUP BY D.DepartmentId, D.Rate, D.BusinessEntityID

SELECT
	E.BusinessEntityID AS [EmployeeId],
	MAX(P.Rate) AS [CurrentRate],
	IIF(COUNT(P.BusinessEntityID)>1, dbo.ufnGetEmployeePreviousRate(P.BusinessEntityID), MAX(P.Rate)) AS [PreviousRate],
	IIF(COUNT(P.BusinessEntityID)>1, MAX(P.Rate)-dbo.ufnGetEmployeePreviousRate(P.BusinessEntityID), 0) AS [DiffRate]
FROM HumanResources.Employee AS E
	INNER JOIN HumanResources.EmployeePayHistory AS P ON P.BusinessEntityID=E.BusinessEntityID
GROUP BY E.BusinessEntityID, P.BusinessEntityID

EXECUTE dbo.uspGetPersonLocationContactInfo 'James', 'R', 'Hamilton'

