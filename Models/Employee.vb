
Public Class Employee

    Function getEmployeeList(ByVal strKeyWord As String) As DataTable
        Dim strSQL As String
        Dim dtaTable As DataTable

        strSQL = "SELECT TOP 50 emp.ID, " &
         " (emp.Surname + ', ' + emp.FirstName + ' ' + emp.MiddleName) as EmployeeName " &
         " FROM LibraryEmployeeRecord emp " &
         " WHERE (emp.Surname + ', ' + emp.FirstName + ' ' + emp.MiddleName) " &
         " LIKE '%" & strKeyWord & "%' " &
         " ORDER BY (emp.Surname + ', ' + emp.FirstName + ' ' + emp.MiddleName) ASC"

        With Common.WebRequest
            dtaTable = .ProData.Execute(strSQL)
        End With

        Return dtaTable

    End Function

    Function getEmployeeInfo(ByVal strID As String) As DataTable

        Dim strSQL As String
        Dim dtaTable As DataTable

        strSQL = "SELECT " &
            " emp.ID," &
            " emp.EmployeeID," &
            " emp.Surname," &
            " emp.FirstName," &
            " emp.MiddleName," &
            " emp.PresentAddressStreet," &
            " emp.PresentAddressCityID," &
            " precity.[Name] as PresentAddressCity," &
            " emp.PresentAddressCountryID," &
            " precountry.[Name] as PresentAddressCountry," &
            " emp.PermanentAddressStreet," &
            " emp.PermanentAddressCityID," &
            " percity.[Name] as PermanentAddressCity," &
            " emp.PermanentAddressCountryID," &
            " percountry.[Name] as PermanentAddressCountry," &
            " emp.DateOfBirth," &
            " emp.PlaceOfBirthID," &
            " pbirth.[Name] as PlaceOfBirth," &
            " emp.Gender," &
            " emp.CitizenshipID," &
            " ctzenship.Citizenship as Citizenship," &
            " emp.Height," &
            " emp.HeightMeasurement," &
            " emp.[Weight]," &
            " emp.WeightMeasurement," &
            " emp.CivilStatus," &
            " emp.SpouseSurname," &
            " emp.SpouseFirstName," &
            " emp.SpouseMiddleName," &
            " emp.FatherSurname," &
            " emp.FatherFirstName," &
            " emp.FatherMiddleName," &
            " emp.FatherOccupationID," &
            " focc.Occupation as FatherOccupation," &
            " emp.MotherSurname," &
            " emp.MotherFirstName," &
            " emp.MotherMiddleName," &
            " emp.MotherOccupationID," &
            " mocc.Occupation as MotherOccupation," &
            " emp.ParentsAddressStreet," &
            " emp.ParentsAddressCityID," &
            " parcity.[Name] as ParentAddressCity," &
            " emp.ParentsAddressCountryID," &
            " parcountry.[Name] as ParentAddressCountry," &
            " emp.ResidenceCertNo," &
            " emp.ResidenceCertIssuedAt," &
            " emp.ResidenceCertIssuedOn," &
            " emp.SSSNo," &
            " emp.PhilHealthNo," &
            " emp.TIN," &
            " emp.HDMFNo," &
            " emp.PassportNo," &
            " emp.PassportIssuedAt," &
            " emp.PassportIssuedOn," &
            " emp.NBINo " &
        " FROM dbo.LibraryEmployeeRecord emp " &
        " LEFT JOIN dbo.LibraryCities precity ON (emp.PresentAddressCityID = precity.ID) " &
        " LEFT JOIN dbo.LibraryCountries precountry ON (emp.PresentAddressCountryID = precountry.ID) " &
        " LEFT JOIN dbo.LibraryCities percity ON (emp.PermanentAddressCityID = percity.ID) " &
        " LEFT JOIN dbo.LibraryCountries percountry ON (emp.PermanentAddressCountryID = percountry.ID) " &
        " LEFT JOIN dbo.LibraryCities pbirth ON (emp.PlaceOfBirthID = pbirth.ID) " &
        " LEFT JOIN dbo.LibraryCitizenship ctzenship ON (emp.CitizenshipID = ctzenship.ID) " &
        " LEFT JOIN dbo.LibraryOccupation focc ON (emp.FatherOccupationID = focc.ID) " &
        " LEFT JOIN dbo.LibraryOccupation mocc ON (emp.MotherOccupationID = mocc.ID) " &
        " LEFT JOIN dbo.LibraryCities parcity ON (emp.ParentsAddressCityID = parcity.ID) " &
        " LEFT JOIN dbo.LibraryCountries parcountry ON (emp.ParentsAddressCountryID = parcountry.ID) " &
        " WHERE emp.ID = '" & strID & "'"

        With Common.WebRequest
            dtaTable = .ProData.Execute(strSQL)
        End With

        Return dtaTable

    End Function

End Class
