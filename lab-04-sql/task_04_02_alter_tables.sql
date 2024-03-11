USE AdventureWorks2017 EXEC sp_changedbowner 'sa'

--(1)
CREATE TABLE StateProvince
(
	StateProvinceID INT IDENTITY(1,1) NOT NULL
		CONSTRAINT PK_StateProvince_StateProvinceID PRIMARY KEY,
	StateProvinceCode NCHAR(3) NOT NULL,
	CountryRegionCode NVARCHAR(3) NOT NULL 
		CONSTRAINT FK_StateProvince_CountryRegion_CountryRegionCode
		FOREIGN KEY REFERENCES Person.CountryRegion(CountryRegionCode),
	IsOnlyStateProvinceFlag FLAG NOT NULL,
	Name NAME NOT NULL,
	TerritoryID INT NOT NULL
		CONSTRAINT FK_StateProvince_SalesTerritory_TerritoryID
		FOREIGN KEY REFERENCES Sales.SalesTerritory(TerritoryID),
	ModifiedDate DATETIME NOT NULL
)

--(2)
ALTER TABLE StateProvince
ADD CONSTRAINT UQ_StateProvince_Name UNIQUE (Name)

--(3)
ALTER TABLE StateProvince
ADD CONSTRAINT CK_StateProvince_CountryRegionCode CHECK (CountryRegionCode NOT LIKE '%[0-9]%')

--(4)
ALTER TABLE StateProvince
ADD CONSTRAINT DF_StateProvince_ModifiedDate DEFAULT GETDATE() FOR ModifiedDate

--(5)
INSERT INTO StateProvince
SELECT
	S.StateProvinceCode AS [StateProvinceCode],
	S.CountryRegionCode AS [CountryRegionCode],
	S.IsOnlyStateProvinceFlag AS [IsOnlyStateProvinceFlag],
	S.Name AS [Name],
	S.TerritoryID AS [TerritoryID],
	S.ModifiedDate AS [ModifiedDate]
FROM Person.StateProvince AS S
	INNER JOIN Person.CountryRegion AS C ON C.CountryRegionCode=S.CountryRegionCode

--(6)
ALTER TABLE StateProvince
DROP COLUMN IsOnlyStateProvinceFlag
ALTER TABLE StateProvince
ADD Population INT NULL