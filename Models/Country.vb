Public Class Country
    Public Property MaxReturnRow As Integer = 50

    Public Property ID As String
    Public Property CountryID As String

    Public Property Code As String
    Public Property CountryName As String
    Public Property Alpha2Code As String
    Public Property NumericCode As String
    Public Property ISOCode As String
    Public Property CallingCode As String
    Public Property NoOfDigits As String
    Public Property IsVATReference As String
    Public Property VATRate As String

    Public Property CountryLanguage As String
    Public Property IsActive As String
    Public Property UserID As String

    Function CheckCountryUniqueName() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cntry.ID"
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "cntry.Code"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "cntry.[Name]"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCountries cntry"

            .SqlWhere = "WHERE cntry.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND cntry.[Name] = " & ReadyString(CountryName)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getCountryList(ByVal strKeyWord As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cntry.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "cntry.Code"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "cntry.[Name]"
                End With
                With .Add("Alpha2Code", SqlDbType.VarChar)
                    .Expression = "cntry.Alpha2Code"
                End With
                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "cntry.CallingCode"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCountries cntry"
            .SearchString = strKeyWord
            .SqlOrder = "ORDER BY cntry.[Name] ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function

    Function getCountryListAll() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cntry.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "cntry.Code"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "cntry.[Name]"
                End With
                With .Add("Alpha2Code", SqlDbType.VarChar)
                    .Expression = "cntry.Alpha2Code"
                End With
                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "cntry.CallingCode"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCountries cntry"
            .SqlOrder = "ORDER BY cntry.[Name] ASC"
            pDT = .Requery

        End With

        Return pDT

    End Function
    Function getCountryInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "cntry.ID"
                    .Searchable = False
                End With
                With .Add("Code", SqlDbType.VarChar)
                    .Expression = "cntry.Code"
                End With
                With .Add("Country", SqlDbType.VarChar)
                    .Expression = "cntry.[Name]"
                End With
                With .Add("Alpha2Code", SqlDbType.VarChar)
                    .Expression = "cntry.Alpha2Code"
                End With
                With .Add("NumericCode", SqlDbType.Int)
                    .Expression = "cntry.NumericCode"
                End With
                With .Add("ISOCode", SqlDbType.VarChar)
                    .Expression = "cntry.ISOCode"
                End With
                With .Add("CallingCode", SqlDbType.VarChar)
                    .Expression = "cntry.CallingCode"
                End With
            End With

            .SqlFrom = "FROM LibraryCountries cntry"

            .SqlWhere = IIf(strID.Trim().Length = 0, "WHERE cntry.ID IS NULL", " WHERE cntry.ID = " + ReadyString(strID))

            pDT = .Requery

        End With

        Return pDT

    End Function

    Function SaveCountryInfo() As String
        Dim p As New InsertUpdate("LibraryCountries")
        Dim pSql As String
        Dim strIDReturn As String

        p("Code") = ReadyString(Code)
        p("Name") = ReadyString(CountryName)
        p("Alpha2Code") = ReadyString(Alpha2Code)
        p("NumericCode") = NumericCode
        p("ISOCode") = ReadyString(ISOCode)
        p("CallingCode") = ReadyString(CallingCode)
        p("NoOfDigits") = ReadyString(NoOfDigits)
        p("UserID") = IIf(WebSession.Instance().UserID.Trim().Length <= 0, "0", WebSession.Instance().UserID)
        p("LastUpdated") = "getdate()"

        Try

            Common.WebRequest.ProData.BeginTransaction()

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.ExecuteSQL(pSql)

            CountryID = strIDReturn

            Common.WebRequest.ProData.CommitTransaction()

            Return strIDReturn

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try
    End Function

    Function CheckCountryLanguage() As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylang.ID"
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylang.CountryID"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylang.Language"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCountriesLanguage ctrylang "

            .SqlWhere = " WHERE ctrylang.CountryID = " + ReadyString(CountryID) &
                        " AND ctrylang.ID " & IIf(ID.Trim().Length = 0, " IS NOT NULL", " <> " & ReadyString(ID)) &
                        " AND ctrylang.Language = " & ReadyString(CountryLanguage)

            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getCountryLanguageList(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData
            .MaxRecords = MaxReturnRow

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylang.ID"
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylang.CountryID"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylang.Language"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCountriesLanguage ctrylang "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE ctrylang.CountryID IS NULL", " WHERE ctrylang.CountryID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function getCountryLanguageInfo(ByVal strID As String) As DataTable
        Dim pFinder As Finder
        Dim pDT As DataTable

        pFinder = New Finder
        With pFinder
            .ProData = Common.WebRequest.ProData

            With .Fields
                With .Add("ID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylang.ID"
                End With
                With .Add("CountryID", SqlDbType.UniqueIdentifier)
                    .Expression = "ctrylang.CountryID"
                End With
                With .Add("Language", SqlDbType.VarChar)
                    .Expression = "ctrylang.Language"
                End With
                With .Add("IsActive", SqlDbType.Bit)
                    .Expression = "ctrylang.IsActive"
                End With
            End With

            .SqlFrom = " FROM dbo.LibraryCountriesLanguage ctrylang "

            .SqlWhere = IIf(strID.Trim().Length = 0, " WHERE ctrylang.ID IS NULL", " WHERE ctrylang.ID = " + ReadyString(strID))
            pDT = .Requery
        End With

        Return pDT

    End Function

    Function SaveCountryLanguageName() As String
        Dim p As New InsertUpdate("LibraryCountriesLanguage")
        Dim pSql As String
        Dim strIDReturn As String
        Dim strIDPostSuffixRec As String

        Try

            p("CountryID") = ReadyString(CountryID)
            p("Language") = IIf(CountryLanguage.Trim().Length = 0, "NULL", ReadyString(CountryLanguage))
            p("IsActive") = 1

            If (WebSession.Instance().EmployeeID = Nothing) Then
                p("UserID") = "NULL"
            Else
                p("UserID") = IIf(WebSession.Instance().EmployeeID.Trim().Length <= 0, "NULL", WebSession.Instance().EmployeeID)
            End If

            p("LastUpdated") = "getdate()"

            If (ID.Trim().Length <= 0) Then
                strIDReturn = Guid.NewGuid.ToString()
                p("ID") = ReadyString(strIDReturn)
                pSql = p.GetInsert()
            Else
                strIDReturn = ID
                pSql = p.GetUpdate(" ID = " & ReadyString(ID))
            End If

            Common.WebRequest.ProData.BeginTransaction()
            Common.WebRequest.ProData.ExecuteSQL(pSql)
            Common.WebRequest.ProData.CommitTransaction()

        Catch ex As Exception
            Common.WebRequest.ProData.RollbackTransaction()
        End Try

        Return strIDReturn

    End Function

    Function RemoveCountryLanguage() As Boolean
        Dim pSql As String
        Dim blnReturn As Boolean

        Try
            blnReturn = False

            pSql = "DELETE " &
                   " FROM dbo.LibraryCountriesLanguage " &
                   " WHERE ID = " & ReadyString(ID)
            Common.WebRequest.ProData.ExecuteSQL(pSql)

            blnReturn = True

        Catch ex As Exception

            blnReturn = False

        End Try

        Return blnReturn

    End Function


End Class
