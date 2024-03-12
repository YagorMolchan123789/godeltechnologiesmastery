--(1)
CREATE TABLE Person_New
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
	MofifiedDate DATETIME NOT NULL,
	PersonId INT IDENTITY(3,5) NOT NULL
		CONSTRAINT PK_PersonNew_PersonId PRIMARY KEY
)

--(2)
ALTER TABLE Person_New
ADD Salutation NVARCHAR(80) 

--(3)
INSERT INTO Person_New
SELECT
	P.BusinessEntityID AS [BusinessEntityID],
	P.PersonType AS [PersonType],
	P.NameStyle AS [NameStyle],
	CASE WHEN E.Gender='M' THEN 'Mr.'
	     WHEN E.Gender='F' THEN 'Ms.'
    END AS [Title],
	P.FirstName AS [FirstName],
	P.MiddleName AS [MiddleName],
	P.LastName AS [LastName],
	P.Suffix AS [Suffix],
	P.EmailPromotion AS [EmailPromotion],
	P.MofifiedDate AS [ModifiedDate],
	NULL AS [Salutation]
FROM Person AS P
	INNER JOIN HumanResources.Employee AS E ON E.BusinessEntityID=P.BusinessEntityID

--(4)
UPDATE Person_New
SET Salutation = CONCAT(Title, ' ', FirstName)

--(5)
DELETE FROM Person_New
WHERE LEN(Salutation) > 10

--(6)
ALTER TABLE Person
DROP CONSTRAINT PK_Person_PersonId
ALTER TABLE Person
DROP CONSTRAINT CK_Person_MiddleName
ALTER TABLE Person
DROP CONSTRAINT DF_Person_Title

--(7)
ALTER TABLE Person
DROP COLUMN PersonId

--(8)
DROP TABLE Person
DROP TABLE Person_New
