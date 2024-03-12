--(1) DONE
SELECT
	E.BusinessEntityID AS [EmployeeId],
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
	E.ModifiedDate AS [ModifiedDate],
	MAX(P.Rate) AS [MaxRate]
FROM HumanResources.Employee AS E
	INNER JOIN HumanResources.EmployeePayHistory AS P ON E.BusinessEntityID=P.BusinessEntityID
GROUP BY E.BusinessEntityID, E.NationalIDNumber, E.LoginID, E.OrganizationNode, E.OrganizationLevel, E.JobTitle, 
E.BirthDate, E.MaritalStatus, E.Gender, E.HireDate, E.SalariedFlag, E.VacationHours, E.SickLeaveHours, E.CurrentFlag,
E.rowguid, E.ModifiedDate

--(2) DONE
SELECT 
	ROW_NUMBER() OVER (ORDER BY P.Rate) AS [RateRank],
	P.Rate AS [Rate]
FROM HumanResources.EmployeePayHistory AS P
GROUP BY P.Rate

--(3) DONE
SELECT
	E.BusinessEntityID AS [EmployeeId],
	DH.DepartmentID AS [DepartmentId],
	DH.StartDate AS [StartDate],
	DH.EndDate AS [EndDate]
FROM HumanResources.Employee AS E
	INNER JOIN HumanResources.EmployeePayHistory AS PH ON PH.BusinessEntityID=E.BusinessEntityID
	INNER JOIN HumanResources.EmployeeDepartmentHistory AS DH ON DH.BusinessEntityID=PH.BusinessEntityID
	INNER JOIN (
		SELECT 
			H.BusinessEntityID AS [BusinessEntityId],
			COUNT(H.BusinessEntityID) OVER (PARTITION BY H.BusinessEntityID) AS [DepartmentCount]
		FROM HumanResources.EmployeeDepartmentHistory AS H
	) AS D ON D.BusinessEntityId=E.BusinessEntityID
GROUP BY E.BusinessEntityID, DH.DepartmentID, DH.StartDate, DH.EndDate, D.DepartmentCount
HAVING COUNT(PH.Rate)>1 AND D.DepartmentCount=1 AND DH.EndDate IS NULL

--(4) DONE
SELECT
	D.DepartmentID AS [DepartmentId],
	D.Name AS [Name],
	D.GroupName AS [GroupName],
	D.ModifiedDate AS [ModifiedDate],
	COUNT(H.DepartmentID) AS [EmployeeCount]
FROM HumanResources.Department AS D
	INNER JOIN HumanResources.EmployeeDepartmentHistory AS H ON H.DepartmentID=D.DepartmentID
GROUP BY D.DepartmentID, D.Name, D.GroupName, D.ModifiedDate, H.EndDate
HAVING H.EndDate IS NULL

--(5) DONE
SELECT
	E.BusinessEntityID AS [EmployeeId],
	MAX(P.Rate) AS [CurrentRate],
	IIF(COUNT(P.BusinessEntityID)>1, ( SELECT PV.Rate FROM HumanResources.EmployeePayHistory AS PV WHERE PV.BusinessEntityID=P.BusinessEntityID
	ORDER BY PV.RateChangeDate DESC OFFSET 1 ROW FETCH NEXT 1 ROWS ONLY), MAX(P.Rate)) AS [PreviousRate],
	IIF(COUNT(P.BusinessEntityID)>1, MAX(P.Rate)-( SELECT PV.Rate FROM HumanResources.EmployeePayHistory AS PV WHERE PV.BusinessEntityID=P.BusinessEntityID
	ORDER BY PV.RateChangeDate DESC OFFSET 1 ROW FETCH NEXT 1 ROWS ONLY), 0) AS [DiffRate]
FROM HumanResources.Employee AS E
	INNER JOIN HumanResources.EmployeePayHistory AS P ON P.BusinessEntityID=E.BusinessEntityID
GROUP BY E.BusinessEntityID, P.BusinessEntityID

--(6) DONE
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

--(7) DONE
SELECT
	E.BusinessEntityID AS [EmployeeId],
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
	E.ModifiedDate AS [ModifiedDate], 
	S.Name AS [ShiftName]
FROM HumanResources.Employee AS E
	INNER JOIN HumanResources.EmployeeDepartmentHistory AS H ON H.BusinessEntityID=E.BusinessEntityID
	INNER JOIN HumanResources.Shift AS S ON S.ShiftID=H.ShiftID
WHERE S.Name='Evening'

--8 DONE
SELECT
	E.BusinessEntityID AS [EmployeeId],
	H.DepartmentID AS [DepartmentId],
	H.StartDate AS [StartDate],
	H.EndDate AS [EndDate],
	IIF(H.EndDate IS NOT NULL, FLOOR(DATEDIFF(DAY, H.StartDate,H.EndDate)/365), FLOOR(DATEDIFF(DAY, H.StartDate, GETDATE())/365)) AS [Experience]
FROM HumanResources.Employee AS E
	INNER JOIN HumanResources.EmployeeDepartmentHistory AS H ON H.BusinessEntityID=E.BusinessEntityID