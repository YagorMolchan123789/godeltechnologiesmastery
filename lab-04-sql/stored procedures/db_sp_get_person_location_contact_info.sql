USE [AdventureWorks2017]
GO
/****** Object:  StoredProcedure [dbo].[uspGetPersonLocationContactInfo]    Script Date: 3/13/2024 2:24:04 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[uspGetPersonLocationContactInfo]
	-- Add the parameters for the stored procedure here
	@FirstName NAME,
	@MiddleName NAME,
	@LastName NAME
AS
BEGIN

	SET NOCOUNT ON;

	SELECT 
		P.BusinessEntityID AS [PersonId],
		EA.EmailAddress AS [EmailAddress],
		PN.PhoneNumber AS [PhoneNumber],
		A.AddressLine1 AS [AddressLine1],
		A.AddressLine2 AS [AddressLine2],
		SP.Name AS [Province],
		A.City AS [City],
		A.PostalCode AS [PostalCode]
	FROM Person.Person AS P
		INNER JOIN Person.BusinessEntity AS BE ON BE.BusinessEntityID=P.BusinessEntityID
		INNER JOIN Person.BusinessEntityAddress AS BA ON BA.BusinessEntityID=BE.BusinessEntityID
		INNER JOIN Person.Address AS A ON A.AddressID=BA.AddressID
		INNER JOIN Person.StateProvince AS SP ON SP.StateProvinceID=A.StateProvinceID
		INNER JOIN Person.EmailAddress AS EA ON EA.BusinessEntityID=P.BusinessEntityID
		INNER JOIN Person.PersonPhone AS PN ON PN.BusinessEntityID=P.BusinessEntityID
	WHERE P.FirstName=@FirstName AND P.MiddleName=@MiddleName AND P.LastName=@LastName

END
