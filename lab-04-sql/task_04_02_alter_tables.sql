--(1)
CREATE TABLE StateProvince
(
	StateProvinceID INT NOT NULL,
	StateProvinceCode NCHAR(3) NOT NULL,
	CountryRegionCode NVARCHAR(3) NOT NULL,
	IsOnlyStateProvinceFlag FLAG NOT NULL,
	Name NAME NOT NULL,
	TerritoryID INT NOT NULL,
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
    S.StateProvinceID AS [StateProvinceID],
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