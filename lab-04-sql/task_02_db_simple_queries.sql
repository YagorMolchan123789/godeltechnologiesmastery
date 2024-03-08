USE AdventureWorks2017

--(1)
SELECT TOP 8
	 d.DepartmentID AS [Id],
	 d.Name AS [Name],
	 d.GroupName AS [GroupName],
	 d.ModifiedDate AS [ModifiedDate]
FROM HumanResources.Department AS d
ORDER BY d.Name DESC

--(2)
SELECT 
	 E.BusinessEntityID AS [Id],
	 E.NationalIDNumber AS [NationalNumber],
	 E.LoginID AS [LoginId],
	 E.OrganizationNode AS [OrganizationNode],
	 E.OrganizationLevel AS [OrganizationLevel],
	 E.JobTitle AS [JobTitle],
	 E.BirthDate AS [BirthDate],
	 E.MaritalStatus AS [MaritalStatus],
	 E.Gender AS [Gender],
	 E.HireDate AS [HireDate],
	 E.SalariedFlag AS [SalariedFlag],
	 E.VacationHours AS [VacationHours],
	 E.SickLeaveHours AS [SickLeaveHours],
	 E.CurrentFlag AS [CurrentFlag],
	 E.rowguid AS [rowguid],
	 E.ModifiedDate AS [ModifiedDate]
FROM HumanResources.Employee AS E
WHERE YEAR(E.HireDate) - YEAR(E.BirthDate) = 22

--(3)
SELECT 
	 E.BusinessEntityID AS [Id],
	 E.NationalIDNumber AS [NationalNumber],
	 E.LoginID AS [LoginId],
	 E.OrganizationNode AS [OrganizationNode],
	 E.OrganizationLevel AS [OrganizationLevel],
	 E.JobTitle AS [JobTitle],
	 E.BirthDate AS [BirthDate],
	 E.MaritalStatus AS [MaritalStatus],
	 E.Gender AS [Gender],
	 E.HireDate AS [HireDate],
	 E.SalariedFlag AS [SalariedFlag],
	 E.VacationHours AS [VacationHours],
	 E.SickLeaveHours AS [SickLeaveHours],
	 E.CurrentFlag AS [CurrentFlag],
	 E.rowguid AS [rowguid],
	 E.ModifiedDate AS [ModifiedDate]
FROM HumanResources.Employee AS E
WHERE E.JobTitle IN ('Design Engineer', 'Tool Designer', 'Engineering Manager', 'Production Control Manager')
	  AND E.MaritalStatus='M'
ORDER BY E.BirthDate

--(4)
SELECT
	 E.BusinessEntityID AS [Id],
	 E.NationalIDNumber AS [NationalNumber],
	 E.LoginID AS [LoginId],
	 E.OrganizationNode AS [OrganizationNode],
	 E.OrganizationLevel AS [OrganizationLevel],
	 E.JobTitle AS [JobTitle],
	 E.BirthDate AS [BirthDate],
	 E.MaritalStatus AS [MaritalStatus],
	 E.Gender AS [Gender],
	 E.HireDate AS [HireDate],
	 E.SalariedFlag AS [SalariedFlag],
	 E.VacationHours AS [VacationHours],
	 E.SickLeaveHours AS [SickLeaveHours],
	 E.CurrentFlag AS [CurrentFlag],
	 E.rowguid AS [rowguid],
	 E.ModifiedDate AS [ModifiedDate]
FROM HumanResources.Employee AS E
WHERE MONTH(E.HireDate)=3 AND DAY(E.HireDate)=5
ORDER BY E.BusinessEntityID
OFFSET 1 ROWS FETCH NEXT 5 ROWS ONLY

--(5)
SELECT
	 E.BusinessEntityID AS [Id],
	 E.NationalIDNumber AS [NationalNumber],
	 REPLACE(E.LoginID, 'adventure-works','adventure-works2024') AS [LoginId],
	 E.OrganizationNode AS [OrganizationNode],
	 E.OrganizationLevel AS [OrganizationLevel],
	 E.JobTitle AS [JobTitle],
	 E.BirthDate AS [BirthDate],
	 E.MaritalStatus AS [MaritalStatus],
	 E.Gender AS [Gender],
	 E.HireDate AS [HireDate],
	 E.SalariedFlag AS [SalariedFlag],
	 E.VacationHours AS [VacationHours],
	 E.SickLeaveHours AS [SickLeaveHours],
	 E.CurrentFlag AS [CurrentFlag],
	 E.rowguid AS [rowguid],
	 E.ModifiedDate AS [ModifiedDate]
FROM HumanResources.Employee as E
WHERE DATENAME(WEEKDAY,E.HireDate)='Wednesday' AND E.Gender='F'
	
--(6)
SELECT
	SUM(E.VacationHours) AS [VacationSumInHours],
	SUM(E.SickLeaveHours) AS [SicknessSumHours]
FROM HumanResources.Employee AS E

--(7)
SELECT DISTINCT TOP 8
    E.JobTitle AS [JobTitle],
	CASE WHEN E.JobTitle LIKE '% %' THEN RIGHT(E.JobTitle, CHARINDEX(' ', REVERSE(E.JobTitle))-1)
		 ELSE E.JobTitle
	END AS [LastWord]
FROM HumanResources.Employee AS E
ORDER BY E.JobTitle DESC

--(8)
SELECT
	 E.BusinessEntityID AS [Id],
	 E.NationalIDNumber AS [NationalNumber],
	 E.LoginID AS [LoginId],
	 E.OrganizationNode AS [OrganizationNode],
	 E.OrganizationLevel AS [OrganizationLevel],
	 E.JobTitle AS [JobTitle],
	 E.BirthDate AS [BirthDate],
	 E.MaritalStatus AS [MaritalStatus],
	 E.Gender AS [Gender],
	 E.HireDate AS [HireDate],
	 E.SalariedFlag AS [SalariedFlag],
	 E.VacationHours AS [VacationHours],
	 E.SickLeaveHours AS [SickLeaveHours],
	 E.CurrentFlag AS [CurrentFlag],
	 E.rowguid AS [rowguid],
	 E.ModifiedDate AS [ModifiedDate]
FROM HumanResources.Employee AS E
WHERE E.JobTitle LIKE '%Control%'