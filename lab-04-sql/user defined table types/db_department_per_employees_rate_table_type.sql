CREATE TYPE dbo.UTDepartmentPerEmployeesRate AS TABLE 
(
	BusinessEntityID INT NOT NULL,
	DepartmentID INT NOT NULL,
	Rate MONEY NOT NULL
)