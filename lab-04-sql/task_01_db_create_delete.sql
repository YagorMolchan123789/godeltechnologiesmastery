CREATE DATABASE Something

SELECT s.Name AS [Name], s.create_date AS [CreationDate]
FROM sys.databases s
WHERE s.Name = 'Something'

CREATE TABLE Wicked
(Id int not null)

BACKUP DATABASE Something
TO DISK = 'Something.bak'

DROP DATABASE Something

RESTORE DATABASE Something FROM DISK = 'Something.bak'

