USE AdventureWorks2017

--(1)
CREATE TABLE Person
(
	BusinessEntityID INT NOT NULL,
	PersonType NCHAR(2) NOT NULL,
	NameStyle NAMESTYLE NOT NULL,
	Title NVARCHAR(8) NULL,
	FirstName NAME NOT NULL,
	MiddleName NAME NOT NULL,
	LastName NAME NOT NULL,
	Suffix NVARCHAR(10) NULL,
	EmailPromotion INT NOT NULL,
	MofifiedDate DATETIME NOT NULL
)

--(2)
ALTER TABLE Person
ADD PersonId INT IDENTITY(3,5) NOT NULL CONSTRAINT PK_Person_PersonId
PRIMARY KEY

--(3)
ALTER TABLE Person
ADD CONSTRAINT CK_Person_MiddleName CHECK(MiddleName IN ('J', 'L'))

--(4)
ALTER TABLE Person
ADD CONSTRAINT DF_Person_Title DEFAULT 'N/A' FOR Title

--(5)
INSERT INTO Person
SELECT
	P.BusinessEntityID AS [BusinessEntityID],
	P.PersonType AS [PersonType],
	P.NameStyle AS [NameStyle],
	P.Title AS [Title],
	P.FirstName AS [FirstName],
	P.MiddleName AS [MiddleName],
	P.LastName AS [LastName],
	P.Suffix AS [Suffix],
	P.EmailPromotion AS [EmailPromotion],
	P.ModifiedDate AS [ModifiedDate]
FROM Person.Person AS P
	INNER JOIN HumanResources.Employee AS E ON E.BusinessEntityID=P.BusinessEntityID
WHERE E.JobTitle  NOT LIKE '%Finance%' AND P.MiddleName IN ('J','L')

--(6)
ALTER TABLE Person
ALTER COLUMN Title NVARCHAR(6)